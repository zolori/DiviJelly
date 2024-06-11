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
        if (_levelManagerInstance == null)
            return;

        _levelManagerInstance.InitAndLaunchLevel(level);
    }

    public void OpenNextLevelUpdater()
    {
        if (_levelManagerInstance == null)
            return;

        _levelManagerInstance.OpenNextLevel();
    }

    public void RestartLevelUpdater()
    {
        _levelManagerInstance.RestartLevel();
    }
}
