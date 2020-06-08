using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject Room;
    public GameObject Partition;

    public Room GetRoom()
    {
        return Room.GetComponent<Room>();
    }

    public Partition GetPartition()
    {
        return Partition.GetComponent<Partition>();
    }
}