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
    
    private int score;
    private float shownScore;
    private int multiplier = 1;

    // Update is called once per frame
    void Update()
    {
        var scrollSpeed = Mathf.Max(10f, score - shownScore);
        shownScore = Mathf.MoveTowards(shownScore, score, Time.deltaTime * scrollSpeed * 2f);
        display.text = ScoreString();
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

    public void Add(int amount)
    {
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

    public string ScoreString()
    {
        var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
        nfi.NumberGroupSeparator = " ";
        return shownScore.ToString("#,0", nfi);
    }

    public int GetTotal()
    {
        return score;
    }
}
