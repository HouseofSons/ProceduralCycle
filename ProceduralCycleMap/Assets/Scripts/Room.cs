using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public static List<Room> Rooms { get; } = new List<Room>();
    public List<Door> Doors { get; } = new List<Door>();
    public List<Room> Neighbors { get; } = new List<Room>();
    public int Order { get; private set; }

    private Coroutine roomIsMoving;

    private void Update()
    {
        if (Neighbors.Count > 0)
        {
            roomIsMoving = StartCoroutine(MoveTowardNeighbors());
        }
    }

    public void InitializeRoom(int i)
    {
        Rooms.Add(this);
        Order = i;
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

    public IEnumerator MoveTowardNeighbors()
    {
        Vector3 newPosition = Vector3.zero;

        foreach(Room r in Neighbors)
        {
            newPosition += r.transform.position;
        }
        newPosition /= Neighbors.Count;

        transform.position = Vector3.Lerp(transform.position, newPosition, 0.005f);
        yield return null;
    }
}
