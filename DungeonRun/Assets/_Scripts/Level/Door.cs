using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public List<Spawn> SpawnPoints { private set; get; }

    void Start()
    {
        foreach (Spawn s in this.transform.GetComponentsInChildren<Spawn>())
        {
            SpawnPoints.Add(s);
        }
    }

    public Spawn Destination(Vector3 location)
    {
        if(Vector3.SqrMagnitude(location - SpawnPoints[0].SpawnPoint) < Vector3.SqrMagnitude(location - SpawnPoints[1].SpawnPoint))
        {
            return SpawnPoints[1];
        }
        return SpawnPoints[0];
    }
}