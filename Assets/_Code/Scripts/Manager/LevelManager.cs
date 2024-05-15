using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    private LevelData _currLevel;
    private LevelData _nextLevel;
    private string _currLevelScenePath;
    private string _nextLevelScenePath;

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this)
            return;

        DontDestroyOnLoad(gameObject);
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
}