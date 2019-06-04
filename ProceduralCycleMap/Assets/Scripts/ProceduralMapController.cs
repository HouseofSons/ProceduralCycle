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

    public static Graph G { get; private set; }

    private Coroutine roomsAreMoving;

    private static bool roomsShouldStop;

    // Start is called before the first frame update
    void Start()
    {
        roomCount = numberOfRooms;
        cycleCount = numberOfCycles;

        G = new Graph(roomCount);

        SpawnRoomNodes();
        SpawnDoorVertices();

        roomsShouldStop = false;
    }

    private void Update()
    {
        if (!roomsShouldStop)
        {
            roomsAreMoving = StartCoroutine(MoveRoomNodesTowardNeighbors());
            roomsShouldStop = RoomsAreConnected();
        }
    }

    private void SpawnRoomNodes()
    {
        for (int i = 0; i < roomCount; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.position = new Vector3Int(Seed.Random(-roomCount * 3, roomCount * 3), Seed.Random(-roomCount * 3, roomCount * 3), Seed.Random(-roomCount * 3, roomCount * 3));
            go.AddComponent<Room>();
            go.GetComponent<Room>().InitializeRoom(i);
            go.AddComponent<SphereCollider>();
            go.GetComponent<SphereCollider>().isTrigger = true;
            go.AddComponent<Rigidbody>();
            go.GetComponent<Rigidbody>().useGravity = false;
            go.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
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

        foreach (Room r in Room.Rooms)
        {
            foreach (Room n in r.Neighbors)
            {
                newPosition += n.transform.position;
            }
            newPosition /= (r.Neighbors.Count + 1);
            r.transform.position = Vector3.Lerp(r.transform.position, newPosition, Time.deltaTime * 3);
            r.transform.GetComponent<Rigidbody>().isKinematic = true;
            yield return null;
            r.transform.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    private static bool RoomsAreConnected()
    {
        if (Time.frameCount >= 300)
        {
            if (Time.frameCount % 20 == 0) {
                return G.IsGraphConnected();
            }
        }
        return false;
    }
}