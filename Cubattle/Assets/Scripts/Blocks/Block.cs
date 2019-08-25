using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Need to consider if Blocks will be moving during map turn

public class Block : MonoBehaviour
{
    public static List<Block> Blocks = new List<Block>();
    public Vector3Int CurrentMapGridLocation { get; set; }

    public Block[] FaceBlocks = new Block[4];

    protected virtual void Start()
    {
        Blocks.Add(this);
        MapGrid.InitializeGridLocation(this);
    }

    void Update()
    {

    }

    public void UpdateBlockColliders()
    {
        bool[] sidesEnabled = MapGrid.GridLocationHasBlockNeighbors(this,CurrentMapGridLocation);

        for (int i = 0; i < sidesEnabled.Length; i++)
        {
            this.gameObject.transform.GetChild(i).GetComponent<MeshCollider>().enabled = !sidesEnabled[i];//inside
            this.gameObject.transform.GetChild(i + 6).GetComponent<MeshCollider>().enabled = !sidesEnabled[i];//outside
            //Debug.Log(this.gameObject.name + " wall: " + this.gameObject.transform.GetChild(0).GetChild(i).name + " bool: " + false);
        }
    }

    public static void UpdateAllColliders()
    {
        foreach (Block b in Blocks)
        {
            b.UpdateBlockColliders();
        }
    }
}