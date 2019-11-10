using UnityEngine;
using System.Collections;

public class DensityGenerator : MonoBehaviour {

	public float DensityThreshold;

	private static float hexDensityThreshold;

	public int perlinStartWidth;
	public int perlinStartLength;
	public int perlinStartHeight;
	
	private static int PStartWidth;
	private static int PStartLength;
	private static int PStartHeight;
	
	public int mapScaleXY;
	public int mapScaleXZ;
	public int mapScaleYZ;
	
	private static int PMapScaleXY;
	private static int PMapScaleXZ;
	private static int PMapScaleYZ;

	void Awake() {

		hexDensityThreshold = DensityThreshold;
		
		PStartWidth = perlinStartWidth;
		PStartLength = perlinStartLength;
		PStartHeight = perlinStartHeight;

		PMapScaleXY = mapScaleXY;
		PMapScaleXZ = mapScaleXZ;
		PMapScaleYZ = mapScaleYZ;
	}

	public static void GenerateDensityMap() {
		for (int x = 0; x < HexWorldTerrain.GetWidth(); x++) {
			for (int y = 0; y < HexWorldTerrain.GetHeight(); y++) {
				for (int z = 0; z < HexWorldTerrain.GetLength(); z++) {
					if (HexWorldTerrain.GetChunk (x, y, z) != null) {
						if (HexWorldTerrain.InHexagonShapedMap (x, z)) {
							if (HexDenseEnough (x, y, z)) {
								HexWorldTerrain.SetHex (x, y, z, new HexPrismStone ());
							} else {
								HexWorldTerrain.SetHex (x, y, z, new HexPrismAir ());	
							}
						} else {
							HexWorldTerrain.SetHex (x, y, z, new HexPrismAir ());
						}
					} else {
						HexWorldTerrain.CreateChunk (x, y, z);
						if (HexWorldTerrain.InHexagonShapedMap (x, z)) {
							if (HexDenseEnough (x, y, z)) {
								HexWorldTerrain.SetHex (x, y, z, new HexPrismStone ());
							} else {
								HexWorldTerrain.SetHex (x, y, z, new HexPrismAir ());
							}
						} else {
							HexWorldTerrain.SetHex (x, y, z, new HexPrismAir ());
						}
					}
				}
			}
		}
	}

	private static float GetHexDensity(int x, int y, int z) {
		
		float[] perlinNoiseValues = new float[3];
		
		perlinNoiseValues [0] = Mathf.PerlinNoise((float)(x+PStartWidth)/PMapScaleXY,(float)(y+PStartHeight)/PMapScaleXY);
		perlinNoiseValues [1] = Mathf.PerlinNoise((float)(x+PStartWidth)/PMapScaleXZ,(float)(z+PStartLength)/PMapScaleXZ);
		perlinNoiseValues [2] = Mathf.PerlinNoise((float)(y+PStartHeight)/PMapScaleYZ,(float)(z+PStartLength)/PMapScaleYZ);
		
		return (((perlinNoiseValues [0] + perlinNoiseValues [1] + perlinNoiseValues [2]) / 3.0f) - 0.5f)
			+ (0.5f - ((float)y / HexWorldTerrain.GetHeight()));
	}
	
	private static bool HexDenseEnough(int x, int y, int z) {
		if (GetHexDensity(x,y,z) > hexDensityThreshold) {
			return true;
		} else {
			return false;
		}
	}
}
