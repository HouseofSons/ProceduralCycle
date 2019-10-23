using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid : MonoBehaviour
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

    public static void AddFaceBlocks()
    {
        for (int l = 0; l < 2; l++)
        {
            for (int i = 0; i < GridBlocks.GetLength(l == 0 ? 0 : 2); i++)
            {
                for (int j = 0; j < GridBlocks.GetLength(1); j++)
                {
                    List<Block> columnBlocks = new List<Block>();
                    for (int k = 0; k < GridBlocks.GetLength(l == 0 ? 2 : 0); k++)
                    {
                        if (l == 0)
                        {
                            if (GridBlocks[i, j, k] != null)
                                if (!GridBlocks[i, j, k].Cloned)
                                    { columnBlocks.Add(GridBlocks[i, j, k]); }
                        }
                        else
                        {
                            if (GridBlocks[k, j, i] != null)
                                if (!GridBlocks[k, j, i].Cloned)
                                    { columnBlocks.Add(GridBlocks[k, j, i]); }
                        }
                    }
                    if (l == 0)
                    {
                        CloneBlocks(columnBlocks, l, i, j);
                    } else
                    {
                        CloneBlocks(columnBlocks, l, j, i);
                    }

                    columnBlocks = new List<Block>();
                    for (int k = GridBlocks.GetLength(l == 0 ? 2 : 0) - 1; k >=0 ; k--)
                    {
                        if (l == 0)
                        {
                            if (GridBlocks[i, j, k] != null)
                                if (!GridBlocks[i, j, k].Cloned)
                                    { columnBlocks.Add(GridBlocks[i, j, k]); }
                        }
                        else
                        {
                            if (GridBlocks[k, j, i] != null)
                                if (!GridBlocks[k, j, i].Cloned)
                                    { columnBlocks.Add(GridBlocks[k, j, i]); }
                        }
                    }
                    if (l == 0)
                    {
                        CloneBlocks(columnBlocks, l + 2, i, j);
                    }
                    else
                    {
                        CloneBlocks(columnBlocks, l + 2, j, i);
                    }
                }
            }
        }
    }

    private static void CloneBlocks(List<Block> blocks, int direction, int uCoord, int vCoord)
    {   //0, 1, 2, 3
        //z+,x+,z-,x-
        Vector3Int edge;

        switch (direction)
        {
            case 0:
                edge = new Vector3Int(uCoord,vCoord,0);
                break;
            case 1:
                edge = new Vector3Int(0, uCoord, vCoord);
                break;
            case 2:
                edge = new Vector3Int(uCoord, vCoord, GridBlocks.GetLength(2) - 1);
                break;
            default:
                edge = new Vector3Int(GridBlocks.GetLength(0) - 1, uCoord, vCoord);
                break;
        }

        foreach (Block b in blocks)
        {
            if (b.GetType() == typeof(SolidBlock))
            {
                break;
            }

            if (GridBlocks[edge.x, edge.y, edge.z] == null)
            {
                InsideBlock edgeBlock = (Instantiate(Resources.Load("InsideBLock")) as GameObject).GetComponent<InsideBlock>();
                edgeBlock.transform.position = new Vector3Int(edge.x * LevelManager.BlockSize, edge.y * LevelManager.BlockSize, edge.z * LevelManager.BlockSize);
                InitializeGridLocation(edgeBlock);
                Destroy(edgeBlock.transform.GetChild(7).gameObject);
                Destroy(edgeBlock.transform.GetChild(8).gameObject);
                edgeBlock.FaceBlocks[direction] = edgeBlock;
                edgeBlock.Cloned = true;
            }

            b.FaceBlocks[direction] = GridBlocks[edge.x, edge.y, edge.z];
            if (GridBlocks[edge.x, edge.y, edge.z].FaceBlocks[direction] == GridBlocks[edge.x, edge.y, edge.z])
            {
                GridBlocks[edge.x, edge.y, edge.z].FaceBlocks[direction] = b;
            }
        }
    }
}