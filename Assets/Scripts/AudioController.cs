using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AudioController : MonoBehaviour
{
    private string defaultUrl = "http://ssl.gstatic.com/dictionary/static/sounds/oxford/example--_gb_1.mp3";

    private string url;
    
    enum AudioStatus
    {
        Undefined, Downloading, Ready, Playing, Paused, End
    }

    public AudioSource audioSource;

    void Awake() 
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAudio(string word) 
    {
        url = ParseUrl(word);
        if (url != "")
        {
            Debug.Log("Url is "+ url);
            StartCoroutine(GetAudioClip());
        }
    }

    /* If parse fail then return empty string "" */
    private string ParseUrl(string word)
    {
        string s = defaultUrl.Replace("example", word.ToLower());
        if (!s.Contains(" ")) // otherwise word might be a phrase
        {
            return s;
        }
        return "";
    }

    IEnumerator GetAudioClip()
    {
        #if ((UNITY_STANDALONE_WIN) || (UNITY_EDITOR_WIN)) 
            WWW www = new WWW(url);
            while(!www.isDone) 
            {
                yield return 0;
            }
            audioSource.clip = NAudioPlayer.FromMp3Data(www.bytes);
            audioSource.Play();
        #endif
        #if ((UNITY_STANDALONE_OSX) || (UNITY_EDITOR_OSX))
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
            {
                yield return www.Send();

                if (www.isNetworkError)
                {
                    Debug.Log(www.error);
                }
                else
                {   
                    Debug.Log("Playing audio");
                    AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
                    audioSource.clip = audioClip;
                    audioSource.Play();
                }
            }
        #endif
    }
}
