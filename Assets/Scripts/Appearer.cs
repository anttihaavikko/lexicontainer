using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Appearer : MonoBehaviour
{
	public float appearAfter = -1f;
	public float hideDelay;
    public bool silent;
    public Pulsater pulsater;
    public GameObject visuals;

    public TMP_Text text;
    public Animator anim;
    private Vector3 size;

    // Start is called before the first frame update
    void Start()
    {
        size = transform.localScale;
        transform.localScale = Vector3.zero;
        if(visuals) visuals.SetActive(false);

		if (appearAfter >= 0)
			Invoke("Show", appearAfter);
    }

    public void Show()
    {
        if(!silent)
        {
            //AudioManager.Instance.PlayEffectAt(16, Vector3.zero, 0.336f);
            //AudioManager.Instance.PlayEffectAt(17, Vector3.zero, 0.329f);
        }

        if(visuals) visuals.SetActive(true);
		Tweener.Instance.ScaleTo(transform, size, 0.3f, 0f, TweenEasings.BounceEaseOut);

        if(anim)
            anim.SetBool("playing", true);
    }

    public void Hide()
	{
        CancelInvoke("Show");

        if(!silent)
        {
            //AudioManager.Instance.PlayEffectAt(16, Vector3.zero, 0.336f);
            //AudioManager.Instance.PlayEffectAt(17, Vector3.zero, 0.329f);
        }

		Tweener.Instance.ScaleTo(transform, Vector3.zero, 0.2f, 0f, TweenEasings.QuadraticEaseOut);
        
        if(visuals)
            this.StartCoroutine(() => visuals.SetActive(false), 0.2f);
    }

    public void HideWithDelay()
	{
		Invoke("Hide", hideDelay);
	}

    public void ShowWithText(string t, float delay)
    {
        if (text)
            text.text = t;

        Invoke("Show", delay);
    }
}
