using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Flavour
{
	Strawberry,
	Orange,
	Lemon,
	Apple,
	Blueberry,
}

[Serializable]
public struct FlavourData
{
	public Flavour Flavour;
	public Sprite Sprite;
	public Color Color;
	public int Layer;
}

[CreateAssetMenu(fileName = "Flavours", menuName = "Flavour/Flavours", order = 1)]
public class Flavours : ScriptableObject
{
	public List<FlavourData> Data;
}
