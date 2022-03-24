using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

 namespace Shunt.VR.Samples
{

    [RequireComponent(typeof(Interactable))]
    [RequireComponent(typeof(Throwable))]
    public class SampleGun : MonoBehaviour
    {

        public SteamVR_Action_Boolean fireAction;

        protected SteamVR_Input_Sources hand;
        protected Interactable interactable;
        [SerializeField] protected float timeBetweenShots = 1;
        protected float timeTilNextShot = 0;
        [SerializeField] protected Transform barrel;
        [SerializeField] protected float force;

        // Start is called before the first frame update
        protected void Start()
        {
            interactable = GetComponent<Interactable>();
        }

        // Update is called once per frame
        void Update()
        {
            bool actFire;

            if (interactable.attachedToHand)
            {
                //get the hand attached to the interactable
                hand = interactable.attachedToHand.handType;
                //get teh state of the action prescribed for fire above
                actFire = fireAction.GetState(hand);
                Debug.Log("Fire Action" + actFire);
                if (actFire)
                {
                    if (Time.time >= timeTilNextShot)
                    {
                        Debug.Log("Ready to fire!");
                        GameObject bullet = ObjectPool.SharedInstance.GetPooledObject();
                        if (bullet != null)
                        {
                            Debug.Log("PEW!");
                            bullet.transform.position = barrel.transform.position;
                            bullet.transform.rotation = barrel.transform.rotation;
                            bullet.GetComponent<Rigidbody>().velocity = Vector3.zero;

                            bullet.SetActive(true);
                            bullet.GetComponent<Rigidbody>().AddForce(force * barrel.transform.forward);
                            timeTilNextShot = Time.time + timeBetweenShots;
                        }
                    }
                }
            }
        }
    }
}