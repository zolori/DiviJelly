using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTrigger : MonoBehaviour
{
	private bool m_HasBeenReached = false;
	private int m_FinalScore = 0;

	public Action OnEndReached;

	private void Start()
	{

	}

	private void OnTriggerEnter2D(Collider2D iCollision)
	{
		if(m_HasBeenReached)
			return;

		JellyEntity jelly;
		if(!iCollision.gameObject.TryGetComponent(out jelly))
			return;

		m_FinalScore = Mathf.CeilToInt(jelly.GetVolume() * 100);
		m_HasBeenReached = true;
		Debug.Log($"End reached! Score: {m_FinalScore}");

		WinMenuBehaviour m_WinMenu = FindFirstObjectByType<WinMenuBehaviour>(FindObjectsInactive.Include);
		if(m_WinMenu == null)
		{
			Debug.LogError("End menu not found.");
			return;
		}

		m_WinMenu.gameObject.SetActive(true);
		m_WinMenu.SetScore(m_FinalScore);
	}

	public int GetScore()
	{
		return m_FinalScore;
	}
}
