using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class FlavourerGenerator : MonoBehaviour
{
	[SerializeField] private GameObject m_FlavourerPrefab;
	[SerializeField] private Flavours m_Flavours;
	[SerializeField] private Flavour m_Flavour;

	[SerializeField] private float m_TimeToGenerate = 5f;
	[SerializeField] private float m_Variance = 0.1f;
	[SerializeField] private Transform m_SpawnPoint;

	private void Start()
	{
		StartCoroutine(GenerateFlavourers());
		SpriteRenderer renderer = GetComponent<SpriteRenderer>();
		if(renderer != null)
			renderer.color = m_Flavours.GetData(m_Flavour).Color;
	}

	private IEnumerator GenerateFlavourers()
	{
		while(true)
		{
			yield return new WaitForSeconds(m_TimeToGenerate + Random.Range(-m_Variance, m_Variance));
			GameObject flavourDrop = Instantiate(m_FlavourerPrefab, m_SpawnPoint.position, m_SpawnPoint.rotation, m_SpawnPoint);
			Flavourer flavourer = flavourDrop.GetComponent<Flavourer>();
			flavourer.SetFlavour(m_Flavour);
		}
	}
}
