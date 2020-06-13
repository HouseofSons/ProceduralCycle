using UnityEngine;

public class GameManager : MonoBehaviour
{
    //----Inspector Populated Fields START
    //Prefab Player Object reference
    public string playerName;
    //Prefab Level Object reference
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
    //Max number of chosen positions allowed
    [Range(1, 3)]
    public int positionChoiceCount;
    //----Inspector Populated Fields END

    public static bool IsPaused             { get; set; }
    public static bool MoveToSpawnState     { get; set; }
    public static bool PlayerAimingState    { get; set; }
    public static bool PlayerMovingState    { get; set; }
    public static bool EnableEnemyMovement  { get; set; }
    public static bool EnterDoor            { get; set; }
    public static bool MoveCamera           { get; set; }

    public static float CameraSizeMin       { get; set; }
    public static float CameraSizeMax       { get; set; }
    public static float SpeedMin            { get; set; }
    public static float SpeedMax            { get; set; }
    public static float EnergyDefault       { get; set; }

    public static GameManager Manager       { get; private set; }
    public static Player CurrentPlayer      { get; private set; }
    public static Level CurrentLevel        { get; private set; }
    public static MainCamera Camera         { get; private set; }
    public static int StageNumber           { get; private set; }

    void Start ()
    {
        IsPaused = false;
        MoveToSpawnState = false;
        PlayerAimingState = false;
        PlayerMovingState = false;
        EnableEnemyMovement = false;
        EnterDoor = false;
        MoveCamera = false;

        CameraSizeMin = cameraSizeMinimum;
        CameraSizeMax = cameraSizeMaximum;
        SpeedMin = speedInputMinimum;
        SpeedMax = speedInputMaximum;
        EnergyDefault = energyDefault;

        Manager = this;
        CurrentLevel = (Instantiate(Resources.Load(levelName)) as GameObject).GetComponent<Level>();
        CurrentPlayer = (Instantiate(Resources.Load(playerName)) as GameObject).GetComponent<Player>();
        Player.PlayerManualPositionSize = positionChoiceCount;
        Camera = GameObject.Find("MainCamera").GetComponent<MainCamera>();
        Camera.transform.position = new Vector3(0, 20, 0);
        StageNumber = 0;
    }

    void Update()
    {
        if (!IsPaused)
        {
            //Place Holder for paused game
		}

		if (StageNumber == 0)
        {
			//Game Started and Camera is in position
			MoveToSpawnState = true;
            StageNumber = 1;
		}
		if (StageNumber == 1 && !MoveToSpawnState)
        {
            //Player is in position
            PlayerAimingState = true;
            StageNumber = 2;
		}
		if (StageNumber == 2 && PlayerMovingState)
        {
			//Player has been released from original level spawn
			EnableEnemyMovement = true;
			StageNumber = 3;
		}
		if (StageNumber == 3 && EnterDoor)
        {
			//player switch level
			EnterDoor = false;
			MoveToSpawnState = true;
            PlayerAimingState = false;
            PlayerMovingState = false;
            EnableEnemyMovement = false;
            MoveCamera = true;
            StageNumber = 1;
		}
    }
	//Shows Game Over Canvas
	public static void GameOver() {
        Manager.GetComponent<InGameOptions> ().ShowGameOverMenu ();
	}
}