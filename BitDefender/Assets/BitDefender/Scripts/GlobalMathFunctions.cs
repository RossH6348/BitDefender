using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Name: Ross Hutchins
//ID: HUT18001284

//This BitDefender namespace, storing all sorts of useful functions for other scripts.
//Such as converting a latitude/longitude/altitude to a XYZ vector and vice versa.
namespace BitDefender
{
    class GlobalMathFunctions
    {
        //This function given a latitude, longitude and altitude (or just z...) will convert it to a XYZ direction vector.
        public static Vector3 LatLongToXYZ(Vector3 LatLongAlt)
        {

            //Converting them to radians, since I am storing them as degrees for easier readability.
            float LatRad = LatLongAlt.x * Mathf.Deg2Rad;
            float LongRad = LatLongAlt.y * Mathf.Deg2Rad;

            //This turns a given latitude and longitude into a direction vector.
            Vector3 spherePos = new Vector3(
                Mathf.Sin(LatRad) * Mathf.Cos(LongRad),
                Mathf.Sin(LongRad),
                Mathf.Cos(LatRad) * Mathf.Cos(LongRad)
            );

            return spherePos * LatLongAlt.z; //Multiply this by the altitude, known as Z/distance to player.
        }

        //This function will take an existing XYZ direction vector, and convert it back into latitude, longitude and altitude.
        public static Vector3 XYZToLatLong(Vector3 vector)
        {

            //I am going to assume that the altitude is the magnitude of this vector.
            float altitude = vector.magnitude;

            //Convert the XYZ vector into a LatLong and Alt.
            Vector3 LatLongAlt = new Vector3(
                Mathf.Atan2(vector.x, vector.z) * Mathf.Rad2Deg,
                Mathf.Asin(vector.y / altitude) * Mathf.Rad2Deg,
                altitude
            );

            return LatLongAlt;
        }

        public static Vector3 RaySphere(Transform ray, Vector3 spherePos, float radius)
        {
            //If it doesn't intersect, at least make the intersection point the rayOrigin.
            Vector3 hit = ray.position;

            //Produce a vector going from ray position to the sphere position.
            Vector3 toSphere = spherePos - ray.position;

            //Dot product to see how much of that vector projects onto the ray's direction vector.
            float dot = Vector3.Dot(toSphere, ray.forward); //Using the forward vector of a transform as the ray direction.

            //Get the projected point.
            Vector3 point = ray.position + (ray.forward * dot);

            //Squaring the radius value, just to optimize this up a bit by reducing the use of square root by one.
            radius *= radius;

            //Calculate the squared distance from that projected point to the sphere position.
            float distance = (spherePos - point).sqrMagnitude;
            if (distance <= radius)
            {
                //The distance is less or equal to the radius squared, it has intersected!
                distance = Mathf.Sqrt(radius - distance);
                float t0 = dot - distance, t1 = dot + distance;

                //Now since we are inside the sphere, we want the value that is the largest out of the two hit points.
                return (t0 > t1 ? ray.position + ray.forward * t0 : ray.position + ray.forward * t1);
            }
            return hit;
        }

        public static Vector3 RaySphere(Vector3 rayPos, Vector3 rayDir, Vector3 spherePos, float radius)
        {
            Vector3 hit = rayPos;

            Vector3 toSphere = spherePos - rayPos;

            float dot = Vector3.Dot(toSphere, rayDir);
            Vector3 point = rayPos + (rayDir * dot);

            radius *= radius;
            float distance = (spherePos - point).sqrMagnitude;
            if (distance <= radius)
            {
                distance = Mathf.Sqrt(radius - distance);
                float t0 = dot - distance, t1 = dot + distance;
                return (t0 > t1 ? rayPos + rayDir * t0 : rayPos + rayDir * t1);
            }
            return hit;
        }

        //This function will project enemy position onto the aimPoint's imaginary plane, to figure out the direction vector needed
        //To push them AWAY from the aim point.
        public static Vector2 pushForce(Transform enemy, Transform originPoint)
        {

            Vector2 push = Vector2.zero;

            //Get vector from the origin to the enemy.
            Vector3 toEnemy = enemy.position - originPoint.position;

            //Dot product that vector with the up and right vectors of origin (intersection.)
            float projectX = Vector3.Dot(originPoint.right, toEnemy);
            float projectY = Vector3.Dot(originPoint.up, toEnemy);

            //With that dot product, we can project the enemy's position in both X and Y on this intersection's imaginary plane.
            Vector3 projectedX = originPoint.position + (originPoint.right * projectX);
            Vector3 projectedY = originPoint.position + (originPoint.up * projectY);

            //With it properly projected onto this imaginary plane, we can subtract the origin point and do dot product with its right and up vectors to see the final results.
            push.x = -Vector3.Dot(projectedX - originPoint.position, originPoint.right);
            push.y = Vector3.Dot(projectedY - originPoint.position, originPoint.up);
            push = push.normalized;

            //Now the player's aim may be on the opposite end of straight up or down, perform a dot product if we need to change that.
            push.y = (Vector3.Dot(originPoint.up, enemy.up) >= 0.0f ? push.y : -push.y);

            return push;
        }

    }
}