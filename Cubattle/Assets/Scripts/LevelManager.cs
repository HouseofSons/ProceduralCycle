using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static int BlockSize;
    public static int GridSize;
    public static Transform MapOrientation { get; private set; }
    public static int FacingCoordinate { get; private set; }

    public static bool Paused { get; private set; }
    public bool GameHasStarted { get; private set; }

    void Awake()
    {
        GridSize = 64;
        BlockSize = 4;
        MapOrientation = this.gameObject.transform;
        _ = new MapGrid(GridSize, BlockSize);
    }

    private void Start()
    {
        Paused = false;
        GameHasStarted = false;

        UpdateFacingCoordinate();
    }

    public bool testMapRotation;//TESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTEST

    void Update()
    {
        if(!GameHasStarted)
        {
            //Collapse blocks to closest face
            //Move Players to closest Block of closest Face
            Block.UpdateAllBlockColliders();
            GameHasStarted = true;
        } else
        {
            //gamerunning
            if (testMapRotation)//TESTETTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTES
            {
                testMapRotation = false;
                RotateMapAroundAxis(Vector3.up);
            }
        }
    }

    private static void UpdateFacingCoordinate()
    {
        if (Mathf.RoundToInt(MapOrientation.transform.forward.x) == 1)
        {
            FacingCoordinate = 0;
        }
        else if (Mathf.RoundToInt(MapOrientation.transform.forward.y) == 1)
        {
            FacingCoordinate = 1;
        }
        else if (Mathf.RoundToInt(MapOrientation.transform.forward.z) == 1)
        {
            FacingCoordinate = 2;
        }
        else if (Mathf.RoundToInt(MapOrientation.transform.forward.x) == -1)
        {
            FacingCoordinate = 3;
        }
        else if (Mathf.RoundToInt(MapOrientation.transform.forward.y) == -1)
        {
            FacingCoordinate = 4;
        }
        else if (Mathf.RoundToInt(MapOrientation.transform.forward.z) == -1)
        {
            FacingCoordinate = 5;
        }
        else
        {
            Debug.Log("LevelManager Askew");
            FacingCoordinate = -1;
        }
    }

    //Rotates Cameras
    public static void RotateMapAroundAxis(Vector3 axis)
    {
        Paused = true;

        MapOrientation.RotateAround(MapOrientation.position, axis, 90);
        UpdateFacingCoordinate();

        //Move Blocks to Grid positions
        //Move Players to nearest Block of Current Face
        Block.UpdateAllBlockColliders();

        foreach (Player p in Player.Players)
        {
            p.transform.RotateAround(p.transform.position, axis, 90);
        }

        //if (/*all rotations are finished*/) {
        //Collapse blocks to closest face
        //Move Players to closest Block of closest Face
        Paused = false;
        //}
    }
}
