using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputMaster : Singleton<InputMaster>
{
	private InputsActions inputAction;

	public InputsActions InputAction
	{
		get => inputAction;
	}

	public Action OnDeviceChange;

	private const string s_InputBindingPlayerPrefKey = "inputBindings";

	protected override void Awake()
	{
		base.Awake();
		if(Instance != this)
			return;

		DontDestroyOnLoad(gameObject);
		inputAction = new InputsActions();
		string overrideJson = PlayerPrefs.GetString(s_InputBindingPlayerPrefKey, "");
		inputAction.LoadBindingOverridesFromJson(overrideJson);

		inputAction.Enable();

		InputSystem.onDeviceChange += (_, __) => OnDeviceChange?.Invoke();
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

		if(inputAction == null)
			return;

		string overrideJson = inputAction.SaveBindingOverridesAsJson();
		PlayerPrefs.SetString(s_InputBindingPlayerPrefKey, overrideJson);
	}
}
