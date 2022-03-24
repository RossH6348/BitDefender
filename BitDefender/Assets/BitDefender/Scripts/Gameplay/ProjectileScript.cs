using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Name: Ross Hutchins
//ID: HUT18001284

public class ProjectileScript : MonoBehaviour
{

    [SerializeField] private float lifeTime = 10.0f; //How long should the projectile live for?

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0.0f)
            Destroy(gameObject);

    }

    //Delete prematurely if it hits something, like the CPU ground.
    private void OnCollisionEnter(Collision collision)
    {
        if(!collision.collider.tag.Equals("Threat"))
            Destroy(gameObject);
    }

}
