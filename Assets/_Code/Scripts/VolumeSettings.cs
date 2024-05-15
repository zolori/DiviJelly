using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
	[SerializeField] private Slider m_MusicVolumeSlider;
	[SerializeField] private Slider m_SFXVolumeSlider;

	// Start is called before the first frame update
	private void Start()
	{
		AudioManager audioManager = AudioManager.Instance;
		SetupVolumeSlider(m_MusicVolumeSlider, audioManager.SetMusicVolume);
		SetupVolumeSlider(m_SFXVolumeSlider, audioManager.SetSFXVolume);
	}

	private void OnEnable()
	{
		AudioManager audioManager = AudioManager.Instance;
		SetSliderNormedVolume(m_MusicVolumeSlider, audioManager.GetMusicVolume());
		SetSliderNormedVolume(m_SFXVolumeSlider, audioManager.GetSFXVolume());
	}

	private float GetSliderNormedVolume(Slider iSlider, float iVolume)
	{
		return Mathf.InverseLerp(iSlider.minValue, iSlider.maxValue, iVolume);
	}

	private void SetSliderNormedVolume(Slider iSlider, float iVolume)
	{
		iSlider.SetValueWithoutNotify(Mathf.Lerp(iSlider.minValue, iSlider.maxValue, iVolume));
	}

	private UnityAction<float> VolumeAdapter(Slider iSlider, UnityAction<float> iSetVolume)
	{
		return volume => iSetVolume(GetSliderNormedVolume(iSlider, volume));
	}

	private void SetupVolumeSlider(Slider iSlider, UnityAction<float> iSetVolume)
	{
		iSlider.onValueChanged.AddListener(VolumeAdapter(iSlider, iSetVolume));
	}
}
