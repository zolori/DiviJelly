using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OpenMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void OpenWorldSelection()
    {
        SceneManager.LoadScene(1);
    }

    public void OpenLevelSelectionMenu()
    {
        SceneManager.LoadScene(2);
    }

    public void OpenLevelWithNumber(int number)
    {
        SceneManager.LoadScene(number + 2);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
