using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DudeDemoer : MonoBehaviour
{
    public Dude dude;
    public float min, max;

    private void Start()
    {
        Invoke(nameof(ShowDude), Random.Range(1f, 3f));
    }
    
    private void ShowDude()
    {
        var dir = Random.value < 0.5f ? 1f : -1f;
        dude.ShowAt(Random.Range(min, max) * dir, true);
        Invoke(nameof(HideDude), Random.Range(1f, 5f));
    }

    private void HideDude()
    {
        dude.Hide();
        Invoke(nameof(ShowDude), Random.Range(1f, 5f));
    }
}
