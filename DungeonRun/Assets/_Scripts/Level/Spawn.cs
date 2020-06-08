using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    //----Inspector Populated Fields START
    public GameObject Room;
    public GameObject Partition;
    //----Inspector Populated Fields END

    public Room GetRoom()
    {
        return Room.GetComponent<Room>();
    }

    public Partition GetPartition()
    {
        return Partition.GetComponent<Partition>();
    }
}