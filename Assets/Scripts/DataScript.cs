using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public static class DataScript
{
    public enum GameState
    {
        Uninitialized,
        Ready,
        Started,
        Paused,
        Fail,
        Succeed
    }

    public static string difficultyLevel;

    public static int scoreToWin = 40;//XM: TODO: change it to a bigger value

    public static int eachVocabListSize = 2000;//Xiaomin: the size of GRE = size of SAT = ... = 2000
    public static int wordIdx = 0;//Xiaomin: the idx of curr word in the vocabs list
    public static int unitSize = 3;

    public static bool GamePaused = false;

    public static int score = 0;

    public static Vocab word;

    public static int passedWordCount = 0;

    public static int passedUnitCount = 0;

    public static GameState gameState;

    public static List<Vocab> unit = new List<Vocab>();

    public static List<Vocab> vocabs_GRE = new List<Vocab>();
    public static List<Vocab> vocabs_SAT = new List<Vocab>();
    public static List<Vocab> vocabs_TOEFL = new List<Vocab>();
    public static List<Vocab> vocabs_spanish = new List<Vocab>();

    private static bool isDataInitialized = false;

    public static bool isLearnMode = true;

    public static void loadfile(string filename, string vocabsType)
    {
        Debug.Log("filename" + filename);
        TextAsset vocab_ = Resources.Load<TextAsset>(filename);

        string[] data = vocab_.text.Split(new char[] { '\n' });
        Debug.Log("data.Length: " + data.Length);
        // skip the first line and the last line
        if (vocabsType == "Spanish")
        {
            Debug.Log("data.Length in Spanish: " + data.Length);
        }
        for (int i = 1; i < data.Length - 1; i++)
        {
            string[] row = data[i].Split(new char[] { ',' });
            Vocab v = new Vocab();
            int.TryParse(row[0], out v.id);
            v.vocab = row[1];
            v.meaning = row[2];
            switch (vocabsType)
            {
                case "GRE":
                    vocabs_GRE.Add(v);
                    break;
                case "SAT":
                    vocabs_SAT.Add(v);
                    break;
                case "Spanish":
                    vocabs_spanish.Add(v);
                    break;
                default://TOEFL
                    vocabs_TOEFL.Add(v);
                    break;
            }
        }
    }

    public static void InitData()
    {
        if (isDataInitialized) return;
        loadfile("GRE_vocab", "GRE");
        loadfile("SAT_vocab", "SAT");
        loadfile("TOEFL_vocab", "TOEFL");
        loadfile("Spanish1000", "Spanish");
        isDataInitialized = true;
    }
}
