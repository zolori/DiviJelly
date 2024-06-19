using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PauseShortcut : MonoBehaviour
{
	private Button m_Button;
	private InputMaster m_InputMaster;

	private void Awake()
	{
		m_Button = GetComponent<Button>();
		m_InputMaster = InputMaster.Instance;
	}

	private void OnEnable()
	{
		m_InputMaster.InputAction.Jellys.Pause.performed += Shortcut;
	}

	private void OnDisable()
	{
		if(m_InputMaster != null)
			m_InputMaster.InputAction.Jellys.Pause.performed -= Shortcut;
	}

	private void Shortcut(InputAction.CallbackContext _)
	{
		Debug.Log(gameObject.name + " click");
		m_Button.onClick.Invoke();
	}
}
