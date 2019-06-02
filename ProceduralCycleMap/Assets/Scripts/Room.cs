using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public static List<Room> Rooms { get; } = new List<Room>();
    public List<Door> Doors { get; } = new List<Door>();
    public int Order { get; private set; }

    public void InitializeRoom(int i)
    {
        Rooms.Add(this);
        Order = i;
    }

    public void AddDoor(Door d)
    {
        Doors.Add(d);
    }
}
