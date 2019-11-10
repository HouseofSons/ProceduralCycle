using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

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
	//used to establish max grapple charges
	private static float maxGrappleCharges;
	//used to count grapple charges
	private static float grappleCharges;
	//CoRoutine which moves player
	private static Coroutine playerFollowPathCoRoutine;
	//CoRoutine which moves player to spawn
	private static Coroutine moveToSpawnCoRoutine;
	//Direction Player is moving
	private static Vector3 playerMovingDirection;

	private static bool disablePlayerCollisions;

	void Awake () {
		playerPathDistanceMax = 1000;
		playerPathDistance = 0;
		enemyKillCount = 0;
		totalExperiencePoints = 0;
		level = 1;
		grappleCharges = 3.0f;
		maxGrappleCharges = 3.0f;
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

			if (Input.GetMouseButtonUp(0)) {
				if (!EventSystem.current.IsPointerOverGameObject()) {
					GameManager.AimArrowState = false;
					playerMovingDirection = GameManager.AimArrow().transform.up;
					playerFollowPathCoRoutine = StartCoroutine (PlayerFollowPath (GameManager.Speed, GameManager.AimArrow().transform.up));
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
			} else if (col.gameObject.layer == 9 || col.gameObject.layer == 13) {
				RaycastHit hit;
				Ray ray = new Ray (new Ray (this.transform.position, new Vector3 (-playerMovingDirection.x, -playerMovingDirection.y, -playerMovingDirection.z)).GetPoint (1.0f),
				                   playerMovingDirection.normalized);
				Vector3 normal = Vector3.zero;
				LayerMask layerMask = 1 << LayerMask.NameToLayer ("Wall") | 1 << LayerMask.NameToLayer ("InnerWall");
				if (Physics.SphereCast (ray, 1.0f, out hit, 2.0f, layerMask)) {
					normal = hit.normal;
				}
				playerMovingDirection = Vector3.Reflect (playerMovingDirection, normal);
				MainCamera.PlayerWillHitWall = false;//used to avoid stutters in CameraLeadPlayer method
				StopCoroutine (playerFollowPathCoRoutine);
				playerFollowPathCoRoutine = StartCoroutine (PlayerFollowPath (GameManager.Speed, playerMovingDirection));
			}
		}
	}

	public static Coroutine PlayerFollowPathCoRoutine {
		get{return playerFollowPathCoRoutine;}
	}

	public static void EnemyKilled(int experiencePoints) {

		float unitGrappleCharge = 0.5f;

		UpdateExperiencePoints (experiencePoints);
		enemyKillCount += 1;
		if (grappleCharges + unitGrappleCharge < maxGrappleCharges) {
			grappleCharges += unitGrappleCharge;
		}
		UI.UpdateGrappleText (Mathf.FloorToInt(grappleCharges),Mathf.FloorToInt(maxGrappleCharges));
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
	
	public static float MaxGrappleCharges {
		
		get{return maxGrappleCharges;}
		set{maxGrappleCharges = value;}
	}
	
	public static float GrappleCharges {

		get{return grappleCharges;}
		set{grappleCharges = value;}
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
		float yPosition = this.gameObject.transform.position.y;
		float yPositionPrev = this.gameObject.transform.position.y;
		float drag = 1.0f;

		while (playerPathDistanceMax > playerPathDistance) {

			while (GameManager.IsPaused) { //for game pause
				yield return null;
			}

			yPosition = GameManager.YPositionPlayer(this.gameObject.transform.position);

			playerPathDistance += Vector3.Distance (start, this.gameObject.transform.position);
			UI.UpdateEnergyText(Mathf.FloorToInt(Player.Energy));
			start = this.gameObject.transform.position;

			drag = SpeedDrag(speed,Mathf.Abs(yPosition-yPositionPrev));

			this.transform.position = new Vector3(
				rayAimed.GetPoint(drag).x,
				yPosition,
				rayAimed.GetPoint(drag).z);
			yPositionPrev = yPosition;
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
}









