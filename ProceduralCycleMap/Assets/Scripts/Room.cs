using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public static List<Room> Rooms { get; } = new List<Room>();

    public List<Door> Doors { get; private set; } = new List<Door>();

    public void Awake()
    {

    }

    public void Start()
    {

    }

    public void Update()
    {

    }

    public void AddDoor(Room neighbor)
    {
        Doors.Add(new Door(this, neighbor));
    }
}