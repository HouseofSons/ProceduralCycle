using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralMapController : MonoBehaviour
{
    [Range(2, 60)]
    public int numberOfRooms;
    [Range(0, 20)]
    public int numberOfCycles;

    public const int ROOM_SCALE = 8;

    private static int roomCount;
    private static int cycleCount;

    void Awake()
    {
        roomCount = numberOfRooms;
        cycleCount = numberOfCycles;

        GameGrid gameGrid = new GameGrid();

        GenerateRoomGameObjects();
        GenerateRoomEdges();
    }

    private void Start()
    {
        StartCoroutine(PlaceRooms());
    }

    public void Update()
    {
        
    }

    private static void GenerateRoomGameObjects()
    {
        for (int i = 0; i < roomCount; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.AddComponent<Room>();
            go.transform.localScale = new Vector3(ROOM_SCALE, ROOM_SCALE, ROOM_SCALE);
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
            rand = Seed.Random(1, roomCount - 1);
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
                int prevRoomNum = Seed.Random(0, r.Order - 1);
                //Assigns Edge between a lower order room (roomNum) and this room (r)
                r.AddDoor(Room.Rooms[prevRoomNum]);
                //Randomly determines which room to cycle foward from
                int proceedFromRoomNum = Seed.Random(0, r.Order);
                //Assigns Vertex between a Room (proceedFromRoomNum) and this next room (rn.Order+1)
                Room.Rooms[r.Order + 1].AddDoor(Room.Rooms[proceedFromRoomNum]);
            }
            else if (r.Order > 0)
            {
                //if no cycle exists assign Edge to next room in Order
                r.AddDoor(Room.Rooms[r.Order - 1]);
            }
        }
    }

    private static IEnumerator PlaceRooms()
    {
        foreach (Room r in Room.Rooms)
        {
            GameGrid.AddRoomNextToNeighbor(r);
            r.transform.position =
                new Vector3Int(r.GameGridPosition.x * ROOM_SCALE,
                r.GameGridPosition.y * ROOM_SCALE,
                r.GameGridPosition.z * ROOM_SCALE);
            r.GetComponent<Renderer>().material.SetColor("_Color", Random.ColorHSV());
            GameGrid.ExtendRoomToNeighbors(r);

            for(int i=0;i<5;i++)
            {
                yield return null;
            }
        }
    }
}
