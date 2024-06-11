using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpdater : MonoBehaviour
{
    LevelManager _levelManagerInstance;

    private void Start()
    {
        _levelManagerInstance = LevelManager.Instance;
    }

    public void InitAndLaunchLevelUpdater(LevelData level)
    {
        _levelManagerInstance?.InitAndLaunchLevel(level);
    }

    public void OpenNextLevelUpdater()
    {
        _levelManagerInstance?.OpenNextLevel();
    }

    public void RestartLevelUpdater()
    {
        _levelManagerInstance.RestartLevel();
    }

    public void PauseLevelUpdater()
    {
        _levelManagerInstance?.PauseLevel();
    }

    public void ResumeLevelUpdater()
    {
        _levelManagerInstance?.ResumeLevel();
    }
}
