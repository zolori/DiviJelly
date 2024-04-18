using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellysManager : Singleton<JellysManager>
{
	public Dictionary<Flavour, List<JellyEntity>> m_Jellies = new Dictionary<Flavour, List<JellyEntity>>();
	public List<Flavour> m_Flavours = new List<Flavour>();

	public void RegisterJelly(JellyEntity iJelly)
	{
		Flavour jellyFlavour = iJelly.GetFlavour();
		if(!m_Jellies.ContainsKey(jellyFlavour))
		{
			m_Flavours.Add(jellyFlavour);
			m_Jellies.Add(jellyFlavour, new List<JellyEntity>());
		}
		m_Jellies[jellyFlavour].Add(iJelly);
	}

	public void UnregisterJelly(JellyEntity iJelly)
	{
		Flavour jellyFlavour = iJelly.GetFlavour();
		if(!m_Jellies.ContainsKey(jellyFlavour))
			return;

		List<JellyEntity> jellies = m_Jellies[jellyFlavour];
		jellies.Remove(iJelly);

		if(jellies.Count <= 0)
		{
			m_Jellies.Remove(jellyFlavour);
			m_Flavours.Remove(jellyFlavour);
		}
	}

	/*public Rect GetBBoxForFlavour(Flavour iFlavour)
	{
	}*/
}
