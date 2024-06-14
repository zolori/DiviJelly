using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
	private LevelData _currLevel;
	private LevelData _nextLevel;
	private int _currLevelSceneIndex = -1;
	private int _nextLevelSceneIndex = -1;

	private ScoreRW m_ScoreIO;

	protected override void Awake()
	{
		base.Awake();
		if(Instance != this)
			return;

		DontDestroyOnLoad(gameObject);

		m_ScoreIO = new ScoreRW();
	}

	protected override void OnDestroy()
	{
		if(Instance != this)
			return;

		base.OnDestroy();

		m_ScoreIO.WriteScores();
	}

	public void InitAndLaunchLevel(LevelData level)
	{
		SetCurrentLevel(level);
		SetNextLevel(level._nextLevelData);

		OpenLevel(level);
	}

	public void OpenLevel(LevelData levelData)
	{
		SceneManager.LoadScene(_currLevelSceneIndex);
	}

	public void OpenNextLevel()
	{
		if(_nextLevel != null)
			InitAndLaunchLevel(_nextLevel);
		else
			SceneManager.LoadScene(1);
	}

	public void SetCurrentLevel(LevelData level)
	{
		_currLevel = level;
		_currLevelSceneIndex = _currLevel._currLevelSceneIndex;
	}

	public void SetNextLevel(LevelData level)
	{
		_nextLevel = level;
		if(_nextLevel != null)
			_nextLevelSceneIndex = _nextLevel._currLevelSceneIndex;
		else
			_nextLevelSceneIndex = -1;
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

	///////////////////
	// Scores
	///////////////////

	// negative value means not unlocked
	// otherwise, return best score achieved for this level
	public int GetScore(LevelData iLevel)
	{
		if(iLevel == null)
			return -1;

		return m_ScoreIO.GetScore(iLevel._world - 1, iLevel._level - 1);
	}

	// Idx of the first level of the first world is 0, 0
	public int GetScore(int iWorldIdx, int iLevelIdx)
	{
		return m_ScoreIO.GetScore(iWorldIdx, iLevelIdx);
	}

	public void SetScore(LevelData iLevel, int iScore)
	{
		if(iLevel == null)
		{
			Debug.LogError("Trying to set score for null level");
			return;
		}

		m_ScoreIO.SetScore(iLevel._world - 1, iLevel._level - 1, iScore);
	}

	// Idx of the first level of the first world is 0, 0
	public void SetScore(int iWorldIdx, int iLevelIdx, int iScore)
	{
		m_ScoreIO.SetScore(iWorldIdx, iLevelIdx, iScore);
	}

	public void SetCurrentLevelScore(int iScore)
	{
		if(_currLevel == null)
		{
			Debug.LogError("No current level");
			return;
		}

		SetScore(_currLevel, iScore);
	}

}
