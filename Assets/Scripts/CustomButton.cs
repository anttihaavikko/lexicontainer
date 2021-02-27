using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CustomButton : Button
{
    public TMP_Text text;
    public Image img;

    private Color color;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        color = text.color;
        text.color = img.color = Color.white;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        text.color = img.color = color;
    }
}
