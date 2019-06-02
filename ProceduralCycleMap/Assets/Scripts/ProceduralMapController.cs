using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralMapController : MonoBehaviour
{
    [Range(2, 20)]
    public int numberOfRooms;
    [Range(1, 20)]  
    public int numberOfCycles;
    [Range(0.0f, 1.0f)]
    public float chanceRoomCycles;

    private static int roomCountPerCycle;
    private static int countOfCycles;

    // Start is called before the first frame update
    void Start()
    {
        roomCountPerCycle = numberOfRooms;
        countOfCycles = numberOfCycles;

        for (int i = 0; i < numberOfCycles; i++)
        {
            SpawnRoomNodes(i);
        }

        for(int i=0;i< numberOfCycles;i++) {
            SpawnDoorVertices(i);
        }
    }

    private void SpawnRoomNodes(int cycle)
    {
        for(int i = 0;i< roomCountPerCycle; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.position = new Vector3Int(i, 0, 0);
            go.AddComponent<Room>();
            go.GetComponent<Room>().InitializeRoom(i);
        }
    }

    private void SpawnDoorVertices(int cycle)   
    {
        foreach (Room rn in Room.Rooms)
        {
            //Initial rooms and final rooms shouldn't cycle
            if (rn.Order > 0 && rn.Order != roomCountPerCycle - 1)
            {
                //Randomly determine if room will cycle back
                if (Seed.Random() <= chanceRoomCycles)
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
            } else //BUGGGGG
            {
                if (rn.Order > 0)
                {
                    Door forwardDoor = new Door(Room.Rooms[rn.Order - 1], rn);
                }
            }
        }
    }
}
