using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JelliesController : MonoBehaviour
{
	private int m_FlavoursCount;
	private FlavourData m_ControlledFlavour;
	private int m_ControlledFlavourIndex = 0;

	[SerializeField] private Flavours m_Flavours;
	[SerializeField] private SpriteRenderer m_HUDControlledFlavour;
	[SerializeField] private SpriteRenderer m_HUDNextControlledFlavour;
	[SerializeField] private SpriteRenderer m_HUDPrevControlledFlavour;

	private JelliesManager m_JelliesManager;

	private float m_MovementInputValueCache = 0;

	private void Awake()
	{
		m_JelliesManager = JelliesManager.Instance;
		m_JelliesManager.OnAvailableFlavourChange += UpdateAvailableFlavours;
	}

	public Flavour GetCurrentControlledFlavour()
	{
		return m_ControlledFlavour.Flavour;
	}

	private List<JellyEntity> _GetControlledJellies()
	{
		List<JellyEntity> jellies;
		bool success = m_JelliesManager.m_Jellies.TryGetValue(GetCurrentControlledFlavour(), out jellies);
		if(!success)
			jellies = new List<JellyEntity>();

		if(jellies.Count <= 0)
			Debug.LogWarning($"Could not find jellies of type {GetCurrentControlledFlavour()}");

		return jellies;
	}

	private void _UpdateHUD()
	{
		if(m_FlavoursCount <= 0)
			return;

		_UpdateHUDSpriteWithFlavour(m_HUDControlledFlavour, m_ControlledFlavour);
		_UpdateHUDSpriteWithFlavour(m_HUDNextControlledFlavour, GetFlavourData(FixFlavourIndex(m_ControlledFlavourIndex + 1)));
		_UpdateHUDSpriteWithFlavour(m_HUDPrevControlledFlavour, GetFlavourData(FixFlavourIndex(m_ControlledFlavourIndex - 1)));
	}

	static private void _UpdateHUDSpriteWithFlavour(SpriteRenderer iSpriteRenderer, FlavourData iFlavourData)
	{
		if(iSpriteRenderer == null)
			return;

		iSpriteRenderer.sprite = iFlavourData.Sprite;
		iSpriteRenderer.color = iFlavourData.Color;
		iSpriteRenderer.sprite = iFlavourData.Sprite;
	}

	public void UpdateAvailableFlavours()
	{
		m_FlavoursCount = m_JelliesManager.m_Flavours.Count;
		_UpdateControlledFlavour();
	}

	private int FixFlavourIndex(int iIndex)
	{
		if(m_FlavoursCount == 0 || iIndex >= m_FlavoursCount)
			return 0;
		if(iIndex < 0)
			return m_FlavoursCount - 1;
		return iIndex;
	}

	private FlavourData GetFlavourData(int iFlavourIndex)
	{
		Flavour flavour = m_JelliesManager.m_Flavours[iFlavourIndex];
		return m_Flavours.Data.Find(iFlavour => iFlavour.Flavour == flavour);
	}

	private void _UpdateControlledFlavour()
	{
		if(m_FlavoursCount <= 0)
		{
			Debug.LogWarning("No jelly flavour registered");
			return;
		}

		_UpdateHUD();

		List<JellyEntity> prevJellies = _GetControlledJellies();
		Flavour prevFlavour = GetCurrentControlledFlavour();

		m_ControlledFlavourIndex = FixFlavourIndex(m_ControlledFlavourIndex);
		m_ControlledFlavour = GetFlavourData(m_ControlledFlavourIndex);

		if(GetCurrentControlledFlavour() == prevFlavour)
			return;

		foreach(JellyEntity jelly in prevJellies)
		{
			jelly.SetCanMove(false);
			jelly.SetMovementInputValue(0);
		}
		foreach(JellyEntity jelly in _GetControlledJellies())
		{
			jelly.SetCanMove(true);
			jelly.SetMovementInputValue(m_MovementInputValueCache);
		}
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
		m_MovementInputValueCache = iInputValue.Get<float>();
		foreach(JellyEntity jelly in jellies)
			jelly.SetMovementInputValue(m_MovementInputValueCache);
	}

	public void OnJump()
	{
		List<JellyEntity> jellies = _GetControlledJellies();
		foreach(JellyEntity jelly in jellies)
			jelly.Jump();
	}
}
