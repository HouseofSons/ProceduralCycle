using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralMapController : MonoBehaviour
{
    [Range(2, 60)]
    public int numberOfRooms;
    [Range(0, 20)]
    public int numberOfCycles;

    private static int roomCount;
    private static int cycleCount;

    public static Graph G { get; private set; }

    private Coroutine roomsAreMoving;
    private Coroutine roomsAreGrowing;

    private static bool roomsShouldStop;
    private static bool roomsDoneGrowing;

    // Start is called before the first frame update
    void Start()
    {
        roomCount = numberOfRooms;
        cycleCount = numberOfCycles;

        G = new Graph(roomCount);

        SpawnRoomNodes();
        SpawnDoorVertices();
        AdjustRoomSizes();

        roomsShouldStop = false;
        roomsDoneGrowing = false;
    }

    private void Update()
    {
        if (!roomsShouldStop)
        {
            roomsAreMoving = StartCoroutine(MoveRoomNodesTowardNeighbors());
            roomsShouldStop = RoomsAreConnected();

            if (roomsShouldStop)
            {
                StopAllCoroutines();
                FreezeRoomPlan();
                AlignRoomsToGrid();
            }
        }
        else if (!roomsDoneGrowing)
        {
            roomsAreGrowing = StartCoroutine(GrowRoomNodesToFit());
            roomsDoneGrowing = RoomsAreGrown();

            if (roomsDoneGrowing)
            {

            }
        } else
        {


        }
    }

    private void SpawnRoomNodes()
    {
        for (int i = 0; i < roomCount; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.position = new Vector3Int(Seed.Random(-roomCount * 3, roomCount * 3), Seed.Random(-roomCount, roomCount), Seed.Random(-roomCount * 3, roomCount * 3));
            go.AddComponent<Room>();
            go.GetComponent<Room>().InitializeRoom(i);
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

    private void AdjustRoomSizes()
    {
        int roomWidth;
        int roomLength;
        int roomHeight;

        foreach (Room r in Room.Rooms)
        {
            roomWidth = Mathf.Max(Seed.Random(Mathf.RoundToInt(r.Neighbors.Count), Mathf.RoundToInt(r.Neighbors.Count) + 2),2);
            roomHeight = Seed.Random(1, Mathf.Max(Mathf.RoundToInt(r.Neighbors.Count/2),1));
            roomLength = Mathf.Max(Seed.Random(Mathf.RoundToInt(r.Neighbors.Count), Mathf.RoundToInt(r.Neighbors.Count) + 2),2);
            r.Size = new Vector3Int(roomWidth, roomHeight, roomLength);
            r.transform.localScale = r.Size;
        }
    }

    private static bool RoomsAreConnected()
    {
        if (Time.frameCount >= 300)
        {
            if (Time.frameCount % 20 == 0)
            {
                return G.IsGraphConnected();
            }
        }
        return false;
    }

    private static void FreezeRoomPlan()
    {
        foreach (Room r in Room.Rooms) {
            r.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    private static void AlignRoomsToGrid()
    {
        foreach(Room r in Room.Rooms)
        {
            int x = Mathf.RoundToInt(r.transform.position.x * 2) / 2;
            int y = Mathf.RoundToInt(r.transform.position.y * 2) / 2;
            int z = Mathf.RoundToInt(r.transform.position.z * 2) / 2;

            r.transform.localScale = Vector3.one;
            r.transform.position = new Vector3(x, y, z);
        }
    }

    private static bool RoomsAreGrown()
    {
        foreach(Room r in Room.Rooms)
        {
            if (!r.IsGrown[0] || !r.IsGrown[1] || !r.IsGrown[2])
            {
                return false;
            }
        }
        return true;
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
            //For quick graph collapsing
            if (r.Order == 0)
            {
                newPosition += Room.Rooms[roomCount-1].transform.position;
            } else if (r.Order == roomCount)
            {
                newPosition += Room.Rooms[0].transform.position;
            }

            newPosition /= (r.Neighbors.Count + 1);
            r.transform.position = Vector3.Lerp(r.transform.position, newPosition, Time.deltaTime * 5);
            r.transform.GetComponent<Rigidbody>().isKinematic = true;
            yield return null;
            r.transform.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    private static IEnumerator GrowRoomNodesToFit()
    {
        foreach(Room r in Room.Rooms)
        {
            for (int i=0;i<3;i++)
            {
                r.IsGrown[i] = r.AttemptEdgeGrowth(i);
                yield return null;
            }
        }
    }
}