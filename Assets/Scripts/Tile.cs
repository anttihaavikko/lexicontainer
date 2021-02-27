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
    public static readonly float boomDelay = 2.5f;

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

    public void Boom(Color color)
    {
        letterText.color = color;
        Invoke(nameof(DoBoom), boomDelay);
    }

    private void DoBoom()
    {
        var p = transform.position;
        EffectManager.Instance.AddEffect(0, p);
        var e = EffectManager.Instance.AddEffect(1, p);
        e.transform.Rotate(new Vector3(0, 0, Random.Range(0, 360f)));

        connectors.ForEach(c => c.SetActive(false));
        gameObject.SetActive(false);
    }
}
