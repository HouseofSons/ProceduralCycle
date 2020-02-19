using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public List<Spawn> SpawnPoints { get; set; }

    void Start()
    {
        foreach (Spawn s in this.transform.GetComponentsInChildren<Spawn>())
        {
            SpawnPoints.Add(s);
        }
    }
}