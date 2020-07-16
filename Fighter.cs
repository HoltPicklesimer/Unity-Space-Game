using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/***************************************************************
 * Fighter
 * The enemy fighters that fly around the arena and try to shoot
 * the player. They shoot anything that line up in the direction
 * of their velocity.
 * *************************************************************/
public class Fighter : MonoBehaviour, IBody
{
    public float moveSpeed, maxSpeed, shootSpeed, fireCoolDown, turnTime;

    private Rigidbody rb;
    private Player[] players;
    private ObjectPooler objectPooler;
    private SoundPlayer soundPlayer;
    private float fireTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        players = FindObjectsOfType<Player>();
        objectPooler = ObjectPooler.Instance;
        soundPlayer = SoundPlayer.Instance;
    }

    void Update()
    {
        // Lock on to the nearest player
        var target = FindNearestPlayer();

        if (target != null)
        {
            // Turn toward the target
            var lookRotation = Quaternion.LookRotation(target.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * turnTime);

            // Drive Forward
            rb.AddForce(transform.forward * moveSpeed);
        }

        // Limit the speed
        if (rb.velocity.magnitude > maxSpeed)
            rb.velocity = rb.velocity.normalized * maxSpeed;

        // Cool down for firing blasters
        fireTimer = Mathf.Max(0, fireTimer - Time.deltaTime);

        // Shoot blaster if the player is raycasted and the firetimer is reset
        RaycastHit raycastHitInfo;
        if (fireTimer == 0 && Physics.Raycast(transform.position, transform.forward, out raycastHitInfo))
        {
            var raycastHitObject = raycastHitInfo.collider.gameObject;
            if (raycastHitObject.GetComponent<IBody>() != null && raycastHitObject.GetComponent<Fighter>() == null)
            {
                Shoot(transform.position - transform.right * 3 + transform.up * 2);
                fireTimer = fireCoolDown;
            }
        }

        // Keep the fighter within bounds
        if (Vector3.Distance(transform.position, Vector3.zero) > Constants.FIELD_RADIUS + 10)
        {
            transform.position = transform.position.normalized * (Constants.FIELD_RADIUS + 10);
        }
    }

    /// <summary>
    /// Finds the nearest player in the arena
    /// </summary>
    /// <returns></returns>
    Player FindNearestPlayer()
    {
        Player nearestPlayer = null;
        var closestDistance = Constants.FIELD_RADIUS * 2;

        foreach (var player in players)
        {
            // Ignore dead players
            if (!player.dead)
            {
                var currentDistance = Vector3.Distance(transform.position, player.transform.position);

                if (currentDistance < closestDistance)
                {
                    nearestPlayer = player;
                    closestDistance = currentDistance;
                }
            }
        }

        return nearestPlayer;
    }

    /// <summary>
    /// Fires a blaster/bullet.
    /// </summary>
    /// <param name="position"></param>
    void Shoot(Vector3 position)
    {
        // 90 degrees must be added to rotate the blaster so it is not vertical
        var rotation = transform.rotation * Quaternion.Euler(90, 0, 0);
        var bullet = objectPooler.SpawnFromPool(Constants.BULLET2_TAG, position, rotation);

        // Set the blaster velocity and claim the blaster
        bullet.GetComponent<Rigidbody>().velocity = transform.forward * shootSpeed;
        bullet.GetComponent<Blaster>().owner = gameObject;
    }

    public void GetHit(GameObject projectileOwner)
    {
        // if hit by the bullet and the object is not the owner, die
        if (projectileOwner != gameObject)
        {
            Die();
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        Die();
    }

    void Die()
    {
        // Spawn an explosion and despawn
        objectPooler.SpawnFromPool(Constants.EXPLOSION_TAG, transform.position, Quaternion.identity);
        soundPlayer.PlayExplodeSound();
        gameObject.SetActive(false);
    }
}
