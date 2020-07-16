using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***************************************************************
 * SoundPlayer
 * Global sound effects source.
 * *************************************************************/
public class SoundPlayer : MonoBehaviour
{
    public AudioClip boost, fire, explode;

    private AudioSource aud;

    #region Singleton

    public static SoundPlayer Instance;

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    void Start()
    {
        aud = GetComponent<AudioSource>();
    }

    public void PlayBoostSound()
    {
        if (!aud.isPlaying)
        {
            aud.clip = boost;
            aud.Play();
        }
    }

    public void PlayFireSound()
    {
        aud.clip = fire;
        aud.Play();
    }

    public void PlayExplodeSound()
    {
        aud.clip = explode;
        aud.Play();
    }
}
