using UnityEngine;
using System.Collections;

public class Block {

	public Block() {
	}

	public enum Direction { north, east, south, west, up, down };
	
	public virtual bool IsSolid(Direction direction)
	{
		switch(direction){
		case Direction.north:
			return true;
		case Direction.east:
			return true;
		case Direction.south:
			return true;
		case Direction.west:
			return true;
		case Direction.up:
			return true;
		case Direction.down:
			return true;
		}
		return false;
	}

	public struct Tile { public int x; public int y;}
	
	public virtual Tile TexturePosition(Direction direction)
	{
		Tile tile = new Tile();
		tile.x = 1;
		tile.y = 1;
		return tile;

		//Example for other block types
		//Tile tile = new Tile();
		//switch (direction)
		//{
		//case Direction.up:
		//	tile.x = 2;
		//	tile.y = 0;
		//	return tile;
		//case Direction.down:
		//	tile.x = 1;
		//	tile.y = 0;
		//	return tile;
		//}
		//tile.x = 3;
		//tile.y = 0;
		//return tile;

	}
	
	const float tileSize = 0.25f;
	const float padding = 0.01f;
	
	public virtual Vector2[] FaceUVs(Direction direction)
	{
		Vector2[] UVs = new Vector2[4];
		Tile tilePos = TexturePosition(direction);
		UVs[0] = new Vector2(tileSize * tilePos.x + tileSize - padding,
			tileSize * tilePos.y + padding);
		UVs[1] = new Vector2(tileSize * tilePos.x + tileSize - padding,
			tileSize * tilePos.y + tileSize - padding);
		UVs[2] = new Vector2(tileSize * tilePos.x + padding,
			tileSize * tilePos.y + tileSize - padding);
		UVs[3] = new Vector2(tileSize * tilePos.x + padding,
			tileSize * tilePos.y + padding);
		return UVs;
	}

	public virtual MeshData Blockdata (Wall l,int x, int y, int z, MeshData meshData)
	{
		if (l.BlockExistsAt (x, y + 1, z)) {
			if (!l.GetBlock (x, y + 1, z).IsSolid (Direction.down)) {
				meshData = FaceDataUp (l, x, y, z, meshData);
			}
		} else {
			meshData = FaceDataUp (l, x, y, z, meshData);
		}
		if (l.BlockExistsAt(x, y - 1, z)) {
			if (!l.GetBlock(x, y - 1, z).IsSolid (Direction.up)) {
				meshData = FaceDataDown(l, x, y, z, meshData);
			}
		} else {
			meshData = FaceDataDown (l, x, y, z, meshData);
		}
		if (l.BlockExistsAt(x, y ,z - 1)) {
			if (!l.GetBlock(x, y ,z - 1).IsSolid (Direction.north)) {
				meshData = FaceDataSouth(l, x, y, z, meshData);
			}
		} else {
			meshData = FaceDataSouth (l, x, y, z, meshData);
		}
//		if (l.BlockExistsAt(x, y, z + 1)) {
//			if (!l.GetBlock(x, y, z + 1).IsSolid (Direction.south)) {
//				meshData = FaceDataNorth(l, x, y, z, meshData);
//			}
//		} else {
//			meshData = FaceDataNorth (l, x, y, z, meshData);
//		}
		if (l.BlockExistsAt(x - 1, y, z)) {
			if (!l.GetBlock(x - 1, y, z).IsSolid (Direction.east)) {
				meshData = FaceDataWest(l, x, y, z, meshData);
			}
		} else {
			meshData = FaceDataWest (l, x, y, z, meshData);
		}
		if (l.BlockExistsAt(x + 1, y, z)) {
			if (!l.GetBlock(x + 1, y, z).IsSolid (Direction.west)) {
				meshData = FaceDataEast(l, x, y, z, meshData);
			}
		} else {
			meshData = FaceDataEast (l, x, y, z, meshData);
		}
		return meshData;
	}
	
	protected virtual MeshData FaceDataUp (Wall l, int x, int y, int z, MeshData meshData)
	{
		meshData.useRenderDataForCol = true;
		meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
		meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
		meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
		meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));

		meshData.AddQuadTriangles();
		meshData.uv.AddRange(FaceUVs(Direction.up));
		meshData.useRenderDataForCol = false;
		return meshData;
	}
	protected virtual MeshData FaceDataDown (Wall l, int x, int y, int z, MeshData meshData)
	{
		meshData.useRenderDataForCol = true;
		meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
		meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
		meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
		meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
		
		meshData.AddQuadTriangles();
		meshData.uv.AddRange(FaceUVs(Direction.down));
		meshData.useRenderDataForCol = false;
		return meshData;
	}
	protected virtual MeshData FaceDataNorth (Wall l, int x, int y, int z, MeshData meshData)
	{
		meshData.useRenderDataForCol = true;
		meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
		meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
		meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
		meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
		
		meshData.AddQuadTriangles();
		meshData.uv.AddRange(FaceUVs(Direction.north));
		meshData.useRenderDataForCol = false;
		return meshData;
	}
	protected virtual MeshData FaceDataSouth (Wall l, int x, int y, int z, MeshData meshData)
	{
		meshData.useRenderDataForCol = true;
		meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
		meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
		meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
		meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
		
		meshData.AddQuadTriangles();
		meshData.uv.AddRange(FaceUVs(Direction.south));
		meshData.useRenderDataForCol = false;
		return meshData;
	}
	protected virtual MeshData FaceDataEast (Wall l, int x, int y, int z, MeshData meshData)
	{
		meshData.useRenderDataForCol = true;
		meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
		meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
		meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
		meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
		
		meshData.AddQuadTriangles();
		meshData.uv.AddRange(FaceUVs(Direction.east));
		meshData.useRenderDataForCol = false;
		return meshData;
	}
	protected virtual MeshData FaceDataWest (Wall l, int x, int y, int z, MeshData meshData)
	{
		meshData.useRenderDataForCol = true;
		meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
		meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
		meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
		meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));

		meshData.AddQuadTriangles();
		meshData.uv.AddRange(FaceUVs(Direction.west));
		meshData.useRenderDataForCol = false;
		return meshData;
	}
}





















