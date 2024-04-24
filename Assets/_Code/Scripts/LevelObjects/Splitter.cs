using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splitter : MonoBehaviour
{
	[SerializeField] private bool m_IsLimited = false;
	[ShowIf("m_IsLimited")]
	[SerializeField] private int m_NbActivationLimit = 1;

	[SerializeField] private float m_RecoveryTime = 1f;

	private float m_LastSplitTime = 0f;

	private void OnTriggerEnter2D(Collider2D iCollision)
	{
		if(m_IsLimited && m_NbActivationLimit <= 0)
			return;

		if(Time.time - m_LastSplitTime < m_RecoveryTime)
			return;

		JellyEntity jelly;
		if(!iCollision.gameObject.TryGetComponent(out jelly))
			return;

		if(jelly.Split())
			m_LastSplitTime = Time.time;
	}
}
