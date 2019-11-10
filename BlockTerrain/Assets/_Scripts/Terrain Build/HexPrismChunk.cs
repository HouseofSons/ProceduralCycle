using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class HexPrismChunk : MonoBehaviour {

	private HexPrism[ , , ] hexPrisms = new HexPrism[chunkSize, chunkSize, chunkSize];

	private static List<HexPrismChunk> ListOfChunks = new List<HexPrismChunk>();
	private static int chunkCount = 0;

	public static int chunkSize = 8;
	public bool update = true;

	MeshFilter filter;
	//MeshCollider coll;
	
	public WorldPos pos;

	void Start()
	{
		filter = gameObject.GetComponent<MeshFilter>();
		//coll = gameObject.GetComponent<MeshCollider>();
		ListOfChunks.Add (this.GetComponent<HexPrismChunk>());
		this.gameObject.layer = LayerMask.NameToLayer("Chunk");
	}

	void Update()
	{
		if (update)
		{
			update = false;
			UpdateChunk();
		}
	}

	public void Increment() {
		chunkCount++;
	}

	public int ChunkCount() {
		return chunkCount;
	}

	public static List<HexPrismChunk> GetChunkList() {
		return ListOfChunks;
	}

    public HexPrism[,,] GetHexPrisms()
    {
        return hexPrisms;
    }

    public static bool AllChunksUpdated() {
		foreach (HexPrismChunk chunk in ListOfChunks) {
			if (chunk.update) {
				return false;
			}
		}
		return true;
	}

	public static void UpdateAllChunks() {
		foreach (HexPrismChunk chunk in ListOfChunks) {
			chunk.UpdateChunk();
			chunk.update = false;
		}
	}
	
	public HexPrism GetHexPrism(int x, int y, int z)
	{
		if (InRange (x) && InRange (y) && InRange (z)) {
			return hexPrisms [x, y, z];
		}
		return HexWorldTerrain.GetHex(pos.x + x, pos.y + y, pos.z + z);
	}

	public static bool InRange(int index)
	{
		if (index < 0 || index >= chunkSize)
			return false;
		
		return true;
	}
	
	public void SetHexPrism(int x, int y, int z, HexPrism hex)
	{
		if (InRange(x) && InRange(y) && InRange(z)) {
			hexPrisms[x, y, z] = hex;
			hex.Location = new Vector3(HexWorldTerrain.xHexToWorldCoordinate(pos.x + x,pos.z + z),(pos.y + y) * HexWorldTerrain.GetHexScalar(), HexWorldTerrain.zHexToWorldCoordinate(pos.z + z));
		}
		else {
			HexWorldTerrain.SetHex(pos.x + x, pos.y + y, pos.z + z, hex);
		}
	}
	
	public void UpdateChunk()
	{
		MeshData meshData = new MeshData();
		for (int x = 0; x < chunkSize; x++)
		{
			for (int y = 0; y < chunkSize; y++)
			{
				for (int z = 0; z < chunkSize; z++)
				{
					meshData = hexPrisms[x, y, z].HexPrismData(this, x, y, z, meshData);
				}
			}
		}
		RenderMesh(meshData);
	}

	void RenderMesh(MeshData meshData)
	{
		filter.mesh.Clear();
		filter.mesh.vertices = meshData.vertices.ToArray();
		filter.mesh.triangles = meshData.triangles.	ToArray();

		filter.mesh.uv = meshData.uv.ToArray();
		filter.mesh.RecalculateNormals();

		//coll.sharedMesh = null;
		Mesh mesh = new Mesh();
		mesh.vertices = meshData.colVertices.ToArray();
		mesh.triangles = meshData.colTriangles.ToArray();
		mesh.RecalculateNormals();

		//coll.sharedMesh = mesh;
	}
}