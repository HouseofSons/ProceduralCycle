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
    public bool IsStable { get; set; }

    private Vector3 prevPosition;
    private float distanceMoved;

    public GameObject RoomGameObject { get; set; }

    public void InitializeRoom(int i)
    {
        Rooms.Add(this);
        Order = i;
        HasCycle = false;
        prevPosition = Vector3.zero;
        IsStable = false;
    }

    public void Update()
    {
        if (ProceduralMapController.moveRoomNodes)
        {
            distanceMoved = Mathf.Abs(Vector3.Distance(prevPosition, transform.position));
            prevPosition = transform.position;
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
                if (r.distanceMoved > 0.02f || r.GetComponent<Rigidbody>().)
                {
                    Debug.Log(r.distanceMoved);
                    return false;
                }
                r.IsStable = true;
                r.transform.position = new Vector3(Mathf.RoundToInt(r.transform.position.x), Mathf.RoundToInt(r.transform.position.y), Mathf.RoundToInt(r.transform.position.z));
                r.GetComponent<Rigidbody>().isKinematic = true;
            }
            return true;
        }
        return false;
    }
}
