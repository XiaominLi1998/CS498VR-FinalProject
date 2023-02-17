using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Memorize : MonoBehaviour
{
    private List<Vocab> vocabs;

    private GUIStyle guiWordsStyle;
    private Rect guiWordsPos;

    private GUIStyle guiTimerStyle;
    private Rect guiTimerPos;

    private static double reviewTime = 10.0f;//XM: give 15s for user to review
    private double timeLeft;

    DateTime startTime;
    // Start is called before the first frame update

    void UpdateUnit()
    {
        DataScript.unit.Clear();
        int startIdx = (DataScript.wordIdx / DataScript.unitSize) * DataScript.unitSize;
        for (int i = startIdx; i < startIdx + DataScript.unitSize; i++)
        {
            DataScript.unit.Add(vocabs[i]);
        }
    }

    void Start()
    {
        switch (DataScript.difficultyLevel)
        {
            case "GRE":
                vocabs = DataScript.vocabs_GRE;
                break;
            case "SAT":
                vocabs = DataScript.vocabs_SAT;
                break;

            case "Spanish":
                Debug.Log("Switch to Spanish");
                vocabs = DataScript.vocabs_spanish;
                break;

            default:
                vocabs = DataScript.vocabs_TOEFL;
                break;
        }
        if (DataScript.unit.Count == 0)
        {
            UpdateUnit();
        }

        timeLeft = reviewTime;
        startTime = System.DateTime.Now;

        guiWordsStyle = new GUIStyle();
        guiWordsStyle.alignment = TextAnchor.MiddleLeft;
        guiWordsStyle.fontSize = 40;
        guiWordsStyle.normal.textColor = Color.white;
        guiWordsPos = new Rect(Screen.width / 6, 100,
                       Screen.width / 3 * 2, 100);

        guiTimerStyle = new GUIStyle();
        guiTimerStyle.alignment = TextAnchor.MiddleLeft;
        guiTimerStyle.fontSize = 80;
        guiTimerStyle.normal.textColor = Color.green;
        guiTimerPos = new Rect(100, 20,
                       100, 100);
    }

    // Update is called once per frame
    void Update()
    {
        if (!DataScript.isLearnMode)
        {
            TimeSpan timeElapsed = System.DateTime.Now - startTime;
            // Debug.Log("timeElapsed = " + timeElapsed.TotalSeconds);
            timeLeft = reviewTime - timeElapsed.TotalSeconds;
            if (timeLeft < (float)1 / 3 * reviewTime)
            {
                guiTimerStyle.normal.textColor = Color.red;
            }
            else if (timeLeft < (float)2 / 3 * reviewTime)
            {
                guiTimerStyle.normal.textColor = Color.yellow;
            }
            else
            {
                guiTimerStyle.normal.textColor = Color.green;
            }

            if (timeLeft <= 0)
            {
                timeLeft = 0;
                DataScript.GamePaused = false;//XM: resume game
                SceneManager.LoadScene("GameScene");
            }
        }
    }

    public void UserReady()
    {
        DataScript.GamePaused = false;//XM: resume game
        SceneManager.LoadScene("GameScene");
    }

    void OnGUI()
    {
        if (!DataScript.isLearnMode)
        {
            GUI.Label(guiTimerPos, "Time: " + timeLeft.ToString("F2"), guiTimerStyle);
        }
        for (int i = 0; i < DataScript.unit.Count; i++)
        {
            Rect tempPos = guiWordsPos;
            tempPos.y += i * 70;
            if (GUI.Button(tempPos, DataScript.unit[i].vocab + ": " + DataScript.unit[i].meaning, guiWordsStyle))
            {
                Debug.Log("Try play audio");
                gameObject.GetComponent<AudioController>().PlayAudio(DataScript.unit[i].vocab);
            }
        }
    }
}
