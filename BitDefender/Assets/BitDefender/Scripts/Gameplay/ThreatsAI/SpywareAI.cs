using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Name: Ross Hutchins
//ID: HUT18001284

public class SpywareAI : ThreatAI
{
    private bool coroutineStarted = false;
    protected override void PreAI()
    {
        if (transform.localScale.magnitude <= originalScale.magnitude)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, originalScale, 334.0f * Time.deltaTime);
            if (transform.localScale.magnitude >= originalScale.magnitude && !coroutineStarted)
            {
                coroutineStarted = true;
                StartCoroutine("HideSpyware");
            }
        }
        base.PreAI();
    }

    protected override void AI()
    {
        base.AI();
        //The spyware is going be static, hiding until the player aims near them, revealing themselves.
        position.z = main.spherePlayArea;

        //Calculate to see if the player's aim is within 20 degrees of the enemy's location.
        float dot = Vector3.Dot(transform.localPosition.normalized, main.aimPoint.localPosition.normalized);
        if (dot >= Mathf.Cos(20.0f * Mathf.Deg2Rad) || position.z <= 2.5f)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, originalScale, 500.0f * Time.deltaTime);
        }
        else
        {
            //Only shrink to a small size, not completely invisible, but should be a bit hard to find with the chaos ensued.
            transform.localScale = Vector3.MoveTowards(transform.localScale, originalScale * 0.2f, 500.0f * Time.deltaTime);
        }
    }

    IEnumerator HideSpyware()
    {
        yield return new WaitForSeconds(1.5f);
        position.x = Random.Range(-180.0f, 180.0f);
        position.y = Random.Range(-10.0f, 60.0f);
        state = ThreatState.Attacking;
    }
}
