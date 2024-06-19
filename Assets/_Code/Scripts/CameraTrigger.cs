using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CameraTrigger : MonoBehaviour
{
	[SerializeField] private Flavours m_Flavours;

	private List<JellyEntity> m_Jellies = new List<JellyEntity>();

	private CameraBehaviour m_Camera;
	private JelliesController m_JelliesController;
	bool m_IsActive = false;

	private void Start()
	{
		m_Camera = Camera.main.GetComponent<CameraBehaviour>();
		m_JelliesController = FindAnyObjectByType<JelliesController>();
		if(m_Camera == null)
		{
			Debug.LogWarning("No camera behaviour");
			return;
		}

		StartCoroutine(_UpdateTrigger());
	}

	private IEnumerator _UpdateTrigger()
	{
		var delay = new WaitForSeconds(0.1f);
		while(true)
		{
			// cleaning list
			m_Jellies.RemoveAll(jelly => jelly == null);
			bool shouldActivate = m_Jellies.Count(jelly => jelly.GetFlavour() == m_JelliesController.GetCurrentControlledFlavour()) > 0;
			if(shouldActivate)
			{
				if(!m_IsActive)
				{
					m_IsActive = true;
					m_Camera.AddTargetPoint(transform.position);
				}
			}
			else
			{
				if(m_IsActive)
				{
					m_IsActive = false;
					m_Camera.RemoveTargetPoint(transform.position);
				}
			}

			yield return delay;
		}
	}

	private void OnTriggerEnter2D(Collider2D iCollision)
	{
		JellyEntity jelly = iCollision.GetComponent<JellyEntity>();
		if(jelly)
			m_Jellies.Add(jelly);
	}

	private void OnTriggerExit2D(Collider2D iCollision)
	{
		JellyEntity jelly = iCollision.GetComponent<JellyEntity>();
		if(jelly)
			m_Jellies.Remove(jelly);
	}
}
