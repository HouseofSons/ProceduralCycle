using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Need to consider if Blocks will be moving during map turn
//Map turn process should take enough time to align Blocks with a MapGridPosition

public class Block : MonoBehaviour
{
    public static List<Block> Blocks = new List<Block>();

    public Vector3Int SpawnGridLocation { get; set; }
    public Vector3Int CurrentMapGridLocation { get; set; }

    void Start()
    {
        Blocks.Add(this);
        MapGrid.InitializeGridLocation(this);
    }

    void Update()
    {

    }

    public static void UpdateAllGridLocations()
    {
        foreach (Block b in Blocks)
        {
            b.UpdateBlockColliders();
        }
    }

    public static void UpdateAllColliders()
    {
        foreach (Block b in Blocks)
        {
            b.UpdateBlockColliders();
        }
    }

    public static void MoveAllBlocksToSpawnLocation()
    {
        foreach (Block b in Blocks)
        {
            MapGrid.UpdateGridLocation(b, b.SpawnGridLocation);
        }
    }

    public static void CollapseAllBlocksToFace()
    {
        if (LevelManager.FacingCoordinate == 0 || LevelManager.FacingCoordinate == 3)
        {
            for (int i = 0; i < MapGrid.Blocks.GetLength(1); i++)
            {
                for (int j = 0; j < MapGrid.Blocks.GetLength(2); j++)
                {
                    for (int k = 0; k < MapGrid.Blocks.GetLength(0); k++)
                    {
                        if (LevelManager.FacingCoordinate == 0 && MapGrid.Blocks[0, j, i] == null) {

                            if (MapGrid.Blocks[k, j, i] != null)
                            {
                                MapGrid.UpdateGridLocation(MapGrid.Blocks[k, j, i], new Vector3(0, j * LevelManager.BlockSize, i * LevelManager.BlockSize));
                            }
                        } else
                        {
                            break;
                        }
                    }
                    for (int k = MapGrid.Blocks.GetLength(0) - 1; k >= 0; k--)
                    {
                        if (LevelManager.FacingCoordinate == 3 && MapGrid.Blocks[MapGrid.Blocks.GetLength(0) - 1, j, i] == null)
                        {
                            if (MapGrid.Blocks[k, j, i] != null)
                            {
                                MapGrid.UpdateGridLocation(MapGrid.Blocks[k, j, i], new Vector3((MapGrid.Blocks.GetLength(0) - 1) * LevelManager.BlockSize, j * LevelManager.BlockSize, i * LevelManager.BlockSize));
                            }
                        } else
                        {
                            break;
                        }
                    }
                }
            }
        } else if (LevelManager.FacingCoordinate == 2 || LevelManager.FacingCoordinate == 5)
        {
            for (int i = 0; i < MapGrid.Blocks.GetLength(0); i++)
            {
                for (int j = 0; j < MapGrid.Blocks.GetLength(1); j++)
                {
                    for (int k = 0; k < MapGrid.Blocks.GetLength(2); k++)
                    {
                        if (LevelManager.FacingCoordinate == 2 && MapGrid.Blocks[i, j, 0] == null)
                        {
                            if (MapGrid.Blocks[i, j, k] != null)
                            {
                                MapGrid.UpdateGridLocation(MapGrid.Blocks[i, j, k], new Vector3(i * LevelManager.BlockSize, j * LevelManager.BlockSize, 0));
                            }
                        } else
                        {
                            break;
                        }
                    }
                    for (int k = MapGrid.Blocks.GetLength(2) - 1; k >= 0; k--)
                    {
                        if (LevelManager.FacingCoordinate == 5 && MapGrid.Blocks[i, j, MapGrid.Blocks.GetLength(2) - 1] == null)
                        {
                            if (MapGrid.Blocks[i, j, k] != null)
                            {
                                MapGrid.UpdateGridLocation(MapGrid.Blocks[i, j, k], new Vector3(i * LevelManager.BlockSize, j * LevelManager.BlockSize, (MapGrid.Blocks.GetLength(2) - 1) * LevelManager.BlockSize));
                            }
                        } else
                        {
                            break;
                        }
                    }
                }
            }
        } else
        {
            Debug.Log("Bad Face Coordinate");
        }
    }

    public void UpdateBlockColliders()
    {
        Block[] sidesEnabled = MapGrid.GridLocationHasNeighbors(CurrentMapGridLocation);

        for (int i = 0; i < sidesEnabled.Length; i++)
        {
            if (sidesEnabled[i])
            {
                this.gameObject.transform.GetChild(0).GetChild(i).GetComponent<MeshCollider>().enabled = false;
                //Debug.Log(this.gameObject.name + " wall: " + this.gameObject.transform.GetChild(0).GetChild(i).name + " bool: " + false);
            } else
            {
                this.gameObject.transform.GetChild(0).GetChild(i).GetComponent<MeshCollider>().enabled = true;
                //Debug.Log(this.gameObject.name + " wall: " + this.gameObject.transform.GetChild(0).GetChild(i).name + " bool: " + true);
            }
        }
    }
    //Might be used in future to update block neighbor colliders
    //Assumption is we might update block and neighbor colliders during gameplay
    public static void UpdateNeighboringBlockColliders(Block b)
    {
        b.UpdateBlockColliders(); //update this blocks colliders

        Block[] blocksToUpdate = MapGrid.GridLocationHasNeighbors(b.CurrentMapGridLocation);
        //update neighbors blocks colliders
        foreach (Block btu in blocksToUpdate)
        {
            btu.UpdateBlockColliders();
        }
    }
}
