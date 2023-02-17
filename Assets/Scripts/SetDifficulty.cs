using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDifficulty : MonoBehaviour
{
    // Start is called before the first frame update
    public static void SetDifficultyLevel(string level)
    {
        Debug.Log("set diffculty invoked!");
        DataScript.difficultyLevel = level;
    }
}
