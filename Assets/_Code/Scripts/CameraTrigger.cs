using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CameraTrigger : MonoBehaviour
{
	[SerializeField] private Flavours m_Flavours;

	private CameraBehaviour m_Camera;

	private bool m_IsActive = false;
	private bool m_RequestActive = false;

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
			if(m_RequestActive)
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
			m_RequestActive = false;

			yield return delay;
		}
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		m_RequestActive = true;
	}
}
