using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class FireScript : MonoBehaviour
{

    public SteamVR_Action_Boolean fireAction;

    protected SteamVR_Input_Sources hand;
    protected Interactable interactable;

    [SerializeField] private Transform muzzle;
    [SerializeField] private GameObject projectileTemplate;
    [SerializeField] private float muzzleVelocity = 40.0f;

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
            if (fireAction.GetStateDown(hand)) //Check the input event of that hand's firing action, if it have been pressed, fire the pistol!
            {

                //Create a clone of this projectile.
                GameObject projectile = Instantiate(projectileTemplate);

                //Move the projectile to be at the end of the barrel.
                projectile.transform.position = muzzle.position;

                //Apply velocity vector using the muzzle's forward vector times velocity.
                projectile.GetComponent<Rigidbody>().velocity = muzzle.forward * muzzleVelocity;

                //Enable it.
                projectile.SetActive(true);

            }
        }
    }
}
