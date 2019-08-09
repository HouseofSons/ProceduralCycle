﻿using UnityEngine;
using System.Collections;

public class BlockAir : Block {

	public BlockAir() : base()
	{
	}

	public override MeshData Blockdata (Wall l, int x, int y, int z, MeshData meshData)
	{
		return meshData;
	}

	public override bool IsSolid(Block.Direction direction)
	{
		return false;
	}
}