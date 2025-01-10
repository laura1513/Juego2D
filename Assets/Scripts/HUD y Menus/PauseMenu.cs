using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour

/*
 * CUALQUIER DUDA MIRAR ESTE VIDEO :
 * -->> https://youtu.be/JivuXdrIHK0?si=aT7FQpWOi4bmto_6 <<--
 * 
 */
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;


    void Start()
    {
        pauseMenuUI.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        GameIsPaused = false;
        Debug.Log("MainMenu");
    }
    public void Quitgame()
    {
        Debug.Log("Salir");
        Application.Quit();
    }
}
