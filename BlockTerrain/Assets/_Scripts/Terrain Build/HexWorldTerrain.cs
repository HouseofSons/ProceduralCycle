using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HexWorldTerrain : MonoBehaviour {

	public static Dictionary<WorldPos, HexPrismChunk> chunks;
	public GameObject chunkPrefab;

	private static GameObject chunkGameObject;

	public int mapWidth;
	public int mapHeight;

    private static int terrainWidth;
	private static int terrainHeight;
	private static int terrainLength;

    public int gameBoardWidth;

    private static int boardWidth;

    public float hexScalar;

    private static float hexHeightScalar;

    public enum terrainGenerator {DensityGenerator,BasicPerlinGenerator}

	public terrainGenerator generator;
	
	void Start() {

		terrainWidth = mapWidth;
		terrainHeight = mapHeight;
		terrainLength = mapWidth;

        boardWidth = gameBoardWidth;

        hexHeightScalar = hexScalar;

        chunkGameObject = chunkPrefab;

		chunks = new Dictionary<WorldPos, HexPrismChunk>();

		if (generator == terrainGenerator.DensityGenerator) {
			DensityGenerator.GenerateDensityMap ();
		}
		
		if (generator == terrainGenerator.BasicPerlinGenerator) {
			BasicPerlinGenerator.GeneratePerlinMap ();
		}

	}
	
	public static float xHexToWorldCoordinate(int x,int z) {
		return Mathf.Sqrt (3.00f) * x - (Mathf.Sqrt (3.00f)/2.0f) * z;
	}
	
	public static float zHexToWorldCoordinate(int z) {
		return (3.0f/2.0f) * z;
	}

	public static Vector3 WorldToHexCoordinates(Vector3 worldCoordinates) {
		float z = worldCoordinates.z/(3.0f/2.0f);
		float x = (worldCoordinates.x + ((Mathf.Sqrt (3.00f) / 2.0f) * z)) / Mathf.Sqrt (3.00f);
		return new Vector3 (x,worldCoordinates.y*(1/HexWorldTerrain.GetHexScalar()),z);
    }

    public static int GetWidth()
    {
        return terrainWidth;
    }

    public static int GetHeight()
    {
        return terrainHeight;
    }

    public static int GetLength()
    {
        return terrainLength;
    }

    public static int GetBoardWidth()
    {
        return boardWidth;
    }

    public static float GetHexScalar()
    {
        return hexHeightScalar;
    }

    public static bool InWorldRange(int x, int y, int z)
	{
		if (x < 0 || y < 0 || z < 0 || x >= terrainWidth || y >= terrainHeight || z >= terrainLength) {
			return false;
		}
		return true;
	}
	
	public static void CreateChunk(int x, int y, int z)
	{
		float multiple = HexPrismChunk.chunkSize;
		int chunkx = Mathf.FloorToInt(x / multiple) * HexPrismChunk.chunkSize;
		int chunky = Mathf.FloorToInt(y / multiple) * HexPrismChunk.chunkSize;
		int chunkz = Mathf.FloorToInt(z / multiple) * HexPrismChunk.chunkSize;
		
		WorldPos worldPos = new WorldPos(chunkx, chunky, chunkz);
		
		//Instantiate the chunk at the coordinates using the chunk prefab
		GameObject newChunkObject = Instantiate(
			chunkGameObject, new Vector3(xHexToWorldCoordinate(chunkx,chunkz),chunky*HexWorldTerrain.GetHexScalar(),zHexToWorldCoordinate(chunkz)),
			Quaternion.Euler(Vector3.zero)
			) as GameObject;
		
		HexPrismChunk newChunk = newChunkObject.GetComponent<HexPrismChunk>();
		
		newChunk.pos = worldPos;
		
		//Add it to the chunks dictionary with the position as the key
		chunks.Add(worldPos, newChunk);
		//Properly labels chunks and parents to proper GameObject
		newChunk.transform.parent = GameObject.Find("HexWorldTerrain").transform;
		newChunk.name = "X:" + chunkx + ",Y:" + chunky + ",Z:" + chunkz;
		newChunk.Increment ();

		//Initializes Chunk Hexes as empty
		for (int xi = 0; xi < HexPrismChunk.chunkSize; xi++)
		{
			for (int yi = 0; yi < HexPrismChunk.chunkSize; yi++)
			{
				for (int zi = 0; zi < HexPrismChunk.chunkSize; zi++)
				{
					SetHex(x + xi, y + yi, z + zi, new HexPrismAir());
				}
			}
		}
	}
	
	public static HexPrismChunk GetChunk(int x, int y, int z)
	{
		WorldPos pos = new WorldPos();
		float multiple = HexPrismChunk.chunkSize;
		pos.x = Mathf.FloorToInt(x / multiple) * HexPrismChunk.chunkSize;
		pos.y = Mathf.FloorToInt(y / multiple) * HexPrismChunk.chunkSize;
		pos.z = Mathf.FloorToInt(z / multiple) * HexPrismChunk.chunkSize;

		HexPrismChunk containerChunk = null;
		chunks.TryGetValue(pos, out containerChunk);
		
		return containerChunk;
	}
	
	public static HexPrism GetHex(int x, int y, int z)
	{
		HexPrismChunk containerChunk = GetChunk(x, y, z);
		
		if (containerChunk != null) {
			HexPrism hex = containerChunk.GetHexPrism (
				x - containerChunk.pos.x,
				y - containerChunk.pos.y,
				z - containerChunk.pos.z);
			
			return hex;
		} else {
			return null;
		}
	}

	public static void UpdateIfEqual(int value1, int value2, WorldPos pos)
	{
		if (value1 == value2)
		{
			HexPrismChunk chunk = GetChunk(pos.x, pos.y, pos.z);
			if (chunk != null) {
				chunk.update = true;
			}
		}
	}
	
	public static void SetHex(int x, int y, int z, HexPrism hex)
	{
		HexPrismChunk chunk = GetChunk(x, y, z);

		if (chunk != null)
		{
			chunk.SetHexPrism(x - chunk.pos.x, y - chunk.pos.y, z - chunk.pos.z, hex);
			chunk.update = true;
			UpdateIfEqual(x - chunk.pos.x, 0, new WorldPos(x - 1, y, z));
			UpdateIfEqual(x - chunk.pos.x, HexPrismChunk.chunkSize - 1, new WorldPos(x + 1, y, z));
			UpdateIfEqual(y - chunk.pos.y, 0, new WorldPos(x, y - 1, z));
			UpdateIfEqual(y - chunk.pos.y, HexPrismChunk.chunkSize - 1, new WorldPos(x, y + 1, z));
			UpdateIfEqual(z - chunk.pos.z, 0, new WorldPos(x, y, z - 1));
			UpdateIfEqual(z - chunk.pos.z, HexPrismChunk.chunkSize - 1, new WorldPos(x, y, z + 1));
		}
	}

	public static bool InHexagonShapedMap(int x, int z) {

		if (x >= z - Mathf.RoundToInt(terrainLength/2.0f) && x <= z + Mathf.RoundToInt(terrainLength/2.0f)) {
			return true;
		} else {
			return false;
		}
    }
}
















