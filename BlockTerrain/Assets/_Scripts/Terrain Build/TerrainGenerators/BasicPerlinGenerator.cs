using UnityEngine;
using System.Collections;

public class BasicPerlinGenerator : MonoBehaviour {

	public int perlinStartWidth;
	public int perlinStartLength;
	
	private static int PStartWidth;
	private static int PStartLength;

	public int heightScale;

	private static int PHeightScale;
	
	public float scale;
	
	private static float PMapScaleXY;

	// Use this for initialization
	void Awake () {

		PStartWidth = perlinStartWidth;
		PStartLength = perlinStartLength;

		PHeightScale = heightScale;
		
		PMapScaleXY = scale;
	}

	public static void GeneratePerlinMap() {
        for (int x = 0; x < HexWorldTerrain.GetWidth(); x++) {
            for (int y = 0; y <HexWorldTerrain.GetHeight(); y++) {
                for (int z = 0; z < HexWorldTerrain.GetLength(); z++) {
					if (HexWorldTerrain.GetChunk (x, y, z) != null) {
						if (HexWorldTerrain.InHexagonShapedMap (x, z)) {
							if (TerrainCheck(x,y,z)) {
                                SetHexFlora(x, y, z);
							} else {
								HexWorldTerrain.SetHex (x, y, z, new HexPrismAir ());
							}
						} else {
							HexWorldTerrain.SetHex (x, y, z, new HexPrismAir ());
						}
					} else {
						HexWorldTerrain.CreateChunk (x, y, z);
						if (HexWorldTerrain.InHexagonShapedMap (x, z)) {
							if (TerrainCheck(x,y,z)) {
                                SetHexFlora(x, y, z);
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

    private static void SetHexFlora (int x, int y, int z)
    {
        if (Setting.GetFlora() == Setting.Flora.Beach)
        {
            HexWorldTerrain.SetHex(x, y, z, new HexPrismGrass());
        }
        if (Setting.GetFlora() == Setting.Flora.Desert)
        {
            HexWorldTerrain.SetHex(x, y, z, new HexPrismGrass());
        }
        if (Setting.GetFlora() == Setting.Flora.Forest)
        {
            HexWorldTerrain.SetHex(x, y, z, new HexPrismGrass());
        }
        if (Setting.GetFlora() == Setting.Flora.Meadow)
        {
            HexWorldTerrain.SetHex(x, y, z, new HexPrismGrass());
        }
        if (Setting.GetFlora() == Setting.Flora.Mountain)
        {
            HexWorldTerrain.SetHex(x, y, z, new HexPrismGrass());
        }
    }

    private static bool TerrainCheck(int x, int y, int z) {
		if (Mathf.PerlinNoise ((float)(x + PStartWidth) / PMapScaleXY, (float)(z + PStartLength) / PMapScaleXY) * PHeightScale >= y) {
			return true;
		}
		return false;
	}
}
