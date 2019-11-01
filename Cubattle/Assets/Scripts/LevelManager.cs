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

    public static Vector3 AxisPosition { get; set; }

    void Awake()
    {
        GridSize = gridSize;
        BlockSize = blockSize;
        
        AxisPosition = new Vector3(GridSize / 2 * BlockSize, GridSize / 2 * BlockSize, GridSize / 2 * BlockSize);//for testing, can be adjusted with level design

        MapGrid.GridBlocks = new Block[GridSize, GridSize, GridSize];
    }

    private void Start()
    {
        GameHasStarted = false;
        MapGrid.AddFaceBlocks(); //Should only run for non-moving Blocks
    }

    void Update()
    {
        if(!GameHasStarted)
        {
            GameHasStarted = true;
        } else
        {//game is running

        }
    }
}
