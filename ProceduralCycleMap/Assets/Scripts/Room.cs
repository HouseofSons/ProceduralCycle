using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public static List<Room> Rooms { get; } = new List<Room>();
    public List<Door> Doors { get; } = new List<Door>();
    public List<Room> Neighbors { get; } = new List<Room>();
    public int Order { get; private set; }
    public bool HasCycle { get; set; }

    public void InitializeRoom(int i)
    {
        Rooms.Add(this);
        Order = i;
        HasCycle = false;
    }

    public void AddDoor(Door d)
    {
        Doors.Add(d);
        if (d.RoomOne != this)
        {
            Neighbors.Add(d.RoomOne);
        } else
        {
            Neighbors.Add(d.RoomTwo);
        }
    }

    public static bool RoomsAreStable()
    {
        return false;
    }
}
