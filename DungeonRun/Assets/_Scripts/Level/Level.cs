//This script is attached to a Level Game Object
//The Level Game Object on Start up will catalogue references to it's Room and Door Child Objects
//The Level Game Objects can be created manually but can also be procedurally generated
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public List<Room> Rooms { private set; get; }
    public List<Door> Doors { private set; get; }

    void Start()
    {
        foreach (Room r in transform.GetComponentsInChildren<Room>())
        {
            Rooms.Add(r);
        }

        foreach (Door d in transform.GetComponentsInChildren<Door>())
        {
            Doors.Add(d);
        }
    }
}
