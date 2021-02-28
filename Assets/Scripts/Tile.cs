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
    public Pulsater letterPulsater;

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
        var p = transform.position;
        const float vol = 0.4f;
        AudioManager.Instance.PlayEffectAt(Random.Range(4, 8), p, 1.1f * vol);
        AudioManager.Instance.PlayEffectAt(10, p, 0.6f * vol);
        
        letterText.color = color;
        EffectManager.Instance.AddEffect(2, transform.position);
        Invoke(nameof(DoBoom), boomDelay);
        letterPulsater.Pulsate();
    }

    public void Pulse()
    {
        EffectManager.Instance.AddEffect(3, transform.position);
    }

    private void DoBoom()
    {
        var p = transform.position;
        EffectManager.Instance.AddEffect(0, p);
        var e = EffectManager.Instance.AddEffect(1, p);
        e.transform.Rotate(new Vector3(0, 0, Random.Range(0, 360f)));

        connectors.ForEach(c => c.SetActive(false));
        gameObject.SetActive(false);

        const float vol = 0.7f;
        AudioManager.Instance.PlayEffectAt(Random.Range(4, 8), p, 1.1f * vol);
        AudioManager.Instance.PlayEffectAt(8, p, 1.429f * vol);
        AudioManager.Instance.PlayEffectAt(9, p, 1.707f * vol);
        AudioManager.Instance.PlayEffectAt(10, p, 0.6f * vol);

    }
}
