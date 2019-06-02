using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralMapController : MonoBehaviour
{
    public int numberOfRooms;
    private static int roomCount;

    // Start is called before the first frame update
    void Start()
    {
        roomCount = numberOfRooms;
        SpawnRoomNodes();
    }

    private void SpawnRoomNodes()
    {
        for(int i = 0;i<roomCount;i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.position = new Vector3Int(i, 0, 0);
        }
    }
}
