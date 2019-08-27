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

    void Awake()
    {
        GridSize = gridSize;
        BlockSize = blockSize;

        MapGrid.GridBlocks = new Block[GridSize, GridSize, GridSize];
    }

    private void Start()
    {
        GameHasStarted = false;
        MapGrid.AddFaceBlocks();
    }

    void Update()
    {
        if(!GameHasStarted)
        {
            Block.UpdateAllColliders();
            GameHasStarted = true;
        } else
        {//game is running

        }
    }
}
