using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
	[SerializeField] private List<AudioClip> m_Musics;
	private AudioManager m_AudioManager;

	private int m_LastMusicPlayedIdx = -1;

	private void Awake()
	{
		m_AudioManager = AudioManager.Instance;
	}

	private void Start()
	{
		StartCoroutine(PlayMusic());
	}

	IEnumerator PlayMusic()
	{
		while(true)
		{
			int nextMusicIdx;
			do
				nextMusicIdx = Random.Range(0, m_Musics.Count - 1);
			while(nextMusicIdx == m_LastMusicPlayedIdx);

			m_LastMusicPlayedIdx = nextMusicIdx;
			AudioClip music = m_Musics[nextMusicIdx];
			m_AudioManager.PlayMusic(music);

			yield return new WaitForSeconds(music.length);
		}
	}
}
