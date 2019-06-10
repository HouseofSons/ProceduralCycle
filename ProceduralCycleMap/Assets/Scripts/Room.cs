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
    private bool[] XZFitCheck = new bool[4];
    public bool YFitCheck { get; private set; }

    public bool MoveRoomNode { get; set; }
    public bool RoomReadyToFit { get; set; }

    public void Awake()
    {
        Rooms.Add(this);
        Order = Rooms.Count - 1;
        HasCycle = false;

        MoveRoomNode = false;
        RoomReadyToFit = false;
    }

    public void Start()
    {
        MoveRoomNode = true;
    }

    public void Update()
    {
        if (MoveRoomNode)
        {
            StartCoroutine(MoveRoomNodeTowardNeighbors());//called only once
        }
        if (RoomReadyToFit && ProceduralMapController.RoomNodesConnected)
        {
            TurnRoomSpheresToCubes();//called only once
            StartCoroutine(FitRoom());//called only once
        }
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

    private void TurnRoomSpheresToCubes()
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //InvertMesh.FlipNormals(go.GetComponent<MeshFilter>().mesh);
        this.name = this.Order.ToString();
        this.GetComponent<MeshFilter>().mesh = go.GetComponent<MeshFilter>().mesh;
        Destroy(this.GetComponent<SphereCollider>());
        this.gameObject.AddComponent<BoxCollider>();
        this.transform.position = new Vector3(Mathf.RoundToInt(this.transform.position.x),
            Mathf.RoundToInt(Mathf.RoundToInt(this.transform.position.y / 2) * 2),
            Mathf.RoundToInt(this.transform.position.z));
        this.transform.localScale = new Vector3(1, 4, 1);
        Destroy(go);
    }

    public bool XZFit()
    {
        for(int i=0;i<4;i++)
        {
            if (!XZFitCheck[i])
            {
                return false;
            }
        }
        return true;
    }

    public static bool YFit()
    {
        foreach(Room r in Rooms)
        {
            if (!r.YFitCheck)
            {
                foreach(Room r0 in Rooms)
                {
                    r0.YFitCheck = false;
                }
                return false;
            }
        }
        return true;
    }

    public void BuildOutRoomFrame(char coord, int sign)
    {
        Vector3 curPos = this.transform.position;
        Vector3 curSize = this.transform.localScale;
        Vector3 testPos;
        Vector3 testSize;

        switch (coord)
        {
            case 'x':
                if (curSize.x >= Size.x + 4f)   
                {
                    XZFitCheck[0] = true;
                    XZFitCheck[1] = true;
                    return;
                }
                testPos = new Vector3(curPos.x + 1 * sign, curPos.y, curPos.z);
                testSize = new Vector3(curSize.x + 2, curSize.y, curSize.z);
                break;
            case 'y':
                testPos = new Vector3(curPos.x, curPos.y < 0 ? curPos.y + 2 : (curPos.y > 0 ? curPos.y - 2 : curPos.y), curPos.z);
                testSize = new Vector3(curSize.x, curSize.y, curSize.z);
                break;
            default:
                if (curSize.z >= Size.z + 4f)
                {
                    XZFitCheck[2] = true;
                    XZFitCheck[3] = true;
                    return;
                }
                testPos = new Vector3(curPos.x, curPos.y, curPos.z + 1 * sign);
                testSize = new Vector3(curSize.x, curSize.y, curSize.z + 2);
                break;
        }

        if (!(Physics.OverlapBox(testPos, testSize / 2.01f).Length > 1))
        {
            this.transform.position = testPos;
            this.transform.localScale = testSize;  
        } else
        {
            switch (coord)
            {
                case 'x':
                    if (sign < 0)
                    {
                        XZFitCheck[0] = true;
                    } else
                    {
                        XZFitCheck[1] = true;
                    }
                    break;
                case 'y':
                    YFitCheck = true;
                    break;
                default:
                    if (sign < 0)
                    {
                        XZFitCheck[2] = true;
                    } else
                    {
                        XZFitCheck[3] = true;
                    }
                    break;
            }
        }
    }

    public void CollisionCheck(string message)
    {
        Collider[] cols = Physics.OverlapBox(this.transform.position, this.transform.localScale / 2.01f);

        if (cols.Length > 1)
        {
            string collidingRooms = "";
            foreach (Collider col in cols)
            {
                collidingRooms += " " + col.name;
            }
            Debug.Log(message + ": " + collidingRooms);
        }
    }

    private IEnumerator MoveRoomNodeTowardNeighbors()
    {
        MoveRoomNode = false; //limit coroutine to single call in Update()
        Vector3 newPosition = Vector3.zero;

        while (!ProceduralMapController.RoomNodesConnected)
        {
            foreach (Room n in this.Neighbors)
            {
                newPosition += n.transform.position;
            }

            newPosition /= Neighbors.Count + 1;
            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * 3);
            GetComponent<Rigidbody>().isKinematic = false;
            yield return null;
            GetComponent<Rigidbody>().isKinematic = true;
        }
        RoomReadyToFit = true;
    }

    private IEnumerator FitRoom()
    {
        RoomReadyToFit = false; //limit coroutine to single call in Update()

        char[] CoordOrder = new char[2];

        CoordOrder[0] = 'x';
        CoordOrder[1] = 'z';

        int[] SignOrder = new int[2];

        SignOrder[0] = -1;
        SignOrder[1] = 1;

        List<Room> roomsToBuild = Rooms;

        Seed.Shuffle(roomsToBuild);

        while (!XZFit())
        {
            foreach (char c in CoordOrder)
            {
                foreach (int i in SignOrder)
                {
                    if (int.Parse(name) % 5 == Time.frameCount % 5)
                    {
                        if (!XZFit())
                        {
                            BuildOutRoomFrame(c, i);
                        }
                    }
                    yield return null;
                }
            }
        }
        while (!YFit())
        {
            if (int.Parse(name) % 5 == Time.frameCount % 5)
            {
                BuildOutRoomFrame('y', 0);
            }
            yield return null;
        }
        CollisionCheck("Room Fits");
    }
}