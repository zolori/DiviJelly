using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputDisplayer : MonoBehaviour
{
	[SerializeField] private InputsSprites m_InputsSprites;
	[SerializeField] private string m_ActionName;
	[SerializeField] private TextMeshProUGUI m_TextDisplay;
	[SerializeField] private Image m_ImgDisplay;

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

		int bindingIdx = inputAction.GetBindingIndex(bindingMask);
		m_ImgDisplay.sprite = _FindInputSprite(inputAction.bindings[bindingIdx].effectivePath);
	}

	private Sprite _FindInputSprite(string iKeyName)
	{
		foreach(InputsSprites.ImageInputsPair inputsSprite in m_InputsSprites.InputsImages)
		{
			if(inputsSprite.Inputs.Contains(iKeyName))
				return inputsSprite.Img;
		}

		return m_InputsSprites.DefaultInputImg;
	}
}
