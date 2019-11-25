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

	private static bool disablePlayerCollisions;
    public static List<Vector3> WallCollisionPoints;

	void Awake () {
		playerPathDistanceMax = 1000;
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
			GameManager.MoveToSpawnState = false;
			if (moveToSpawnCoRoutine != null) {
				StopCoroutine(moveToSpawnCoRoutine);
			}
			moveToSpawnCoRoutine = StartCoroutine (MoveToSpawn());
		} 

		if (GameManager.AimArrowState) {
            UpdateWallCollisions();
            PlotGuidePath();
            if (Input.GetMouseButtonUp(0)) {
                if (!EventSystem.current.IsPointerOverGameObject()) {
                    GameManager.PlayerMovingState = true;
                    playerMovingDirection = GameManager.AimArrow().transform.up;
                    if (playerFollowPathCoRoutine != null)
                    {
                        StopCoroutine(playerFollowPathCoRoutine);
                    }
                    playerFollowPathCoRoutine = StartCoroutine (PlayerFollowPath (GameManager.Speed, GameManager.AimArrow().transform.up));
                    //PlotPath();
                }
			}
		}
	}

	void OnTriggerEnter(Collider col) {

		if(!disablePlayerCollisions) {
			if (col.gameObject.name.StartsWith ("Door")) {
				disablePlayerCollisions = true;
				StopCoroutine (playerFollowPathCoRoutine);//stops player movement if Door Hit
				GameManager.DoorHit (col.transform.name);
			} else if (col.gameObject.layer == 9) {
                //TO BE REMOVED WITH NEW LOGIC!!!
				RaycastHit hit;
				Ray ray = new Ray (new Ray (this.transform.position, new Vector3 (-playerMovingDirection.x, -playerMovingDirection.y, -playerMovingDirection.z)).GetPoint (2.0f),
				                   playerMovingDirection.normalized);
				Vector3 normal = Vector3.zero;
				LayerMask layerMask = 1 << LayerMask.NameToLayer ("Wall");
                if (Physics.SphereCast (ray, 1.0f, out hit, 2.0f, layerMask)) {
					normal = hit.normal;
				}
				playerMovingDirection = Vector3.Reflect (playerMovingDirection, normal);
				StopCoroutine (playerFollowPathCoRoutine);
				playerFollowPathCoRoutine = StartCoroutine (PlayerFollowPath (GameManager.Speed, playerMovingDirection));
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
	private IEnumerator PlayerFollowPath(float speed,Vector3 aimedDirection) {

		Ray rayAimed = new Ray (this.gameObject.transform.position,aimedDirection);
		Vector3 start = this.gameObject.transform.position;

		while (playerPathDistanceMax > playerPathDistance) {

			while (GameManager.IsPaused) { //for game pause
				yield return null;
			}

			playerPathDistance += Vector3.Distance (start, this.gameObject.transform.position);
			UI.UpdateEnergyText(Mathf.FloorToInt(Player.Energy));
			start = this.gameObject.transform.position;

			this.transform.position = new Vector3(
                rayAimed.GetPoint(speed).x,
				this.transform.position.y,
				rayAimed.GetPoint(speed).z);
			rayAimed.origin = this.transform.position;
			yield return null;
		}
		GameManager.GameOver ();
		yield return null;
	}
	//Adjust XZ plane speed based player Y change
	private float SpeedDrag(float speed, float yDelta) {
		return speed * (speed / (Mathf.Sqrt (Mathf.Pow (speed, 2) + Mathf.Pow (yDelta, 2))));
	}
	//Moves Player to Spawn Location
	private IEnumerator MoveToSpawn() {
		float smoothing = 0.5f;
		Vector3 spawn = GameManager.GetCurrentSpawn ().transform.position;
		//For Level Changing
		playerMovingDirection = new Vector3(spawn.x,this.transform.position.y,spawn.z) - this.transform.position;

		while (Vector3.Distance(this.gameObject.transform.position,spawnPoint) > 0.1f) {
			while (GameManager.IsPaused) { //for game pause
				yield return null;
			}
			this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position,spawnPoint,Time.deltaTime * smoothing);
			smoothing+=0.5f;
			yield return null;
		}
		while (GameManager.IsPaused) { //for game pause
			yield return null;
		}
		this.gameObject.transform.position = spawnPoint;
		disablePlayerCollisions = false;
		yield return null;
	}
    //Returns all Wall collisions in order
    private void UpdateWallCollisions()
    {
        List<Vector3> collisionPoints = new List<Vector3>();

        int floorWidth = 10;
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
            for (int k = i; Mathf.Abs(k) < distance; k += x)
            {
                collisionPoints.Add(new Vector3(k, this.transform.position.y, slope * k + c));
            }
            for (int k = j; Mathf.Abs(k) < distance; k += z)
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
    private void PlotGuidePath()
    {

    }
}









