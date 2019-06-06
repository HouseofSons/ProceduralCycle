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
    private bool[] BuiltCheck = new bool[6];

    public void Awake()
    {
        Rooms.Add(this);
        Order = Rooms.Count - 1;
        HasCycle = false;
    }

    public void Start()
    {
        StartCoroutine(MoveRoomNodesTowardNeighbors());
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

    public static bool AreAllRoomsBuilt()
    {
        foreach(Room r in Rooms)
        {
            if(!r.Built())
            {
                return false;
            }
        }
        return true;
    }

    public bool Built()
    {
        foreach (bool b in BuiltCheck)
        {
            if (!b)
            {
                return false;
            }
        }
        return false;
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
                if (curSize.x >= Size.x + 0.5f)
                {
                    BuiltCheck[0] = true;
                    BuiltCheck[1] = true;
                    return;
                }
                break;
            case 'y':
                if (curSize.y >= Size.y + 0.5f)
                {
                    BuiltCheck[2] = true;
                    BuiltCheck[3] = true;
                    return;
                }
                break;
            default:
                if (curSize.z >= Size.z + 0.5f)
                {
                    BuiltCheck[4] = true;
                    BuiltCheck[5] = true;
                    return;
                }
                break;
        }

        switch (coord)
        {
            case 'x':
                testPos = new Vector3(curPos.x + 0.25f * sign, curPos.y, curPos.z);
                testSize = new Vector3(curSize.x + 0.5f, curSize.y, curSize.z);
                break;
            case 'y':
                testPos = new Vector3(curPos.x, curPos.y + 0.25f * sign, curPos.z);
                testSize = new Vector3(curSize.x, curSize.y + 0.5f, curSize.z);
                break;
            default:
                testPos = new Vector3(curPos.x, curPos.y, curPos.z + 0.25f * sign);
                testSize = new Vector3(curSize.x, curSize.y, curSize.z + 0.5f);
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
                        BuiltCheck[0] = true;
                    } else
                    {
                        BuiltCheck[1] = true;
                    }
                    break;
                case 'y':
                    if (sign < 0)
                    {
                        BuiltCheck[2] = true;
                    } else
                    {
                        BuiltCheck[3] = true;
                    }
                    break;
                default:
                    if (sign < 0)
                    {
                        BuiltCheck[4] = true;
                    } else
                    {
                        BuiltCheck[5] = true;
                    }
                    break;
            }
        }
    }

    private IEnumerator MoveRoomNodesTowardNeighbors()
    {
        Vector3 newPosition = Vector3.zero;

        while (!ProceduralMapController.RoomsAreConnected)
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
    }
}
