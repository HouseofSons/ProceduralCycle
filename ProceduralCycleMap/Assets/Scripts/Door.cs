using System.Collections;
using System.Collections.Generic;

public class Door
{
    private Room roomOne;
    private Room roomTwo;

    public Door (Room one, Room two)
    {
        Doors.Add(this);

        roomOne = one;
        roomTwo = two;
        roomOne.AddDoor(this);
        roomTwo.AddDoor(this);
    }

    public static List<Door> Doors { get; }
}
