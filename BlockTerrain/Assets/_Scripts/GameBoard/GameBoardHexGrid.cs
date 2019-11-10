using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameBoardHexGrid : MonoBehaviour {

	private static GameBoardHex[,,] gameBoardHexs;
	public GameObject hexPrefab;
	private static GameObject hexagonalPrismPrefab;

	
	// Use this for initialization
	void Start () {
		hexagonalPrismPrefab = hexPrefab;
		gameBoardHexs = new GameBoardHex[HexWorldTerrain.GetWidth(), HexWorldTerrain.GetHeight(), HexWorldTerrain.GetWidth()];
	}

	public static void CreateGameBoardHex(int x, int y, int z) {	
		if (gameBoardHexs [x, y, z] == null) {
			GameObject terrainBlockPiece = Instantiate (hexagonalPrismPrefab, new Vector3 (HexWorldTerrain.xHexToWorldCoordinate (x, z), y * HexWorldTerrain.GetHexScalar(), HexWorldTerrain.zHexToWorldCoordinate (z)), Quaternion.identity) as GameObject;
            terrainBlockPiece.transform.localScale = new Vector3(terrainBlockPiece.transform.localScale.x, terrainBlockPiece.transform.localScale.y * (HexWorldTerrain.GetHexScalar()/0.5f), terrainBlockPiece.transform.localScale.z);
            terrainBlockPiece.AddComponent<GameBoardHex> ();
			terrainBlockPiece.GetComponent<GameBoardHex> ().setCoordinates (x, y, z);
			gameBoardHexs [x, y, z] = terrainBlockPiece.GetComponent<GameBoardHex> ();
			terrainBlockPiece.name = "X:" + x + ",Y:" + y + ",Z:" + z;
			terrainBlockPiece.transform.parent = GameObject.Find ("GameBoardHexGrid").transform;
		}
	}
	
	public static GameBoardHex[,,] GameBoard() {
		return gameBoardHexs;
	}

	public static void DestroyGameBoardHex (GameBoardHex hex) {
		if (hex.IsOccupied()) {
			Character.DestroyCharacter(hex.GetOccupant());
		}
		GameObject.Destroy(hex.gameObject);
		GameBoardHexGrid.GameBoard()[hex.GetXCoord(),hex.GetYCoord(),hex.GetZCoord()] = null;
	}

    public static bool InGameBoardBounds(Vector3 coordinate)
    {
        if (Mathf.FloorToInt(coordinate.x) < 0 || Mathf.FloorToInt(coordinate.x) > gameBoardHexs.GetLength(0) - 1)
        {
            return false;
        }
        if (Mathf.FloorToInt(coordinate.y) < 0 || Mathf.FloorToInt(coordinate.y) > gameBoardHexs.GetLength(1) - 1)
        {
            return false;
        }
        if (Mathf.FloorToInt(coordinate.z) < 0 || Mathf.FloorToInt(coordinate.z) > gameBoardHexs.GetLength(2) - 1)
        {
            return false;
        }
        return true;
    }

    public static List<GameBoardHex> ColumnOfNeighboringHexs(int x, int y, int z, int radius, int verticalDistance) {

		List<GameBoardHex> hexs = new List<GameBoardHex>();

		for (int i = x-radius; i<=x+radius; i++) {
			for (int j = z-Mathf.Max(radius,-i-radius); j<=z+Mathf.Max(radius,-i+radius);j++) {
				for (int k = y-verticalDistance;k<=y+verticalDistance;k++) {
					if (HexWorldTerrain.InWorldRange(i,k,j)) {
						if (gameBoardHexs[i,k,j] != null) {
							hexs.Add(gameBoardHexs[i,k,j]);
						}
					}
				}
			}
		}
		return hexs;
	}

	public static List<GameBoardHex> CharacterVisibleGameBoardPieces(Character engagedCharacter) {
		GameBoardHex engagedHex = engagedCharacter.GetOccupiedHex ();
		List<GameBoardHex> visibleHexs = new List<GameBoardHex> ();
		Vector3 aboveGameBoardPiece = new Vector3(engagedHex.gameObject.transform.position.x,engagedHex.gameObject.transform.position.y+0.50f/*yChange*/,engagedHex.gameObject.transform.position.z);

		foreach(GameBoardHex hex in ColumnOfNeighboringHexs(engagedHex.GetXCoord(),engagedHex.GetYCoord(),engagedHex.GetZCoord(),engagedCharacter.VisionDistance,engagedCharacter.VisionDistance)) {
			Vector3 aboveHex = new Vector3(hex.gameObject.transform.position.x,hex.gameObject.transform.position.y+0.50f/*yChange*/,hex.gameObject.transform.position.z);

			if(!Physics.Raycast(aboveGameBoardPiece,aboveHex-aboveGameBoardPiece,Vector3.Distance(aboveGameBoardPiece,aboveHex),1<<LayerMask.NameToLayer("Terrain"))) {
				visibleHexs.Add (hex);
			}
		}
		return visibleHexs;
	}

	public static GameBoardHex ClosestGameBoardHex (Vector3 pos) {
		Vector3 tempLocation = HexWorldTerrain.WorldToHexCoordinates (pos);

		for (int i=0;i<gameBoardHexs.GetLength(0);i++) {
			for (int j=0;j<gameBoardHexs.GetLength(1);j++) {
				for (int k=0;k<gameBoardHexs.GetLength(2);k++) {
					if (gameBoardHexs[i,j,k] != null) {
						if (gameBoardHexs[i,j,k].GetXCoord() == Mathf.RoundToInt(tempLocation.x) &&
						    gameBoardHexs[i,j,k].GetYCoord() == Mathf.RoundToInt(tempLocation.y) &&
						    gameBoardHexs[i,j,k].GetZCoord() == Mathf.RoundToInt(tempLocation.z)) {
							return gameBoardHexs[i,j,k];
						}
					}
				}
			}
		}
		return null;
	}
}
	
















