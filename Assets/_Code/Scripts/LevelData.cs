using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Levels", order = 1)]
public class LevelData : ScriptableObject
{
    public string _name;
    public int _world;
    public int _level;
    public int _score;
    public int _starCount;
    public SceneAsset _currLevelScene;
    public LevelData _nextLevelData;
}
