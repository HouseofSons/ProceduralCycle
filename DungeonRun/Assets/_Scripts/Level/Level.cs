using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public List<Room> Rooms { get; set; }
    public List<Door> Doors { get; set; }

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
