using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid
{
    public static Block[,,] Blocks { get; private set; }

    public MapGrid()
    {
        Blocks = new Block[LevelManager.GridSize, LevelManager.GridSize, LevelManager.GridSize];
    }

    public static Vector3Int UpdateBlockLocation(Block b, Vector3Int prevLocation, Vector3 newLocation)
    {
        if (prevLocation != Vector3Int.down)
        {
            Blocks[Mathf.RoundToInt(prevLocation.x), Mathf.RoundToInt(prevLocation.y), Mathf.RoundToInt(prevLocation.z)] = null;
        }
        Blocks[Mathf.RoundToInt(newLocation.x), Mathf.RoundToInt(newLocation.y), Mathf.RoundToInt(newLocation.z)] = b;
        return new Vector3Int(Mathf.RoundToInt(newLocation.x), Mathf.RoundToInt(newLocation.y), Mathf.RoundToInt(newLocation.z));
    }

    public static bool[] BlockHasColumnNeighbors(Vector3Int blockLocation, int playerForwardCoordinate)
    {
        //+X,-X,+Y,-Y,+Z,-Z
        bool xp,xm,yp,ym,zp,zm;

        if (playerForwardCoordinate == 0)
        {
            xp = false;
            xm = false;
            yp = ColumnOccupied(new Vector2Int(blockLocation.y + 1, blockLocation.z), playerForwardCoordinate);
            ym = ColumnOccupied(new Vector2Int(blockLocation.y - 1, blockLocation.z), playerForwardCoordinate);
            zp = ColumnOccupied(new Vector2Int(blockLocation.y, blockLocation.z + 1), playerForwardCoordinate);
            zm = ColumnOccupied(new Vector2Int(blockLocation.y, blockLocation.z - 1), playerForwardCoordinate);
        }
        else if (playerForwardCoordinate == 1)
        {
            xp = ColumnOccupied(new Vector2Int(blockLocation.x + 1, blockLocation.z), playerForwardCoordinate);
            xm = ColumnOccupied(new Vector2Int(blockLocation.x - 1, blockLocation.z), playerForwardCoordinate);
            yp = false;
            ym = false;
            zp = ColumnOccupied(new Vector2Int(blockLocation.x, blockLocation.z + 1), playerForwardCoordinate);
            zm = ColumnOccupied(new Vector2Int(blockLocation.x, blockLocation.z - 1), playerForwardCoordinate);
        }
        else /*(playerFacingCoordinate == 2)*/
        {
            xp = ColumnOccupied(new Vector2Int(blockLocation.x + 1, blockLocation.y), playerForwardCoordinate);
            xm = ColumnOccupied(new Vector2Int(blockLocation.x - 1, blockLocation.y), playerForwardCoordinate);
            yp = ColumnOccupied(new Vector2Int(blockLocation.x, blockLocation.y + 1), playerForwardCoordinate);
            ym = ColumnOccupied(new Vector2Int(blockLocation.x, blockLocation.y - 1), playerForwardCoordinate);
            zp = false;
            zm = false;
        }

        return new bool[6] { xp, xm, yp, ym, zp, zm };
    }

    private static bool ColumnOccupied(Vector2Int plane, int colCoord)
    {
        int q, r, s, t, u, v;
        if (colCoord == 0)         { q = 0; r = Blocks.GetLength(0); s = plane.x; t = plane.x; u = plane.y; v = plane.y; }
        else if (colCoord == 1)    { q = plane.x; r = plane.x; s = 0; t = Blocks.GetLength(1); u = plane.y; v = plane.y; }
        else  /*(colCoord == 2)*/  { q = plane.x; r = plane.x; s = plane.y; t = plane.y; u = 0; v = Blocks.GetLength(2); }

        for (int i = q; i < r; i++) {
            for (int j = s; j < t; j++) {
                for (int k = u; k < v; k++) {
                    if (Blocks[i, j, k] != null) {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
