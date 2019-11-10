using UnityEngine;
using System.Collections;

public class HexPrism {

	private Vector3 location;
    private bool isTop;

	public HexPrism(){
	}

	public Vector3 Location {
		get { return location ;}
		set { location = value ;}
	}

	public enum Direction { east, northeast, southeast, west, northwest, southwest, up, down };

	public virtual bool IsSolid(Direction direction)
	{
		switch(direction){
		case Direction.northeast:
			return true;
		case Direction.east:
			return true;
		case Direction.southeast:
			return true;
		case Direction.southwest:
			return true;
		case Direction.west:
			return true;
		case Direction.northwest:
			return true;
		case Direction.up:
			return true;
		case Direction.down:
			return true;
		}
		return false;
	}

	public virtual MeshData HexPrismData (HexPrismChunk chunk, int x, int y, int z, MeshData meshData)
	{
		meshData.useRenderDataForHex = true;

        int centerMapX = Mathf.FloorToInt(HexWorldTerrain.GetWidth() / 2.0f);
        int centerMapZ = Mathf.FloorToInt(HexWorldTerrain.GetWidth() / 2.0f);

        if (HexWorldTerrain.InWorldRange (chunk.pos.x + x, chunk.pos.y + y + 1, chunk.pos.z + z)) {
			if (!chunk.GetHexPrism (x, y + 1, z).IsSolid (Direction.down)) {
				meshData = FaceDataUp (chunk, x, y, z, meshData);
				if (ProcessLevel.GetStageAt () == 1/*Terrain Building Phase*/) {
					if (!chunk.GetHexPrism (x, y + 2, z).IsSolid (Direction.down)) {
                        chunk.GetHexPrism(x, y + 2, z).IsTop = true;
                        if (Pathing.AxialHexDistance(centerMapX,centerMapZ, chunk.pos.x + x, chunk.pos.z + z) <= Mathf.RoundToInt(HexWorldTerrain.GetBoardWidth()/2.0f))
                        {
                            GameBoardHexGrid.CreateGameBoardHex(chunk.pos.x + x, chunk.pos.y + y, chunk.pos.z + z);
                        }
					}
				}
			} else {
				if (chunk != HexWorldTerrain.GetChunk(chunk.pos.x + x, chunk.pos.y + y + 1, chunk.pos.z + z)) {
					meshData.useRenderDataForHex = false;
					meshData = FaceDataUp (chunk, x, y, z, meshData);
					meshData.useRenderDataForHex = true;
				}
			}
		} else {
			meshData = FaceDataUp (chunk, x, y, z, meshData);
		}

		if (HexWorldTerrain.InWorldRange (chunk.pos.x + x, chunk.pos.y + y - 1, chunk.pos.z + z)) {		
			if (!chunk.GetHexPrism(x, y - 1, z).IsSolid(Direction.up))
			{
				meshData = FaceDataDown(chunk, x, y, z, meshData);
			} else {
				if (chunk != HexWorldTerrain.GetChunk(chunk.pos.x + x, chunk.pos.y + y - 1, chunk.pos.z + z)) {
					meshData.useRenderDataForHex = false;
					meshData = FaceDataDown (chunk, x, y, z, meshData);
					meshData.useRenderDataForHex = true;
				}
			}
		} else {
			meshData = FaceDataDown (chunk, x, y, z, meshData);
		}
		
		if (HexWorldTerrain.InWorldRange (chunk.pos.x + x - 1, chunk.pos.y + y, chunk.pos.z + z)) {
			if (!chunk.GetHexPrism(x - 1, y, z).IsSolid(Direction.east))
			{
				meshData = FaceDataWest(chunk, x, y, z, meshData);
			} else {
				if (chunk != HexWorldTerrain.GetChunk(chunk.pos.x + x - 1, chunk.pos.y + y, chunk.pos.z + z)) {
					meshData.useRenderDataForHex = false;
					meshData = FaceDataWest (chunk, x, y, z, meshData);
					meshData.useRenderDataForHex = true;
				}
			}
		} else {
			meshData = FaceDataWest (chunk, x, y, z, meshData);
		}
		
		if (HexWorldTerrain.InWorldRange (chunk.pos.x + x + 1, chunk.pos.y + y, chunk.pos.z + z)) {
			if (!chunk.GetHexPrism(x + 1, y, z).IsSolid(Direction.west))
			{
				meshData = FaceDataEast(chunk, x, y, z, meshData);
			} else {
				if (chunk != HexWorldTerrain.GetChunk(chunk.pos.x + x + 1, chunk.pos.y + y, chunk.pos.z + z)) {
					meshData.useRenderDataForHex = false;
					meshData = FaceDataEast (chunk, x, y, z, meshData);
					meshData.useRenderDataForHex = true;
				}
			}
		} else {
			meshData = FaceDataEast (chunk, x, y, z, meshData);
		}
		
		if (HexWorldTerrain.InWorldRange (chunk.pos.x + x, chunk.pos.y + y, chunk.pos.z + z - 1)) {
			if (!chunk.GetHexPrism(x, y, z - 1).IsSolid(Direction.northwest))
			{
				meshData = FaceDataSouthEast(chunk, x, y, z, meshData);
			} else {
				if (chunk != HexWorldTerrain.GetChunk(chunk.pos.x + x, chunk.pos.y + y, chunk.pos.z + z - 1)) {
					meshData.useRenderDataForHex = false;
					meshData = FaceDataSouthEast (chunk, x, y, z, meshData);
					meshData.useRenderDataForHex = true;
				}
			}
		} else {
			meshData = FaceDataSouthEast (chunk, x, y, z, meshData);
		}
		
		if (HexWorldTerrain.InWorldRange (chunk.pos.x + x - 1, chunk.pos.y + y, chunk.pos.z + z - 1)) {
			if (!chunk.GetHexPrism(x - 1, y, z - 1).IsSolid(Direction.northeast))
			{
				meshData = FaceDataSouthWest(chunk, x, y, z, meshData);
			} else {//Added
				if (chunk != HexWorldTerrain.GetChunk(chunk.pos.x + x - 1, chunk.pos.y + y, chunk.pos.z + z - 1)) {
					meshData.useRenderDataForHex = false;
					meshData = FaceDataSouthWest (chunk, x, y, z, meshData);
					meshData.useRenderDataForHex = true;
				}
			}
		} else {
			meshData = FaceDataSouthWest (chunk, x, y, z, meshData);
		}

		if (HexWorldTerrain.InWorldRange (chunk.pos.x + x + 1, chunk.pos.y + y, chunk.pos.z + z + 1)) {
			if (!chunk.GetHexPrism(x + 1, y, z + 1).IsSolid(Direction.southwest))
			{
				meshData = FaceDataNorthEast(chunk, x, y, z, meshData);
			} else {
				if (chunk != HexWorldTerrain.GetChunk(chunk.pos.x + x + 1, chunk.pos.y + y, chunk.pos.z + z + 1)) {
					meshData.useRenderDataForHex = false;
					meshData = FaceDataNorthEast (chunk, x, y, z, meshData);
					meshData.useRenderDataForHex = true;
				}
			}
		} else {
			meshData = FaceDataNorthEast (chunk, x, y, z, meshData);
		}
		
		if (HexWorldTerrain.InWorldRange (chunk.pos.x + x, chunk.pos.y + y, chunk.pos.z + z + 1)) {
			if (!chunk.GetHexPrism(x, y, z + 1).IsSolid(Direction.southeast))
			{
				meshData = FaceDataNorthWest(chunk, x, y, z, meshData);
			} else {
				if (chunk != HexWorldTerrain.GetChunk(chunk.pos.x + x, chunk.pos.y + y, chunk.pos.z + z + 1)) {
					meshData.useRenderDataForHex = false;
					meshData = FaceDataNorthWest (chunk, x, y, z, meshData);
					meshData.useRenderDataForHex = true;
				}
			}
		} else {
			meshData = FaceDataNorthWest (chunk, x, y, z, meshData);
		}

		return meshData;
	}

	protected virtual MeshData FaceDataUp (HexPrismChunk chunk, int x, int y, int z, MeshData meshData)
	{
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) + 0.00f				   ,y * HexWorldTerrain.GetHexScalar() + HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) + 1.00f));
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) + Mathf.Sqrt(3.00f)/2.00f,y * HexWorldTerrain.GetHexScalar() + HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) + (1.00f/2.00f)));
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) + Mathf.Sqrt(3.00f)/2.00f,y * HexWorldTerrain.GetHexScalar() + HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) - (1.00f/2.00f)));
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) + 0.00f				   ,y * HexWorldTerrain.GetHexScalar() + HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) - 1.00f));
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) - Mathf.Sqrt(3.00f)/2.00f,y * HexWorldTerrain.GetHexScalar() + HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) - (1.00f/2.00f)));
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) - Mathf.Sqrt(3.00f)/2.00f,y * HexWorldTerrain.GetHexScalar() + HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) + (1.00f/2.00f)));

		meshData.AddHexTriangles();
		if (meshData.useRenderDataForHex) {
			meshData.uv.AddRange(FaceUVs(Direction.up));
		}
		return meshData;
	}
	
	protected virtual MeshData FaceDataDown (HexPrismChunk chunk, int x, int y, int z, MeshData meshData)
	{
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) + 0.00f				   ,y * HexWorldTerrain.GetHexScalar() - HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) + 1.00f));
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) - Mathf.Sqrt(3.00f)/2.00f,y * HexWorldTerrain.GetHexScalar() - HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) + (1.00f/2.00f)));
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) - Mathf.Sqrt(3.00f)/2.00f,y * HexWorldTerrain.GetHexScalar() - HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) - (1.00f/2.00f)));
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) + 0.00f				   ,y * HexWorldTerrain.GetHexScalar() - HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) - 1.00f));
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) + Mathf.Sqrt(3.00f)/2.00f,y * HexWorldTerrain.GetHexScalar() - HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) - (1.00f/2.00f)));
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) + Mathf.Sqrt(3.00f)/2.00f,y * HexWorldTerrain.GetHexScalar() - HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) + (1.00f/2.00f)));
		                                 
		meshData.AddHexTriangles();
		if (meshData.useRenderDataForHex) {
			meshData.uv.AddRange (FaceUVs (Direction.down));
		}
		return meshData;
	}
	
	protected virtual MeshData FaceDataWest (HexPrismChunk chunk, int x, int y, int z, MeshData meshData)
	{
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) - Mathf.Sqrt(3.00f)/2.00f,y * HexWorldTerrain.GetHexScalar() + HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) - (1.00f/2.00f)));
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) - Mathf.Sqrt(3.00f)/2.00f,y * HexWorldTerrain.GetHexScalar() + HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) + (1.00f/2.00f)));
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) - Mathf.Sqrt(3.00f)/2.00f,y * HexWorldTerrain.GetHexScalar() - HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) - (1.00f/2.00f)));
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) - Mathf.Sqrt(3.00f)/2.00f,y * HexWorldTerrain.GetHexScalar() - HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) + (1.00f/2.00f)));

		meshData.AddQuadTriangles();
		if (meshData.useRenderDataForHex) {
			meshData.uv.AddRange (FaceUVs (Direction.west));
		}
		return meshData;
	}
	
	protected virtual MeshData FaceDataNorthEast (HexPrismChunk chunk, int x, int y, int z, MeshData meshData)
	{
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) + 0.00f				   ,y * HexWorldTerrain.GetHexScalar() + HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) + 1.00f));
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) + Mathf.Sqrt(3.00f)/2.00f,y * HexWorldTerrain.GetHexScalar() + HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) + (1.00f/2.00f)));
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) + 0.00f				   ,y * HexWorldTerrain.GetHexScalar() - HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) + 1.00f));
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) + Mathf.Sqrt(3.00f)/2.00f,y * HexWorldTerrain.GetHexScalar() - HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) + (1.00f/2.00f)));

		meshData.AddQuadTriangles();
		if (meshData.useRenderDataForHex) {
			meshData.uv.AddRange (FaceUVs (Direction.northeast));
		}
		return meshData;
	}
	
	protected virtual MeshData FaceDataNorthWest (HexPrismChunk chunk, int x, int y, int z, MeshData meshData)
	{
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) - Mathf.Sqrt(3.00f)/2.00f,y * HexWorldTerrain.GetHexScalar() + HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) + (1.00f/2.00f)));
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) + 0.00f				   ,y * HexWorldTerrain.GetHexScalar() + HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) + 1.00f));
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) - Mathf.Sqrt(3.00f)/2.00f,y * HexWorldTerrain.GetHexScalar() - HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) + (1.00f/2.00f)));
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) + 0.00f				   ,y * HexWorldTerrain.GetHexScalar() - HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) + 1.00f));

		meshData.AddQuadTriangles();
		if (meshData.useRenderDataForHex) {
			meshData.uv.AddRange (FaceUVs (Direction.northwest));
		}
		return meshData;
	}
	
	protected virtual MeshData FaceDataEast (HexPrismChunk chunk, int x, int y, int z, MeshData meshData)
	{
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) + Mathf.Sqrt(3.00f)/2.00f,y * HexWorldTerrain.GetHexScalar() + HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) + (1.00f/2.00f)));
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) + Mathf.Sqrt(3.00f)/2.00f,y * HexWorldTerrain.GetHexScalar() + HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) - (1.00f/2.00f)));
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) + Mathf.Sqrt(3.00f)/2.00f,y * HexWorldTerrain.GetHexScalar() - HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) + (1.00f/2.00f)));
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) + Mathf.Sqrt(3.00f)/2.00f,y * HexWorldTerrain.GetHexScalar() - HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) - (1.00f/2.00f)));

		meshData.AddQuadTriangles();
		if (meshData.useRenderDataForHex) {
			meshData.uv.AddRange (FaceUVs (Direction.east));
		}
		return meshData;
	}
	
	protected virtual MeshData FaceDataSouthEast (HexPrismChunk chunk, int x, int y, int z, MeshData meshData)
	{
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) + Mathf.Sqrt(3.00f)/2.00f,y * HexWorldTerrain.GetHexScalar() + HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) - (1.00f/2.00f)));
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) + 0.00f				   ,y * HexWorldTerrain.GetHexScalar() + HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) - 1.00f));
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) + Mathf.Sqrt(3.00f)/2.00f,y * HexWorldTerrain.GetHexScalar() - HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) - (1.00f/2.00f)));
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) + 0.00f				   ,y * HexWorldTerrain.GetHexScalar() - HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) - 1.00f));

		meshData.AddQuadTriangles();
		if (meshData.useRenderDataForHex) {
			meshData.uv.AddRange (FaceUVs (Direction.southeast));
		}
		return meshData;
	}
	
	protected virtual MeshData FaceDataSouthWest (HexPrismChunk chunk, int x, int y, int z, MeshData meshData)
	{
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) + 0.00f				   ,y * HexWorldTerrain.GetHexScalar() + HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) - 1.00f));
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) - Mathf.Sqrt(3.00f)/2.00f,y * HexWorldTerrain.GetHexScalar() + HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) - (1.00f/2.00f)));
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) + 0.00f				   ,y * HexWorldTerrain.GetHexScalar() - HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) - 1.00f));
		meshData.AddVertex(new Vector3(HexWorldTerrain.xHexToWorldCoordinate(x,z) - Mathf.Sqrt(3.00f)/2.00f,y * HexWorldTerrain.GetHexScalar() - HexWorldTerrain.GetHexScalar()/2.0f,HexWorldTerrain.zHexToWorldCoordinate(z) - (1.00f/2.00f)));

		meshData.AddQuadTriangles();
		if (meshData.useRenderDataForHex) {
			meshData.uv.AddRange (FaceUVs (Direction.southwest));
		}
		return meshData;
	}

	public struct Tile { public int x; public int y;}

    public bool IsTop
    {
        get { return isTop; }
        set { isTop = value; }
    }

    public virtual Vector2 TexturePosition(Direction direction)
	{
        return new Vector2(1,0);
	}

	public virtual Vector2[] FaceUVs(Direction direction)
	{
		Vector2[] UVs;
		Vector2 tilePos;
		//Will need adjustment given different tile sheet
		float tileSize = 0.25f;

		if (direction == Direction.up || direction == Direction.down) {
			UVs = new Vector2[6];
			tilePos = TexturePosition (direction);

            UVs [0] = new Vector2(tileSize * tilePos.x + tileSize/2.0f									,tileSize * tilePos.y + tileSize								);
			UVs [1] = new Vector2(tileSize * tilePos.x + tileSize/2.0f + (Mathf.Sqrt(3.00f)/2.00f)/8.0f	,tileSize * tilePos.y + tileSize/2.0f + ((1.00f/2.00f))/8.0f	);
			UVs [2] = new Vector2(tileSize * tilePos.x + tileSize/2.0f + (Mathf.Sqrt(3.00f)/2.00f)/8.0f	,tileSize * tilePos.y + tileSize/2.0f - ((1.00f/2.00f))/8.0f	);
			UVs [3] = new Vector2(tileSize * tilePos.x + tileSize/2.0f									,tileSize * tilePos.y											);
			UVs [4] = new Vector2(tileSize * tilePos.x + tileSize/2.0f - (Mathf.Sqrt(3.00f)/2.00f)/8.0f	,tileSize * tilePos.y + tileSize/2.0f - ((1.00f/2.00f))/8.0f	);
			UVs [5] = new Vector2(tileSize * tilePos.x + tileSize/2.0f - (Mathf.Sqrt(3.00f)/2.00f)/8.0f	,tileSize * tilePos.y + tileSize/2.0f + ((1.00f/2.00f))/8.0f	);
		} else {
			UVs = new Vector2[4];
			tilePos = TexturePosition (direction);
			UVs [0] = new Vector2 (tileSize * tilePos.x + tileSize,
			                       tileSize * tilePos.y + tileSize);
			UVs [1] = new Vector2 (tileSize * tilePos.x,
			                       tileSize * tilePos.y + tileSize);
			UVs [2] = new Vector2 (tileSize * tilePos.x + tileSize,
			                       tileSize * tilePos.y);
			UVs [3] = new Vector2 (tileSize * tilePos.x,
			                       tileSize * tilePos.y);
		}
		return UVs;
	}
}












