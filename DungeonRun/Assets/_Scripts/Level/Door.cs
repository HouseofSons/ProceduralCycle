//This script is attached to a Door Game Object
//On Start up will catalogue references to it's Spawn Child Objects
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public List<Spawn> SpawnPoints { private set; get; }

    void Start()
    {
        SpawnPoints = new List<Spawn>();

        foreach (Spawn s in transform.GetComponentsInChildren<Spawn>())
        {
            SpawnPoints.Add(s);
        }
    }
    //Destination Method is triggered when a Player object interacts with a Door
    //Returns which Spawn location the Player should move towards given which side of the Door the Player is on
    public Spawn Destination(Vector3 location)
    {
        if(Vector3.SqrMagnitude(location - SpawnPoints[0].transform.position) < Vector3.SqrMagnitude(location - SpawnPoints[1].transform.position))
        {
            return SpawnPoints[1];
        }
        return SpawnPoints[0];
    }
}