using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InputsSprites", menuName = "Inputs")]
public class InputsSprites : ScriptableObject
{
	[Serializable]
	public struct ImageInputsPair
	{
		public Sprite Img;
		public List<string> Inputs;
	}

	public Sprite DefaultInputImg;
	public List<ImageInputsPair> InputsImages = new List<ImageInputsPair>();
}
