using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class CameraBehaviour : MonoBehaviour
{
	[SerializeField] private Vector2 m_TargetBBoxSize;
	private Vector2 m_InvTargetBBoxSize;
	[SerializeField] private float m_MinZoom = 5;
	[SerializeField] private float m_MinSpeed = 0.02f;
	[SerializeField] private float m_DistanceSpeedCoef = 0.05f;

	private Camera m_Camera;
	private JelliesController m_JelliesController;
	private JelliesManager m_JelliesManager;

	private List<Vector2> m_TargetPoints = new List<Vector2>();

	private void Awake()
	{
		m_Camera = Camera.main;
		m_JelliesController = FindFirstObjectByType<JelliesController>();
		m_JelliesManager = JelliesManager.Instance;
		m_InvTargetBBoxSize = new Vector2(1 / m_TargetBBoxSize.x, 1 / m_TargetBBoxSize.y);
	}

	private void FixedUpdate()
	{
		if(m_JelliesController == null || m_JelliesManager == null)
			return;

		Rect bBox = m_JelliesManager.GetBBoxForFlavour(m_JelliesController.GetCurrentControlledFlavour());
		Vector2 minPos = bBox.min;
		Vector2 maxPos = bBox.max;
		foreach(Vector2 targetPoint in m_TargetPoints)
		{
			minPos.x = Mathf.Min(minPos.x, targetPoint.x);
			maxPos.x = Mathf.Max(maxPos.x, targetPoint.x);
			minPos.y = Mathf.Min(minPos.y, targetPoint.y);
			maxPos.y = Mathf.Max(maxPos.y, targetPoint.y);
		}
		bBox.min = minPos;
		bBox.max = maxPos;

		Vector3 newPos = bBox.center;
		newPos.z = transform.position.z;
		float distance = (newPos - transform.position).magnitude;
		transform.position = Vector3.MoveTowards(transform.position, newPos, distance * m_DistanceSpeedCoef + m_MinSpeed);

		float targetCamWidth = bBox.size.x * m_InvTargetBBoxSize.x;
		float targetCamHeight = bBox.size.y * m_InvTargetBBoxSize.y;
		float newZoom = Mathf.Max(m_MinZoom, Mathf.Max(targetCamWidth / m_Camera.aspect, targetCamHeight)) * 0.5f;
		float zoomDelta = Mathf.Abs(newZoom - m_Camera.orthographicSize);
		m_Camera.orthographicSize = Mathf.MoveTowards(m_Camera.orthographicSize, newZoom, zoomDelta * m_DistanceSpeedCoef + m_MinSpeed);
	}

	public void AddTargetPoint(Vector2 iTargetPoint)
	{
		m_TargetPoints.Add(iTargetPoint);
	}

	public void RemoveTargetPoint(Vector2 iTargetPoint)
	{
		m_TargetPoints.Remove(iTargetPoint);
	}

	private void OnDestroy()
	{
		m_JelliesManager = null;
		m_JelliesController = null;
	}
}
