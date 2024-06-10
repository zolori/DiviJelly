using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitedLifetime : MonoBehaviour
{
	[SerializeField] private float m_Lifetime;

	private void Start()
	{
		Destroy(gameObject, m_Lifetime);
	}

	private void OnCollisionEnter2D(Collision2D iCollision)
	{
		Destroy(gameObject);
	}
}
