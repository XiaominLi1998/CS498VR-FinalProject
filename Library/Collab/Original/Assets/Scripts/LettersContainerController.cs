using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LettersContainerController : MonoBehaviour
{
    private GameController gameController;

    /* If letter blocks are on the surface of a rotating cylinder */
    public float rotateSpeed = 4.0f;

    public float radius = 50f;
    public float layerDistance = 10f;
    public static int letterCountPerLayer = 12;
    public static int layerCount = 4;

    public static int letterCountTotal = layerCount * letterCountPerLayer;

    public string word;

    private List<LetterWrapper> letterList;

    void Start()
    {
        gameController = this.transform.parent.GetComponent<GameController>();
        letterList = new List<LetterWrapper>();
        InitLetterObjects(layerCount * letterCountPerLayer);
    }

    public void UpdateContainers(string newWord)
    {
        word = newWord;
        char[] rawLetterList = GetLetterArray(word, letterCountTotal);

        int i = 0;
        foreach (LetterWrapper letterWrapper in letterList)
        {
            letterWrapper.letter = rawLetterList[i];
            letterWrapper.text.text = rawLetterList[i].ToString();
            letterWrapper.text.color = Color.black;
            i++;
        }

    }

    public void ClearLetters()
    {
        foreach (LetterWrapper letterWrapper in letterList)
        {
            letterWrapper.letter = ' ';
            letterWrapper.text.text = "";
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!gameController.isGameActive()) return;
        transform.Rotate(0.0f, rotateSpeed * Time.fixedDeltaTime, 0.0f);
    }

    public char GetLetter(int index)
    {
        return letterList[index].letter;
    }

    public void OnLetterHit(int index, bool isCorrect)
    {
        if (isCorrect)
        {
            letterList[index].text.color = Color.green;
            letterList[index].collider.enabled = false;
        }
        else
        {
            letterList[index].text.color = Color.red;
        }
    }

    public void InitLetterObjects(int letterCount)
    {
        char[] rawLetterList = GetLetterArray(word, letterCount);
        for (int i = 0; i < layerCount; i++)
        {
            for (int j = 0; j < letterCountPerLayer; j++)
            {
                // Debug.Log("AddLetterObject : " + rawLetterList[i * letterCountPerLayer + j]);
                AddLetterObject(rawLetterList[i * letterCountPerLayer + j],
                    new Vector3(
                        (float)Math.Cos(j * 2 * Math.PI / letterCountPerLayer) * radius,
                        (i + 1) * layerDistance,
                        (float)Math.Sin(j * 2 * Math.PI / letterCountPerLayer) * radius)
                );
            }
        }
    }

    char[] GetLetterArray(string word, int letterCount)
    {
        System.Random random = new System.Random();
        char[] array = new char[letterCount];
        int i, j;
        char tmp;
        for (i = 0; i < word.Length; i++)
        {
            array[i] = word[i];
        }
        for (i = word.Length; i < letterCount; i++)
        {
            array[i] = (char)('a' + random.Next(0, 26));
        }

        // shuffle
        for (i = letterCount - 1; i > 0; i--)
        {
            j = random.Next(0, i + 1);
            tmp = array[i];
            array[i] = array[j];
            array[j] = tmp;

        }
        return array;
    }

    void AddLetterObject(char letter, Vector3 pos)
    {
        GameObject letterObj = new GameObject();
        letterObj.name = "LetterObject";
        letterObj.transform.parent = transform;
        letterObj.transform.position = pos;
        letterObj.transform.LookAt(transform);
        letterObj.transform.Rotate(0f, 180f, 0f);
        letterObj.transform.localScale = new Vector3(3f, 3f, 3f);
        Canvas canvas = letterObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        CanvasScaler cs = letterObj.AddComponent<CanvasScaler>();
        cs.scaleFactor = 1f;
        cs.dynamicPixelsPerUnit = 10f;

        GameObject textObj = new GameObject();
        textObj.name = textObj.GetInstanceID().ToString();
        textObj.transform.parent = letterObj.transform;
        textObj.transform.localPosition = new Vector3(0f, 0f, 0f);
        textObj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        Text text = textObj.AddComponent<Text>();
        textObj.GetComponent<RectTransform>().
            SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 3.0f);
        textObj.GetComponent<RectTransform>().
            SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 3.0f);
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        Font arialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        text.font = arialFont;
        text.fontSize = 3;
        text.text = Char.ToString(letter);
        text.enabled = true;
        text.color = Color.black;

        GameObject brickObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        brickObj.transform.parent = letterObj.transform;
        brickObj.transform.localPosition = new Vector3(0f, 0f, 1.1f);
        brickObj.transform.localScale = new Vector3(2f, 2f, 2f);
        brickObj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        brickObj.layer = 8;
        BoxCollider boxCollider = brickObj.AddComponent<BoxCollider>();

        LetterWrapper letterWrapper = new LetterWrapper();
        letterWrapper.gameObject = letterObj;
        letterWrapper.letter = letter;
        letterWrapper.text = text;
        letterWrapper.collider = boxCollider;
        letterList.Add(letterWrapper);
        brickObj.name = "Letter" + (letterList.Count - 1).ToString();
    }

}

public class LetterWrapper
{
    public GameObject gameObject;
    public char letter;
    public Text text;
    public Collider collider;
}