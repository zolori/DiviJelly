using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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