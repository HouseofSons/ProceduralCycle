using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	
	//used to determine current spawn location
	private Vector3 spawnPoint;

    //CoRoutine which moves player to spawn
    private static Coroutine moveToSpawnCoRoutine;

	//Direction Player is moving
	private static Vector3 playerMovingDirection;
    //Disables Collisions when traveling between levels
	private static bool disablePlayerCollisions;

    //Wall points calculated by Players position and direction
    public static List<Vector3> WallCollisionPoints;
    //Wall points calculated by Players chosen path
    public static List<Vector3> PathPoints;

    void Awake () {
		PlayerPathDistanceMax = 100;
		PlayerPathDistance = 0;
		TotalExperiencePoints = 0;
		Level = 1;
		playerMovingDirection = Vector3.zero;
		disablePlayerCollisions = false;
    }

	void Start () {
		UI.InitializeUI ();
	}

	void Update () {

		if (GameManager.MoveToSpawnState) {
            GameManager.PathLine().enabled = false;
            GameManager.PathChosenLine().enabled = false;
			if (moveToSpawnCoRoutine != null) {
				StopCoroutine(moveToSpawnCoRoutine);
			}
			moveToSpawnCoRoutine = StartCoroutine (MoveToSpawn());
		} 

		if (GameManager.AimArrowState) {
            UpdateWallCollisions();
            UpdateGuidePath();

            if (Input.GetMouseButton(0)) {
                GameManager.PathLine().enabled = true;
                GameManager.PathChosenLine().enabled = false;
            }

            if (Input.GetMouseButtonUp(0)) {
                if (!EventSystem.current.IsPointerOverGameObject()) {
                    GameManager.PlayerMovingState = true;
                    GameManager.PathLine().enabled = false;
                    GameManager.PathChosenLine().enabled = true;
                    playerMovingDirection = GameManager.AimArrow().transform.up;
                    PathPoints = WallCollisionPoints;
                    if (PlayerFollowPathCoRoutine != null)
                    {
                        StopCoroutine(PlayerFollowPathCoRoutine);
                    }
                    PlayerFollowPathCoRoutine = StartCoroutine (PlayerFollowPath ());
                    UpdateChosenPath();
                }
			}
		}
	}

	void OnTriggerEnter(Collider col) {

		if(!disablePlayerCollisions) {
            if (col.gameObject.name.StartsWith("Door")) {
                disablePlayerCollisions = true;
                StopCoroutine(PlayerFollowPathCoRoutine);//stops player movement if Door Hit
                GameManager.DoorHit(col.transform.name);
            }
		}
	}

    public static Coroutine PlayerFollowPathCoRoutine { get; private set; }

    public static void UpdateExperiencePoints (int experiencePoints) {
		TotalExperiencePoints += experiencePoints;

		UI.UpdateExperienceText (TotalExperiencePoints);
		UI.UpdateLevelText (Level);
	}

    public static float PlayerPathDistanceMax { get; set; }

    public static float PlayerPathDistance { get; set; }

    public static float Energy {
		get {return (PlayerPathDistanceMax - PlayerPathDistance) > 0.0f ? (PlayerPathDistanceMax - PlayerPathDistance):0.0f;}
	}

    public static int TotalExperiencePoints { get; set; }

    public static int Level { get; set; }

    public static Vector3 PlayerMovingDirection {
		get{return playerMovingDirection;}
	}
	//Determines where player will spawn
	public Vector3 SpawnPoint {
		get {return spawnPoint;}
		set {spawnPoint = value;}
	}
	//Resets Path lengths for Player
	public static void LevelStatsReset(float distanceMax) {
		PlayerPathDistanceMax = distanceMax;
		PlayerPathDistance = 0;
		UI.UpdateEnergyText(Mathf.FloorToInt(Energy));
	}
	//Moves Player across Level
	private IEnumerator PlayerFollowPath() {

        int index = 0;
        Vector3 prevPosition = transform.position;
        Vector3 nextPosition = PathPoints[index];
        Vector3 lastOccupiedPosition = transform.position;

        while (PlayerPathDistance < PlayerPathDistanceMax) {

			while (GameManager.IsPaused) { //for game pause
				yield return null;
			}
            //checks if player has passed nextPosition
            if(Mathf.Abs(Vector3.Distance(transform.position,prevPosition)) >= Mathf.Abs(Vector3.Distance(nextPosition, prevPosition))-0.1)
            {
                transform.position = nextPosition;
                prevPosition = nextPosition;
                index++;
                if (PathPoints.Count > index) {
                    nextPosition = PathPoints[index];
                }
            }
            if (PathPoints.Count > index)
            {
                transform.position = Vector3.MoveTowards(transform.position, nextPosition, GameManager.Speed);
                PlayerPathDistance += Mathf.Abs(Vector3.Distance(transform.position, lastOccupiedPosition));
                lastOccupiedPosition = transform.position;
                UI.UpdateEnergyText(Mathf.FloorToInt(Energy));
            }

			yield return null;
		}
		GameManager.GameOver ();
		yield return null;
	}
	//Moves Player to Spawn Location
	private IEnumerator MoveToSpawn() {
		Vector3 spawn = GameManager.GetCurrentSpawn ().transform.position;
		//For Level Changing
		playerMovingDirection = new Vector3(spawn.x,transform.position.y,spawn.z) - transform.position;

		while (Vector3.Distance(gameObject.transform.position,spawnPoint) > 0.1f) {
			while (GameManager.IsPaused) { //for game pause
				yield return null;
			}
			gameObject.transform.position = Vector3.Lerp(gameObject.transform.position,spawnPoint,Time.deltaTime * 3);
			yield return null;
		}
		while (GameManager.IsPaused) { //for game pause
			yield return null;
		}
		gameObject.transform.position = spawnPoint;
		disablePlayerCollisions = false;
        GameManager.MoveToSpawnState = false;
        yield return null;
	}
    //Returns all Wall collisions in order
    private void UpdateWallCollisions()
    {
        List<Vector3> collisionPoints = new List<Vector3>();

        int floorWidth = 15;
        int floorHeight = 10;

        float distance = PlayerPathDistanceMax - PlayerPathDistance;
        Vector3 forward = new Vector3(
            GameManager.AimArrow().transform.up.x + transform.position.x,
            transform.position.y,
            GameManager.AimArrow().transform.up.z + transform.position.z);

        //y = (rise/run)x + c
        //ax + by + c = 0
        float rise = forward.z - transform.position.z;
        float run = forward.x - transform.position.x;
        float c = transform.position.z - (System.Math.Abs(run) < Mathf.Epsilon ? 0 : ((rise / run) * transform.position.x));
        float slope = System.Math.Abs(run) < Mathf.Epsilon ? 0 : rise / run;

        int x, z, i, j;

        if (rise > 0)
        {
            if (run > 0) { x = floorWidth; z = floorHeight; i = floorWidth; j = floorHeight; }
            else { x = -floorWidth; z = floorHeight; i = 0; j = floorHeight; }
        }
        else
        {
            if (run > 0) { x = floorWidth; z = -floorHeight; i = floorWidth; j = 0; }
            else { x = -floorWidth; z = -floorHeight; i = 0; j = 0; }
        }

        if (System.Math.Abs(rise) > Mathf.Epsilon && System.Math.Abs(run) > Mathf.Epsilon)
        {

            for (int k = i; Mathf.Abs(k) <= distance * Mathf.Abs(Mathf.Abs(forward.x)-Mathf.Abs(transform.position.x)) + floorWidth; k += x)
            {
                collisionPoints.Add(new Vector3(k, transform.position.y, slope * k + c));
            }
            for (int k = j; Mathf.Abs(k) <= distance * Mathf.Abs(Mathf.Abs(forward.z) - Mathf.Abs(transform.position.z)) + floorHeight; k += z)
            {
                collisionPoints.Add(new Vector3((k - c) / slope, transform.position.y, k));
            }
        }
        //orders list of positions by distance from player
        collisionPoints.Sort((v1, v2) => (v1 - transform.position).sqrMagnitude.CompareTo((v2 - transform.position).sqrMagnitude));

        for (int l = 0; l < collisionPoints.Count; l++)
        {
            collisionPoints[l] =
                new Vector3(

                    collisionPoints[l].x >= 0 ?
                    (
                        Mathf.FloorToInt(collisionPoints[l].x / floorWidth) % 2 == 0 ?
                            collisionPoints[l].x % floorWidth :
                            floorWidth - (collisionPoints[l].x % floorWidth)
                    ) :
                    (
                        Mathf.CeilToInt(collisionPoints[l].x / floorWidth) % 2 == 0 ?
                        - (collisionPoints[l].x % floorWidth) :
                        floorWidth + (collisionPoints[l].x % floorWidth)
                    ),
                    collisionPoints[l].y,
                    collisionPoints[l].z >= 0 ?
                    (
                        Mathf.FloorToInt(collisionPoints[l].z / floorHeight) % 2 == 0 ?
                        collisionPoints[l].z % floorHeight :
                        floorHeight - (collisionPoints[l].z % floorHeight)
                    ) :
                    (
                        Mathf.CeilToInt(collisionPoints[l].z / floorHeight) % 2 == 0 ?
                        -(collisionPoints[l].z % floorHeight) :
                        floorHeight + (collisionPoints[l].z % floorHeight)
                    )
                );
        }

        WallCollisionPoints = collisionPoints;
    }
    //Plots path following arrow
    private void UpdateGuidePath() {
        if (WallCollisionPoints.Count > 1) {
            GameManager.PathLine().positionCount = WallCollisionPoints.Count - 1;
            GameManager.PathLine().SetPosition(0, transform.position);
            for (int i = 1; i < GameManager.PathLine().positionCount; i++) {
                GameManager.PathLine().SetPosition(i, WallCollisionPoints[i - 1]);
            }
        }
    }
    //Plots path following arrow
    private void UpdateChosenPath() {
        if (PathPoints.Count > 1) {
            GameManager.PathChosenLine().positionCount = PathPoints.Count - 1;
            GameManager.PathChosenLine().SetPosition(0, transform.position);
            for (int i = 1; i < GameManager.PathChosenLine().positionCount; i++) {
                GameManager.PathChosenLine().SetPosition(i, PathPoints[i - 1]);
            }
        }
    }
}









