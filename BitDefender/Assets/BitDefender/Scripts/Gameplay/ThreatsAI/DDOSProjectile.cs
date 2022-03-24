using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Name: Ross Hutchins
//ID: HUT18001284

//The entire purpose of this script is just to delete itself if it hits the shield, as well the intro animation and always facing the player.
public class DDOSProjectile : MonoBehaviour
{

    [SerializeField] private MainGameScript main;

    //Just incase if the projectile misses the CPU, at least it will delete after 30 seconds.
    private float lifeTime = 30.0f;
    public int DMG = 3; //How much damage to deal to the player?

    private Vector3 spawnPosition;
    private void OnEnable()
    {
        //spawnPosition will be its local position, and since it going toward origin of its parent (MainGame).
        //We can just dot product with the position alone, since position - origin will still be position.
        spawnPosition = transform.localPosition;
    }

    void Update()
    {

        transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(50.0f, 50.0f, 50.0f), 200.0f * Time.deltaTime);

        //Perform a dot product based on its current position to its spawnPosition from origin.
        //If it goes negative then it means it went past the player, thus damaging them.
        if(Vector3.Dot(transform.localPosition,spawnPosition) <= 0.0f)
            main.Damage(gameObject);

        //Self-destruct if it has existed for too long.
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0.0f)
            Destroy(gameObject);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Shield"))
            Destroy(gameObject);
    }
}
