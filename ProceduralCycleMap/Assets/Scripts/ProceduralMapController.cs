using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralMapController : MonoBehaviour
{
    [Range(2, 2000)]
    public int numberOfRooms;
    [Range(0, 40)]
    public int numberOfCycles;
    [Range(0, 10)]
    public int pathRoomDistance;

    public const int ROOM_SCALE = 16;

    private static int roomCount;
    private static int cycleCount;

    private static bool extendRooms;

    void Awake()
    {
        roomCount = numberOfRooms;
        cycleCount = numberOfCycles;

        _ = new GameGrid();

        GenerateRoomGameObjects();
        GenerateRoomEdges();
    }

    private void Start()
    {
        StartCoroutine(PlaceRooms());
    }

    public void Update()
    {
        if (extendRooms) {
            StartCoroutine(ExtendRooms());
        }
    }

    private static void GenerateRoomGameObjects()
    {
        for (int i = 0; i < roomCount; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.AddComponent<Room>();
            go.transform.localScale = new Vector3(ROOM_SCALE, ROOM_SCALE, ROOM_SCALE);
            go.transform.name = "Room: " + go.GetComponent<Room>().Order;
            Room.Rooms.Add(go.GetComponent<Room>());
        }
    }

    private void GenerateRoomEdges()
    {
        //Need to Add Option for Multiple Characters to Map!!
        int counter = 0;
        int rand;

        while (counter < cycleCount)
        {
            rand = Seed.Random(2, roomCount - 2);
            if (!Room.Rooms[rand].HasCycle)
            {
                Room.Rooms[rand].HasCycle = true;
                counter++;
            }
        }
        
        foreach (Room r in Room.Rooms)
        {
            if (r.HasCycle)
            {
                //Randomly determines which room to cycle back to
                int prevRoomNum = Seed.Random(Mathf.Max(0, r.Order - 2 - pathRoomDistance), r.Order - 2);
                //Assigns Edge between a lower order room (roomNum) and this room (r)
                r.AddDoor(Room.Rooms[prevRoomNum]);
                //Randomly determines which room to cycle foward from
                int proceedFromRoomNum = Seed.Random(Mathf.Max(0, prevRoomNum - 2), r.Order - 1);
                //Assigns Vertex between a Room (proceedFromRoomNum) and this next room (rn.Order+1)
                Room.Rooms[proceedFromRoomNum].AddDoor(Room.Rooms[r.Order + 1]);
            }
            else if (r.Order < roomCount - 1)
            {
                //if no cycle exists assign Edge to next room in Order
                r.AddDoor(Room.Rooms[r.Order + 1]);
            }
        }
    }

    private static IEnumerator PlaceRooms()
    {
        Color color;
        Vector3Int size;
        Vector2Int scalar;

        foreach (Room r in Room.Rooms)
        {
            GameGrid.AddRoomNextToNeighbor(r);
            color = Random.ColorHSV();
            size = new Vector3Int(Mathf.RoundToInt(r.transform.localScale.x), Mathf.RoundToInt(r.transform.localScale.y), Mathf.RoundToInt(r.transform.localScale.z));
            scalar = r.RoomSize();
            r.transform.localScale = new Vector3Int(size.x * scalar[0], size.y, size.z * scalar[1]);
            r.transform.position = r.RoomPosition();
            r.GetComponent<Renderer>().material.SetColor("_Color", color);
            for (int i = 0; i < 5; i++)
            {
                yield return null;
            }
        }
        extendRooms = true;
    }

    private static IEnumerator ExtendRooms()
    {
        extendRooms = false;    

        foreach (Room r in Room.Rooms)
        {
            GameGrid.ExtendRoomToNeighbors(r);
            for (int i = 0; i < 5; i++)
            {
                yield return null;
            }
        }
        foreach (Room r in Room.Rooms)
        {
            foreach (Door d in r.Doors)
            {
                Debug.Log("Room: " + r.Order + " to Door: " + d.RoomSecond.Order);
            }
        }
    }
}
