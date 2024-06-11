using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScoreRW
{
	[Serializable]
	public class ScoreData
	{
		[Serializable]
		public class WorldData
		{
			public List<int> Data = new List<int>();
		}

		public List<WorldData> Data = new List<WorldData>();
	}

	private const string s_FileName = "save.dat";
	private ScoreData m_Scores = new ScoreData();

	public ScoreRW()
	{
		ReadScores();
	}

	// negative value means not unlocked
	// otherwise, return best score achieved for this level
	public int GetScore(int iWorldIdx, int iLevelIdx)
	{
		if(iWorldIdx == m_Scores.Data.Count && iLevelIdx == 0)
			return 0;

		if(iWorldIdx >= m_Scores.Data.Count)
			return -1;

		List<int> worldScores = m_Scores.Data[iWorldIdx].Data;

		if(iLevelIdx == worldScores.Count)
			return 0;
		if(iLevelIdx > worldScores.Count)
			return -1;

		return worldScores[iLevelIdx];
	}

	public void SetScore(int iWorldIdx, int iLevelIdx, int iScore)
	{
		for(int worldIdx = m_Scores.Data.Count; worldIdx <= iWorldIdx; worldIdx++)
			m_Scores.Data.Add(new ScoreData.WorldData());

		List<int> worldScores = m_Scores.Data[iWorldIdx].Data;
		for(int levelIdx = worldScores.Count; levelIdx <= iLevelIdx; levelIdx++)
			worldScores.Add(-1);

		worldScores[iLevelIdx] = iScore;
	}

	public void WriteScores()
	{
		string scoreJson = JsonUtility.ToJson(m_Scores);

		string filePath = Path.Combine(Application.persistentDataPath, s_FileName);
		try
		{
			File.WriteAllText(filePath, scoreJson);
		}
		catch(Exception e)
		{
			Debug.LogError($"Failed to write scores with exception {e}");
		}
	}

	private void ReadScores()
	{
		string filePath = Path.Combine(Application.persistentDataPath, s_FileName);
		try
		{
			string scoreJson = File.ReadAllText(filePath);
			m_Scores = JsonUtility.FromJson<ScoreData>(scoreJson);
		}
		catch(Exception e)
		{
			Debug.LogWarning($"Failed to read from score with exception {e}");
		}

	}
}
