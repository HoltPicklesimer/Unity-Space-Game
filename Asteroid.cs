using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***************************************************************
 * Asteroid
 * The asteroid class that if shot, can split up into more asteroids
 * or is destroyed. Players score points by shooting asteroids.
 * *************************************************************/
public class Asteroid : MonoBehaviour, IBody
{
    private Rigidbody rb;
    private ObjectPooler objectPooler;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        objectPooler = ObjectPooler.Instance;
    }

    void Update()
    {
        // Keep the asteroid within bounds
        if (Vector3.Distance(transform.position, Vector3.zero) > Constants.FIELD_RADIUS + transform.localScale.x)
        {
            // Put the asteroid back at the radius of the playing field
            transform.position = transform.position.normalized * (Constants.FIELD_RADIUS + transform.localScale.x);

            // Set the velocity to make it bounce toward the center
            rb.velocity = -transform.position.normalized * rb.velocity.magnitude;
        }
    }

    public void GetHit(GameObject projectileOwner)
    {
        // Split into two Asteroids or get destroyed if small enough
        if (transform.localScale.x > Constants.SMALL_ASTEROID_SIZE)
        {
            // Make the asteroids half the size and set them at opposite sides within the old asteroid's space
            var offset = Random.insideUnitSphere * transform.localScale.x / 2;

            // Spawn another asteroid for the first broken piece
            var newAsteroid = objectPooler.SpawnFromPool(Constants.ASTEROID_TAG, transform.position + offset, transform.rotation);
            newAsteroid.transform.localScale = transform.localScale / 2;

            // Reuse the existing asteroid for the other broken piece
            transform.localScale /= 2;
            transform.position -= offset;
        }
        else
        {
            // Destroy the asteroid since it is too small
            gameObject.SetActive(false);
        }    
    }
}
