using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMenuBehavior : MonoBehaviour
{
	private LevelManager _levelManagerInstance;

	[Serializable]
	private struct LevelButton
	{
		public Button Button;
		public Image Star1;
		public Image Star2;
		public Image Star3;
	}

	[SerializeField] private List<LevelButton> m_LevelButtons;
	[SerializeField] private Sprite m_CompleteStar;
	[SerializeField] private Sprite m_EmptyStar;

	void Start()
	{
		_levelManagerInstance = LevelManager.Instance;

		SetButtonsInteractability();
	}

	private void SetButtonsInteractability()
	{
		for(int i = 0; i < m_LevelButtons.Count; i++)
		{
			LevelButton levelButton = m_LevelButtons[i];
			int levelScore = _levelManagerInstance.GetScore(0, i);

			if(levelScore < 0)
			{
				levelButton.Button.interactable = false;
				levelButton.Star1.enabled = false;
				levelButton.Star2.enabled = false;
				levelButton.Star3.enabled = false;
				continue;
			}

			levelButton.Button.interactable = true;
			levelButton.Star1.enabled = true;
			levelButton.Star2.enabled = true;
			levelButton.Star3.enabled = true;
			levelButton.Star1.sprite = levelScore >= 50 ? m_CompleteStar : m_EmptyStar;
			levelButton.Star2.sprite = levelScore >= 70 ? m_CompleteStar : m_EmptyStar;
			levelButton.Star3.sprite = levelScore >= 90 ? m_CompleteStar : m_EmptyStar;
		}
	}
}
