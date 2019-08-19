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

    public static bool InitializeGridLocation(Block b)
    {
        Vector3Int location = new Vector3Int(Mathf.RoundToInt(b.transform.position.x), Mathf.RoundToInt(b.transform.position.y), Mathf.RoundToInt(b.transform.position.z));
        
        if (location.x % BlockSize == 0 && location.y % BlockSize == 0 && location.z % BlockSize == 0)
        {
            Vector3Int gridLocation = new Vector3Int(location.x / LevelManager.BlockSize, location.y / LevelManager.BlockSize, location.z / LevelManager.BlockSize);
            b.SpawnGridLocation = location;//for initial map positions no need to divide by BlockSize
            b.CurrentMapGridLocation = gridLocation;
            Blocks[b.CurrentMapGridLocation.x, b.CurrentMapGridLocation.y, b.CurrentMapGridLocation.z] = b;
            return true;
        }
        Debug.Log("Block Askew: " + b.transform.position);
        return false;
    }
    
    public static bool UpdateGridLocation(Block b, Vector3 location)
    {
        Vector3Int nl = new Vector3Int(Mathf.RoundToInt(location.x), Mathf.RoundToInt(location.y), Mathf.RoundToInt(location.z));

        if (nl.x % BlockSize == 0 && nl.y % BlockSize == 0 && nl.z % BlockSize == 0)
        {
            Vector3Int newLocation = new Vector3Int(nl.x / LevelManager.BlockSize, nl.y / LevelManager.BlockSize, nl.z / LevelManager.BlockSize);

            if (Blocks[b.CurrentMapGridLocation.x, b.CurrentMapGridLocation.y, b.CurrentMapGridLocation.z] == b)
            {
                Blocks[b.CurrentMapGridLocation.x, b.CurrentMapGridLocation.y, b.CurrentMapGridLocation.z] = null;
            }

            b.CurrentMapGridLocation = newLocation;
            Blocks[b.CurrentMapGridLocation.x, b.CurrentMapGridLocation.y, b.CurrentMapGridLocation.z] = b;
            b.transform.position = new Vector3Int(
                b.CurrentMapGridLocation.x * LevelManager.BlockSize,
                b.CurrentMapGridLocation.y * LevelManager.BlockSize,
                b.CurrentMapGridLocation.z * LevelManager.BlockSize);
            return true;
        }
        return false;
    }

    public static Block[] GridLocationHasNeighbors(Vector3Int location)
    {
        //+X,-X,+Y,-Y,+Z,-Z
        Block[] neighbors = new Block[6];

        Vector3Int[] neighborCheck = new Vector3Int[6];

        neighborCheck[0] = new Vector3Int(location.x + 1, location.y, location.z);
        neighborCheck[1] = new Vector3Int(location.x - 1, location.y, location.z);
        neighborCheck[2] = new Vector3Int(location.x, location.y + 1, location.z);
        neighborCheck[3] = new Vector3Int(location.x, location.y - 1, location.z);
        neighborCheck[4] = new Vector3Int(location.x, location.y, location.z + 1);
        neighborCheck[5] = new Vector3Int(location.x, location.y, location.z - 1);

        for (int i = 0; i < neighborCheck.Length; i++)
        {
            if (!(neighborCheck[i].x < 0 || neighborCheck[i].x > LevelManager.GridSize - 1 ||
                neighborCheck[i].y < 0 || neighborCheck[i].y > LevelManager.GridSize - 1 ||
                neighborCheck[i].z < 0 || neighborCheck[i].z > LevelManager.GridSize - 1))
            {
                neighbors[i] = Blocks[neighborCheck[i].x, neighborCheck[i].y, neighborCheck[i].z];
            }
        }
        return neighbors;
    }
}
