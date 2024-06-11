using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Levels", order = 1)]
public class LevelData : ScriptableObject
{
	public string _name;
	public int _world;
	public int _level;
	public SceneAsset _currLevelScene;
	public LevelData _nextLevelData;
}
