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
            foreach(Block b in Block.Blocks)//should be removed in future
            {
                b.UpdateBlockColliders();
            }
            gameHasStarted = true;
        }
    }

    public static int FacingCoordinate()
    {
        if (Mathf.RoundToInt(GameOrientation.transform.forward.x) == 1)
        {
            return 0;
        }
        if (Mathf.RoundToInt(GameOrientation.transform.forward.y) == 1)
        {
            return 1;
        }
        return 2;
    }
}
