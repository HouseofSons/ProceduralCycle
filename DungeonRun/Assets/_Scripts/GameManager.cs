using UnityEngine;

public class GameManager : MonoBehaviour
{
    //used for development purposes
    public string playerName;
	public string levelName;

	//used to determine minimum speed of player
	public float speedInputMinimum;
    //used to determine maximun speed of player
    public float speedInputMaximum;
    //used to determine speed of player
    public float energyDefault;
    //used to determine minimum Camera size
    public float cameraSizeMinimum;
    //used to determine maximum Camera size
    public float cameraSizeMaximum;

    //used to establish current player level
    private static GameObject currentPlayer;
	private static GameObject currentLevel;
	private static GameObject gameCamera;
	private static GameObject gameManager;

    //used to attach visual line from player when aiming for Debugging
    private static LineRenderer pathLine;

    void Start () {
        gameManager = this.gameObject;
        gameCamera = GameObject.Find("MainCamera");

		StageNumber = 0;
		IsPaused = false;

		MoveToSpawnState = false;
        PlayerMovingState = false;
		EnableEnemyMovement = false;
		EnterDoor = false;
        MoveCamera = false;

        SpeedMin = speedInputMinimum;
        SpeedMax = speedInputMaximum;
        Energy = energyDefault;

        CameraSizeMin = cameraSizeMinimum;
        CameraSizeMax = cameraSizeMaximum;

        InitializePlayerInLevel(playerName,levelName);
        InitializeCameraInLevel();
		
		pathLine = (Instantiate(Resources.Load("Line")) as GameObject).GetComponent<LineRenderer>();
        pathLine.material = new Material(Shader.Find("Sprites/Default"));
        pathLine.positionCount = 4;
        pathLine.numCapVertices = 90;
        pathLine.numCornerVertices = 90;
        pathLine.widthMultiplier = 0.06f;
        pathLine.enabled = false;
    }

    void Update() {

        if (!IsPaused) {
            //Place Holder for paused game
		}

		if (StageNumber == 0) {
			//Game Started and Camera is in position
			MoveToSpawnState = true;
            StageNumber = 1;
		}
		if (StageNumber == 1 && !MoveToSpawnState) {
            //Player is in position
            PlayerAimingState = true;
            StageNumber = 2;
		}
		if (StageNumber == 2 && PlayerMovingState) {
			//Player has been released from original level spawn
			EnableEnemyMovement = true;
			StageNumber = 3;
		}
		if (StageNumber == 3 && EnterDoor) {
			//player switch level
			EnterDoor = false;
			MoveToSpawnState = true;
            PlayerAimingState = false;
            PlayerMovingState = false;
            EnableEnemyMovement = false;
            MoveCamera = true;
            StageNumber = 1;
		}
        //		moveToSpawnState
        //      playerMovingState
        //		enableEnemyMovement
        //		enterDoor
    }

    public static int StageNumber { get; private set; }
    public static bool IsPaused { get; set; }
    public static bool MoveToSpawnState { get; set; }
    public static bool PlayerAimingState { get; set; }
    public static bool PlayerMovingState { get; set; }
    public static bool EnableEnemyMovement { get; set; }
    public static bool EnterDoor { get; set; }
    public static bool MoveCamera { get; set; }
    public static float CameraSizeMin { get; set; }
    public static float CameraSizeMax { get; set; }
    public static float SpeedMin { get; set; }
    public static float SpeedMax { get; set; }
    public static float Energy { get; set; }

    public static Level GetCurrentLevel() {
		return currentLevel.GetComponent<Level>();
	}
	
	public static Player GetCurrentPlayer() {
		return currentPlayer.GetComponent<Player>();
	}

    public static GameObject GetCamera() {
		return gameCamera;
	}

    public static LineRenderer PathLine()
    {
        return pathLine;
    }

    private static void InitializePlayerInLevel(string player,string level)
    { 
        currentPlayer = Instantiate(Resources.Load(player)) as GameObject;
        currentLevel = Instantiate(Resources.Load(level)) as GameObject;
        currentPlayer.GetComponent<Player>().LatestSpawn =
            currentLevel.transform.Find("InitialSpawn").GetComponent<Spawn>();
    }

	public static void DoorHit(Door door) {
        currentPlayer.GetComponent<Player>().LatestSpawn =
                door.Destination(currentPlayer.transform.position);
        Player.LevelStatsReset(Energy);
		EnterDoor = true;
	}

	private void InitializeCameraInLevel()
	{
        gameCamera.transform.position = new Vector3(0, 20, 0);
	}
	//Shows Game Over Canvas
	public static void GameOver() {
		gameManager.GetComponent<InGameOptions> ().ShowGameOverMenu ();
	}
}