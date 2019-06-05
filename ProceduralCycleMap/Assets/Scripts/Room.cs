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
    public Vector3 Size { get; set; }
    public bool[] IsGrown { get; set; } = new bool[3];

    public GameObject RoomGameObject { get; set; }

    public void InitializeRoom(int i)
    {
        Rooms.Add(this);
        Order = i;
        HasCycle = false;
    }

    public void OnCollisionEnter(Collision col)
    {
        ProceduralMapController.G.AddEdge(this.Order,col.gameObject.GetComponent<Room>().Order);
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

    public bool AttemptEdgeGrowth(int coord)
    {
        Vector3 intialRoomSize = this.transform.localScale;

        if (coord == 0)
        {
            if (Mathf.RoundToInt(intialRoomSize.x + 1) <= Mathf.RoundToInt(Size.x) + 1)
            {
                this.transform.localScale = new Vector3Int(Mathf.RoundToInt(intialRoomSize.x + 1), Mathf.RoundToInt(intialRoomSize.y), Mathf.RoundToInt(intialRoomSize.z));
            } else
            {
                return true;
            }
        } else if(coord == 1)
        {
            if (Mathf.RoundToInt(intialRoomSize.y + 1) <= Mathf.RoundToInt(Size.y))
            {
                this.transform.localScale = new Vector3Int(Mathf.RoundToInt(intialRoomSize.x), Mathf.RoundToInt(intialRoomSize.y + 1), Mathf.RoundToInt(intialRoomSize.z));
            }
            else
            {
                return true;
            }
        } else
        {
            if (Mathf.RoundToInt(intialRoomSize.z + 1) <= Mathf.RoundToInt(Size.z) + 1)
            {
                this.transform.localScale = new Vector3Int(Mathf.RoundToInt(intialRoomSize.x), Mathf.RoundToInt(intialRoomSize.y), Mathf.RoundToInt(intialRoomSize.z + 1));
            }
            else
            {
                return true;
            }
        }

        foreach (Room r in Rooms)
        {
            if (r != this)
            {
                if (Physics.OverlapBox(this.transform.position, (this.transform.localScale / 2.01f), this.transform.rotation).Length > 1)
                {
                    this.transform.localScale = intialRoomSize;
                    return true;
                }
            }
        }
        return false;
    }
}
