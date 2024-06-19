using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PauseShortcut : MonoBehaviour
{
	private Button m_Button;

	void Start()
	{
		m_Button = GetComponent<Button>();
	}

	private void OnEnable()
	{
		InputMaster.Instance.InputAction.Jellys.Pause.performed += Shortcut;
	}

	private void OnDisable()
	{
		InputMaster.Instance.InputAction.Jellys.Pause.performed -= Shortcut;
	}

	private void Shortcut(InputAction.CallbackContext _)
	{
		Debug.Log(gameObject.name + " click");
		m_Button.onClick.Invoke();
	}
}
