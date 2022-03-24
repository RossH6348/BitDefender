using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shunt.VR.Samples
{
    public class AnotherSampleGun : SampleGun
    {
        [SerializeField] ParticleSystem particles;

        // Start is called before the first frame update
        void Start()
        {
            //Debug.Log(SampleGun.force);
            Debug.Log(timeTilNextShot);
            base.Start();
        }

        // Update is called once per frame
        void Update()
        {
            bool actFire;

            if (interactable.attachedToHand)
            {
                //get the hand attached to the interactable
                hand = interactable.attachedToHand.handType;
                //get the state of the action prescribed for fire above
                actFire = fireAction.GetState(hand);
                Debug.Log("Fire Action" + actFire);

                if (actFire && !particles.isPlaying)
                {
                    particles.Play();
                }
                else if (!actFire && particles.isPlaying) 
                {
                    particles.Stop();
                }
            }
        }
    }
}
