using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlavourerGenerator : MonoBehaviour
{
	[SerializeField] private GameObject m_FlavourerPrefab;

	[SerializeField] private float m_TimeToGenerate = 5f;
	[SerializeField] private float m_Variance = 0.1f;

	private void Start()
	{
		StartCoroutine(GenerateFlavourers());
	}

	private IEnumerator GenerateFlavourers()
	{
		while(true)
		{
			yield return new WaitForSeconds(m_TimeToGenerate + Random.Range(-m_Variance, m_Variance));
			Instantiate(m_FlavourerPrefab, transform.position, transform.rotation, transform.parent);
		}
	}
}
