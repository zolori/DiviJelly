using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JelliesManager : Singleton<JelliesManager>
{
	public Dictionary<Flavour, List<JellyEntity>> m_Jellies = new Dictionary<Flavour, List<JellyEntity>>();
	public List<Flavour> m_Flavours = new List<Flavour>();

	public Action OnAvailableFlavourChange;

	public void RegisterJelly(JellyEntity iJelly)
	{
		Flavour jellyFlavour = iJelly.GetFlavour();
		if(m_Jellies.ContainsKey(jellyFlavour))
			m_Jellies[jellyFlavour].Add(iJelly);
		else
		{
			m_Flavours.Add(jellyFlavour);
			List<JellyEntity> jellies = new List<JellyEntity>();
			jellies.Add(iJelly);
			m_Jellies.Add(jellyFlavour, jellies);
			OnAvailableFlavourChange?.Invoke();
		}
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
			OnAvailableFlavourChange?.Invoke();
		}
	}

	public Rect GetBBoxForFlavour(Flavour iFlavour)
	{
		Vector2 minPos = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
		Vector2 maxPos = -minPos;

		foreach(JellyEntity jelly in m_Jellies.GetValueOrDefault(iFlavour, new List<JellyEntity>()))
		{
			Rect jellyBBox = jelly.GetBBox();
			minPos.x = Mathf.Min(minPos.x, jellyBBox.x);
			maxPos.x = Mathf.Max(maxPos.x, jellyBBox.xMax);
			minPos.y = Mathf.Min(minPos.y, jellyBBox.y);
			maxPos.y = Mathf.Max(maxPos.y, jellyBBox.yMax);
		}

		return new Rect(minPos, maxPos - minPos);
	}
}
