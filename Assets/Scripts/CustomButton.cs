using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class CustomButton : Button
{
    public TMP_Text text;
    public Image img;

    private Color color;
    private Camera cam;

    protected override void Start()
    {
        cam = Camera.main;
        base.Start();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        color = text.color;
        text.color = img.color = Color.white;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        text.color = img.color = color;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        var p = cam.ScreenToWorldPoint(transform.position);
        const float vol = 0.4f;
        AudioManager.Instance.PlayEffectAt(Random.Range(4, 8), p, 1.1f * vol);
        AudioManager.Instance.PlayEffectAt(10, p, 0.6f * vol);
        base.OnPointerClick(eventData);
    }
}
