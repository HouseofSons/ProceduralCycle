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
    public static bool RoomsAreConnected { get; private set; }
    public static bool RoomsAreBuilt { get; private set; }

    private bool disableRoomFitting;

    // Start is called before the first frame update
    void Awake()
    {
        roomCount = numberOfRooms;
        cycleCount = numberOfCycles;

        G = new Graph(roomCount);
        RoomsAreConnected = false;

        SpawnRoomNodes();
        SpawnDoorVertices();
        AdjustPlannedRoomProperties();

        RoomsAreBuilt = false;
        disableRoomFitting = false;
    }

    void Start()
    {
        //Signals end of Room Movement
        StartCoroutine(CheckIfRoomsAreConnected());
    }

    public void Update()
    {
        if (!disableRoomFitting && RoomsAreConnected)
        {
            StartCoroutine(FitRoom());
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
            roomWidth = Seed.Random(3f, r.Neighbors.Count + 4);
            roomLength = Seed.Random(3f, r.Neighbors.Count + 4);
            roomHeight = Seed.Random(2, 4);
            r.transform.localScale = new Vector3(Mathf.Max(roomWidth,roomHeight,roomLength),
                Mathf.Max(roomWidth, roomHeight, roomLength),
                Mathf.Max(roomWidth, roomHeight, roomLength));
            r.Size = r.transform.localScale;

            r.transform.position = new Vector3Int(Seed.Random(-100, 100), Seed.Random(-100, 100), Seed.Random(-100, 100));
        }
    }

    private static void TurnRoomSpheresToCubes()
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);

        foreach (Room r in Room.Rooms)
        {
            r.name = r.Order.ToString();
            r.GetComponent<MeshFilter>().mesh = go.GetComponent<MeshFilter>().mesh;
            Destroy(r.GetComponent<SphereCollider>());
            r.gameObject.AddComponent<BoxCollider>();
            r.transform.position = new Vector3(Mathf.RoundToInt(r.transform.position.x),
                Mathf.RoundToInt(r.transform.position.y),
                Mathf.RoundToInt(r.transform.position.z));
            r.transform.localScale = Vector3.one;
        }
        Destroy(go);
    }

    private static IEnumerator CheckIfRoomsAreConnected()
    {
        while (!RoomsAreConnected)
        {
            if (Time.frameCount >= 100)
            {
                if (Time.frameCount % 20 == 0)
                {
                    RoomsAreConnected = G.IsGraphConnected();
                }
            }
            yield return null;
        }
        TurnRoomSpheresToCubes();
    }

    private IEnumerator FitRoom()
    {
        disableRoomFitting = true;

        char[] CoordOrder = new char[3];

        CoordOrder[0] = 'x';
        CoordOrder[1] = 'y';
        CoordOrder[2] = 'z';

        int[] SignOrder = new int[2];

        SignOrder[0] = -1;
        SignOrder[1] = 1;

        List<Room> roomsToBuild = Room.Rooms;

        Seed.Shuffle(roomsToBuild);

        while (!RoomsAreBuilt)
        {
            foreach(char c in CoordOrder)
            {
                foreach(int i in SignOrder)
                {
                    foreach (Room r in roomsToBuild)
                    {
                        if (!r.Built())
                        {
                            r.BuildOutRoomFrame(c, i);
                            RoomsAreBuilt = Room.AreAllRoomsBuilt();
                        }
                    }
                    yield return null;
                }
            }
        }
    }
}