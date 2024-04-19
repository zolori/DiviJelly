using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flavourer : MonoBehaviour
{
	[SerializeField] private Flavour m_Flavour;

	private void OnTriggerEnter2D(Collider2D iCollision)
	{
		JellyEntity jelly;
		if(!iCollision.gameObject.TryGetComponent(out jelly))
			return;

		jelly.SetFlavour(m_Flavour);
	}
}
