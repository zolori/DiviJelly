using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CameraTrigger : MonoBehaviour
{
	[SerializeField] private Flavours m_Flavours;

	private List<GameObject> m_Jellies = new List<GameObject>();

	private CameraBehaviour m_Camera;
	bool m_IsActive = false;

	private void Start()
	{
		m_Camera = Camera.main.GetComponent<CameraBehaviour>();
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
			if(m_Jellies.Count > 0)
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
		if(iCollision.GetComponent<JellyEntity>())
			m_Jellies.Add(iCollision.gameObject);
	}

	private void OnTriggerExit2D(Collider2D iCollision)
	{
		m_Jellies.Remove(iCollision.gameObject);
	}
}
