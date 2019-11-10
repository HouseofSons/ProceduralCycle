using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnvironmentCube {

	private static MapGenerator map;

	private static EnvironmentCube[,,] environmentCubes;
	private static int cubeCount = 0;
	private static GameObject cubes;
	private int[] coordinates;
	private float[] perlinNoiseValues;
	private GameObject cubePrimitive;
	private float cubeDensity;
	public int neighborCount;
	private EnvironmentCube[] neighbors;

	public EnvironmentCube(int xCoord, int yCoord, int zCoord, int cubeHeight) {
		if (cubeCount == 0) {
			map = GameObject.Find ("MapGenerator").GetComponent<MapGenerator>();
			cubes = GameObject.Find ("Cubes");
			environmentCubes = new EnvironmentCube[map.MapWidth,map.MapHeight,map.MapLength];
		}
		cubePrimitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
		coordinates = new int[3];
		coordinates[0] = xCoord;
		coordinates[1] = yCoord;
		coordinates[2] = zCoord;
		CubePrimitive.transform.localScale = new Vector3 (1, 1.0f/4.0f * cubeHeight, 1);
		SetPerlinNoise (xCoord,yCoord,zCoord);
		environmentCubes[xCoord,yCoord,zCoord] = this;
		cubeCount++;
		cubePrimitive.transform.position = new Vector3(xCoord,yCoord * (1.0f/4.0f * cubeHeight),zCoord);
		cubePrimitive.transform.rotation = Quaternion.identity;
		cubePrimitive.name = "Environment_Cube_" + xCoord + "," + yCoord + "," + zCoord;
		cubePrimitive.transform.parent = cubes.transform;
		neighborCount = 0;
		neighbors = new EnvironmentCube[6];
	}

	public static EnvironmentCube[,,] Cubes {

		get{ return environmentCubes;}
	}

	public GameObject CubePrimitive {

		get{ return cubePrimitive;}
	}

	public static int CubeCount() {

		return cubeCount;
	}

	public float[] PerlinNoiseValues {

		get{ return perlinNoiseValues;}

		set{ perlinNoiseValues = value;}
	}

	public float CubeDensity {
		
		get{ return cubeDensity;}
	}

	public int NeighborCount {
		get{ return neighborCount;}
	}

	private void SetPerlinNoise(int x, int y, int z) {

		float[] noises = new float[3];
		
		noises [0] = Mathf.PerlinNoise((x+map.PStartWidth)/map.PMapScaleXY,(y+map.PStartHeight)/map.PMapScaleXY);
		noises [1] = Mathf.PerlinNoise((x+map.PStartWidth)/map.PMapScaleXZ,(z+map.PStartLength)/map.PMapScaleXZ);
		noises [2] = Mathf.PerlinNoise((y+map.PStartHeight)/map.PMapScaleYZ,(z+map.PStartLength)/map.PMapScaleYZ);

		perlinNoiseValues = noises;
		SetCubeDensityAndColor (y);
	}

	private void SetCubeDensityAndColor(int yCoord) {

		cubeDensity =  (((perlinNoiseValues[0] + perlinNoiseValues[1] + perlinNoiseValues[2])/3.0f) - 0.5f)
			+ (0.5f-((float)yCoord/map.MapHeight));

		float color = (((perlinNoiseValues[0] + perlinNoiseValues[1] + perlinNoiseValues[2])/3.0f) - 0.5f)
			+ ((float)yCoord/map.MapHeight);

		cubePrimitive.gameObject.GetComponent<Renderer> ().material.color = new Color (color, color, color, color);
	}
	
	public static void ActivateCubes() {
		for (int x=0; x < environmentCubes.GetLength(0); x++) {
			for (int y=0; y < environmentCubes.GetLength(1); y++) {
				for (int z=0; z < environmentCubes.GetLength(2); z++) {
					environmentCubes[x,y,z].cubePrimitive.SetActive(true);
				}
			}
		}

		for (int x=0; x < environmentCubes.GetLength(0); x++) {
			for (int y=0; y < environmentCubes.GetLength(1); y++) {
				for (int z=0; z < environmentCubes.GetLength(2); z++) {
					environmentCubes[x,y,z].MeetActiveCubeNeighbors();
				}
			}
		}
	}

	public static void CullDenseCubes() {
		for (int x=0; x < environmentCubes.GetLength(0); x++) {
			for (int y=0; y < environmentCubes.GetLength(1); y++) {
				for (int z=0; z < environmentCubes.GetLength(2); z++) {
					if (environmentCubes[x,y,z].cubeDensity < 0) {
						environmentCubes[x,y,z].cubePrimitive.SetActive(false);
					}
				}
			}
		}
		
		for (int x=0; x < environmentCubes.GetLength(0); x++) {
			for (int y=0; y < environmentCubes.GetLength(1); y++) {
				for (int z=0; z < environmentCubes.GetLength(2); z++) {
					environmentCubes[x,y,z].MeetActiveCubeNeighbors();
				}
			}
		}
	}

	public static void CullOccludedCubes() {
		for (int x=0; x < environmentCubes.GetLength(0); x++) {
			for (int y=0; y < environmentCubes.GetLength(1); y++) {
				for (int z=0; z < environmentCubes.GetLength(2); z++) {
					if (environmentCubes[x,y,z].neighborCount == 6) {
						environmentCubes[x,y,z].cubePrimitive.SetActive(false);
					}
				}
			}
		}
		
		for (int x=0; x < environmentCubes.GetLength(0); x++) {
			for (int y=0; y < environmentCubes.GetLength(1); y++) {
				for (int z=0; z < environmentCubes.GetLength(2); z++) {
					environmentCubes[x,y,z].MeetActiveCubeNeighbors();
				}
			}
		}
	}

	private void MeetActiveCubeNeighbors() {
		
		neighborCount = 0;

		neighbors [0] = null;
		neighbors [1] = null;
		neighbors [2] = null;
		neighbors [3] = null;
		neighbors [4] = null;
		neighbors [5] = null;

		for (int x=coordinates[0]-1; x <= coordinates[0]+1; x++) {
			for (int y=coordinates[1]-1; y <= coordinates[1]+1; y++) {
				for (int z=coordinates[2]-1; z <= coordinates[2]+1; z++) {
					if (x >= 0 && x < environmentCubes.GetLength (0) &&
						y >= 0 && y < environmentCubes.GetLength (1) &&
						z >= 0 && z < environmentCubes.GetLength (2)) {
						if(environmentCubes [x, y, z].cubePrimitive.activeSelf) {
							if (x != coordinates [0] || y != coordinates [1] || z != coordinates [2]) {
								if (coordinates [0] == x && coordinates [1] == y) {
									if (z == coordinates [2] + 1) {
										neighbors [4] = environmentCubes [x, y, z];
										neighborCount++;
									} else {
										neighbors [5] = environmentCubes [x, y, z];
										neighborCount++;
									}
								}
								if (coordinates [0] == x && coordinates [2] == z) {
									if (y == coordinates [1] + 1) {
										neighbors [2] = environmentCubes [x, y, z];
										neighborCount++;
									} else {
										neighbors [3] = environmentCubes [x, y, z];
										neighborCount++;
									}
								}
								if (coordinates [1] == y && coordinates [2] == z) {
									if (x == coordinates [0] + 1) {
										neighbors [0] = environmentCubes [x, y, z];
										neighborCount++;
									} else {
										neighbors [1] = environmentCubes [x, y, z];
										neighborCount++;
									}
								}
							}
						}
					}
				}
			}
		}
	}
}
























