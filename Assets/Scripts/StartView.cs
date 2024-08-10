using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class StartView : MonoBehaviour
{
    public ScoreManager scoreManager;
    public TMP_Text leaderboardNames, leaderboardScores;
    public TMP_Text wotd;
    public WordDictionary dict;
    [SerializeField] private TMP_Text modeDescription;
    [SerializeField] private List<ModeToggle> modeToggles;
    [SerializeField] private TMP_Text leaderboardTitle;
    [SerializeField] private GameObject dayNext, dayPrev;

    private int page;
    
    private void Start()
    {
        scoreManager.onLoaded += ScoresLoaded;
        // scoreManager.LoadLeaderBoards(page);
        wotd.text = $"Word of the day is <size=5>{dict.RandomWord().ToUpper()}</size>!";
        modeToggles[(int)ModeToggle.GetMode()].Select();
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

    public void SelectMode()
    {
        // Manager.Instance.Day = DateTime.Today;
        Setup();
    }

    private void Setup()
    {
        page = 0;
        var mode = ModeToggle.GetMode();
        scoreManager.ChangeGame(ModeToggle.GetLeaderboard(mode, Manager.Instance.Day), true);
        modeDescription.text = ModeToggle.GetDescription(mode);
        Manager.Instance.Seed = mode == GameMode.Daily ? ModeToggle.GetSeed(Manager.Instance.Day) : Environment.TickCount;
        leaderboardTitle.text = mode == GameMode.Daily ? $"LEADERBOARDS <size=25>({Manager.Instance.DailyString})</size>" : "LEADERBOARDS";
        dayNext.SetActive(mode == GameMode.Daily);
        dayPrev.SetActive(mode == GameMode.Daily);
    }

    public void ChangeDay(int dir)
    {
        if (dir > 0 && Manager.Instance.Day >= DateTime.Today) return; 
        Manager.Instance.Day = Manager.Instance.Day.AddDays(dir);
        Setup();
    }
}
