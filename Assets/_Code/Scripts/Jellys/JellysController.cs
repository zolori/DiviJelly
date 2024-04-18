using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JellysController : MonoBehaviour
{
	private int m_FlavoursCount;
	private Flavour m_ControlledFlavour;
	private int m_ControlledFlavourIndex = 0;

	private List<JellyEntity> _GetControlledJellies()
	{
		List<JellyEntity> jellies;
		bool success = JellysManager.Instance.m_Jellies.TryGetValue(m_ControlledFlavour, out jellies);
		if(!success)
			jellies = new List<JellyEntity>();

		if(jellies.Count <= 0)
			Debug.LogError($"Could not find jellies of type {m_ControlledFlavour}");

		return jellies;
	}

	private void _UpdateControlledFlavour()
	{
		m_FlavoursCount = JellysManager.Instance.m_Flavours.Count;
		if(m_FlavoursCount == 0)
		{
			Debug.LogWarning("No jelly flavour registered");
			return;
		}

		foreach(JellyEntity jelly in _GetControlledJellies())
			jelly.SetCanMove(false);

		if(m_ControlledFlavourIndex < 0)
			m_ControlledFlavourIndex = m_FlavoursCount - 1;
		if(m_ControlledFlavourIndex >= m_FlavoursCount)
			m_ControlledFlavourIndex = 0;
		m_ControlledFlavour = (Flavour)m_ControlledFlavourIndex;

		foreach(JellyEntity jelly in _GetControlledJellies())
			jelly.SetCanMove(true);
	}

	public void OnNextFlavour()
	{
		m_ControlledFlavourIndex++;
		_UpdateControlledFlavour();
	}

	public void OnPrevFlavour()
	{
		m_ControlledFlavourIndex--;
		_UpdateControlledFlavour();
	}

	public void OnHorizontalMovement(InputValue iInputValue)
	{
		List<JellyEntity> jellies = _GetControlledJellies();
		float movementInputValue = iInputValue.Get<float>();
		foreach(JellyEntity jelly in jellies)
			jelly.SetMovementInputValue(movementInputValue);
	}

	public void OnJump()
	{
		List<JellyEntity> jellies = _GetControlledJellies();
		foreach(JellyEntity jelly in jellies)
			jelly.Jump();
	}
}
