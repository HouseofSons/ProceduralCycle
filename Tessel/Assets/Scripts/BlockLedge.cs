using UnityEngine;
using System.Collections;

public class BlockLedge : Block {

	public BlockLedge() : base()
	{
	}

	public override bool IsSolid(Block.Direction direction)
	{
		return false;
	}

	public override Tile TexturePosition(Direction direction)
	{
		Tile tile = new Tile();
		tile.x = 1;
		tile.y = 1;
		return tile;
	}

	public override MeshData Blockdata (Wall l,int x, int y, int z, MeshData meshData)
	{
		meshData = FaceDataUp (l, x, y, z, meshData);
		meshData = FaceDataSouth (l, x, y, z, meshData);
		return meshData;
	}
}
