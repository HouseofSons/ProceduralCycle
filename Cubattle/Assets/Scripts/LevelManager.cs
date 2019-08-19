using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static int BlockSize;
    public static int GridSize;
    public static Transform MapOrientation { get; private set; }
    public static int FacingCoordinate { get; private set; }

    public static bool PlayerFreeze { get; private set; }
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
        PlayerFreeze = false;
        GameHasStarted = false;

        UpdateFacingCoordinate();
    }

    public bool testMapRotation;//TESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTEST

    void Update()
    {
        if(!GameHasStarted)
        {
            Block.CollapseAllBlocksToFace();
            Block.UpdateAllColliders();
            GameHasStarted = true;
        } else
        {
            //gamerunning
            if (testMapRotation)//TESTETTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTES
            {
                testMapRotation = false;
                StartCoroutine(RotateMapAroundAxis(Vector3.up));
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

    public static IEnumerator RotateMapAroundAxis(Vector3 axis)
    {
        PlayerFreeze = true;

        MapOrientation.RotateAround(MapOrientation.position, axis, 90);
        Player.MovePlayersTo3DPosition(axis);
        UpdateFacingCoordinate();
        Block.MoveAllBlocksToSpawnLocation();
        

        foreach (Player p in Player.Players)
        {
            p.transform.RotateAround(p.transform.position, axis, 90);
        }

        foreach (PlayerCamera pc in PlayerCamera.playerCameras)
        {
            pc.RotationInProgress = true;
        }

        while (PlayerCamera.CamerasTurning())
        {
            yield return null;
        }

        Block.CollapseAllBlocksToFace();
        Block.UpdateAllColliders();

        PlayerFreeze = false;
    }
}
