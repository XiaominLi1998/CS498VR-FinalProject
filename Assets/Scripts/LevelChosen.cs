using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelChosen : MonoBehaviour
{
    public void DifficultyLevelChosen(string level)
    {
        DataScript.InitData();
        DataScript.difficultyLevel = level;
        DataScript.isLearnMode = true;
        SceneManager.LoadScene("MemorizeScene");        
    }
}
