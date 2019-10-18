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

    public bool RotateMap; //Is Public for Inspector Testing
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
            if (RotateMap)
            {
                RotateMap = false;
                foreach(Player p in Player.Players)
                {
                    p.RotatingMap_Coroutine = true;
                }
            }
        }
    }
}
