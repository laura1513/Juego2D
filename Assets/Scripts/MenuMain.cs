using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


public class MenuMain : MonoBehaviour
{
    public void SelectLevel()
    {
        SceneManager.LoadSceneAsync(1);
    }
    public void GoMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }
    public void Level1()
    {
        SceneManager.LoadSceneAsync(3);
    }
    public void Level2()
    {
        SceneManager.LoadSceneAsync(4);
    }
    public void Level3()
    {
        SceneManager.LoadSceneAsync(5);
    }
    public void Level4()
    {
        SceneManager.LoadSceneAsync(6);
    }
    public void Level5()
    {
        SceneManager.LoadSceneAsync(7);
    }
    public void Level6()
    {
        SceneManager.LoadSceneAsync(8);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
