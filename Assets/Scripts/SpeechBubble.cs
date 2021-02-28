using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SpeechBubble : MonoBehaviour {

	public TextMeshPro textArea;

	private bool shown;
	private string message = "";
	private int messagePos = -1;
    private bool hidesWithAny = false;
    public Appearer appearer;
    public Transform soundPosition;

    public bool done = false;

	private AudioSource audioSource;

	private List<string> messageQue;

	public Color hiliteColor;

	bool useColors = true;
    private bool canSkip = false;

    private string[] options;
    private string[] optionActions;
    private int optionSelection;

    private Vector3 helpImageSize;

    private void Awake()
    {
	    messageQue = new List<string> ();
    }

    private void Start () {
		audioSource = GetComponent<AudioSource> ();
		Invoke("EnableSkip", 0.25f);
    }

    private void EnableSkip()
    {
        canSkip = true;
    }

    // Update is called once per frame
    private void Update () {

		if (Random.value < 0.4f) {
			return;
		}

		if (messagePos >= 0 && !done) {
			messagePos++;

            if (messagePos > message.Length) return;

			string msg = message.Substring (0, messagePos);

			int openCount = msg.Split('(').Length - 1;
			int closeCount = msg.Split(')').Length - 1;

            if (openCount > closeCount && useColors) {
				msg += ")";
			}

			string letter = message.Substring (messagePos - 1, 1);

            if(letter == "#")
            {
                done = true;
                textArea.text += " ";
                return;
            }

            var hex = "#" + ColorUtility.ToHtmlStringRGB(hiliteColor);
            textArea.text = useColors ? msg.Replace("(", "<color=" + hex + ">").Replace(")", "</color>") : msg;

            if (messagePos == 1 || letter == " " && Random.value < 0.5f) {
	            AudioManager.Instance.PlayEffectAt(Random.Range(12, 28), soundPosition.position, 3f);
            }

			if (messagePos >= message.Length) {
				messagePos = -1;

				done = true;
			}
		}
	}

	public int QueCount() {
		return messageQue.Count;
	}

	public void SkipMessage() {
		done = true;
		messagePos = -1;
		var hex = "#" + ColorUtility.ToHtmlStringRGB(hiliteColor);
		textArea.text = useColors ? message.Replace("(", "<color=" + hex + ">").Replace(")", "</color>") : message;;
	}

    public void ShowMessage(string str, bool colors = true)
    {
	    appearer.Show();
	    
        hidesWithAny = false;
        
        canSkip = false;
        Invoke("EnableSkip", 0.25f);

        //AudioManager.Instance.PlayEffectAt(9, transform.position, 1f);
        //AudioManager.Instance.PlayEffectAt(27, transform.position, 0.7f);

        useColors = colors;

        //AudioManager.Instance.Highpass ();

		done = false;
		shown = true;
		message = str;
		textArea.text = "";

		Invoke ("ShowText", 0.2f);
    }

	public void QueMessage(string str) {
		messageQue.Add (str);
	}

	public void CheckQueuedMessages() {
		if (messageQue.Count > 0 && !shown) {
			PopMessage ();
		}
	}

	public void PopMessage() {
		string msg = messageQue [0];
		messageQue.RemoveAt (0);
		ShowMessage (msg);
	}

	private void ShowText() {
		messagePos = 0;
	}

	public void HideAfter (float delay) {
		Invoke ("Hide", delay);
	}

	public void Hide()
	{
		appearer.Hide();

        //AudioManager.Instance.Highpass (false);

        //AudioManager.Instance.PlayEffectAt (9, transform.position, 1f);
        //AudioManager.Instance.PlayEffectAt(27, transform.position, 0.7f);

		shown = false;
		textArea.text = "";
	}

	public bool IsShown() {
		return shown;
	}
}
