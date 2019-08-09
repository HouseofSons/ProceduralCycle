using UnityEngine;
using System.Collections;

public class BlockBackground : Block {

	public BlockBackground() : base()
	{
	}
	
	public override bool IsSolid(Block.Direction direction)
	{
		return false;
	}

	public override Tile TexturePosition(Direction direction)
	{
		Tile tile = new Tile();
		tile.x = 2;
		tile.y = 0;
		return tile;
	}

	public override MeshData Blockdata (Wall l,int x, int y, int z, MeshData meshData)
	{
		return FaceDataNorth (l, x, y, z, meshData);
	}

	protected override MeshData FaceDataNorth (Wall l, int x, int y, int z, MeshData meshData)
	{
		
		//meshData.useRenderDataForCol = true;
		meshData.useForBackgroundTile = true;
		meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
		meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
		meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
		meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));

		meshData.AddQuadTriangles();
		meshData.uv.AddRange(FaceUVs(Direction.north));
		meshData.useForBackgroundTile = false;
		//meshData.useRenderDataForCol = false;
		return meshData;
	}
}
