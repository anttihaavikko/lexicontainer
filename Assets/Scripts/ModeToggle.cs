using System;
using System.Collections.Generic;
using UnityEngine;

public class ModeToggle : MonoBehaviour
{
    [SerializeField] private CustomButton button;
    [SerializeField] private Color onColor, offColor;
    [SerializeField] private List<ModeToggle> others;
    [SerializeField] private GameMode mode;
    [SerializeField] private StartView startView;

    public void Select()
    {
        others.ForEach(o => o.Deselect());
        button.SetColor(onColor);
        transform.localScale = Vector3.one * 1.2f;
        PlayerPrefs.SetInt("LexMode", (int)mode);
        startView.SelectMode();
        button.OnPointerExit(null);
    }

    private void Deselect()
    {
        button.SetColor(offColor);
        button.OnPointerExit(null);
        transform.localScale = Vector3.one;
    }

    public static string GetDescription(GameMode gameMode)
    {
        return gameMode switch
        {
            GameMode.Fresh => "New dictionary & leaderboards!",
            GameMode.Classic => "Original dictionary & leaderboards",
            GameMode.Daily => "Same daily puzzle for everyone",
            _ => throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null)
        };
    }

    public static string GetLeaderboard(GameMode gameMode, DateTime day)
    {
        return gameMode switch
        {
            GameMode.Fresh => "lexicontainer",
            GameMode.Classic => "wowie",
            GameMode.Daily => $"lex-{GetSeed(day)}",
            _ => throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null)
        };
    }

    public static bool IsClassic()
    {
        return GetMode() == GameMode.Classic;
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
    Daily = 2
}