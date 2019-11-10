using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraOcclusionManager : MonoBehaviour {
	
	private static List<Vector3> occludingHexPrismsHexCoords;
	private static List<Vector3> occludingHexPrismsCoords;
	private static List<HexPrismChunk> occludingHexPrismChunks;
	private static GameBoardHex[,,] gameBoardHexs;
	private static List<GameBoardHex> engagedGameBoardHexs = new List<GameBoardHex> ();

	void Start () {
		occludingHexPrismsHexCoords = new List<Vector3> ();
		occludingHexPrismsCoords = new List<Vector3> ();
		occludingHexPrismChunks = new List<HexPrismChunk> ();
	}

	public static void AddEngagedHex(GameBoardHex engagedHex) {
		if (!engagedGameBoardHexs.Contains (engagedHex)) {
			engagedGameBoardHexs.Add (engagedHex);
		}
	}

	public static void RemoveEngagedHex(GameBoardHex engagedHex) {
		engagedGameBoardHexs.Remove (engagedHex);
	}

	public static void GameBoardHexVisualPriorityList() {

		//for each HexPrism in Previously Intersected List reset to former HexPrism type
		ReplaceOccludedHexPrisms ();
		//Update Affected Chunks to prep for next Occluding Phasea
		UpdateOccludingChunks ();
		//Recreate Previously Intersected List and re-initialize variables for spherecasting
		occludingHexPrismsHexCoords = new List<Vector3> ();
		occludingHexPrismsCoords = new List<Vector3> ();
		occludingHexPrismChunks = new List<HexPrismChunk> ();
		Vector3 cameraPosition = GameObject.Find ("Main Camera").transform.position;
		Vector3 AboveGameBoardPiece;
		RaycastHit hexStandInHit;
		Vector3 hexPrismCoord;
		GameObject hexStandIn = new GameObject();
		hexStandIn.transform.position = new Vector3(0,0,0);
		hexStandIn.AddComponent<BoxCollider>();
		hexStandIn.GetComponent<BoxCollider>().size = new Vector3(Mathf.Sqrt(3),2/*1*/,2);
		hexStandIn.layer = LayerMask.NameToLayer("Terrain");

		//for each engaged hex create raycast from orthographic camera to hex
		foreach (GameBoardHex boardHex in GameBoardHexGrid.CharacterVisibleGameBoardPieces(GameBoardHex.GetCTCHex().GetOccupant())) {
			//SphereCast origin placed above Hex Board Piece
			AboveGameBoardPiece = new Vector3(boardHex.gameObject.transform.position.x,boardHex.gameObject.transform.position.y+1,boardHex.gameObject.transform.position.z);
			//Loop through all Chunks hit by SphereCast
			foreach(RaycastHit hitChunk in Physics.SphereCastAll(cameraPosition,0.25f,AboveGameBoardPiece - cameraPosition,Vector3.Distance(boardHex.gameObject.transform.position,cameraPosition)-1,1<<LayerMask.NameToLayer("Chunk"))) {
				//for each HexPrismChunk intersecting SphereCast create bounding box around each child HexPrism and check if hit by SphereCast
				foreach(Vector3 hexLocation in hitChunk.collider.gameObject.GetComponent<HexPrismChunk>().GetHexPrismsLocations()) {
					//Use Standin Hex to determine interesecting HexPrism
					hexStandIn.transform.position = hexLocation;
					//If Standin Hex intersects add HexPrism and HexChunk to Occluding List
					if (Physics.SphereCast(cameraPosition,0.25f,AboveGameBoardPiece - cameraPosition,out hexStandInHit,Vector3.Distance(boardHex.gameObject.transform.position,cameraPosition)-1,1<<LayerMask.NameToLayer("Terrain"))) {
						AddOccludingHexCoords(hexLocation);
						AddOccludingChunk(hitChunk.collider.gameObject.GetComponent<HexPrismChunk>());
					}
				}
			}
		}
		GameObject.Destroy(hexStandIn);

		foreach(Vector3 hexLocation in occludingHexPrismsHexCoords) {
			hexPrismCoord = HexWorldTerrain.WorldToHexCoordinates(hexLocation);
			HexWorldTerrain.SetHex(Mathf.RoundToInt(hexPrismCoord.x),Mathf.RoundToInt(hexPrismCoord.y),Mathf.RoundToInt(hexPrismCoord.z),new HexPrismTransparent());
			occludingHexPrismsCoords.Add(new Vector3(Mathf.RoundToInt(hexPrismCoord.x),Mathf.RoundToInt(hexPrismCoord.y),Mathf.RoundToInt(hexPrismCoord.z)));
		}
		UpdateOccludingChunks ();
	}

	public static void ReplaceOccludedHexPrisms() {
		if (occludingHexPrismsCoords != null) {
			foreach (Vector3 hexCoords in occludingHexPrismsCoords) {
				switch (HexWorldTerrain.GetHex((int)hexCoords.x,(int)hexCoords.y,(int)hexCoords.z).prevType)
				{
					case 0:
						HexWorldTerrain.SetHex((int)hexCoords.x,(int)hexCoords.y,(int)hexCoords.z,new HexPrism());
						break;
					case 1:
						HexWorldTerrain.SetHex((int)hexCoords.x,(int)hexCoords.y,(int)hexCoords.z,new HexPrismAir());
						break;
					case 2:
						HexWorldTerrain.SetHex((int)hexCoords.x,(int)hexCoords.y,(int)hexCoords.z,new HexPrismGrass());
						break;
					case 3:
						HexWorldTerrain.SetHex((int)hexCoords.x,(int)hexCoords.y,(int)hexCoords.z,new HexPrismTransparent());
						break;
					case 4:
						HexWorldTerrain.SetHex((int)hexCoords.x,(int)hexCoords.y,(int)hexCoords.z,new HexPrismStone());
						break;
					default:
						HexWorldTerrain.SetHex((int)hexCoords.x,(int)hexCoords.y,(int)hexCoords.z,new HexPrism());
						break;
				}
			}
		}
	}

	public static void AddOccludingChunk(HexPrismChunk chunk) {
		if (occludingHexPrismChunks != null) {
			if (!occludingHexPrismChunks.Contains (chunk)) {
				occludingHexPrismChunks.Add (chunk);
			}
		} else {
			occludingHexPrismChunks.Add (chunk);
		}
	}

	public static void AddOccludingHexCoords(Vector3 location) {
		if (occludingHexPrismsHexCoords != null) {
			if (!occludingHexPrismsHexCoords.Contains(location)) {
				occludingHexPrismsHexCoords.Add (location);
			}
		} else {
			occludingHexPrismsHexCoords.Add (location);
		}
	}

	public static void UpdateOccludingChunks() {
		if (occludingHexPrismChunks != null) {
			foreach(HexPrismChunk chunk in occludingHexPrismChunks) {
				if (chunk.update) {
					chunk.UpdateChunk();
					chunk.update = false;
				}

				if (HexWorldTerrain.GetChunk(chunk.pos.x+1,chunk.pos.y,chunk.pos.z) != null) {
					if (HexWorldTerrain.GetChunk(chunk.pos.x+1,chunk.pos.y,chunk.pos.z).update) {
						HexWorldTerrain.GetChunk(chunk.pos.x+1,chunk.pos.y,chunk.pos.z).UpdateChunk();
						HexWorldTerrain.GetChunk(chunk.pos.x+1,chunk.pos.y,chunk.pos.z).update = false;
					}
				}
				if (HexWorldTerrain.GetChunk(chunk.pos.x-1,chunk.pos.y,chunk.pos.z) != null) {
					if (HexWorldTerrain.GetChunk(chunk.pos.x-1,chunk.pos.y,chunk.pos.z).update) {
						HexWorldTerrain.GetChunk(chunk.pos.x-1,chunk.pos.y,chunk.pos.z).UpdateChunk();
						HexWorldTerrain.GetChunk(chunk.pos.x-1,chunk.pos.y,chunk.pos.z).update = false;
					}
				}
				if (HexWorldTerrain.GetChunk(chunk.pos.x,chunk.pos.y+1,chunk.pos.z) != null) {
					if (HexWorldTerrain.GetChunk(chunk.pos.x,chunk.pos.y+1,chunk.pos.z).update) {
						HexWorldTerrain.GetChunk(chunk.pos.x,chunk.pos.y+1,chunk.pos.z).UpdateChunk();
						HexWorldTerrain.GetChunk(chunk.pos.x,chunk.pos.y+1,chunk.pos.z).update = false;
					}
				}
				if (HexWorldTerrain.GetChunk(chunk.pos.x,chunk.pos.y-1,chunk.pos.z) != null) {
					if (HexWorldTerrain.GetChunk(chunk.pos.x,chunk.pos.y-1,chunk.pos.z).update) {
						HexWorldTerrain.GetChunk(chunk.pos.x,chunk.pos.y-1,chunk.pos.z).UpdateChunk();
						HexWorldTerrain.GetChunk(chunk.pos.x,chunk.pos.y-1,chunk.pos.z).update = false;
					}
				}
				if (HexWorldTerrain.GetChunk(chunk.pos.x,chunk.pos.y,chunk.pos.z+1) != null) {
					if (HexWorldTerrain.GetChunk(chunk.pos.x,chunk.pos.y,chunk.pos.z+1).update) {
						HexWorldTerrain.GetChunk(chunk.pos.x,chunk.pos.y,chunk.pos.z+1).UpdateChunk();
						HexWorldTerrain.GetChunk(chunk.pos.x,chunk.pos.y,chunk.pos.z+1).update = false;
					}
				}
				if (HexWorldTerrain.GetChunk(chunk.pos.x,chunk.pos.y,chunk.pos.z-1) != null) {
					if (HexWorldTerrain.GetChunk(chunk.pos.x,chunk.pos.y,chunk.pos.z-1).update) {
						HexWorldTerrain.GetChunk(chunk.pos.x,chunk.pos.y,chunk.pos.z-1).UpdateChunk();
						HexWorldTerrain.GetChunk(chunk.pos.x,chunk.pos.y,chunk.pos.z-1).update = false;
					}
				}
			}
		}
	}
}















