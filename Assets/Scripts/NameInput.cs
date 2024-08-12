using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class NameInput : MonoBehaviour
{
    public TMP_InputField field;
    
    private static NameInput instance;
    
    [DllImport("__Internal")]
    private static extern void PromptName(Action<IntPtr> action);

    private void Awake()
    {
        if (instance != null && instance != this) {
            Destroy (gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        field.text = "Anon";
        field.onValueChanged.AddListener(ToUpper);
        Invoke(nameof(FocusInput), 0.6f);
        
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            this.StartCoroutine(() => PromptName(SetName), 1f);
        }
    }
    
    [MonoPInvokeCallback(typeof(Action<IntPtr>))]
    public static void SetName(IntPtr ptr)
    {
        instance.field.text = Marshal.PtrToStringAuto(ptr);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            Save();
        }
    }

    private void FocusInput()
	{
        EventSystem.current.SetSelectedGameObject(field.gameObject, null);
        field.OnPointerClick(new PointerEventData(EventSystem.current));
    }

    private void ToUpper(string value)
    {
        field.text = value;
    }

    public void Save()
    {
        if (string.IsNullOrEmpty(field.text)) return;
        PlayerPrefs.SetString("PlayerName", field.text);
        SceneChanger.Instance.ChangeScene("Main");
    }
}
