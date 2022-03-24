using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

//Name: Ross Hutchins
//ID: HUT18001284

public class FireScript : MonoBehaviour
{

    public SteamVR_Action_Boolean fireAction;

    protected SteamVR_Input_Sources hand;
    protected Interactable interactable;

    [SerializeField] private Transform muzzle;
    [SerializeField] private GameObject projectileTemplate;
    [SerializeField] private float muzzleVelocity = 20.0f;
    [SerializeField] private float fireRate = 5.0f;
    private float lastFire = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<Interactable>();
    }

    // Update is called once per frame
    void Update()
    {

        if (interactable.attachedToHand) //Check if this gun is attached to a hand.
        {
            hand = interactable.attachedToHand.handType; //Get the hand it is attached to currently.
            if (fireAction.GetState(hand) && Time.realtimeSinceStartup - lastFire >= (1.0f / fireRate)) //Check the input event of that hand's firing action, and enough time have passed, then fire.
            {

                //Create a clone of this projectile.
                GameObject projectile = Instantiate(projectileTemplate);

                //Move the projectile to be at the end of the barrel.
                projectile.transform.position = muzzle.position;
                projectile.transform.LookAt(projectile.transform.position + muzzle.forward);

                //Apply velocity vector using the muzzle's forward vector times velocity.
                projectile.GetComponent<Rigidbody>().velocity = muzzle.forward * muzzleVelocity;

                lastFire = Time.realtimeSinceStartup;

                //Enable it.
                projectile.SetActive(true);

            }
        }
    }
}
