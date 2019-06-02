using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralMapController : MonoBehaviour
{
    [Range(2, 20)]
    public int numberOfRooms;
    [Range(0.0f, 1.0f)]
    public float chanceRoomCyclesBack;

    private static int roomCount;

    // Start is called before the first frame update
    void Start()
    {
        roomCount = numberOfRooms;
        SpawnRoomNodes();
        //PlanDoors(); <--- here is bug!!!
    }

    private void SpawnRoomNodes()
    {
        for(int i = 0;i<roomCount;i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.position = new Vector3Int(i, 0, 0);
            go.AddComponent<Room>();
            go.GetComponent<Room>().AddOrder(i);
        }
    }

    private void PlanDoors()
    {
        foreach(Room r in Room.Rooms)
        {
            if(r.Order() > 0)
            {
                if(Seed.Random() <= chanceRoomCyclesBack)
                {
                    foreach(Room r0 in Room.Rooms)
                    {
                        if (r0.Order() == Seed.Random(0, r.Order()- 1)) {
                            Door d = new Door(r, r0);
                            break;
                        }
                    }
                }
            }
        }
    }
}
