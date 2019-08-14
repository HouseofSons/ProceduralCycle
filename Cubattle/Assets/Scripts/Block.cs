using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public static List<Block> Blocks = new List<Block>();

    public Vector3Int MapGridLocation { get; set; }
    public Vector3Int PrevMapGridLocation { get; set; }

    public bool Moving { get; set; }

    void Start()
    {
        Blocks.Add(this);
        MapGrid.UpdateBlockLocation(this,this.gameObject.transform.position);
    }

    void Update()
    {
        if (Moving)
        {
            MapGrid.UpdateBlockLocation(this, this.gameObject.transform.position);
        }
    }

    public void UpdateBlockColliders()
    {
        int facingCoordinate;
        bool [] sidesEnabled;

        if (Mathf.RoundToInt(LevelManager.GameOrientation.transform.forward.x) == 1)
        {
            facingCoordinate = 0;
        } else if (Mathf.RoundToInt(LevelManager.GameOrientation.transform.forward.y) == 1)
        {
            facingCoordinate = 1;
        } else /*(Mathf.RoundToInt(LevelManager.GameOrientation.transform.forward.z) == 1)*/
        {
            facingCoordinate = 2;
        }

        sidesEnabled = MapGrid.BlockHasColumnNeighbors(MapGridLocation, facingCoordinate);

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
