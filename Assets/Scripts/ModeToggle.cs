using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ModeToggle : MonoBehaviour
{
    [SerializeField] private CustomButton button;
    [SerializeField] private Color onColor, offColor;
    [SerializeField] private List<ModeToggle> others;
    [SerializeField] private StartView startView;
    [SerializeField] private GameObject tooltip;
    [SerializeField] private UnityEvent<bool> onSelect;
    
    private void Start()
    {
        if(tooltip) button.onHover += state => tooltip.SetActive(state);
    }

    public void SetMode(bool daily)
    {
        Manager.Instance.Daily = daily;
        SaveMode();
    }

    public void SetDictionary(bool classic)
    {
        Manager.Instance.Classic = classic;
        SaveMode();
    }

    private void SaveMode()
    {
        var mode = Manager.Instance.Classic ? 1 : 0;
        if (Manager.Instance.Daily) mode += 2;
        PlayerPrefs.SetInt("LexMode", mode);
    }

    public void Select()
    {
        Select(true);
    }

    public void Select(bool propagate)
    {
        onSelect.Invoke(true);
        others.ForEach(o => o.Deselect());
        button.SetColor(onColor);
        transform.localScale = Vector3.one * 1.2f;
        if(propagate) startView.SelectMode();
        button.OnPointerExit(null);
    }

    private void Deselect()
    {
        button.SetColor(offColor);
        button.OnPointerExit(null);
        transform.localScale = Vector3.one;
    }

    public static string GetLeaderboard(GameMode gameMode, DateTime day)
    {
        return gameMode switch
        {
            GameMode.Fresh => "lexicontainer",
            GameMode.Classic => "wowie",
            GameMode.Daily => $"lex-{GetSeed(day)}",
            GameMode.ClassicDaily => $"wowie-{GetSeed(day)}",
            _ => throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null)
        };
    }

    public static bool IsClassic()
    {
        return GetMode() is GameMode.Classic or GameMode.ClassicDaily;
    }

    public static GameMode GetMode()
    {
        return (GameMode)PlayerPrefs.GetInt("LexMode", 0);
    }

    public static int GetSeed(DateTime day)
    {
        return int.Parse(day.ToString("yyyyMMdd"));
    }
}

public enum GameMode
{
    Fresh = 0,
    Classic = 1,
    Daily = 2,
    ClassicDaily = 3
}