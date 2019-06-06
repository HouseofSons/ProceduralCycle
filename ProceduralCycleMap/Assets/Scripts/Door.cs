using System.Collections.Generic;

public class Door
{
    public static List<Door> Doors { get; } = new List<Door>();

    public Room RoomOne { get; }
    public Room RoomTwo { get; }

    public Door (Room one, Room two)
    {
        Doors.Add(this);

        RoomOne = one;
        RoomTwo = two;
        RoomOne.AddDoor(this);
        RoomTwo.AddDoor(this);
    }
}
