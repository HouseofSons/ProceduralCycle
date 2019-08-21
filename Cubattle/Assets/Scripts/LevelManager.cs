using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int gridSize; //for Inspector
    public int blockSize; //for Inspector

    public static int GridSize;
    public static int BlockSize;
    public static Transform MapOrientation { get; private set; }
    public static int FacingCoordinate { get; private set; }
    public static Vector3 MapCenter { get; private set; }

    public static bool PlayerFreeze { get; private set; }
    public static bool GameHasStarted { get; private set; }
    public static Vector3 MoveAxis { get; private set; }

    void Awake()
    {
        GridSize = gridSize;
        BlockSize = blockSize;
        MapOrientation = this.gameObject.transform;
        MapCenter = new Vector3(
            ((GridSize - 1) * BlockSize) / 2.0f,
            ((GridSize - 1) * BlockSize) / 2.0f,
            ((GridSize - 1) * BlockSize) / 2.0f);
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
            Block.CollapseBlocksToFace();
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
        MoveAxis = axis;

        Block.MoveBlocksToSpawnLocation();

        MapOrientation.Rotate(axis, 90);
        UpdateFacingCoordinate();

        foreach (Player p in Player.Players)
        {
            p.transform.position = new Vector3(p.OccupiedBlock.transform.position.x,p.transform.position.y,p.OccupiedBlock.transform.position.z);
            p.transform.Rotate(axis, 90);
        }

        foreach (PlayerCamera pc in PlayerCamera.playerCameras)
        {
            pc.RotationInProgress = true;
        }

        while (PlayerCamera.CamerasTurning())
        {
            yield return null;
        }

        Player.MovePlayersTo3DPosition(MoveAxis);

        Block.CollapseBlocksToFace();
        Block.UpdateAllColliders();

        PlayerFreeze = false;
    }
}
