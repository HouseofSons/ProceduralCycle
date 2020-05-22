//IMPORTANT: Script is set in the Execution Order to run last
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //used for development purposes
    public string playerName;
	public string levelName;

	//used to determine speed of player
	public float speedInput;

    //used to establish current player level and spawn
    private static GameObject currentPlayer;
	private static GameObject currentLevel;
	private static GameObject gameCamera;
	private static GameObject gameManager;

    //used to attach visual arrow over player when aiming
    private static GameObject aimArrow;
    //used to attach visual line from player when aiming
    private static LineRenderer pathLine;
    //used to attach visual line from player after path is chosen
    private static LineRenderer pathChosenLine;

    void Start () {
        gameManager = this.gameObject;
        gameCamera = GameObject.Find("MainCamera");

		StageNumber = 0;
		IsPaused = false;

		MoveToSpawnState = false;
		AimArrowState = false;
        PlayerMovingState = false;
		EnableEnemyMovement = false;
		EnterDoor = false;
        MoveCamera = false;

        Speed = speedInput;

		InitializePlayerInLevel(playerName,levelName);
        InitializeCameraInLevel();
		
		aimArrow = currentPlayer.transform.Find("AimArrow").gameObject;
		aimArrow.gameObject.GetComponent<SpriteRenderer>().enabled = false;
		
		pathLine = (Instantiate(Resources.Load("Line")) as GameObject).GetComponent<LineRenderer>();
        pathLine.material = new Material(Shader.Find("Sprites/Default"));
        pathLine.positionCount = 4;
        pathLine.numCapVertices = 90;
        pathLine.numCornerVertices = 90;
        pathLine.widthMultiplier = 0.06f;
        pathLine.enabled = false;
		
		pathChosenLine = (Instantiate(Resources.Load("Line")) as GameObject).GetComponent<LineRenderer>();
        pathChosenLine.material = new Material(Shader.Find("Sprites/Default"));
        pathChosenLine.positionCount = 4;
        pathChosenLine.numCapVertices = 90;
        pathChosenLine.numCornerVertices = 90;
        pathChosenLine.widthMultiplier = 0.06f;
        pathChosenLine.enabled = false;
    }

    void Update() {

        if (!IsPaused) {
			currentPlayer.transform.LookAt(MouseLocation ());
		}

		if (StageNumber == 0) {
			//Game Started and Camera is in position
			MoveToSpawnState = true;
            StageNumber = 1;
		}
		if (StageNumber == 1 && !MoveToSpawnState) {
            //Player is in position
			AimArrowState = true;
            StageNumber = 2;
		}
		if (StageNumber == 2 && PlayerMovingState) {
			//Player has been released from original level spawn
			EnableEnemyMovement = true;
            pathChosenLine.enabled = true;
			StageNumber = 3;
		}
		if (StageNumber == 3 && EnterDoor) {
			//player switch level
			EnterDoor = false;
			MoveToSpawnState = true;
            AimArrowState = false;
            PlayerMovingState = false;
            EnableEnemyMovement = false;
            MoveCamera = true;
            StageNumber = 1;
		}
        //		moveToSpawnState
        //		aimArrowState
        //      playerMovingState
        //		enableEnemyMovement
        //		enterDoor
    }

    public static int StageNumber { get; private set; }
    public static bool IsPaused { get; set; }
    public static bool MoveToSpawnState { get; set; }
    public static bool AimArrowState { get; set; }
    public static bool PlayerMovingState { get; set; }
    public static bool EnableEnemyMovement { get; set; }
    public static bool EnterDoor { get; set; }
    public static bool MoveCamera { get; set; }
    public static float Speed { get; set; }

    public static Level GetCurrentLevel() {
		return currentLevel.GetComponent<Level>();
	}
	
	public static Player GetCurrentPlayer() {
		return currentPlayer.GetComponent<Player>();
	}

	public static GameObject GetCamera() {
		return gameCamera;
	}

	public static GameObject AimArrow() {
		return aimArrow;
	}

    public static LineRenderer PathLine()
    {
        return pathLine;
    }

    public static LineRenderer PathChosenLine()
    {
        return pathChosenLine;
    }

    private static void InitializePlayerInLevel(string player,string level)
    { 
        currentPlayer = Instantiate(Resources.Load(player)) as GameObject;
        currentLevel = Instantiate(Resources.Load(level)) as GameObject;
        currentPlayer.GetComponent<Player>().LatestSpawn =
            currentLevel.transform.Find("InitialSpawn").GetComponent<Spawn>();
        currentPlayer.GetComponent<Player>().OccupiedPartition =
            currentPlayer.GetComponent<Player>().LatestSpawn.SpawnPartition.GetComponent<Partition>();
    }

	public static void DoorHit(Door door) {
        currentPlayer.GetComponent<Player>().LatestSpawn =
                door.Destination(currentPlayer.transform.position);
        currentPlayer.GetComponent<Player>().OccupiedPartition =
            currentPlayer.GetComponent<Player>().LatestSpawn.SpawnPartition.GetComponent<Partition>();
        Player.LevelStatsReset(100);
		EnterDoor = true;
	}

	private void InitializeCameraInLevel()
	{
        gameCamera.transform.position = new Vector3(0, 20, 0);
        gameCamera.transform.parent = currentLevel.transform;
	}

	public static Vector3 MouseLocation () {

		Plane plane=new Plane(Vector3.up, new Vector3(0,currentPlayer.transform.position.y,0));
		Ray ray=gameCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        else
        {
            return currentPlayer.transform.position;
        }
    }
	//Shows Game Over Canvas
	public static void GameOver() {
		gameManager.GetComponent<InGameOptions> ().ShowGameOverMenu ();
	}
}









