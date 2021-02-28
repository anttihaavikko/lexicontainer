using System;
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
    
    private void Start()
    {
        scoreManager.onLoaded += ScoresLoaded;
        scoreManager.LoadLeaderBoards(page);

        wotd.text = dict.RandomWord();
    }

    private void Update()
    {
        if (Application.isEditor && Input.GetKeyDown(KeyCode.D))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetString("Identifier", "a09c0a94-0f76-42e7-94da-198400d86fe8");
            SceneChanger.Instance.ChangeScene("Start");
        }
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
