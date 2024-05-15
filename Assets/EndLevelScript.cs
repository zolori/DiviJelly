using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevelScript : MonoBehaviour
{
    [SerializeField] private GameObject endScreen;

    private void OnTriggerEnter2D(Collider2D iCollision)
    {
        endScreen.SetActive(true);
    }
}
