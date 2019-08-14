using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Vector3Int MapGridLocation { get; private set; }

    void Start()
    {
        MapGridLocation = MapGrid.UpdateBlockLocation(this,Vector3Int.down,this.gameObject.transform.position);
        this.gameObject.transform.position = MapGridLocation * LevelManager.BlockSize;
        UpdateBlockColliders();
    }

    public void UpdateBlockColliders()
    {
        int facingCoordinate;
        bool [] sides;

        if (Mathf.RoundToInt(LevelManager.GameOrientation.transform.forward.x) == 1)
        {
            facingCoordinate = 0;
        } else if (Mathf.RoundToInt(LevelManager.GameOrientation.transform.forward.y) == 1)
        {
            facingCoordinate = 1;
        }
        else /*(Mathf.RoundToInt(LevelManager.GameOrientation.transform.forward.z) == 1)*/
        {
            facingCoordinate = 2;
        }

        sides = MapGrid.BlockHasColumnNeighbors(MapGridLocation, facingCoordinate);

        for (int i = 0; i < 6; i++)
        {
            if(sides[i])
            {
                this.gameObject.transform.GetChild(0).GetChild(i).GetComponent<MeshCollider>().enabled = false;
            } else
            {
                this.gameObject.transform.GetChild(0).GetChild(i).GetComponent<MeshCollider>().enabled = true;
            }
        }
    }
}
