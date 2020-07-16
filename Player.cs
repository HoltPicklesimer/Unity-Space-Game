using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/***************************************************************
 * Player
 * A ship that is controlled by a player.
 * *************************************************************/
public class Player : MonoBehaviour, IBody
{
    // The player number represents the controller number.
    // 4 different controllers and keyboard and mouse are supported
    // with 4 different players at once.
    public int playerNum;
    public float forwardSpeed, backwardSpeed, maxSpeed, lightSpeed,
        tiltSpeed, rotateSpeed, lookSpeed, shootSpeed, fireCoolDown;
    public bool useLightSpeed, dead;
    public GameObject fieldObject;

    private Rigidbody rb;
    private List<Light> lights;
    private ObjectPooler objectPooler;
    private ScoreKeeper scoreKeeper;
    private SoundPlayer soundPlayer;
    private Collider col;
    private float fireTimer, lightIntensity, lightRange;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lights = GetComponentsInChildren<Light>().ToList();
        objectPooler = ObjectPooler.Instance;
        scoreKeeper = ScoreKeeper.Instance;
        soundPlayer = SoundPlayer.Instance;
        col = GetComponent<Collider>();
    }

    void Update()
    {
        if (!dead)
            Movement();
    }

    void Movement()
    {
        // These are abstracted from the Unity Input Manager.
        var moveX = "moveX" + playerNum.ToString();
        var moveY = "moveY" + playerNum.ToString();
        var lookX = "lookX" + playerNum.ToString();
        var lookY = "lookY" + playerNum.ToString();
        var fire = "fire" + playerNum.ToString();
        var boost = "boost" + playerNum.ToString();

        // Sets the brightness of the lights
        lightIntensity = 3;
        lightRange = 3;

        // Drive Forward/Backward
        if (Input.GetAxis(moveY) > 0)
        {
            rb.AddForce(transform.forward * Input.GetAxisRaw(moveY) * forwardSpeed);
            // Add to Light Intensity and Range
            lightIntensity = 10;
            lightRange = 5;
        }
        else if (Input.GetAxis(moveY) < 0)
        {
            rb.AddForce(transform.forward * Input.GetAxisRaw(moveY) * backwardSpeed);
        }

        // Limit the speed
        if (rb.velocity.magnitude > maxSpeed)
            rb.velocity = rb.velocity.normalized * maxSpeed;

        // Rotate Left/Right
        rb.AddForce(transform.right * Input.GetAxis(moveX) * tiltSpeed * Time.deltaTime);
        transform.Rotate(0, 0, -Input.GetAxis(moveX) * rotateSpeed * Time.deltaTime);

        // Look Up/Down/Left/Right
        transform.Rotate(-Input.GetAxis(lookY) * lookSpeed, Input.GetAxis(lookX) * lookSpeed, 0);

        // Shoot blasters when the fire button is pressed
        if (Input.GetButtonDown(fire) || Input.GetAxisRaw(fire) != 0)
        {
            Cursor.lockState = CursorLockMode.Locked;

            if (!useLightSpeed && fireTimer == 0)
            {
                ShootBullet(transform.position - transform.right * 3 + transform.up * 2);
                ShootBullet(transform.position + transform.right * 3 + transform.up * 2);
                fireTimer = fireCoolDown;
            }
        }

        // Boost
        useLightSpeed = Input.GetButton(boost) || Input.GetAxisRaw(boost) != 0;

        // Light speed increases the ship speed dramatically and makes the lights bright
        if (useLightSpeed)
        {
            soundPlayer.PlayBoostSound();
            rb.velocity = transform.forward * lightSpeed;
            lightIntensity = 100;
            lightRange = 10;
        }

        // Apply Light Intensity
        foreach (var light in lights)
        {
            light.intensity = lightIntensity;
            light.range = lightRange;
        }

        // Shooting cooldown timer
        fireTimer = Mathf.Max(0, fireTimer - Time.deltaTime);

        // Keep the player within bounds
        if (Vector3.Distance(transform.position, Vector3.zero) > Constants.FIELD_RADIUS)
        {
            transform.position = transform.position.normalized * Constants.FIELD_RADIUS;
        }

        // Show the player is nearly out of bounds
        if (Vector3.Distance(transform.position, Vector3.zero) > Constants.FIELD_RADIUS - 1)
        {
            // The field object is a 2-D force field-like square that visually shows the player
            // is out of bounds
            if (!fieldObject.activeSelf)
                fieldObject.SetActive(true);

            fieldObject.transform.forward = Vector3.zero - transform.position;
            fieldObject.transform.position = transform.position.normalized * (Constants.FIELD_RADIUS + 5);
        }
        else
        {
            fieldObject.SetActive(false);
        }
    }

    void ShootBullet(Vector3 position)
    {
        // 90 degrees must be added to rotate the blaster so it is not vertical
        var rotation = transform.rotation * Quaternion.Euler(90, 0, 0);
        var bullet = objectPooler.SpawnFromPool(Constants.BULLET1_TAG, position, rotation);

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

    void Die()
    {
        // Spawn an explosion
        objectPooler.SpawnFromPool(Constants.EXPLOSION_TAG, transform.position, Quaternion.identity);
        soundPlayer.PlayExplodeSound();

        // Disable the object and child objects
        dead = true;
        col.enabled = false;
        rb.velocity = Vector3.zero;
        foreach (Transform child in transform)
        {
            // Disable all objects except the camera
            if (!child.gameObject.GetComponent<Cam>() && !child.gameObject.GetComponent<Canvas>())
                child.gameObject.SetActive(false);
        }

        CheckForGameover();
    }

    public void OnCollisionEnter(Collision collision)
    {
        Die();
    }

    private void CheckForGameover()
    {
        var players = FindObjectsOfType<Player>();
        var gameover = true;
        
        // if all players are dead, the game is over
        foreach (var player in players)
        {
            if (!player.dead)
            {
                gameover = false;
            }
        }

        if (gameover)
        {
            StartCoroutine(Gameover());
        }
    }

    IEnumerator Gameover ()
    {
        yield return new WaitForSeconds(1);
        var scores = scoreKeeper.scores;
        for (int i = 0; i < scores.Length; ++i)
        {
            // Save the scores to use in the gameover screen
            PlayerPrefs.SetInt(Constants.PREF_SCORE + i.ToString(), scores[i]);
            SceneManager.LoadScene(Constants.GAMEOVER_SCREEN);
        }
    }
}
