using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    public static GameManager Instance => instance;

    enum GameStateEnum
    {
        Playing,
        Paused,
        Win,
        Idle
    }

    private GameStateEnum GameState { get; set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);

        GameState = GameStateEnum.Playing;
    }

    private void DisplayWinScreen()
    {
        GameObject winMenu = GameObject.FindWithTag("WinMenu");

        if (winMenu != null)
        {
            winMenu.SetActive(true);
            GameState = GameStateEnum.Idle;
        }
    }
}