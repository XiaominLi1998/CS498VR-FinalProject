using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
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

    public GameState gameState;

    private Vocab word;
    private List<Vocab> unit = new List<Vocab>();

    private int unitSize = 3;

    private int wordIdx;//Xiaomin: the idx of curr word in the vocabs list

    private int eachVocabListSize = 2000;//Xiaomin: the size of GRE = size of SAT = ... = 2000

    public static string difficultyLevel;

    public static int scoreToWin = 20;//XM: TODO: change it to a bigger valud

    public static List<Vocab> vocabs_GRE = new List<Vocab>();
    public static List<Vocab> vocabs_SAT = new List<Vocab>();
    public static List<Vocab> vocabs_TOEFL = new List<Vocab>();

    private int passedLetterCount;
    private int passedWordCount;
    private Camera camera;
    private LettersContainerController lettersContainerController;

    public int scoreValue = 0;

    /* HUD GUI */
    private GUIStyle guiAimStyle, guiTextStyle, guiScoreStyle;
    private Rect guiAimPos, guiHintPos, guiLetterPos, guiScorePos; // guiLetterPos: pos of selected letters

    /* Reference: Monobehaviour lifecycle */
    void Awake()
    {
        Debug.Log("AWAKEEEEEEEEEE");
        gameState = GameState.Uninitialized;
        GetGameAttributes();
        passedLetterCount = 0;
        passedWordCount = 0;
        camera = (Camera)GameObject.Find("TPSCamera").GetComponent("Camera");

    }

    void Start()
    {
        Debug.Log("STARTTTTTTTT");

        lettersContainerController = (LettersContainerController)GameObject.
           Find("LettersContainer").GetComponent("LettersContainerController");
        lettersContainerController.word = word.vocab;

        /* HUD GUI */
        guiAimStyle = new GUIStyle();
        guiAimStyle.alignment = TextAnchor.MiddleCenter;
        guiAimStyle.fontSize = 30;
        guiAimStyle.normal.textColor = Color.white;
        guiTextStyle = new GUIStyle();
        guiTextStyle.alignment = TextAnchor.MiddleCenter;
        guiTextStyle.fontSize = 20;
        guiTextStyle.normal.textColor = Color.white;

        guiScoreStyle = new GUIStyle();
        guiScoreStyle.alignment = TextAnchor.MiddleCenter;
        guiScoreStyle.fontSize = 40;
        guiScoreStyle.fontStyle = FontStyle.Bold;
        guiScoreStyle.normal.textColor = Color.green;


        guiAimPos = new Rect(Screen.width / 2 - 50, Screen.height / 2 - 50,
                100, 100);
        guiHintPos = new Rect(Screen.width / 2 - 300, Screen.height - 200,
                600, 50);
        guiLetterPos = new Rect(Screen.width / 2 - 300, Screen.height - 100,
                600, 50);
        guiScorePos = new Rect(Screen.width / 2, Screen.height - 100,
                600, 50);
        /* Game State */
        gameState = GameState.Started;

        wordIdx = 0;
        Debug.Log("At Start difficultyLevel is:" + difficultyLevel);

    }

    void Update()
    {

        if (gameState == GameState.Started)
        {
            if (word.vocab != lettersContainerController.word)
            {
                lettersContainerController.ClearLetters();
                lettersContainerController.UpdateContainers(word.vocab);//use new word to get new containers
            }

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = camera.ScreenPointToRay(new Vector3(
                    camera.pixelWidth / 2,
                    camera.pixelHeight / 2,
                    0));
                Debug.DrawRay(ray.origin, ray.direction * 1000, Color.yellow);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    GameObject obj = hit.collider.gameObject;
                    if (obj.name != null && obj.name.Contains("Letter"))
                    {
                        try
                        {
                            int index = Int32.Parse(obj.name.Substring(6));
                            char letter = lettersContainerController.GetLetter(index);
                            bool isCorrect = letter == word.vocab[passedLetterCount];
                            OnLetterSelected(isCorrect);
                            lettersContainerController.OnLetterHit(index, isCorrect);
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine($"Unable to parse '{obj.name.Substring(6)}'");
                        }
                    }
                }
            }
        }
        else if (gameState == GameState.Succeed)
        {
            SceneManager.LoadScene("SuccessScene");
        }
        else if (gameState == GameState.Fail)
        {
            SceneManager.LoadScene("DeadScene");
        }
    }

    public void loadfile(string filename, string vocabsType)
    {
        TextAsset vocab_ = Resources.Load<TextAsset>(filename);
        string[] data = vocab_.text.Split(new char[] { '\n' });
        // skip the first line and the last line
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
                default://TOEFL
                    vocabs_TOEFL.Add(v);
                    break;
            }
        }


    }
    //Xiaomin: wordIdx should be constantly changing, when UpdateWord() is called;
    //we just set word = vocabs_???[wordIdx]
    void UpdateWord()
    {
        switch (difficultyLevel)
        {
            case "GRE":
                word = vocabs_GRE[wordIdx];
                break;
            case "SAT":
                word = vocabs_SAT[wordIdx];
                break;
            default:
                word = vocabs_TOEFL[wordIdx];
                break;
        }
        Debug.Log("WORD: " + word.vocab + ", MEANING:" + word.meaning);
    }

    void GetGameAttributes()
    {
        loadfile("GRE_vocab", "GRE");
        loadfile("SAT_vocab", "SAT");
        loadfile("TOEFL_vocab", "TOEFL");

        UpdateWord();
    }

    void OnLetterSelected(bool isCorrect)
    {
        if (isCorrect)
        {
            passedLetterCount += 1;
            if (passedLetterCount == word.vocab.Length)
            {
                //Xiaomin: got a whole word correct, receive score:
                scoreValue += 10;
                passedLetterCount = 0;
                passedWordCount += 1;

                //Xiaomin: got a whole unit:
                if (passedWordCount >= unitSize)
                {
                    passedWordCount = 0;
                    gameState = GameState.Succeed;
                    unit.Clear();
                }

                wordIdx++;
                wordIdx %= eachVocabListSize;
                UpdateWord();
                unit.Add(word);
            }

            // //XM: if scoreValue >= scoreToWin, then the user wins the game!
            // if (scoreValue >= scoreToWin)
            // {
            //     gameState = GameState.Succeed;
            // }

        }
    }

    public bool isGameActive()
    {
        return gameState == GameState.Started;
    }


    void OnGUI()
    {
        GUI.Label(guiAimPos, "+", guiAimStyle);
        GUI.Label(guiHintPos, word.meaning, guiTextStyle);
        GUI.Label(guiLetterPos,
                String.Concat(word.vocab.Substring(0, passedLetterCount),
                        new String('_', word.vocab.Length - passedLetterCount)),
                guiTextStyle);
        GUI.Label(guiScorePos, "Score: " + scoreValue, guiScoreStyle);
    }
}