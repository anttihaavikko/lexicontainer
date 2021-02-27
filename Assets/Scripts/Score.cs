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
    
    private int score;
    private float shownScore;
    private int multiplier = 1;

    private int moves;

    private void Start()
    {
        Debug.Log("Player is " + PlayerPrefs.GetString("PlayerName"));
    }

    private void Update()
    {
        var scrollSpeed = Mathf.Max(10f, score - shownScore);
        shownScore = Mathf.MoveTowards(shownScore, score, Time.deltaTime * scrollSpeed * 2f);
        display.text = ScoreString(score);
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
        GenerateIdIfNeeded();
        scoreManager.SubmitScore(PlayerPrefs.GetString("PlayerName"), score, moves, PlayerPrefs.GetString("Identifier"));
    }

    private static void GenerateIdIfNeeded()
    {
        if (!PlayerPrefs.HasKey("Identifier"))
        {
            PlayerPrefs.SetString("Identifier", System.Guid.NewGuid().ToString());
        }
    }

    public void Add(int amount)
    {
        moves++;
        
        if (amount == 0)
        {
            AddMulti();
            return;
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
    }

    public static string ScoreString(float score)
    {
        var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
        nfi.NumberGroupSeparator = " ";
        return score.ToString("#,0", nfi);
    }
}
