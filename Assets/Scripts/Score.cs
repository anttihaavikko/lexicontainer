using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Globalization;

public class Score : MonoBehaviour
{
    public Pulsater pulsater, multiPulsater;
    public TMP_Text display, multi, addition;
    public Appearer additionAppearer;
    public ScoreManager scoreManager;

    public const string HiScoreKey = "HiScore";
    
    private int score;
    private float shownScore;
    private int multiplier = 1;

    private int moves;

    private void Update()
    {
        var scrollSpeed = Mathf.Max(10f, score - shownScore);
        shownScore = Mathf.MoveTowards(shownScore, score, Time.deltaTime * scrollSpeed * 2f);
        display.text = ScoreString(shownScore);
    }

    private void AddMulti()
    {
        multiplier++;
        UpdateMulti();
    }

    private void ClearMulti()
    {
        multiplier = 1;
        UpdateMulti();
    }

    private void UpdateMulti()
    {
        multiPulsater.Pulsate();
        multi.text = "x" + multiplier;
    }

    public void UploadScore()
    {
        if (IsBest() || !PlayerPrefs.HasKey(HiScoreKey))
        {
            Debug.Log("New hi score, " + score);
            PlayerPrefs.SetInt(HiScoreKey, score);
        };
        
        GenerateIdIfNeeded();
        scoreManager.ChangeGame(ModeToggle.GetLeaderboard(ModeToggle.GetMode(), Manager.Instance.Day), false);
        scoreManager.SubmitScore(PlayerPrefs.GetString("PlayerName"), score, moves, PlayerPrefs.GetString("Identifier"));
    }

    private static void GenerateIdIfNeeded()
    {
        if (!PlayerPrefs.HasKey("Identifier"))
        {
            PlayerPrefs.SetString("Identifier", System.Guid.NewGuid().ToString());
        }
    }

    public int Add(int amount)
    {
        moves++;
        
        if (amount == 0)
        {
            AddMulti();
            return 0;
        }
        
        var amt = (int)Mathf.Pow(amount, 2) * multiplier;
        addition.text = "+" + amt;
        additionAppearer.Show();
        
        this.StartCoroutine(() =>
        {
            score += amt;
            pulsater.Pulsate();
            additionAppearer.Hide();
            ClearMulti();
        }, 2f);

        return amt;
    }

    public static string ScoreString(float score)
    {
        var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
        nfi.NumberGroupSeparator = " ";
        return score.ToString("#,0", nfi);
    }

    public bool IsBest()
    {
        if (PlayerPrefs.HasKey(HiScoreKey))
        {
            return score > PlayerPrefs.GetInt(HiScoreKey);
        }

        return false;
    }
}
