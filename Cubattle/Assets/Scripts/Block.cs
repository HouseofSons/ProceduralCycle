using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public static List<Block> Blocks = new List<Block>();//might be removed in future

    public Vector3Int MapGridLocation { get; set; }
    public Vector3Int PrevMapGridLocation { get; set; }

    public bool moved;

    void Start()
    {
        Blocks.Add(this);//might be removed in future
        MapGrid.UpdateBlockLocation(this,this.gameObject.transform.position);
    }

    void Update()
    {
        if (moved)
        {
            MapGrid.UpdateBlockLocation(this, this.gameObject.transform.position);
        }
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
        bool [] sidesEnabled;
        
        sidesEnabled = MapGrid.GridLocationHasNeighbors(MapGridLocation);
        
        for (int i = 0; i < 6; i++)
        {
            if(sidesEnabled[i])
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
}
