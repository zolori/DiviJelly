using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "MixerLink", menuName = "Audio")]
public class MixerLink : ScriptableObject
{
	public AudioMixer Mixer;
}
