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

    private Vector3 prevPosition;
    private float distanceMoved;

    public void InitializeRoom(int i)
    {
        Rooms.Add(this);
        Order = i;
        HasCycle = false;
        prevPosition = Vector3.zero;
    }

    public void Update()
    {
        if (ProceduralMapController.moveRoomNodes)
        {
            distanceMoved = Mathf.Abs(Vector3.Distance(prevPosition, transform.position));
        }
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
        if (Time.frameCount > 100)
        {
            foreach (Room r in Rooms)
            {
                if (r.distanceMoved > 4)
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }
}
