using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    // Start is called before the first frame update
    public void ChangeToScene(string sceneName)
    {
        Debug.Log("Now change to scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

}
