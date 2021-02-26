using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isDecoration;
    public List<GameObject> connectors;
    public SpriteRenderer focus;
    public TMP_Text letterText;

    private string letter;

    public void SetLetter(string l)
    {
        letterText.text = l.ToUpper();
        letter = l;
    }

    public string GetLetter()
    {
        return letter;
    }

    public void Boom()
    {
        letterText.color = Color.red;
        Invoke(nameof(DoBoom), 2.5f);
    }

    private void DoBoom()
    {
        connectors.ForEach(c => c.SetActive(false));
        gameObject.SetActive(false);
    }
}
