using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Name: Ross Hutchins
//ID: HUT18001284

//I am going to use enums to make readability easier in what state the threat is currently in.
public enum ThreatState
{
    Penetrate = 1,
    Preparing = 2,
    Attacking = 4,
    Destroyed = 8,
}

//This will act as the main class for each threats/enemies, handling the movement and AI update logics.
public class ThreatAI : MonoBehaviour
{

    [SerializeField] protected MainGameScript main;

    [SerializeField] protected float Speed = 10.0f; //How fast can they move in X and Y.
    [SerializeField] protected int secondsToReach = 60; //How long should it take for them to close the distance?
    [SerializeField] protected int Health = 1; //How many times do they need to be hit in order to kill?
    public int DMG = 10; //How much damage to deal to the player?

    //Position isn't actually position... It is going to be latitude, longitude and distance from player.
    protected Vector3 position = Vector3.zero;
    protected Vector3 originalScale = new Vector3(1.0f, 1.0f, 1.0f);
    public ThreatState state;

    void Start()
    {
        //Automatically make them scale 0 for intro appearing.
        originalScale = transform.localScale;
        transform.localScale = Vector3.zero;

        position.z = main.spherePlayArea;
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        //Set the current status of this threat to Preparing, as in it just have infested the system.
        state = ThreatState.Penetrate;
    }

    // Update is called once per frame
    void Update()
    {

        //Rotate them to always be facing the CPU.
        transform.LookAt(main.transform.position);

        //Only allow the Update function to do positioning and AI once the threat have entered Attacking state.
        if (state == ThreatState.Attacking)
        {

            //Always clamp their position so that X and Y is between -180 to 180.
            //This will make figuring out where on the sphere the player is aiming at easier to calculate for spyware.
            while (position.x < -180.0f)
                position.x += 360.0f;

            while (position.x > 180.0f)
                position.x -= 360.0f;

            //If they go too far up or down, reset the Y component but flip their X component, flipping over to the otherside.
            if(position.y > 90.0f || position.y < -90.0f)
            {
                position.y = Mathf.Clamp(position.y, -90.0f, 90.0f);
                if (position.x <= 0.0f)
                    position.x += 180.0f;
                else
                    position.x -= 180.0f;
            }


            //Given the distance the play area extends, and the time the enemy takes to reach, we can calculate speed.
            position.z -= (main.spherePlayArea / (float)secondsToReach) * Time.deltaTime;

            //Convert XYZ to Spherical Coordinates.
            transform.localPosition = BitDefender.GlobalMathFunctions.LatLongToXYZ(position);

            //Carry out AI.
            AI();

            //See if they are close enough to attack.
            if ((transform.position - main.transform.position).magnitude <= 0.75f)
                main.Damage(gameObject);

        }
        else if(state == ThreatState.Penetrate)
        {
            //Carry out the PreAI, this may mostly be used for intro animations.
            PreAI();
        }else if(state == ThreatState.Destroyed)
        {
            //Carry out the AfterAI, this may mostly be used for death animations.
            AfterAI();
        }
    }

    protected virtual void PreAI()
    {
        //The actions AI it should do should be coded in here.
    }

    protected virtual void AI()
    {
        //The movements AI it should do should be coded in here.
    }

    protected virtual void AfterAI()
    {
        //The animations/actions to do when killed or destroyed, but for now I will provide a simple flattened animation to inherit.
        if (transform.localScale.y > 0.0f)
            transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(transform.localScale.x, 0.0f, transform.localScale.z), 750.0f * Time.deltaTime);
        else
            Destroy(gameObject);
    }

    //Set how much max health it will have.
    public void setHealth(int HP)
    {
        Health = HP;
    }

    //Allow setting positions of enemy.
    public void setPosition(Vector3 pos)
    {
        position = pos;
        transform.localPosition = BitDefender.GlobalMathFunctions.LatLongToXYZ(position);
    }

    //Simple damage once function.
    protected virtual void Damage()
    {
        //Only allow them to be damaged during their attacking phase.
        if (state == ThreatState.Attacking)
        {
            Health--;
            if (Health < 1)
                state = ThreatState.Destroyed; //The virus have been killed.
        }
    }

    //This will find out if there is a spyware present in the system.
    protected bool isSpyPresent()
    {
        bool isSpy = false;
        for (int i = main.Threats.Count - 1; i > -1; i--)
        {

            if (main.Threats[i] == null)
                continue; //Skip this empty one for now.

            //If this threat object contains Spyware in its name, check if it not in destroyed state, elsewise continue.
            if (main.Threats[i].name.Contains("Spyware") && main.Threats[i].GetComponent<ThreatAI>().state != ThreatState.Destroyed)
            {
                //Spyware is present, stop loop and set to true.
                isSpy = true;
                break;
            }
        }
        return isSpy;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Projectile"))
        {
            Destroy(other.gameObject);
            Damage();
        }
    }

}
