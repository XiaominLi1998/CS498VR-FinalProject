using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (DataScript.GamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }

        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;//game time back to normal
        DataScript.GamePaused = false;
    }
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;//game stopped!
        DataScript.GamePaused = true;
    }

    public void BackToMenu()
    {
        // Debug.Log("Back to menu!");
        Time.timeScale = 1f;//game time back to normal
        SceneManager.LoadScene("MenuScene");
    }

    //XM: user can use this to jump back to memorize scene
    public void Review()
    {
        DataScript.isLearnMode = false;
        SceneManager.LoadScene("MemorizeScene");
    }

    public void QuitGame()
    {
        Debug.Log("Quit game!");
        Application.Quit();
    }

}
