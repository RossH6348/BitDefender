using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Name: Ross Hutchins
//ID: HUT18001284

public class VirusAI : ThreatAI
{

    public Vector3 targetPos = Vector3.zero;

    protected override void PreAI()
    {
        if (transform.localScale.magnitude >= originalScale.magnitude)
        {
            targetPos = new Vector3(Random.Range(-180.0f, 180.0f), Random.Range(-10.0f, 70.0f), position.z);
            state = ThreatState.Attacking;
        }
        else
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(100.0f, 100.0f, 100.0f), 500.0f * Time.deltaTime);
        }
        base.PreAI();
    }

    protected override void AI()
    {

        base.AI();

        //Best to update this value to position's z, as they are constantly moving toward player.
        targetPos.z = position.z;

        if ((targetPos - position).magnitude <= 0.1f)
        {
            targetPos = new Vector3(Random.Range(-180.0f, 180.0f), Random.Range(-10.0f, 70.0f), position.z);
        }
        else
        {
            //Modifier in how fast should the virus move, using this for spyware.
            float Modifier = 1.0f;

            //Calculate to see if the player's aim is within 20 degrees of the enemy's location.
            float dot = Vector3.Dot(transform.localPosition.normalized, main.aimPoint.localPosition.normalized);
            if (isSpyPresent() && dot >= Mathf.Cos(20.0f * Mathf.Deg2Rad))
            {
                //Yes it is, proceed to move away from their aim point at 5x speed.
                Modifier = 5.0f;
                Vector2 offset = BitDefender.GlobalMathFunctions.pushForce(transform, main.aimPoint);
                targetPos.x = position.x + offset.x;
                targetPos.y = position.y + offset.y;
            }

            //Make the Virus move around.
            position = Vector3.MoveTowards(position, targetPos, Modifier * Speed * Time.deltaTime);
        }

    }
}
