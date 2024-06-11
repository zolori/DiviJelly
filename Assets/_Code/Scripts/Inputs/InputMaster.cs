using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputMaster : Singleton<InputMaster>
{
	private InputsActions inputAction;
	public InputsActions InputAction
	{
		get => inputAction;
	}

	private InputControlScheme controlScheme;
	public InputControlScheme ControlScheme
	{
		get => controlScheme;
	}

	public Action OnControlSchemeChanged;
	public Action OnRebind;

	private const string s_InputBindingPlayerPrefKey = "inputBindings";

	private class InputObserver : IObserver<InputControl>
	{
		InputMaster m_InputMaster;
		public InputObserver(InputMaster iInputMaster)
		{
			m_InputMaster = iInputMaster;
		}

		public void OnCompleted()
		{
		}

		public void OnError(Exception error)
		{
		}

		public void OnNext(InputControl iInputControl)
		{
			m_InputMaster._RefreshControlSchemeWithLastInput(iInputControl);
		}
	}


	protected override void Awake()
	{
		base.Awake();
		if(Instance != this)
			return;

		DontDestroyOnLoad(gameObject); // need it because of "onAnyButtonPress" action that we can't unsubscribe

		InitInputActions();

		InputSystem.onDeviceChange += _RefreshControlSchemeFromDeviceChange;
		InputSystem.onAnyButtonPress.Subscribe(new InputObserver(this));

		// try for every devices to set control scheme
		foreach(InputDevice device in InputSystem.devices.Reverse())
			SetControlSchemeFromDevice(device);
	}

	private void _RefreshControlSchemeFromDeviceChange(InputDevice iDevice, InputDeviceChange iDeviceChange)
	{
		switch(iDeviceChange)
		{
			case InputDeviceChange.Added:
			case InputDeviceChange.ConfigurationChanged:
				SetControlSchemeFromDevice(iDevice);
				break;
		}
	}

	private void _RefreshControlSchemeWithLastInput(InputControl iInputControl)
	{
		SetControlSchemeFromDevice(iInputControl.device);
	}

	private void SetControlSchemeFromDevice(InputDevice iDevice)
	{
		foreach(InputControlScheme controlScheme in inputAction.controlSchemes)
		{
			if(controlScheme.SupportsDevice(iDevice))
			{
				SetControlScheme(controlScheme);
				return;
			}
		}
	}

	private void SetControlScheme(InputControlScheme iInputControlScheme)
	{
		if(controlScheme.Equals(iInputControlScheme))
			return;

		controlScheme = iInputControlScheme;
		OnControlSchemeChanged?.Invoke();
	}

	private void InitInputActions()
	{
		inputAction = new InputsActions();
		string overrideJson = PlayerPrefs.GetString(s_InputBindingPlayerPrefKey, "");
		inputAction.LoadBindingOverridesFromJson(overrideJson);
		inputAction.Enable();
	}

	private void SaveInputActions()
	{
		if(inputAction == null)
			return;

		string overrideJson = inputAction.SaveBindingOverridesAsJson();
		PlayerPrefs.SetString(s_InputBindingPlayerPrefKey, overrideJson);
	}

	public void ResetInputAction()
	{
		SaveInputActions();

		OnControlSchemeChanged = null;
		OnRebind = null;
		inputAction.Dispose();

		InitInputActions();
	}

	private void OnEnable()
	{
		inputAction.Enable();
	}

	private void OnDisable()
	{
		if(inputAction == null)
		{
			Debug.LogWarning("Disabling while no input action");
			return;
		}

		inputAction.Disable();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		SaveInputActions();
	}
}
