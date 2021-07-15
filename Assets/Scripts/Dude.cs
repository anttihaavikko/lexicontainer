using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Dude : MonoBehaviour
{
    public Animator anim;
    public Face face;
    public SpeechBubble bubble;
    public Hand hand;
    public bool main;

    private static readonly int Show = Animator.StringToHash("show");

    private bool showing;
    private bool willTutorial;
    private static readonly int Thumb = Animator.StringToHash("thumb");
    private float verticalPos;
    
    private void Start()
    {
        verticalPos = transform.position.y;
        if(main) ShowTutorial(Tutorial.Intro);
    }

    private static string GetTutorialName(Tutorial tut)
    {
        return "Tutorial" + Enum.GetName(typeof(Tutorial), tut);
    }

    public void ShowTutorial(Tutorial tut)
    {
        var tutorialName = GetTutorialName(tut);
        string msg1, msg2;

        if (PlayerPrefs.HasKey(tutorialName) || willTutorial) return;
        
        // need word/min tutorial first
        var wordDone = PlayerPrefs.HasKey(GetTutorialName(Tutorial.Word));
        var threeDone = PlayerPrefs.HasKey(GetTutorialName(Tutorial.Three));
        if (tut == Tutorial.Multiplier && !wordDone && !threeDone) return;

        // need multiplier first
        var resetDone = PlayerPrefs.HasKey(GetTutorialName(Tutorial.Multiplier));
        if (tut == Tutorial.MultiReset && !resetDone) return;

        willTutorial = true;
        PlayerPrefs.SetInt(tutorialName, 1);

        switch (tut)
        {    
            case Tutorial.Intro:
                msg1 = "Hi I'm (Lex)! I'll guide you how to play this game...";
                msg2 = "Drag the (tetrominos) to the board and form (words)!";
                break;
            case Tutorial.Word:
                msg1 = "You get (more points) the (more letters) are removed at once!";
                msg2 = string.Empty;
                break;
            case Tutorial.Multiplier:
                msg1 = "Every time you don't form a word, the (multiplier) increases!";
                msg2 = string.Empty;
                break;
            case Tutorial.Three:
                msg1 = "Only words with (three letters) and up are counted!";
                msg2 = string.Empty;
                break;
            case Tutorial.MultiReset:
                msg1 = "When you form a (word), your (multiplier) will (reset)!";
                msg2 = "You can use this as kind of a (risk) vs. (reward) tactic!";
                break;
            case Tutorial.HiScore:
                msg1 = "(Good), you're already (doing better) than your previous best!";
                msg2 = string.Empty;
                break;
            case Tutorial.BigRound:
                msg1 = "(Wow)! That was a (majestic play). You're really getting the hang of this.";
                msg2 = string.Empty;
                break;
            default:
                msg1 = string.Empty;
                msg2 = string.Empty;
                break;
        }
        
        this.StartCoroutine(() => ShowBubble(msg1, msg2), 1.2f);
    }

    private void ShowBubble(string message, string secondMessage = null)
    {
        ShowAt(0, true);
        this.StartCoroutine(() =>
        {
            showing = true;
            bubble.ShowMessage(message);

            if (!string.IsNullOrEmpty(secondMessage))
            {
                bubble.QueMessage(secondMessage);
            }
            
        }, 0.5f);
    }

    private void Update()
    {
        if (!showing) return;
        if (!Input.anyKeyDown) return;

        if (!bubble.done)
        {
            bubble.SkipMessage();
            return;
        }
        
        if (bubble.QueCount() > 0)
        {
            bubble.PopMessage();
            return;
        }

        bubble.Hide();
        showing = false;
        Hide();
    }

    public void NiceAt(float x, bool down, bool veryNice = false)
    {
        ShowAt(x, down, veryNice);
        
        this.StartCoroutine(() =>
        {
            var idx = veryNice ? 28 : Random.Range(0, 4);
            AudioManager.Instance.PlayEffectAt(idx, face.mouth.position, 2f);
            face.OpenMouth(0.3f);
        }, 0.5f);
        
        Invoke(nameof(Hide), veryNice ? 1.2f : 0.8f);
    }

    public void ShowAt(float x, bool down, bool thumb = false)
    {
        var t = transform;
        t.position = new Vector3(x, down ? verticalPos : -verticalPos, 0f);
        t.localScale = new Vector3(1f, down ? 1f : -1f, 1f);
        anim.SetBool(Thumb, thumb);
        anim.SetBool(Show, true);
        Invoke(nameof(MoveSound), 0.2f);
    }

    public void Hide()
    {
        if (willTutorial) hand.Spawn();
        
        willTutorial = false;
        anim.SetBool(Thumb, false);
        anim.SetBool(Show, false);
        Invoke(nameof(MoveSound), 0.2f);
    }

    public void MoveSound()
    {
        var position = transform.position;
        AudioManager.Instance.PlayEffectAt(29, position, 1.804f);
        AudioManager.Instance.PlayEffectAt(9, position, 1.094f);
        
        if(Random.value < 0.5f)
            AudioManager.Instance.PlayEffectAt(Random.Range(12, 28), face.mouthSprite.transform.position, 2f);
    }

    public bool HasSomething()
    {
        return willTutorial;
    }
}

public enum Tutorial
{
    Intro,
    Word,
    Multiplier,
    Three,
    MultiReset,
    HiScore,
    BigRound
}