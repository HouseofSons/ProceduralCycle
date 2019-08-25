using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int gridSize; //for Inspector
    public int blockSize; //for Inspector

    public static int GridSize;
    public static int BlockSize;

    public static bool GameHasStarted { get; private set; }
    public static bool PausePlayerMovement { get; private set; }
    //Coroutines
    public bool Coroutine_RCAA; //Inspector Property for Testing

    void Awake()
    {
        GridSize = gridSize;
        BlockSize = blockSize;

        MapGrid.GridBlocks = new Block[GridSize, GridSize, GridSize];
    }

    private void Start()
    {
        GameHasStarted = false;
        PausePlayerMovement = false;
    }

    void Update()
    {
        if(!GameHasStarted)
        {
            Block.UpdateAllColliders();
            GameHasStarted = true;
        } else
        {//game is running
            if(Coroutine_RCAA)
            {
                Coroutine_RCAA = false;
                StartCoroutine(RotateCharactersAroundAxis(Vector3.up,90));
            }
        }
    }

    public static IEnumerator RotateCharactersAroundAxis(Vector3 axis, int degrees)
    {
        PausePlayerMovement = true;
        
        foreach (Player p in Player.Players)
        {
            p.RotatePlayer(axis, degrees);
        }

        while (PlayerCamera.AllCamerasTurning())
        {
            yield return null;
        }

        PausePlayerMovement = false;
    }
}
