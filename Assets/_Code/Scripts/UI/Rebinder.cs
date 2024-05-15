using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Rebinder : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI m_BoundKeyText;
	[SerializeField] private Button m_RebindButton;
	[SerializeField] private Button m_RemoveBindindButton;

	[SerializeField] private string m_ActionName;
	[SerializeField] private int m_BindingIndex;

	private InputAction m_Action;
	private bool m_IsRebinding = false;

	private InputActionRebindingExtensions.RebindingOperation m_Operation = null;

	void Start()
	{
		m_RebindButton.onClick.AddListener(Rebind);
		m_RemoveBindindButton.onClick.AddListener(RemoveBinding);
		SetInputAction(InputMaster.Instance.InputAction.FindAction(m_ActionName));
		InputMaster.Instance.OnDeviceChange += Refresh;
	}

	public void SetInputAction(InputAction iInputAction)
	{
		m_Action = iInputAction;
		Refresh();
	}

	private bool IsValidBinding()
	{
		return m_Action != null && 0 <= m_BindingIndex && m_BindingIndex < m_Action.bindings.Count;
	}

	private void Refresh()
	{
		if(!IsValidBinding())
		{
			m_BoundKeyText.text = "";
			return;
		}

		m_BoundKeyText.text = m_Action.GetBindingDisplayString(m_BindingIndex);
	}

	private void Rebind()
	{
		if(!IsValidBinding())
		{
			Debug.LogError("No action...");
			return;
		}

		if(m_IsRebinding)
			return;

		m_Action.Disable();
		m_IsRebinding = true;
		InputBinding inputBinding = m_Action.bindings[m_BindingIndex];
		string layout = InputControlPath.TryGetDeviceLayout(inputBinding.path);
		Debug.Log($"Found layout {layout}");

		string controlType = m_Action.bindings[m_BindingIndex].isPartOfComposite ? "Button" : m_Action.expectedControlType;

		m_Operation = m_Action.PerformInteractiveRebinding(m_BindingIndex)
			.WithExpectedControlType(controlType)
			.WithControlsHavingToMatchPath($"<{layout}>")
			.WithCancelingThrough(Keyboard.current.escapeKey)
			.OnMatchWaitForAnother(0.1f)
			.OnComplete(operation =>
			{
				operation.Dispose();
				m_Operation = null;
				m_Action.Enable();
				m_IsRebinding = false;
				Refresh();
			})
			.OnCancel(operation =>
			{
				operation.Dispose();
				m_Operation = null;
				m_IsRebinding = false;
				m_Action.Enable();
			})
			.Start();
	}

	private void RemoveBinding()
	{
		if(m_Action == null)
			return;

		m_Action.RemoveBindingOverride(m_BindingIndex);
		Refresh();
	}

	private void OnDisable()
	{
		m_Operation?.Cancel();
	}

	private void OnDestroy()
	{
		m_Operation?.Cancel();
	}
}
