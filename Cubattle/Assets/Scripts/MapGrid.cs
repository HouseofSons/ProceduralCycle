using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid
{
    public static Block[,,] Blocks { get; private set; }
    public static int BlockSize { get; private set; }

    public MapGrid(int gridSize, int blockSize)
    {
        Blocks = new Block[gridSize, gridSize, gridSize];
        BlockSize = blockSize;
    }

    public static bool UpdateBlockLocation(Block b, Vector3 location)
    {
        Vector3Int nl = new Vector3Int(Mathf.RoundToInt(location.x), Mathf.RoundToInt(location.y), Mathf.RoundToInt(location.z));

        if (nl.x % BlockSize == 0 && nl.y % BlockSize == 0 && nl.z % BlockSize == 0)
        {
            Vector3Int newLocation = new Vector3Int(nl.x / 4, nl.y / 4, nl.z / 4);

            if (newLocation != b.MapGridLocation)
            {
                Blocks[Mathf.RoundToInt(b.MapGridLocation.x), Mathf.RoundToInt(b.MapGridLocation.y), Mathf.RoundToInt(b.MapGridLocation.z)] = null;
                b.PrevMapGridLocation = b.MapGridLocation;
                b.MapGridLocation = newLocation;
                Blocks[newLocation.x, newLocation.y, newLocation.z] = b;
                UpdateMapWalls(b);
                return true;
            }
        }
        return false;
    }

    public static bool[] GridLocationHasNeighbors(Vector3Int blockLocation)
    {
        //+X,-X,+Y,-Y,+Z,-Z
        bool[] neighbors = new bool[6];

        Vector3Int[] neighborCheck = new Vector3Int[6];

        neighborCheck[0] = new Vector3Int(blockLocation.x + 1, blockLocation.y, blockLocation.z);
        neighborCheck[1] = new Vector3Int(blockLocation.x - 1, blockLocation.y, blockLocation.z);
        neighborCheck[2] = new Vector3Int(blockLocation.x, blockLocation.y + 1, blockLocation.z);
        neighborCheck[3] = new Vector3Int(blockLocation.x, blockLocation.y - 1, blockLocation.z);
        neighborCheck[4] = new Vector3Int(blockLocation.x, blockLocation.y, blockLocation.z + 1);
        neighborCheck[5] = new Vector3Int(blockLocation.x, blockLocation.y, blockLocation.z - 1);

        for (int i = 0; i < neighborCheck.Length; i++)
        {
            if (!(neighborCheck[i].x < 0 || neighborCheck[i].x > LevelManager.GridSize - 1 ||
                neighborCheck[i].y < 0 || neighborCheck[i].y > LevelManager.GridSize - 1 ||
                neighborCheck[i].z < 0 || neighborCheck[i].z > LevelManager.GridSize - 1))
            {
                if (Blocks[neighborCheck[i].x, neighborCheck[i].y, neighborCheck[i].z] == null)
                {
                    neighbors[i] = false;
                } else
                {
                    neighbors[i] = true;
                }
            }
        }
        return neighbors;
    }

    public static void UpdateMapWalls(Block b)
    {
        List<Block> blocksToUpdate = GridLocationColumnNeighborBlocks(b.MapGridLocation, Mathf.Abs(LevelManager.FacingCoordinate));
        blocksToUpdate.AddRange(GridLocationColumnNeighborBlocks(b.PrevMapGridLocation, Mathf.Abs(LevelManager.FacingCoordinate)));

        foreach (Block btu in blocksToUpdate)
        {
            btu.UpdateBlockColliders();
        }
    }

    public static List<Block> GridLocationColumnNeighborBlocks(Vector3Int blockLocation, int playerForwardCoordinate)
    {
        List<Block> columnNeighbors = new List<Block>();

        if (playerForwardCoordinate == 0 || playerForwardCoordinate == 3)
        {
            columnNeighbors.AddRange(ColumnBlocks(blockLocation.y + 1, blockLocation.z, playerForwardCoordinate));
            columnNeighbors.AddRange(ColumnBlocks(blockLocation.y - 1, blockLocation.z, playerForwardCoordinate));
            columnNeighbors.AddRange(ColumnBlocks(blockLocation.y, blockLocation.z + 1, playerForwardCoordinate));
            columnNeighbors.AddRange(ColumnBlocks(blockLocation.y, blockLocation.z - 1, playerForwardCoordinate));
        }
        else if (playerForwardCoordinate == 1 || playerForwardCoordinate == 4)
        {
            columnNeighbors.AddRange(ColumnBlocks(blockLocation.x + 1, blockLocation.z, playerForwardCoordinate));
            columnNeighbors.AddRange(ColumnBlocks(blockLocation.x - 1, blockLocation.z, playerForwardCoordinate));
            columnNeighbors.AddRange(ColumnBlocks(blockLocation.x, blockLocation.z + 1, playerForwardCoordinate));
            columnNeighbors.AddRange(ColumnBlocks(blockLocation.x, blockLocation.z - 1, playerForwardCoordinate));
        }
        else /*(playerForwardCoordinate == 2 || playerForwardCoordinate == 5)*/
        {
            columnNeighbors.AddRange(ColumnBlocks(blockLocation.x + 1, blockLocation.y, playerForwardCoordinate));
            columnNeighbors.AddRange(ColumnBlocks(blockLocation.x - 1, blockLocation.y, playerForwardCoordinate));
            columnNeighbors.AddRange(ColumnBlocks(blockLocation.x, blockLocation.y + 1, playerForwardCoordinate));
            columnNeighbors.AddRange(ColumnBlocks(blockLocation.x, blockLocation.y - 1, playerForwardCoordinate));
        }

        return columnNeighbors;
    }

    private static List<Block> ColumnBlocks(int x, int y, int colCoord)
    {
        List<Block> colBlocks = new List<Block>();

        if (x < 0 || x > LevelManager.GridSize - 1 ||
            y < 0 || y > LevelManager.GridSize - 1)
        {
            return colBlocks;
        }
        int q, r, s, t, u, v;
        if (colCoord == 0) { q = 0; r = Blocks.GetLength(0); s = x; t = x + 1; u = y; v = y + 1; }
        else if (colCoord == 1) { q = x; r = x + 1; s = 0; t = Blocks.GetLength(1); u = y; v = y + 1; }
        else  /*(colCoord == 2)*/  { q = x; r = x + 1; s = y; t = y + 1; u = 0; v = Blocks.GetLength(2); }

        for (int i = q; i < r; i++)
        {
            for (int j = s; j < t; j++)
            {
                for (int k = u; k < v; k++)
                {
                    if (Blocks[i, j, k] != null)
                    {
                        colBlocks.Add(Blocks[i, j, k]);
                    }
                }
            }
        }
        return colBlocks;
    }
}
