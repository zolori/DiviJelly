using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinMenuBehaviour : MonoBehaviour
{
	[SerializeField] private Sprite m_FullStar;
	[SerializeField] private Sprite m_EmptyStar;

	[Serializable]
	struct ScoreStar
	{
		public int Score;
		public Image Image;
	}

	[SerializeField] private List<ScoreStar> m_Stars = new List<ScoreStar>();
	[SerializeField] private TextMeshProUGUI m_ScoreTxt;
	private int m_Score = 0;

	public void SetScore(int iScore)
	{
		m_Score = iScore;
		LevelManager.Instance.SetCurrentLevelScore(iScore);
		m_ScoreTxt.text = m_Score.ToString();
		UpdateStars();
	}

	private void UpdateStars()
	{
		foreach(ScoreStar scoreStar in m_Stars)
			scoreStar.Image.sprite = (m_Score >= scoreStar.Score ? m_FullStar : m_EmptyStar);
	}
}
