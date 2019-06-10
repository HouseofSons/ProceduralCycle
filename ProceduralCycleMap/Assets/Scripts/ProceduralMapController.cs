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

    public static bool CheckingIfRoomsConnect { get; private set; }
    public static bool RoomNodesConnected { get; private set; }

    public static bool CheckingIfRoomsAllFit { get; private set; }
    public static bool RoomsAllFit { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        roomCount = numberOfRooms;
        cycleCount = numberOfCycles;

        G = new Graph(roomCount);

        SpawnRoomNodes();
        SpawnDoorVertices();
        AdjustPlannedRoomProperties();

        CheckingIfRoomsConnect = false;
        RoomNodesConnected = false;

        CheckingIfRoomsAllFit = false;
        RoomsAllFit = false;
    }

    private void Start()
    {
        CheckingIfRoomsConnect = true;
    }

    public void Update()
    {
        if (CheckingIfRoomsConnect)
        {
            StartCoroutine(AreAllRoomsConnected());//called only once
        }
    }

    private static void SpawnRoomNodes()
    {
        for (int i = 0; i < roomCount; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.AddComponent<Room>();
        }
    }

    private static void SpawnDoorVertices()
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

    private static void AdjustPlannedRoomProperties()
    {
        float roomWidth;
        float roomLength;
        float roomHeight;

        foreach (Room r in Room.Rooms)
        {
            r.gameObject.AddComponent<Rigidbody>();
            r.GetComponent<Rigidbody>().useGravity = false;
            r.GetComponent<Rigidbody>().mass = r.Neighbors.Count;
            r.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            roomWidth = r.Neighbors.Count + 4;
            roomLength = r.Neighbors.Count + 4;
            roomHeight = 4;
            r.transform.localScale = new Vector3(Mathf.Max(roomWidth,roomHeight,roomLength),
                Mathf.Max(roomWidth, roomHeight, roomLength),
                Mathf.Max(roomWidth, roomHeight, roomLength));
            r.Size = r.transform.localScale;

            r.transform.position = new Vector3Int(Seed.Random(-100, 100), Seed.Random(-100, 100), Seed.Random(-100, 100));
        }
    }

    private static IEnumerator AreAllRoomsConnected()
    {
        CheckingIfRoomsConnect = false; //limit coroutine to single call in Update()
        bool done = false;

        while (!done)
        {
            if (Time.frameCount % 20 == 0)
            {
                done = G.IsGraphConnected();
            }
            yield return null;
        }
        RoomNodesConnected = true;
        CheckingIfRoomsAllFit = true;
    }

    private static IEnumerator AreAllRoomsFitting()
    {
        CheckingIfRoomsAllFit = false; //limit coroutine to single call in Update()
        bool done = false;

        while (!done)
        {
            done = true;

            if (Time.frameCount % 20 == 0)
            {
                foreach(Room r in Room.Rooms)
                {
                    if(!r.XZFit())
                    {
                        done = false;
                        break;
                    }
                }

                if(done)
                {
                    if (!Room.YFit())
                    {
                        done = false;
                    }
                }
            }
            yield return null;
        }
        RoomsAllFit = true;
    }
}