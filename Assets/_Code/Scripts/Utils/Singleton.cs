using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour
	where T : Component
{
	private static T s_Instance;

	public static T Instance
	{
		get
		{
			if(s_Instance == null)
			{
				s_Instance = FindObjectOfType<T>();
				if(s_Instance == null)
				{
					GameObject obj = new GameObject();
					obj.name = typeof(T).Name;
					Debug.Log($"Created {obj.name}");
					s_Instance = obj.AddComponent<T>();
				}
			}
			return s_Instance;
		}
	}

	public virtual void Awake()
	{
		if(s_Instance == null)
			s_Instance = this as T;
		else
		{
			if(s_Instance != this as T)
				Destroy(gameObject);
		}
	}

	protected virtual void OnDestroy()
	{
		if(s_Instance == this as T)
			s_Instance = null;
	}
}