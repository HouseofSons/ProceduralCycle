using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public static List<Block> Blocks = new List<Block>();

    public Vector3Int MapGridLocation { get; set; }
    public Vector3Int PrevMapGridLocation { get; set; }

    void Start()
    {
        Blocks.Add(this);
        MapGrid.UpdateGridLocation(this, this.gameObject.transform.position);
    }

    void Update()
    {

    }

    public static void UpdateAllBlockColliders()
    {
        foreach(Block b in Blocks)
        {
            b.UpdateBlockColliders();
        }
    }

    public void UpdateBlockColliders()
    {
        Block[] sidesEnabled = MapGrid.GridLocationHasNeighbors(MapGridLocation);

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
    //Assumption is blocks may move during gameplay and will need to update colliders
    public static void UpdateNeighboringBlockColliders(Block b)
    {
        b.UpdateBlockColliders(); //update this blocks colliders

        Block[] blocksToUpdate = MapGrid.GridLocationHasNeighbors(b.MapGridLocation);
        //update neighbors blocks colliders
        foreach (Block btu in blocksToUpdate)
        {
            btu.UpdateBlockColliders();
        }
    }
}
