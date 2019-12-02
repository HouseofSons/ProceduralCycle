using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	
	//used to determine current spawn location
	private Vector3 spawnPoint;
	//used to determine players current experience level
	private static int level;
	//used to determine the distance the player can pass
	private static float playerPathDistanceMax;
	//used to determine the distance the player has currently moved
	private static float playerPathDistance;
	//used to determine enemy killed count
	private static int totalExperiencePoints;
	//used to determine enemy killed count
	private static int enemyKillCount;

	//CoRoutine which moves player
	private static Coroutine playerFollowPathCoRoutine;
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
		playerPathDistanceMax = 100;
		playerPathDistance = 0;
		enemyKillCount = 0;
		totalExperiencePoints = 0;
		level = 1;
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
                    if (playerFollowPathCoRoutine != null)
                    {
                        StopCoroutine(playerFollowPathCoRoutine);
                    }
                    playerFollowPathCoRoutine = StartCoroutine (PlayerFollowPath ());
                    UpdateChosenPath();
                }
			}
		}
	}

	void OnTriggerEnter(Collider col) {

		if(!disablePlayerCollisions) {
            if (col.gameObject.name.StartsWith("Door")) {
                disablePlayerCollisions = true;
                StopCoroutine(playerFollowPathCoRoutine);//stops player movement if Door Hit
                GameManager.DoorHit(col.transform.name);
            }
		}
	}

	public static Coroutine PlayerFollowPathCoRoutine {
		get{return playerFollowPathCoRoutine;}
	}

	public static void EnemyKilled(int experiencePoints) {
		UpdateExperiencePoints (experiencePoints);
		enemyKillCount += 1;
	}

	public static void UpdateExperiencePoints (int experiencePoints) {
		totalExperiencePoints += experiencePoints;

		int prevLevel = level;

		if (totalExperiencePoints >= 50) {
			level = 2;
		} else if (totalExperiencePoints >= 150) {
			level = 3;
		} else if (totalExperiencePoints >= 225) {
			level = 4;
		} else if (totalExperiencePoints >= 340) {
			level = 5;
		} else if (totalExperiencePoints >= 500) {
			level = 6;
		} else {
			level = 1;
		}

		if (prevLevel != level) {
			Enemy.UpdateEnemiesDifficultyColor ();
		}

		UI.UpdateExperienceText (totalExperiencePoints);
		UI.UpdateLevelText (level);
	}

	public static float PlayerPathDistanceMax {
		
		get{return playerPathDistanceMax;}
		set{playerPathDistanceMax = value;}
	}
	
	public static float PlayerPathDistance {
		
		get{return playerPathDistance;}
		set{playerPathDistance = value;}
	}

	public static float Energy {
		get {return (playerPathDistanceMax - playerPathDistance) > 0.0f ? (playerPathDistanceMax - playerPathDistance):0.0f;}
	}
	
	public static int TotalExperiencePoints {
		
		get{return totalExperiencePoints;}
		set{totalExperiencePoints = value;}
	}
	
	public static int Level {
		
		get{return level;}
		set{level = value;}
	}

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
		playerPathDistanceMax = distanceMax;
		playerPathDistance = 0;
		UI.UpdateEnergyText(Mathf.FloorToInt(Player.Energy));
	}
	//Moves Player across Level
	private IEnumerator PlayerFollowPath() {

        int index = 0;
        Vector3 prevPosition = this.transform.position;
        Vector3 nextPosition = PathPoints[index];
        Vector3 lastOccupiedPosition = this.transform.position;

        while (playerPathDistance < playerPathDistanceMax) {

			while (GameManager.IsPaused) { //for game pause
				yield return null;
			}
            //checks if player has passed nextPosition
            if(Mathf.Abs(Vector3.Distance(this.transform.position,prevPosition)) >= Mathf.Abs(Vector3.Distance(nextPosition, prevPosition))-0.1)
            {
                this.transform.position = nextPosition;
                prevPosition = nextPosition;
                index++;
                if (PathPoints.Count > index) {
                    nextPosition = PathPoints[index];
                }
            }
            if (PathPoints.Count > index)
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, nextPosition, GameManager.Speed);
                playerPathDistance += Mathf.Abs(Vector3.Distance(this.transform.position, lastOccupiedPosition));
                lastOccupiedPosition = this.transform.position;
                UI.UpdateEnergyText(Mathf.FloorToInt(Player.Energy));
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
		playerMovingDirection = new Vector3(spawn.x,this.transform.position.y,spawn.z) - this.transform.position;

		while (Vector3.Distance(this.gameObject.transform.position,spawnPoint) > 0.1f) {
			while (GameManager.IsPaused) { //for game pause
				yield return null;
			}
			this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position,spawnPoint,Time.deltaTime * 3);
			yield return null;
		}
		while (GameManager.IsPaused) { //for game pause
			yield return null;
		}
		this.gameObject.transform.position = spawnPoint;
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

        float distance = playerPathDistanceMax - playerPathDistance;
        Vector3 forward = new Vector3(GameManager.AimArrow().transform.up.x + this.transform.position.x, this.transform.position.y, GameManager.AimArrow().transform.up.z + this.transform.position.z);

        //y = (rise/run)x + c
        //ax + by + c = 0
        float rise = forward.z - this.transform.position.z;
        float run = forward.x - this.transform.position.x;
        float c = this.transform.position.z - (System.Math.Abs(run) < Mathf.Epsilon ? 0 : ((rise / run) * this.transform.position.x));
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

            for (int k = i; Mathf.Abs(k) <= distance * Mathf.Abs(Mathf.Abs(forward.x)-Mathf.Abs(this.transform.position.x)) + floorWidth; k += x)
            {
                collisionPoints.Add(new Vector3(k, this.transform.position.y, slope * k + c));
            }
            for (int k = j; Mathf.Abs(k) <= distance * Mathf.Abs(Mathf.Abs(forward.z) - Mathf.Abs(this.transform.position.z)) + floorHeight; k += z)
            {
                collisionPoints.Add(new Vector3((k - c) / slope, this.transform.position.y, k));
            }
        }
        //orders list of positions by distance from player
        collisionPoints.Sort((v1, v2) => (v1 - this.transform.position).sqrMagnitude.CompareTo((v2 - this.transform.position).sqrMagnitude));

        for (int l = 0; l < collisionPoints.Count; l++)
        {
            collisionPoints[l] =
                new Vector3(

                    collisionPoints[l].x >= 0 ?
                    (
                        Mathf.FloorToInt(collisionPoints[l].x / floorWidth) % 2 == 0 ? collisionPoints[l].x % floorWidth : floorWidth - (collisionPoints[l].x % floorWidth)
                    ) :
                    (
                        Mathf.CeilToInt(collisionPoints[l].x / floorWidth) % 2 == 0 ? - (collisionPoints[l].x % floorWidth) : floorWidth + (collisionPoints[l].x % floorWidth)
                    ),
                    collisionPoints[l].y,
                    collisionPoints[l].z >= 0 ?
                    (
                        Mathf.FloorToInt(collisionPoints[l].z / floorHeight) % 2 == 0 ? collisionPoints[l].z % floorHeight : floorHeight - (collisionPoints[l].z % floorHeight)
                    ) :
                    (
                        Mathf.CeilToInt(collisionPoints[l].z / floorHeight) % 2 == 0 ? -(collisionPoints[l].z % floorHeight) : floorHeight + (collisionPoints[l].z % floorHeight)
                    )
                );
        }

        WallCollisionPoints = collisionPoints;
    }
    //Plots path following arrow
    private void UpdateGuidePath() {
        if (WallCollisionPoints.Count > 1) {
            GameManager.PathLine().positionCount = WallCollisionPoints.Count - 1;
            GameManager.PathLine().SetPosition(0, this.transform.position);
            for (int i = 1; i < GameManager.PathLine().positionCount; i++) {
                GameManager.PathLine().SetPosition(i, WallCollisionPoints[i - 1]);
            }
        }
    }
    //Plots path following arrow
    private void UpdateChosenPath() {
        if (PathPoints.Count > 1) {
            GameManager.PathChosenLine().positionCount = PathPoints.Count - 1;
            GameManager.PathChosenLine().SetPosition(0, this.transform.position);
            for (int i = 1; i < GameManager.PathChosenLine().positionCount; i++) {
                GameManager.PathChosenLine().SetPosition(i, PathPoints[i - 1]);
            }
        }
    }
}









