using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {

    private void Awake()
    {
        Time.timeScale = 1;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("hw");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LevelTwo()
    {
        SceneManager.LoadScene("hw_2");
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("hwMenu");
    }
}
