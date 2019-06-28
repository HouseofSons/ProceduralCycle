using System.Collections.Generic;
using UnityEngine;

public class Door
{
    public static List<Door> Doors { get; } = new List<Door>();

    public Room RoomFirst { get; }
    public Room RoomSecond { get; }

    public Vector3Int RoomFirstLocation { get; set; }
    public Vector3Int RoomSecondLocation { get; set; }

    public bool remote { get; set; }

    public Door (Room one, Room two)
    {
        Doors.Add(this);
        RoomFirst = one;
        RoomSecond = two;
        remote = false;
    }
}
