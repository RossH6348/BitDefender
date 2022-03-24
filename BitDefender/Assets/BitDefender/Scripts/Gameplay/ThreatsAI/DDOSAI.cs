using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Name: Ross Hutchins
//ID: HUT18001284

public class DDOSAI : ThreatAI
{

    [SerializeField] private float deniedVelocity = 5.0f; //How fast should its projectiles be fired at the player.
    [SerializeField] private GameObject deniedProjectile; //The projectile to use.

    [SerializeField] private int deniedCount = 18; //How many to fire?
    [SerializeField] private float deniedFirerate = 3.0f; //How much should it fire in a second?

    protected override void PreAI()
    {
        if (transform.localScale.magnitude >= originalScale.magnitude)
            StartCoroutine("PreparingStrike");
        else
            transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(100.0f, 100.0f, 100.0f), 200.0f * Time.deltaTime);
        base.PreAI();
    }

    private float fireTick = 0.0f;
    protected override void AI()
    {

        //DDOS is also a static enemy, reset their Z position to stop them from moving forward.
        position.z = main.spherePlayArea;

        fireTick += Time.deltaTime * deniedFirerate;
        if(fireTick >= 1.0f && deniedCount > 0)
        {
            //Decrease their projectile count by one, and spawn a projectile to fire at enemy.
            deniedCount--;

            GameObject newDenied = Instantiate(deniedProjectile, transform.parent);

            //Generate a random spawn point to start at.
            Vector3 spawnPos = transform.position;

            float angle = Random.Range(0.0f, 360.0f) * Mathf.Deg2Rad;
            float sample = Random.Range(0.0f, 0.4f);
            spawnPos += Mathf.Sin(angle) * newDenied.transform.up * sample;
            spawnPos += Mathf.Cos(angle) * newDenied.transform.right * sample;

            newDenied.transform.position = spawnPos;
            newDenied.transform.LookAt(main.transform.position);

            //Get rigidbody to apply our velocity, going toward the player.
            Rigidbody rigid = newDenied.GetComponent<Rigidbody>();
            rigid.velocity = (main.transform.position - spawnPos).normalized * deniedVelocity;

            main.Threats.Add(newDenied); //Add the projectile to the list of threats.

            newDenied.SetActive(true);
            fireTick = 0.0f; //Reset the tick.
        }
        else if(deniedCount < 1)
        {
            state = ThreatState.Destroyed; //It has finished its attack, so it will now self-destruct.
        }

        base.AI();
    }

    protected override void Damage()
    {
        return; //DDOS is invincible, it will self-destruct after its strike.
    }

    IEnumerator PreparingStrike()
    {
        state = ThreatState.Preparing;
        yield return new WaitForSeconds(5);
        state = ThreatState.Attacking;
    }

}
