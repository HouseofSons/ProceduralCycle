using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static int BlockSize { get; private set; }
    public static int GridSize { get; private set; }
    public static Transform GameOrientation { get; private set; }

    public bool gameHasStarted;

    void Awake()
    {
        GridSize = 64;
        BlockSize = 4;
        GameOrientation = this.gameObject.transform;
        _ = new MapGrid(GridSize, BlockSize);
        gameHasStarted = false;
    }

    void Update()
    {
        if(!gameHasStarted)
        {
            foreach(Block b in Block.Blocks)
            {
                b.UpdateBlockColliders();
            }
            gameHasStarted = true;
        }
    }
}
