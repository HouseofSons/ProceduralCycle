using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

	[Range(1,100)]
	public int mapWidth;
	[Range(1,100)]
	public int mapLength;
	[Range(1,100)]
	public int mapHeight;

	[Range(1,8)]
	public int environmentCubeHeight;

	[Range(1,100)]
	public int pStartWidth;
	[Range(1,100)]
	public int pStartLength;
	[Range(1,100)]
	public int pStartHeight;

	[Range(1.0f,10.0f)]
	public float pMapScaleXY;
	[Range(1.0f,10.0f)]
	public float pMapScaleXZ;
	[Range(1.0f,10.0f)]
	public float pMapScaleYZ;

	void Start() {
		for (int x=0; x < mapWidth; x++) {
			for (int y=0; y < mapHeight; y++) {
				for (int z=0; z < mapLength; z++) {
					EnvironmentCube cube = new EnvironmentCube(x,y,z,environmentCubeHeight);
				}
			}
		}
		EnvironmentCube.ActivateCubes ();
		EnvironmentCube.CullDenseCubes ();
		EnvironmentCube.CullOccludedCubes ();
	}

	public int MapWidth {
		get {return mapWidth;}
	}

	public int MapLength {
		get {return mapLength;}
	}

	public int MapHeight {
		get {return mapHeight;}
	}

	public int PStartWidth {
		get{return pStartWidth;}
	}
	
	public int PStartLength {
		get{return pStartLength;}
	}
	
	public int PStartHeight {
		get{return pStartHeight;}
	}

	public float PMapScaleXY {
		get{return pMapScaleXY;}
	}
	
	public float PMapScaleXZ {
		get{return pMapScaleXZ;}
	}
	
	public float PMapScaleYZ {
		get{return pMapScaleYZ;}
	}
}

















