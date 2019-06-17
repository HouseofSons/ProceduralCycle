using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public static List<Room> Rooms { get; } = new List<Room>();
    public List<Door> Doors { get; private set; } = new List<Door>();
    public int Order { get; private set; }
    public bool HasCycle;
    public Vector3Int GameGridPosition { get; set; }

    public void Awake()
    {
        HasCycle = false;
        Order = Rooms.Count;
    }

    public void Start()
    {

    }

    public void Update()
    {

    }

    public void AddDoor(Room neighbor)
    {
        Doors.Add(new Door(this, neighbor));
    }

    public List<Room> GetNeighbors()
    {
        List<Room> neighbors = new List<Room>();

        foreach(Door d in Doors)
        {
            if(d.RoomOne == this)
            {
                neighbors.Add(d.RoomTwo);
            } else
            {
                neighbors.Add(d.RoomOne);
            }
        }
        return neighbors;
    }
}