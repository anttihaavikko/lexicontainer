using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] private CanvasScaler canvasScaler;
    [SerializeField] private Transform nonCanvasElems;
    [SerializeField] private GameObject mobileHalftones;
    [SerializeField] private Transform mainTitle;
    [SerializeField] private DudeDemoer demoer;

    private int page;
    private bool landscape;
    
    private void Start()
    {
        ChangeLayout();
        scoreManager.onLoaded += ScoresLoaded;
        // scoreManager.LoadLeaderBoards(page);
        wotd.text = $"Word of the day is <size=5>{dict.RandomWord().ToUpper()}</size>!";
        var mode = ModeToggle.GetMode();
        modeToggles[mode is GameMode.Fresh or GameMode.Daily ? 0 : 1].Select(false);
        modeToggles[mode is GameMode.Daily or GameMode.ClassicDaily ? 2 : 3].Select(true);
    }

    private void ChangeLayout()
    {
        landscape = IsLandscape();
        canvasScaler.referenceResolution = landscape ? new Vector2(800, 600) : new Vector2(500, 800);
        canvasScaler.matchWidthOrHeight = landscape ? 1 : 0;
        // nonCanvasElems.position = new Vector3(0, landscape ? 0 : -0.95f, 0);
        nonCanvasElems.localScale = Vector3.one * (landscape ? 1 : 0.6f);
        mobileHalftones.SetActive(!landscape);
        mainTitle.localScale = Vector3.one * (landscape ? 1 : 0.75f);
        demoer.min = landscape ? 3.5f : 0;
        demoer.max = landscape ? 7f : 1.5f;
    }

    private void Update()
    {
        if (landscape != IsLandscape())
        {
            ChangeLayout();
        }
        
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

    public static string GetMainScene()
    {
        return IsLandscape() ? "Main" : "Mobile";
    }

    private static bool IsLandscape()
    {
        return Screen.width * 1f / Screen.height > 1;
    }

    public void StartGame()
    {
        var scene = PlayerPrefs.HasKey("PlayerName") ? GetMainScene() : "Name";
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
        var isDaily = Manager.Instance.Daily;
        scoreManager.ChangeGame(ModeToggle.GetLeaderboard(mode, Manager.Instance.Day), true);
        Manager.Instance.Seed = isDaily ? ModeToggle.GetSeed(Manager.Instance.Day) : Environment.TickCount;
        leaderboardTitle.text = isDaily ? $"LEADERBOARDS <size=25>({Manager.Instance.DailyString})</size>" : "LEADERBOARDS";
        dayNext.SetActive(isDaily);
        dayPrev.SetActive(isDaily);
    }

    public void ChangeDay(int dir)
    {
        if (dir > 0 && Manager.Instance.Day >= DateTime.Today) return; 
        Manager.Instance.Day = Manager.Instance.Day.AddDays(dir);
        Setup();
    }
}
