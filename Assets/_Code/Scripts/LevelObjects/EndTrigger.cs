using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTrigger : MonoBehaviour
{
	private bool m_HasBeenReached = false;
	private int m_FinalScore = 0;

	public Action OnEndReached;

	private void OnTriggerEnter2D(Collider2D iCollision)
	{
		if(m_HasBeenReached)
			return;

		JellyEntity jelly;
		if(!iCollision.gameObject.TryGetComponent(out jelly))
			return;

		Vector3 jellyPos = jelly.transform.position;
		Flavour jellyFlavour = jelly.GetFlavour();
		m_FinalScore = Mathf.CeilToInt(jelly.GetVolume() * 100);
		JellyEntity[] jellies = FindObjectsOfType<JellyEntity>();
		foreach(JellyEntity otherJelly in jellies)
		{
			otherJelly.SetFlavour(jellyFlavour);
			jelly.Merge(otherJelly);
		}

		jelly.transform.position = jellyPos;
		m_HasBeenReached = true;

		Debug.Log($"End reached! Score: {m_FinalScore}");
	}

	public int GetScore()
	{
		return m_FinalScore;
	}
}
