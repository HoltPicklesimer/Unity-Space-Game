using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/***************************************************************
 * Results
 * The controller for the Gameover screen.
 * *************************************************************/
public class Results : MonoBehaviour
{
    public TextMesh highscore, names, scores, winner;

    void Start()
    {
        // The player names represent the player screens
        // Top Left - Player 1, Top Right - Player 2, Bottom Left - Player 3, Bottom Right - Player 4
        var playerNames = new string[] { "Top Left", "Top Right", "Bottom Left", "Bottom Right" };
        var playerScores = new List<int>();

        // Display the names of the screens
        var players = PlayerPrefs.GetString(Constants.PREF_PLAYERS);
        for (int i = 0; i < players.Length; ++i)
        {
            playerScores.Add(PlayerPrefs.GetInt(Constants.PREF_SCORE + i));
            names.text += playerNames[i] + ":\n";
        }

        // Get the top score and winning player
        var topScore = -1;
        var winningPlayer = -1;
        for (int i = 0; i < playerScores.Count; ++i)
        {
            scores.text += playerScores[i] + "\n";

            if (playerScores[i] > topScore)
            {
                topScore = playerScores[i];
                winningPlayer = i;
            }
        }

        // Check if the high score was beat, and add to the text if it was
        var additionalText = "";
        if (topScore > PlayerPrefs.GetInt(Constants.PREF_HIGHSCORE))
        {
            additionalText = playerNames[winningPlayer] +
                " beat the high score!\nThe high score has changed from " +
                PlayerPrefs.GetInt(Constants.PREF_HIGHSCORE) + " to " + topScore + ".";
            PlayerPrefs.SetInt(Constants.PREF_HIGHSCORE, topScore);
        }

        // Display the highscore
        highscore.text = "High Score: " + PlayerPrefs.GetInt(Constants.PREF_HIGHSCORE).ToString();

        // Display the winner if multiple players are playing
        if (playerScores.Count != 1)
        {
            winner.text = playerNames[winningPlayer] + " won the game!";
        }

        // Add that the highscore was beat if it was beat
        winner.text += additionalText;
    }

    void Update()
    {
        // Go back to the title screen
        if (Input.GetButtonDown("start"))
        {
            SceneManager.LoadScene(Constants.TITLE_SCREEN);
        }
    }
}
