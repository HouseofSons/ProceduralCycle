using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*Need to Add multiplayer*/
/*Need to add attacks to Player*/
/*Need to make map destructible*/
/*Need to determine how map gets turned*/
/*Need to determine how player dies when leaving map*/

public class LevelManager: MonoBehaviour {

	public GameObject wallPrefab;//for Unity Inspector

	private static GameCamera cam;
	public static Coroutine cameraMapTurn;
	public static bool needToPullObjectsToMap;

	public int wallCount;//for Unity Inspector

	public static bool isPaused;

	public static int currentWallNumber;
	public static int prevWallNumber;

	public static List<Face> mapFaces;
	public static List<Wall> walls;
	public static GameObject player;

	private static int[,][] edgeConnectionKeys;

	public const int FACE_MAGNITUDE = 64;//Must be an Even Number
	public const float FACE_CENTER_SPACING = 2.25f;//the smaller this value the less spacing from raw edge to face origin for border to travel
	public const int FACE_ORIGIN_BUFFER = (int)((FACE_MAGNITUDE/2.00f)/FACE_CENTER_SPACING);
	public const float FACE_MAGNITUDE_MIN = FACE_MAGNITUDE / 12.00f;
	public const float FACE_MAGNITUDE_MAX = FACE_MAGNITUDE / 9.00f;
	public static Vector3 FALSE_VECTOR = new Vector3 (-0.5f,-0.5f,-0.5f);

	private enum Direction { up, down, left, right };

	void Awake () {
	/*INITIALIZE VARIABLES*/
		cam = GameObject.Find ("Camera").GetComponent<GameCamera>();
		mapFaces = new List<Face> ();
		walls = new List<Wall> ();
		player = GameObject.Find("player");
        currentWallNumber = 0; //Assign
		prevWallNumber = 0; //Assign
		isPaused = false;
		needToPullObjectsToMap = false;
	}

	void Start () {
	/*CREATE WALLS*/
		for (int i=0; i<wallCount; i++) {
			CreateWall();
		}

	/*IDENTIFY CONNECTIONS BETWEEN WALLS*/
		InitializeEdgeConnectionKeys ();

	/*BUILD INITIAL WALL*/
		LevelManager.walls[0].BuildGameGrid ();
		walls [0].WallVisible (true);
    /*ADD PLAYER*/
        AddPlayer(player);
    }

	void Update () {
		if (needToPullObjectsToMap) {//should include any moveable objects in the future
			needToPullObjectsToMap = false;
			MoveObjectsToNewMap(player);
		}
	}

	public static void AddPlayer(GameObject player) {
		player.transform.parent = walls [0].gameObject.transform;
		player.transform.localPosition = new Vector3 (18,16,FACE_MAGNITUDE*2);//Should update to identify available platforms
	}

	private void CreateWall() {
		Face f;
		do {
			f = new Face (new Vector3(0,0,FACE_MAGNITUDE), Vector3.forward);
		} while(!f.EdgeOneBorderTouched ());
		mapFaces.Add (f);
		GameObject go = Instantiate (wallPrefab,Vector3.zero,Quaternion.Euler(Vector3.zero)) as GameObject;
		go.name = "Wall_" + (mapFaces.Count);
		Wall wall = go.GetComponent<Wall> ();
		wall.AssignFaceToWall (f);
		walls.Add(wall);
		wall.WallVisible (false);
		go.layer = LayerMask.NameToLayer("map_border");

		GameObject cameraPos = new GameObject ();
		cameraPos.name = "a_camera_position";
		cameraPos.transform.parent = go.transform;
		cameraPos.transform.position = new Vector3 (FACE_MAGNITUDE,FACE_MAGNITUDE/2.00f,-1);
		
		GameObject cameraLookAt = new GameObject ();
		cameraLookAt.name = "b_camera_look_at";
		cameraLookAt.transform.parent = go.transform;
		cameraLookAt.transform.position = new Vector3 (FACE_MAGNITUDE,FACE_MAGNITUDE/2.00f,FACE_MAGNITUDE*2+1);
	}

	private void InitializeEdgeConnectionKeys () {
		//Assign Connection between edge keys
		edgeConnectionKeys = new int[mapFaces.Count, 4][];

		for (int i=0; i<mapFaces.Count; i++) {
			for (int j=0; j<4; j++) {
				if (j == 0 || j == 2) {
					edgeConnectionKeys [i, j] = new int[FACE_MAGNITUDE * 2 + 1];
				} else {
					edgeConnectionKeys [i, j] = new int[FACE_MAGNITUDE + 1];
				}
			}
		}
		int faceCount = 0;

		foreach (Face f in mapFaces) {

			for (int i=0; i<FACE_MAGNITUDE+1; i++) {
				edgeConnectionKeys [faceCount, 1] [i] = f.gridRowMin [i];
				edgeConnectionKeys [faceCount, 3] [i] = f.edgeGrid.GetLength (0) - f.gridRowMax [i];
			}
			for (int i=0; i<FACE_MAGNITUDE*2+1; i++) {
				edgeConnectionKeys [faceCount, 0] [i] = f.edgeGrid.GetLength (1) - f.gridColMax [i];
				edgeConnectionKeys [faceCount, 2] [i] = f.gridColMin [i];
			}
			faceCount++;
		}
	}
	
	public void TurnMapLeft() {
	
        player.GetComponent<Player>().PlayerToggleActive();
        player.transform.position = MoveObjectMapLocation(Direction.left, player);
		
		BuildNextMap (Direction.left);
		Physics.gravity = walls [currentWallNumber].transform.up * -9.81f;//changes gravity direction after turn
		StartCoroutine (MapReset (false));
	}
	
	public void TurnMapRight() {
		

        player.GetComponent<Player>().PlayerToggleActive();
        player.transform.position = MoveObjectMapLocation(Direction.right, player);
		
		BuildNextMap (Direction.right);
		Physics.gravity = walls [currentWallNumber].transform.up * -9.81f;//changes gravity direction after turn
		StartCoroutine (MapReset (false));
	}
	
	public void TurnMapUp() {
		            
        player.GetComponent<Player>().PlayerToggleActive();
        player.transform.position = MoveObjectMapLocation(Direction.up, player);
		
		BuildNextMap (Direction.up);
		Physics.gravity = walls [currentWallNumber].transform.up * -9.81f;//changes gravity direction after turn
		StartCoroutine (MapReset (true));
	}
	
	public void TurnMapDown() {

        player.GetComponent<Player>().PlayerToggleActive();
        player.transform.position = MoveObjectMapLocation(Direction.down, player);
		
		BuildNextMap (Direction.down);
		Physics.gravity = walls [currentWallNumber].transform.up * -9.81f;//changes gravity direction after turn
		StartCoroutine (MapReset (true));
	}

	private IEnumerator MapReset(bool turnHorizon) {

		Vector3 prevWallLocalCamPos = LevelManager.walls [LevelManager.prevWallNumber].gameObject.transform.GetChild (0).localPosition;
		prevWallLocalCamPos = new Vector3(prevWallLocalCamPos.x,prevWallLocalCamPos.y,(turnHorizon ? -LevelManager.FACE_MAGNITUDE/16 : -LevelManager.FACE_MAGNITUDE));
		LevelManager.walls [LevelManager.prevWallNumber].gameObject.transform.GetChild (0).localPosition = prevWallLocalCamPos;

		cam.gameObject.transform.position = LevelManager.walls [LevelManager.prevWallNumber].gameObject.transform.GetChild (0).position;

		yield return StartCoroutine(cam.GetComponent<MatrixBlender>().LerpFromTo(cam.GetComponent<Camera>().projectionMatrix,MatrixBlender.perspective, 2f, 20f));
		yield return StartCoroutine(cam.CameraMapTurn(turnHorizon));
		yield return StartCoroutine(cam.GetComponent<MatrixBlender>().LerpFromTo(cam.GetComponent<Camera>().projectionMatrix,MatrixBlender.ortho, 0.5f));
		yield return null;
		yield return StartCoroutine (LevelManager.ResetMap ());
	}

	private void BuildNextMap(Direction d) {

		int nextWallNumber;
		Vector3 currentPosition = walls [currentWallNumber].gameObject.transform.position;
		Quaternion currentRotation = walls [currentWallNumber].gameObject.transform.rotation;

		if (currentWallNumber == walls.Count-1) {
			nextWallNumber = 0;
		} else {
			nextWallNumber = currentWallNumber + 1;
		}

		if(d == Direction.left) {
			walls[currentWallNumber].BendGameGrid (1,edgeConnectionKeys[nextWallNumber,3]);
			walls[nextWallNumber].BendGameGrid (3,edgeConnectionKeys[currentWallNumber,1]);
			walls[nextWallNumber].gameObject.transform.position = currentPosition;
			walls[nextWallNumber].gameObject.transform.rotation = currentRotation;
			walls[nextWallNumber].gameObject.transform.Rotate(new Vector3(0,270,0));

			walls[nextWallNumber].gameObject.transform.position =
				walls[nextWallNumber].gameObject.transform.position - walls[nextWallNumber].gameObject.transform.forward * FACE_MAGNITUDE*2 -
					walls[nextWallNumber].gameObject.transform.right;

		} else if (d == Direction.right) {
			walls[currentWallNumber].BendGameGrid (3,edgeConnectionKeys[nextWallNumber,1]);
			walls[nextWallNumber].BendGameGrid (1,edgeConnectionKeys[currentWallNumber,3]);
			walls[nextWallNumber].gameObject.transform.position = currentPosition;
			walls[nextWallNumber].gameObject.transform.rotation = currentRotation;
			walls[nextWallNumber].gameObject.transform.Rotate(new Vector3(0,90,0));
			
			walls[nextWallNumber].gameObject.transform.position =
				walls[nextWallNumber].gameObject.transform.position + walls[nextWallNumber].gameObject.transform.forward -
					walls[nextWallNumber].gameObject.transform.right * FACE_MAGNITUDE*2;

		} else if (d == Direction.up) {
			walls[currentWallNumber].BendGameGrid (0,edgeConnectionKeys[nextWallNumber,2]);
			walls[nextWallNumber].BendGameGrid (2,edgeConnectionKeys[currentWallNumber,0]);
			walls[nextWallNumber].gameObject.transform.position = currentPosition;
			walls[nextWallNumber].gameObject.transform.rotation = currentRotation;
			walls[nextWallNumber].gameObject.transform.Rotate(new Vector3(270,0,0));

			walls[nextWallNumber].gameObject.transform.position =
				walls[nextWallNumber].gameObject.transform.position - walls[nextWallNumber].gameObject.transform.up * FACE_MAGNITUDE*2 -
					walls[nextWallNumber].gameObject.transform.forward * (FACE_MAGNITUDE-1);
			
		} else /*down*/{
			walls[currentWallNumber].BendGameGrid (2,edgeConnectionKeys[nextWallNumber,0]);
			walls[nextWallNumber].BendGameGrid (0,edgeConnectionKeys[currentWallNumber,2]);
			walls[nextWallNumber].gameObject.transform.position = currentPosition;
			walls[nextWallNumber].gameObject.transform.rotation = currentRotation;
			walls[nextWallNumber].gameObject.transform.Rotate(new Vector3(90,0,0));
			
			walls[nextWallNumber].gameObject.transform.position =
				walls[nextWallNumber].gameObject.transform.position + walls[nextWallNumber].gameObject.transform.up * (FACE_MAGNITUDE-1) -
					walls[nextWallNumber].gameObject.transform.forward * FACE_MAGNITUDE*2;

		}

		prevWallNumber = currentWallNumber;
		currentWallNumber = nextWallNumber;
		walls[currentWallNumber].WallVisible (true);
	}

	private static Vector3 MoveObjectMapLocation(Direction d, GameObject go) {

		//for first check, should have funciton which attaches objects to wall ahead of time
		go.transform.parent = walls [currentWallNumber].gameObject.transform;
		
		float distance;

		if (d == Direction.left) {
			distance = (FACE_MAGNITUDE * 2 + 1) - go.transform.localPosition.x;
		} else if (d == Direction.right) {
			distance = go.transform.localPosition.x;
		} else if (d == Direction.up) {
			distance = go.transform.localPosition.y;
		} else /*Direction.down*/ {
			distance = (FACE_MAGNITUDE + 1) - go.transform.localPosition.y;
		}
		return go.transform.position + (-walls [currentWallNumber].gameObject.transform.forward * distance);
	}

	public static void MoveObjectsToNewMap(GameObject go) {

		Vector3 prevPosition = go.transform.localPosition;

		go.transform.parent = walls [currentWallNumber].gameObject.transform;
		go.transform.localPosition = new Vector3(prevPosition.x,prevPosition.y,FACE_MAGNITUDE*2);
	}

	public static IEnumerator ResetMap() {//used by GameCamera to reset for new map
		LevelManager.cameraMapTurn = null;
		LevelManager.walls[LevelManager.prevWallNumber].WallVisible (false);
		LevelManager.walls [LevelManager.prevWallNumber].gameObject.GetComponent<Collider> ().enabled = false;
		LevelManager.walls[LevelManager.currentWallNumber].BuildGameGrid();
		LevelManager.walls [LevelManager.currentWallNumber].gameObject.GetComponent<Collider> ().enabled = true;
		LevelManager.needToPullObjectsToMap = true;
		yield return null;
        //ReActivate Players
        player.GetComponent<Player>().PlayerToggleActive();
	}
}





















