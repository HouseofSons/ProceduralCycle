using UnityEngine;
using System.Collections;

public class HexPrismAir : HexPrism {

	public HexPrismAir() : base() {
	}

	public override MeshData HexPrismData (HexPrismChunk chunk, int x, int y, int z, MeshData meshData)
	{
		return meshData;
	}

	public override bool IsSolid(HexPrism.Direction direction)
	{
		return false;
	}
}
