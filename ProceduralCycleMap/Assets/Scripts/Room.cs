using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    private static List<Room> rooms = new List<Room>();

    private List<Door> doors;
    private GameObject roomNode;

    private int order;

    public Room ()
    {
        rooms.Add(this);
    }

    public static List<Room> Rooms { get; }

    public void AddOrder(int i)
    {
        order = i;
    }

    public int Order ()
    {
        return order;
    }

    public void AddDoor(Door d)
    {
        doors.Add(d);
    }

    public void AddRoomNode(GameObject go)
    {
        roomNode = go;
    }

    public GameObject RoomNode()
    {
        return roomNode;
    }
}
