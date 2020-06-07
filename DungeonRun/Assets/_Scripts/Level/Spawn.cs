using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject Room;

    public Room GetRoom()
    {
        return Room.GetComponent<Room>();
    }
}