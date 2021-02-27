using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dude : MonoBehaviour
{
    public Animator anim;
    public Face face;
    
    private static readonly int Show = Animator.StringToHash("show");

    public void NiceAt(float x, bool down)
    {
        ShowAt(x, down);
        
        this.StartCoroutine(() =>
        {
            AudioManager.Instance.PlayEffectAt(Random.Range(0, 4), face.mouth.position, 2f);
        }, 0.5f);
        
        Invoke(nameof(Hide), 0.8f);
    }

    public void ShowAt(float x, bool down)
    {
        var t = transform;
        t.position = new Vector3(x, down ? -5f : 5f, 0f);
        t.localScale = new Vector3(1f, down ? 1f : -1f, 1f);
        anim.SetBool(Show, true);
    }

    public void Hide()
    {
        anim.SetBool(Show, false);
    }
}
