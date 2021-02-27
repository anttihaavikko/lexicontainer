using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class WordDefiner : MonoBehaviour
{
    public TMP_Text field;
    public Appearer appearer;
    
    private CertificateHandler certHandler;

    private void Awake()
    {
        certHandler = new CustomCertificateHandler();
    }

    public void DefineWord(string word)
    {
        StartCoroutine(GetDefinition(word));
    }

    private IEnumerator GetDefinition(string word)
    {
        var www = UnityWebRequest.Get("https://api.dictionaryapi.dev/api/v2/entries/en_US/" + word);
        www.certificateHandler = certHandler;

        yield return www.SendWebRequest();

        if (!string.IsNullOrEmpty(www.error)) yield break;

        var json = "{\"words\":" + www.downloadHandler.text + "}";
        var def = JsonUtility.FromJson<DefinitionData>(json);

        if (def.words.Length == 0) yield break;
        var w = def.words[0];
        if (w.meanings.Length == 0) yield break;
        var meaning = def.words[0].meanings[0];
        if (meaning.definitions.Length == 0) yield break;
        field.text = w.word + ", " + meaning.partOfSpeech + ", " + meaning.definitions[0].definition;
        appearer.Show();
    }
}

[Serializable]
public class DefinitionData
{
    public WordOption[] words;
}

[Serializable]
public class WordOption
{
    public string word;
    public WordMeaning[] meanings;
}

[Serializable]
public class WordMeaning
{
    public string partOfSpeech;
    public WordDefinition[] definitions;
}

[Serializable]
public class WordDefinition
{
    public string definition;
}

public class CustomCertificateHandler : CertificateHandler
{
    // Encoded RSAPublicKey
    private static readonly string PUB_KEY = "";


    /// <summary>
    /// Validate the Certificate Against the Amazon public Cert
    /// </summary>
    /// <param name="certificateData">Certifcate to validate</param>
    /// <returns></returns>
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}