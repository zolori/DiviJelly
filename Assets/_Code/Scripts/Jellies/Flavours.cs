using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
	public Color Color;
	public Sprite HUDSprite;
	public int Layer;
}

[CreateAssetMenu(fileName = "Flavours", menuName = "Flavour/Flavours", order = 1)]
public class Flavours : ScriptableObject
{
	public List<FlavourData> Data;

	public FlavourData GetData(Flavour iFlavour)
	{
		return Data.First(data => data.Flavour == iFlavour);
	}
}
