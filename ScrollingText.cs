using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/***************************************************************
 * ScrollingText
 * Essentially the title screen controller that starts the game.
 * The scrolling text aspect scrolls 3-D text away from the camera.
 * *************************************************************/
public class ScrollingText : MonoBehaviour
{
    public Vector3 velocity;
    public TextMesh score, players;

    private List<int> playerNums;

    void Start()
    {
        GetComponent<Rigidbody>().velocity = velocity;
        score.text = "Highscore: " + PlayerPrefs.GetInt("HIGHSCORE");
        playerNums = new List<int>();
    }

    private void Update()
    {
        // Check for input from all of the players
        for (int i = 0; i < 5; ++i)
        {
            if (Input.GetButtonDown("join" + i.ToString()))
            {
                AddPlayer(i);
            }

            if (Input.GetButtonDown("boost" + i.ToString()))
            {
                RemovePlayer(i);
            }
        }

        if (Input.GetButtonDown("start"))
        {
            StartGame();
        }
    }

    /// <summary>
    /// Join a controller to the game.
    /// </summary>
    /// <param name="playerNum"></param>
    void AddPlayer(int playerNum)
    {
        if (!playerNums.Contains(playerNum) && playerNums.Count < 4)
        {
            playerNums.Add(playerNum);
            players.text = "Players: " + playerNums.Count.ToString();
        }
    }

    /// <summary>
    /// Remove a controller from the game.
    /// </summary>
    /// <param name="playerNum"></param>
    void RemovePlayer(int playerNum)
    {
        if (playerNums.Contains(playerNum))
        {
            playerNums.Remove(playerNum);
            players.text = "Players: " + playerNums.Count.ToString();
        }
    }

    /// <summary>
    /// The player string stored in player prefs stores which controller
    /// number is stored with which screen (the order that the players joined.
    /// For example, if gamepad 1 (controller 1) joined, then the keyboard and
    /// mouse joined next (controller 0) joined, then gamepad 2 (controller 2)
    /// joined, the playerString = "102". So then gamepad 1 would be the top left
    /// screen, mouse and keyboard would be the top right screen, and gamepad 2
    /// would be the bottom left screen.
    /// </summary>
    void StartGame()
    {
        // Verify there is at least 1 player
        if (playerNums.Count >= 1)
        {
            // Assign the players/controllers to their individual screens
            // and start the game
            var playerString = string.Empty;
            for (int i = 0; i < playerNums.Count; ++i)
            {
                playerString += playerNums[i];
            }

            PlayerPrefs.SetString(Constants.PREF_PLAYERS, playerString);
            SceneManager.LoadScene(Constants.GAME_SCREEN);
        }
    }
}
