using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall
{
    public static List<Wall> Walls { get; } = new List<Wall>();
    private FloorTile firstRoom;
    private FloorTile secondRoom;
    private Door door;

    public Wall(FloorTile first, FloorTile second)
    {
        Walls.Add(this);
        firstRoom = first;
        secondRoom = second;
        //door = FindDoor(this);
    }
}
