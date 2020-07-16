using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/***************************************************************
 * ScoreKeeper
 * Keeps track of score.
 * *************************************************************/
public class ScoreKeeper : MonoBehaviour
{
    public int[] scores;
    public Text[] scoreText;
    public List<int> playerNums;

    #region Singleton

    public static ScoreKeeper Instance;

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    private void Start()
    {
        // Initialize the scores
        scores = new int[4];
        playerNums = new List<int>();

        // Get the players and match them to their screens
        var players = PlayerPrefs.GetString(Constants.PREF_PLAYERS);
        for (int i = 0; i < players.Length; ++i)
        {
            playerNums.Add((int)char.GetNumericValue(players[i]));
        }

        // hide unused score text
        for (int i = playerNums.Count; i < scoreText.Length; ++i)
        {
            scoreText[i].enabled = false;
        }
    }

    /// <summary>
    /// Get the points of a player using their playerNum.
    /// </summary>
    /// <param name="playerNumIndex"></param>
    /// <returns></returns>
    public int GetPoints(int playerNumIndex)
    {
        var i = playerNums.IndexOf(playerNumIndex);
        return scores[i];
    }

    /// <summary>
    /// Add a point for a player using their playerNum.
    /// </summary>
    /// <param name="playerNumIndex"></param>
    public void AddPoints(int playerNumIndex)
    {
        var i = playerNums.IndexOf(playerNumIndex);
        scores[i]++;
        scoreText[i].text = "Score: " + scores[i];
    }
}
