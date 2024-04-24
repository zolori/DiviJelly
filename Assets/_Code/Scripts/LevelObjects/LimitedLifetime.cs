using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitedLifetime : MonoBehaviour
{
	[SerializeField] private float m_Lifetime;

	// Start is called before the first frame update
	private void Start()
	{
		Destroy(gameObject, m_Lifetime);
	}
}
