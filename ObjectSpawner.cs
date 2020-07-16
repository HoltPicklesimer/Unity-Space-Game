using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***************************************************************
 * ObjectSpawner
 * Manages/Spawns the objects from the object pool.
 * *************************************************************/
public class ObjectSpawner : MonoBehaviour
{
    public float minSpawnSpeed, maxSpawnSpeed, spawnRate;
    public Player player;

    private ObjectPooler objectPooler;
    private ScoreKeeper scoreKeeper;

    void Start()
    {
        objectPooler = ObjectPooler.Instance;
        scoreKeeper = ScoreKeeper.Instance;
        StartCoroutine(SpawnObjects());
        SpawnPlayers();
    }

    IEnumerator SpawnObjects()
    {
        // Execute on the spawnRate incremented timer
        yield return new WaitForSeconds(spawnRate);
        StartCoroutine(SpawnObjects());

        // Difficulty is based on the highest score of the players
        var maxScore = 0;
        foreach (var score in scoreKeeper.scores)
        {
            if (score > maxScore)
                maxScore = score;
        }

        // Calculate the appropriate number of asteroids and fighters to add
        var numAsteroids = Mathf.Min(maxScore, objectPooler.poolDictionary["Asteroid"].Count / Constants.ASTEROID_LIMITER);
        var numFighters = Mathf.Min(maxScore / 5 - 2, objectPooler.poolDictionary["Fighter"].Count);

        var asteroidsToAdd = numAsteroids - FindObjectsOfType<Asteroid>().Length;
        var fightersToAdd = numFighters - FindObjectsOfType<Fighter>().Length;

        for (int i = 0; i < asteroidsToAdd; ++i)
        {
            SpawnAsteroid();
        }

        for (int i = 0; i < fightersToAdd; ++i)
        {
            SpawnFighter();
        }
    }

    void SpawnAsteroid()
    {
        // Spawn an asteroid with varying size, position, rotation
        var size = Random.Range(5f, 100f);
        var position = Random.insideUnitSphere.normalized * (Constants.FIELD_RADIUS + size);
        var rotation = Quaternion.Euler(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f));
        var asteroid = objectPooler.SpawnFromPool("Asteroid", position, rotation);
        asteroid.transform.localScale = size * Vector3.one;

        // Add a varying force to the asteroid
        var force = Random.insideUnitSphere.normalized * Random.Range(minSpawnSpeed, maxSpawnSpeed);
        var forcePos = Random.insideUnitSphere * asteroid.transform.localScale.x;
        asteroid.GetComponent<Rigidbody>().AddForceAtPosition(force, forcePos);
    }

    void SpawnFighter()
    {
        // Spawn a fighter with varying position and rotation
        var position = Random.insideUnitSphere.normalized * (Constants.FIELD_RADIUS + 10);
        var rotation = Quaternion.Euler(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f));
        objectPooler.SpawnFromPool("Fighter", position, rotation);
    }

    /// <summary>
    /// Spawn the players into the game. Up to four players supported.
    /// </summary>
    void SpawnPlayers()
    {
        // Get the number of players
        var players = PlayerPrefs.GetString(Constants.PREF_PLAYERS);
        var numPlayers = players.Length;

        for (int i = 0; i < numPlayers; ++i)
        {
            var playerNum = (int)char.GetNumericValue(players[i]);
            var newPlayer = Instantiate(player);
            newPlayer.playerNum = playerNum;

            float posX = 0, posY = 0, width = 1, height = 1;
            var dist = Constants.FIELD_RADIUS;

            // Apply the different scenarios of player placement and camera size and position
            switch (numPlayers)
            {
                default:
                case 1:
                    break;
                case 2:
                    posX = (i % 2) * 0.5f;
                    width = 0.5f;
                    newPlayer.transform.position = new Vector3(-dist / 2 + dist * (i % 2), 0, -dist / 2 + dist * (i / 2));
                    break;
                case 3:
                case 4:
                    posX = (i % 2) * 0.5f;
                    posY = (i < 2) ? 0.5f : 0;
                    width = 0.5f;
                    height = 0.5f;
                    newPlayer.transform.position = new Vector3(-dist / 2 + dist * (i % 2), 0, -dist / 2 + dist * (i / 2));
                    break;
            }

            var camera = newPlayer.GetComponentInChildren<Camera>();
            camera.rect = new Rect(posX, posY, width, height);
        }
    }
}
