using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    public static List<Vocab> vocabs;

    private Camera camera;
    private LettersContainerController lettersContainerController;
    private Laser laser;

    private int passedLetterCount;
    private int streakWordCount, streakLetterCount;

    /* HUD GUI */
    private GUIStyle guiAimStyle, guiTextStyle, guiScoreStyle, guiHPStyle;
    private Rect guiAimPos, guiHintPos, guiLetterPos, guiScorePos, guiUnitCountPos; // guiLetterPos: pos of selected letters
    private Texture2D emptyProgressBar;
    private Texture2D fullProgressBar;
    private float progressBarWidth = 200f;
    private float progressBarHeight = 40f;


    private float hp = 0;

    /* Reference: Monobehaviour lifecycle */
    void Awake()
    {
        Debug.Log("Game Controller AWAKE");
        DataScript.gameState = DataScript.GameState.Uninitialized;
        GetGameAttributes();
        passedLetterCount = 0;
        streakLetterCount = 0;
        streakWordCount = 0;
        camera = (Camera)GameObject.Find("TPSCamera").GetComponent("Camera");
        laser = (Laser)GameObject.Find("Laser").GetComponent("Laser");


        hp = 100;
    }

    void Start()
    {
        Debug.Log("Game controller: START");

        lettersContainerController = (LettersContainerController)GameObject.
           Find("LettersContainer").GetComponent("LettersContainerController");
        lettersContainerController.word = DataScript.word.vocab;

        /* Download and play audio -- debug only */
        // todo: decide whether the audio is played once
        gameObject.GetComponent<AudioController>().PlayAudio(DataScript.word.vocab);

        /* initlize bars to be white*/
        emptyProgressBar = Texture2D.whiteTexture;
        fullProgressBar = Texture2D.whiteTexture;

        guiAimStyle = new GUIStyle();
        guiAimStyle.alignment = TextAnchor.MiddleCenter;
        guiAimStyle.fontSize = 30;
        // guiAimStyle.richText = true;
        guiAimStyle.normal.textColor = Color.white;

        guiTextStyle = new GUIStyle();
        // guiTextStyle.richText = true;
        guiTextStyle.alignment = TextAnchor.MiddleCenter;
        guiTextStyle.fontSize = 45;
        // guiTextStyle.normal.textColor = Color.magenta;
        guiTextStyle.normal.textColor = new Color32(255, 136, 136, 255);
        guiTextStyle.fontStyle = FontStyle.Italic;

        guiScoreStyle = new GUIStyle();
        guiScoreStyle.alignment = TextAnchor.MiddleLeft;
        guiScoreStyle.richText = true;
        guiScoreStyle.fontSize = 50;
        guiScoreStyle.fontStyle = FontStyle.Bold;
        guiScoreStyle.normal.textColor = new Color32(180, 200, 255, 255);


        guiHPStyle = new GUIStyle();
        guiHPStyle.fontSize = 20;
        guiHPStyle.alignment = TextAnchor.MiddleCenter;


        guiAimPos = new Rect(Screen.width / 2 - 50, Screen.height / 2 - 50,
                100, 100);
        guiHintPos = new Rect(Screen.width / 2 - 300, Screen.height - 200,
                600, 50);
        guiLetterPos = new Rect(Screen.width / 2 - 300, Screen.height - 100,
                600, 50);
        guiScorePos = new Rect(Screen.width / 2 + 250, Screen.height - 100,
                600, 50);
        guiUnitCountPos = new Rect(Screen.width / 2 + 250, Screen.height - 170,
       600, 50);

        /* Game State */
        DataScript.gameState = DataScript.GameState.Started;
    }

    void Update()
    {
        // Debug.Log("^^^^^^^^passedLetterCount = " + passedLetterCount + ", wordIdx  = " + DataScript.wordIdx);
        // Debug.Log("^^^^^^^^passedWordCount = " + DataScript.passedWordCount);
        if (DataScript.gameState == DataScript.GameState.Started)
        {
            //XM: below pause/resume is to solve: back to game scene from mem scene
            if (DataScript.GamePaused)
            {
                Time.timeScale = 0f;//game stopped!
            }
            else
            {
                Time.timeScale = 1f;//game time back to normal
            }

            if (DataScript.word.vocab != lettersContainerController.word)
            {
                lettersContainerController.ClearLetters();
                lettersContainerController.UpdateContainers(DataScript.word.vocab);//use new word to get new containers
            }

            //change HP + bar:
            if (hp >= 60)
                ChangeBarColor(0, 255, 0, 200);//green
            else if (hp >= 20)
                ChangeBarColor(255, 230, 0, 200);//orange
            else
                ChangeBarColor(255, 0, 0, 200);// red


            //shoot
            if (Input.GetMouseButton(0))
            {
                Ray ray = camera.ScreenPointToRay(new Vector3(
                    camera.pixelWidth / 2,
                    camera.pixelHeight / 2,
                    0));
                if (laser.Shoot(ray.direction))
                { // can shoot
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
                                bool isCorrect = letter == DataScript.word.vocab[passedLetterCount];
                                OnLetterSelected(isCorrect);
                                lettersContainerController.OnLetterHit(index, isCorrect);
                                if (!isCorrect)
                                {
                                    DropAnimal.shouldDrop = true;
                                    AddHP(-5);
                                    transform.Find("Animal").gameObject.SetActive(true);
                                }
                            }
                            catch (FormatException)
                            {
                                Console.WriteLine($"Unable to parse '{obj.name.Substring(6)}'");
                            }
                        }
                    }
                }
            }
        }
        else if (DataScript.gameState == DataScript.GameState.Succeed)
        {
            SceneManager.LoadScene("GameSuccessScene");
        }
        else if (DataScript.gameState == DataScript.GameState.Fail)
        {
            SceneManager.LoadScene("DeadScene");
        }
    }

    void ChangeBarColor(byte r, byte g, byte b, byte a)
    {
        int w = fullProgressBar.width;
        int h = fullProgressBar.height;
        fullProgressBar = new Texture2D(w, h, TextureFormat.RGBA32, false);
        Color32 barColor = new Color32(r, g, b, a);

        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                fullProgressBar.SetPixel(i, j, barColor);
            }
        }
        fullProgressBar.Apply();

    }

    void AddHP(int changeValue)
    {
        hp += changeValue;
        if (hp < 0)
        {
            hp = 0;
            DataScript.gameState = DataScript.GameState.Fail;
        }
        if (hp > 100)
            hp = 100;
    }

    //Xiaomin: DataScript.wordIdx should be constantly changing, when UpdateWord() is called;
    //we just set word = vocabs_???[DataScript.wordIdx]
    void UpdateWord()
    {
        DataScript.word = vocabs[DataScript.wordIdx];
        Debug.Log("WORD: " + DataScript.word.vocab + ", MEANING:" + DataScript.word.meaning);
    }

    //XM: update DataScript.unit according to wordIdx
    void UpdateUnit()
    {
        DataScript.unit.Clear();
        int startIdx = (DataScript.wordIdx / DataScript.unitSize) * DataScript.unitSize;
        for (int i = startIdx; i < startIdx + DataScript.unitSize; i++)
        {
            DataScript.unit.Add(vocabs[i]);
        }
    }

    void GetGameAttributes()
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
        Debug.Log("At Start, vocabs is set! length = " + vocabs.Count + ", vocabs[0].vocab = " + vocabs[0].vocab);
        UpdateUnit();
        UpdateWord();
    }

    void OnLetterSelected(bool isCorrect)
    {
        if (isCorrect)
        {

            AddHP(2);

            streakLetterCount += 1;
            passedLetterCount += 1;
            if (passedLetterCount == DataScript.word.vocab.Length)
            {
                //Xiaomin: got a whole word correct, receive score:
                streakWordCount += 1;
                DataScript.score += 10;
                passedLetterCount = 0;
                DataScript.passedWordCount += 1;
                DataScript.wordIdx++;
                DataScript.wordIdx %= DataScript.eachVocabListSize;
                Debug.Log("DataScript.wordIdx++, now wordIdx =" + DataScript.wordIdx);


                if (DataScript.score >= DataScript.scoreToWin)
                {
                    DataScript.gameState = DataScript.GameState.Succeed;
                }

                //Xiaomin: got a whole unit:
                if (DataScript.passedWordCount >= DataScript.unitSize)
                {
                    AddHP(10);
                    DataScript.passedUnitCount++;
                    DataScript.passedWordCount = 0;
                    UpdateUnit();//TODO:

                    SceneManager.LoadScene("UnitSuccessScene");
                }
                else


                    UpdateWord();
                gameObject.GetComponent<AudioController>().PlayAudio(DataScript.word.vocab);
            }
        }
        else
        {
            streakLetterCount = 0;
        }
    }

    public bool isGameActive()
    {
        return DataScript.gameState == DataScript.GameState.Started;
    }

    void OnGUI()
    {
        GUI.Label(guiAimPos, "+", guiAimStyle);
        GUI.Label(guiHintPos, DataScript.word.meaning, guiTextStyle);
        GUI.Label(guiLetterPos,
                String.Concat(DataScript.word.vocab.Substring(0, passedLetterCount),
                        new String('_', DataScript.word.vocab.Length - passedLetterCount)),
                guiTextStyle);
        GUI.Label(guiScorePos, "<b>Score: </b>" + DataScript.score, guiScoreStyle);
        GUI.Label(guiUnitCountPos, "Learned " + DataScript.passedUnitCount + " Units", guiScoreStyle);



        GUI.DrawTexture(new Rect(0, 0, (hp / 100f) * progressBarWidth, progressBarHeight), fullProgressBar);
        GUI.DrawTexture(new Rect((hp / 100f) * progressBarWidth, 0, (1 - hp / 100f) * progressBarWidth, progressBarHeight), emptyProgressBar);

        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUI.Label(new Rect(0, 0, 100, 50), string.Format("{0:N0}%", hp), guiHPStyle);
    }
}