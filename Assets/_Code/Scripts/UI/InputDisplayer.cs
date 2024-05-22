using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputDisplayer : MonoBehaviour
{
	[SerializeField] private string m_ActionName;
	[SerializeField] private TextMeshProUGUI m_TextDisplay;

	InputMaster m_InputMaster;

	private void Start()
	{
		m_InputMaster = InputMaster.Instance;
		m_InputMaster.OnControlSchemeChanged += _UpdateInputDisplay;
		m_InputMaster.OnRebind += _UpdateInputDisplay;
		_UpdateInputDisplay();
	}

	private void _UpdateInputDisplay()
	{
		InputAction inputAction = m_InputMaster.InputAction.FindAction(m_ActionName);
		InputBinding bindingMask = InputBinding.MaskByGroup(m_InputMaster.ControlScheme.bindingGroup);
		m_TextDisplay.text = inputAction.GetBindingDisplayString(bindingMask);
	}
}
