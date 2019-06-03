using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralMapController : MonoBehaviour
{
    [Range(2, 20)]
    public int numberOfRooms;
    [Range(0, 20)]
    public int numberOfCycles;

    private static int roomCount;
    private static int cycleCount;

    public bool moveRoomNodes;

    public static List<Room> MoveableRooms { get; private set; } = new List<Room>();
    private static Coroutine roomsAreMoving;
    private static Room roomToBuild;
    private static Coroutine roomBeingBuilt;

    // Start is called before the first frame update
    void Start()
    {
        roomCount = numberOfRooms;
        cycleCount = numberOfCycles;

        SpawnRoomNodes();

        MoveableRooms = Room.Rooms;

        SpawnDoorVertices();
    }

    private void Update()
    {
        if (moveRoomNodes)
        {
            if (MoveableRooms.Count > 0)
            {
                roomsAreMoving = StartCoroutine(MoveRoomNodesTowardNeighbors());
                FindRoomToBuild();
            }

            if (roomToBuild != null) 
            {
                roomBeingBuilt = StartCoroutine(BuildRoom());
            }
        }
    }

    private void SpawnRoomNodes()
    {
        for (int i = 0; i < roomCount; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.position = new Vector3Int(Seed.Random(-roomCount, roomCount), Seed.Random(-roomCount, roomCount), Seed.Random(-roomCount, roomCount));
            go.AddComponent<Room>();
            go.GetComponent<Room>().InitializeRoom(i);
            go.AddComponent<SphereCollider>();
            go.AddComponent<Rigidbody>();
            go.GetComponent<Rigidbody>().useGravity = false;
        }
    }

    private void SpawnDoorVertices()
    {
        int counter = 0;
        int rand;

        while (counter < cycleCount) {
            rand = Seed.Random(1, roomCount - 1);
            if (!Room.Rooms[rand].HasCycle)
            {
                Room.Rooms[rand].HasCycle = true;
                counter++;
            }
        }

        foreach (Room rn in Room.Rooms)
        {
            if (rn.HasCycle)
            {
                //Randomly determines which room to cycle back to
                int prevRoomNum = Seed.Random(0, rn.Order - 1);
                //Assigns Vertex between a lower order room (roomNum) and this room (r)
                Door backDoor = new Door(rn, Room.Rooms[prevRoomNum]);
                //Randomly determines which room to cycle foward from
                int proceedFromRoomNum = Seed.Random(0, rn.Order);
                //Assigns Vertex between a Room (proceedFromRoomNum) and this next room (rn.Order+1)
                Door forwardDoor = new Door(Room.Rooms[proceedFromRoomNum], Room.Rooms[rn.Order + 1]);
            }
            else if (rn.Order > 0)
            {
                Door forwardDoor = new Door(Room.Rooms[rn.Order - 1], rn);
            }
        }
    }

    private static IEnumerator MoveRoomNodesTowardNeighbors()
    {
        Vector3 newPosition = Vector3.zero;

        foreach (Room r in MoveableRooms)
        {
            foreach (Room n in r.Neighbors)
            {
                newPosition += n.transform.position;
            }
            newPosition /= (r.Neighbors.Count + 1);
            r.transform.position = Vector3.Lerp(r.transform.position, newPosition, 0.05f);
            r.transform.GetComponent<Rigidbody>().isKinematic = true;
            yield return null;
            r.transform.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    private static void FindRoomToBuild()
    {
        //if rooms are stable and no room is being built then assign room to roomToBuild and remove room from MoveableRooms
        if(Room.RoomsAreStable() && roomToBuild == null)
        {
            Room iteratedRoom = null;

            foreach(Room r in MoveableRooms)
            {
                if(iteratedRoom == null)
                {
                    iteratedRoom = r;
                } else
                {
                    if(r.Neighbors.Count > iteratedRoom.Neighbors.Count)
                    {
                        iteratedRoom = r;
                    }
                }
            }
            roomToBuild = iteratedRoom;
            roomToBuild.GetComponent<Rigidbody>().isKinematic = true;
            MoveableRooms.Remove(iteratedRoom);
        }
    }

    private static IEnumerator BuildRoom()
    {
        //Build room
        //once built assign roomToBuild to null
        yield return null;
    }
}