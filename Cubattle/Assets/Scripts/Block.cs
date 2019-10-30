using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
	public static List<Block> Blocks = new List<Block>();
	public Vector3Int CurrentMapGridLocation { get; set; }

	public Block[] FaceBlocks = new Block[4];
	public bool Cloned { get; set; }
    public bool Moving { get; set; }

    void Start()
	{
		Blocks.Add(this);
		MapGrid.InitializeGridLocation(this);
	}
}