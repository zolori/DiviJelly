using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flavourer : MonoBehaviour
{
	[SerializeField] private Flavours m_Flavours;
	[SerializeField] private Flavour m_Flavour;
	[SerializeField] private SpriteRenderer m_Renderer;

	private void Start()
	{
		UpdateFlavour();
	}

	private void OnTriggerEnter2D(Collider2D iCollision)
	{
		JellyEntity jelly;
		if(!iCollision.gameObject.TryGetComponent(out jelly))
			return;

		jelly.SetFlavour(m_Flavour);
	}

	public void SetFlavour(Flavour iFlavour)
	{
		m_Flavour = iFlavour;
		UpdateFlavour();
	}

	private void UpdateFlavour()
	{
		m_Renderer.color = m_Flavours.GetData(m_Flavour).Color;
	}
}
