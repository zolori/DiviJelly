using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiSfxPlayer : MonoBehaviour
{
	[SerializeField] private AudioClip m_Sound;
	private AudioManager m_AudioManager;

	private void Awake()
	{
		m_AudioManager = AudioManager.Instance;
	}

	private void Start()
	{
		if(TryGetComponent(out Button button))
			button.onClick.AddListener(PlaySound);
		if(TryGetComponent(out Slider slider))
			slider.onValueChanged.AddListener(_ => PlaySound());
	}

	public void PlaySound(AudioClip iSound)
	{
		m_AudioManager.PlaySFX(iSound);
	}

	public void PlaySound()
	{
		PlaySound(m_Sound);
	}
}
