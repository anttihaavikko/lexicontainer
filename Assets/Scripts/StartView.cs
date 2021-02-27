using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartView : MonoBehaviour
{
    public ScoreManager scoreManager;
    public TMP_Text leaderboardNames, leaderboardScores;
    public TMP_Text wotd;
    public WordDictionary dict;

    private int page;
    
    // Start is called before the first frame update
    void Start()
    {
        scoreManager.onLoaded += ScoresLoaded;
        scoreManager.LoadLeaderBoards(page);

        wotd.text = dict.RandomWord();
    }

    private void ScoresLoaded()
    {
        leaderboardNames.text = scoreManager.leaderBoardPositionsString;
        leaderboardScores.text = scoreManager.leaderBoardScoresString;
    }

    public void ChangePage(int direction)
    {
        if (page + direction < 0 || direction > 0 && scoreManager.endReached) return;
        page = Mathf.Max(page + direction, 0);
        scoreManager.LoadLeaderBoards(page);
    } 

    public void StartGame()
    {
        var scene = PlayerPrefs.HasKey("PlayerName") ? "Main" : "Name";
        SceneChanger.Instance.ChangeScene(scene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
