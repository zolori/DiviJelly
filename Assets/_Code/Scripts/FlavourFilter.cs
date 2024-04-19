using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FlavourFilter : MonoBehaviour
{
	[SerializeField] private LayerMask m_FilteredFlavours = 0;

	private void Awake()
	{
		Collider2D collider = GetComponent<Collider2D>();
		collider.includeLayers = m_FilteredFlavours;
	}
}
