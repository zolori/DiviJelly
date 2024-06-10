using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMenuBehavior : MonoBehaviour
{
    private LevelManager _levelManagerInstance;

    public List<Button> buttons;
    private int _lastCompletedLevel;

    // Start is called before the first frame update
    void Start()
    {
        _levelManagerInstance = LevelManager.Instance;

        _lastCompletedLevel = _levelManagerInstance.lastCompletedLevel;

        SetButtonsInteractability();
    }

    private void SetButtonsInteractability()
    {
        for (int i = 0; i <= _lastCompletedLevel || i < buttons.Count; i++)
        {
            buttons[i].interactable = true;
        }

        for (int i = _lastCompletedLevel + 1; i < buttons.Count; i++)
        {
            buttons[i].interactable = false;
        }
    }
}
