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

	private List<JellyEntity> m_CollidedJellies = new List<JellyEntity>();

	private float m_LastSplitTime = 0f;

	private void Awake()
	{
		InputMaster.Instance.InputAction.Jellys.Interact.performed += _ => SplitJellies();
	}

	private void SplitJellies()
	{
		if(m_IsLimited && m_NbActivationLimit <= 0)
			return;

		if(Time.time - m_LastSplitTime < m_RecoveryTime)
			return;

		bool hasSplit = false;
		foreach(JellyEntity jelly in m_CollidedJellies)
		{
			if(!jelly.CanMove())
				continue;

			if(jelly.Split())
				hasSplit = true;
		}

		if(hasSplit)
			m_LastSplitTime = Time.time;
	}

	private void OnTriggerEnter2D(Collider2D iCollision)
	{
		JellyEntity jelly;
		if(!iCollision.gameObject.TryGetComponent(out jelly))
			return;

		m_CollidedJellies.Add(jelly);
	}

	private void OnTriggerExit2D(Collider2D iCollision)
	{
		JellyEntity jelly;
		if(!iCollision.gameObject.TryGetComponent(out jelly))
			return;

		m_CollidedJellies.Remove(jelly);
	}
}
