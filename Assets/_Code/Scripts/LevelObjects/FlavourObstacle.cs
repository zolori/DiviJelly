using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FlavourObstacle : MonoBehaviour
{
	[SerializeField] private Flavours m_Flavours;
	[SerializeField] private Flavour m_Flavour;

	private void Start()
	{
		FlavourData flavourData = m_Flavours.GetData(m_Flavour);

		Collider2D collider = GetComponent<Collider2D>();
		collider.excludeLayers = (1 << LayerMask.NameToLayer("Animation")) | (1 << flavourData.Layer);

		SpriteRenderer renderer = GetComponent<SpriteRenderer>();
		if(renderer != null)
			renderer.color = flavourData.Color;
	}
}
