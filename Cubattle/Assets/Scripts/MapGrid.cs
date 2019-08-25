using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid
{
    public static Block[,,] GridBlocks { get; set; }

    public static bool InitializeGridLocation(Block b)
    {
        Vector3Int nl = new Vector3Int(Mathf.RoundToInt(b.transform.position.x), Mathf.RoundToInt(b.transform.position.y), Mathf.RoundToInt(b.transform.position.z));

        if (nl.x % LevelManager.BlockSize == 0 && nl.y % LevelManager.BlockSize == 0 && nl.z % LevelManager.BlockSize == 0)
        {
            Vector3Int newLocation = new Vector3Int(nl.x / LevelManager.BlockSize, nl.y / LevelManager.BlockSize, nl.z / LevelManager.BlockSize);
            b.CurrentMapGridLocation = newLocation;
            GridBlocks[b.CurrentMapGridLocation.x, b.CurrentMapGridLocation.y, b.CurrentMapGridLocation.z] = b;
            return true;
        }
        return false;
    }

    public static bool[] GridLocationHasBlockNeighbors(Block b, Vector3Int location)
    {
        //+X,-X,+Y,-Y,+Z,-Z
        bool[] neighbors = new bool[6];

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
                if (GridBlocks[neighborCheck[i].x, neighborCheck[i].y, neighborCheck[i].z] != null)
                {
                    if (GridBlocks[neighborCheck[i].x, neighborCheck[i].y, neighborCheck[i].z].GetType() == b.GetType())
                    {
                        neighbors[i] = true;
                    }
                }
            }
        }
        return neighbors;
    }
}