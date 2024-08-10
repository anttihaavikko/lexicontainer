using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonExtension : MonoBehaviour
{
    [SerializeField] private Image icon, dash;
    [SerializeField] private string key;

    private bool toggled;

    private void Start()
    {
        SetState(PlayerPrefs.GetInt(key, 0) == 1);
    }

    private void SetState(bool state)
    {
        toggled = state;
        dash.gameObject.SetActive(toggled);
        PlayerPrefs.SetInt(key, toggled ? 1 : 0);
        
        if(key == "LexMusic") AudioManager.Instance.ChangeMusicVolume(toggled ? 0 : 1);
        if(key == "LexSound") AudioManager.Instance.volume = toggled ? 0 : 1;
    }

    public void SetColor(Color color)
    {
        icon.color = dash.color = color;
    }

    public void Toggle()
    {
        SetState(!toggled);
    }
}