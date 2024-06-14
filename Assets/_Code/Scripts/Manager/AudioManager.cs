using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
	private const string s_MusicKey = "Music";
	private const string s_SFXKey = "SFX";
	private const string s_VolumeKey = "Volume";
	private const string s_MusicVolumeKey = s_MusicKey + s_VolumeKey;
	private const string s_SFXVolumeKey = s_SFXKey + s_VolumeKey;

	private const float s_MinDb = -80;
	private const float s_MaxDb = 0;

	private AudioMixer m_Mixer = null;
	private AudioSource m_MusicSpeaker = null;
	private AudioSource m_SFXSpeaker = null;

	protected override void Awake()
	{
		base.Awake();
		if(Instance != this)
			return;

		DontDestroyOnLoad(gameObject);


		m_Mixer = Resources.Load<MixerLink>("MixerLink").Mixer;
		// m_Mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>("Assets/_Audio/MasterMixer.mixer");
		AudioMixerGroup musicMixerGroup = GetMixerGroup(m_Mixer, s_MusicKey);
		AudioMixerGroup sfxMixerGroup = GetMixerGroup(m_Mixer, s_SFXKey);

		m_MusicSpeaker = CreateSpeaker(musicMixerGroup);
		m_SFXSpeaker = CreateSpeaker(sfxMixerGroup);
	}

	private void Start()
	{
		SetMusicVolume(PlayerPrefs.GetFloat(s_MusicVolumeKey, 0.5f));
		SetSFXVolume(PlayerPrefs.GetFloat(s_SFXVolumeKey, 0.5f));

	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		if(m_Mixer == null)
		{
			Debug.LogWarning("No mixer on destroy.");
			return;
		}

		PlayerPrefs.SetFloat(s_MusicVolumeKey, GetMusicVolume());
		PlayerPrefs.SetFloat(s_SFXVolumeKey, GetSFXVolume());
	}

	private AudioMixerGroup GetMixerGroup(AudioMixer iMixer, string iGroupName)
	{
		AudioMixerGroup[] mixerGroups = iMixer.FindMatchingGroups(iGroupName);
		if(mixerGroups.Length <= 0)
		{
			Debug.LogError($"No {iGroupName} mixer group found.");
			return null;
		}
		if(mixerGroups.Length > 1)
			Debug.LogWarning($"Several {iGroupName} mixer groups were found.");

		return mixerGroups[0];
	}

	public AudioSource CreateSpeaker(AudioMixerGroup iMixerGroup)
	{
		AudioSource speaker = gameObject.AddComponent<AudioSource>();
		speaker.outputAudioMixerGroup = iMixerGroup;
		speaker.playOnAwake = false;
		speaker.spatialBlend = 0;
		return speaker;
	}

	private float ToDb(float iNormedVolume)
	{
		float tmp = Mathf.Lerp(s_MinDb, s_MaxDb, iNormedVolume);
		return tmp;
	}

	private float FromDb(float iDbVolume)
	{
		float tmp = Mathf.InverseLerp(s_MinDb, s_MaxDb, iDbVolume);
		return tmp;
	}

	private void SetVolume(string iVolumeKey, float iVolume)
	{
		if(m_Mixer == null)
		{
			Debug.LogError("Tried to set volume when audio mixer has not been initialized.");
			return;
		}

		if(!m_Mixer.SetFloat(iVolumeKey, ToDb(iVolume)))
			Debug.LogError($"No {iVolumeKey} parameter found in {m_Mixer.name}");
	}

	public void SetMusicVolume(float iVolume)
	{
		SetVolume(s_MusicVolumeKey, iVolume);
	}

	public void SetSFXVolume(float iVolume)
	{
		SetVolume(s_SFXVolumeKey, iVolume);
	}

	private float GetVolume(string iVolumeKey)
	{
		if(m_Mixer == null)
		{
			Debug.LogError("Attempting to get volume when audio mixer has not been initialized.");
			return 0;
		}

		float volume = 0;
		if(!m_Mixer.GetFloat(iVolumeKey, out volume))
		{
			Debug.LogError($"No {iVolumeKey} parameter found in {m_Mixer.name}");
			return 0;
		}

		return FromDb(volume);
	}

	public float GetMusicVolume()
	{
		return GetVolume(s_MusicVolumeKey);
	}

	public float GetSFXVolume()
	{
		return GetVolume(s_SFXVolumeKey);
	}

	public void PlayMusic(AudioClip iClip)
	{
		m_MusicSpeaker.Stop();
		m_MusicSpeaker.clip = iClip;
		m_MusicSpeaker.Play();
	}

	public void PlaySFX(AudioClip iClip)
	{
		m_SFXSpeaker.PlayOneShot(iClip);
	}
}
