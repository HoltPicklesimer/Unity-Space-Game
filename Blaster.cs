using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

/***********************************************************
 * Blaster
 * The bullets that are shot by players and enemies. They
 * can collide with players and asteroids.
 * *********************************************************/
public class Blaster : MonoBehaviour, IPooledObject
{
    public GameObject owner; // Who shot the blaster

    private ObjectPooler objectPooler;
    private ScoreKeeper scoreKeeper;
    private SoundPlayer soundPlayer;

    public void OnObjectSpawn()
    {
        objectPooler = ObjectPooler.Instance;
        scoreKeeper = ScoreKeeper.Instance;
        soundPlayer = SoundPlayer.Instance;
        soundPlayer.PlayFireSound();
    }

    private void OnTriggerEnter(Collider other)
    {
        // If this blaster was shot by someone else
        if (other.gameObject != owner)
        {
            // Check if the collided object should get hit
            var iBody = other.gameObject.GetComponent<IBody>();
            if (iBody != null)
            {
                iBody.GetHit(owner);

                // Spawn an explosion
                var explosion = objectPooler.SpawnFromPool(Constants.EXPLOSION_TAG, transform.position, Quaternion.identity);
                soundPlayer.PlayExplodeSound();
                explosion.GetComponent<ParticleSystem>().Play();

                // Add points if this is from a player
                if (owner != null)
                {
                    var player = owner.GetComponent<Player>();
                    if (player != null)
                    {
                        scoreKeeper.AddPoints(player.playerNum);
                    }
                }
            }

            gameObject.SetActive(false);
        }
    }
}
