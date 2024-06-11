using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : Singleton<LevelManager>
{
    private LevelData _currLevel;
    private LevelData _nextLevel;
    private string _currLevelScenePath;
    private string _nextLevelScenePath;
    public int lastCompletedLevel;

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this)
            return;

        DontDestroyOnLoad(gameObject);

        lastCompletedLevel = 0;
    }

    public void InitAndLaunchLevel(LevelData level)
    {
        SetCurrentLevel(level);
        SetNextLevel(level._nextLevelData);

        OpenLevel(level);
    }

    public void OpenLevel(LevelData levelData)
    {
        SceneManager.LoadScene(_currLevelScenePath);
    }

    public void OpenNextLevel()
    {
        lastCompletedLevel++;

        InitAndLaunchLevel(_nextLevel);

        if (_nextLevel != null)
        {
            if (_nextLevelScenePath != null)
            {
                SceneManager.LoadScene(_nextLevelScenePath);
            }
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }

    public void SetCurrentLevel(LevelData level)
    {
        _currLevel = level;
        _currLevelScenePath = AssetDatabase.GetAssetPath(_currLevel._currLevelScene);
    }

    public void SetNextLevel(LevelData level)
    {
        if (level != null)
        {
            _nextLevel = level;
            _nextLevelScenePath = AssetDatabase.GetAssetPath(_nextLevel._currLevelScene);
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PauseLevel()
    {
        Time.timeScale = 0.0f;
    }

    public void ResumeLevel()
    {
        Time.timeScale = 1.0f;
    }
}
