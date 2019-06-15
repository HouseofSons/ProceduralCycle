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

    // Start is called before the first frame update
    void Awake()
    {
        roomCount = numberOfRooms;
        cycleCount = numberOfCycles;
    }

    private void Start()
    {

    }

    public void Update()
    {

    }
}