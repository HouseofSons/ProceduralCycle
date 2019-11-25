using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	private int health;
	private int level;
	private int experiencePoints;
	private Coroutine moveEnemyCoRoutine;

	// Use this for initialization
	void Awake () {
		health = 1;
		level = 1;
		experiencePoints = 10;
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.EnableEnemyMovement) {
			if (moveEnemyCoRoutine == null) {
				moveEnemyCoRoutine = StartCoroutine(MoveEnemy());
				if (AllEnemiesMoving()) {
					GameManager.EnableEnemyMovement = false;
				}
			}
		}
	}

	void OnTriggerEnter(Collider col) {
		if (col.gameObject == GameManager.GetCurrentPlayer()) {
			Attacked(1); //Need to add more Logic Here
		}
	}

	public int ExperiencePoints {
		get {return experiencePoints;}
	}
	
	private static bool AllEnemiesMoving() {
		foreach(GameObject enemy in Level.LevelEnemies) {
			if (enemy.GetComponent<Enemy>().moveEnemyCoRoutine == null) {
				return false;
			}
		}
		return true;
	}
	
	public static void UpdateEnemiesDifficultyColor() {

		foreach(GameObject enemy in Level.LevelEnemies) {

			int levelDiff = Player.Level - enemy.GetComponent<Enemy>().level;

			if (levelDiff < 0) {
				enemy.GetComponent<MeshRenderer>().material.SetColor("_OutlineColor",Color.red);
			} else if (levelDiff == 0) {
				enemy.GetComponent<MeshRenderer>().material.SetColor("_OutlineColor",Color.green);
			} else {
				enemy.GetComponent<MeshRenderer>().material.SetColor("_OutlineColor",Color.gray);
			}
		}
	}

	//Moves Enemy across Level
	private IEnumerator MoveEnemy() {
		Ray rayAimed = new Ray(this.transform.position,NewEnemyPosition (this.transform.position) - this.transform.position);
		RaycastHit hit;
		Vector3 nextPosition = rayAimed.GetPoint (4.0f);

		LayerMask layerMask = 1<<LayerMask.NameToLayer("Wall") | 1<<LayerMask.NameToLayer("InnerWall");

		while (true) {
			while (!Physics.Raycast (rayAimed, out hit,4.1f,layerMask)) {
				while (Vector3.Distance(this.transform.position,new Vector3(nextPosition.x,this.transform.position.y,nextPosition.z))>0.5f) {
					while (GameManager.IsPaused) { //for game pause
						yield return null;
					}
					this.transform.position = Vector3.Lerp(this.transform.position,nextPosition,0.02f);
					this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
					yield return null;
				}
				while (GameManager.IsPaused) { //for game pause
					yield return null;
				}
				this.transform.position = nextPosition;
				this.transform.position = new Vector3(nextPosition.x, this.transform.position.y, nextPosition.z);
				rayAimed.origin = this.transform.position;
				rayAimed.direction = NewEnemyPosition (this.transform.position) - this.transform.position;
				nextPosition = rayAimed.GetPoint (4.0f);
				yield return null;
			}
			rayAimed.direction = NewEnemyPosition (this.transform.position) - this.transform.position;
			nextPosition = rayAimed.GetPoint (4.0f);
		}
	}

	private Vector3 NewEnemyPosition(Vector3 position) {

		switch (Mathf.FloorToInt (Random.Range (0, 4))) {
		case 0:
			return new Vector3(position.x,position.y,position.z+2.0f);
		case 1:
			return new Vector3(position.x+2.0f,position.y,position.z);
		case 2:
			return new Vector3(position.x,position.y,position.z-2.0f);
		case 3:
			return new Vector3(position.x-2.0f,position.y,position.z);
		}
		return position;
	}

	public void Attacked(int damage) {
		health -= damage;
		if (health <= 0) {
			Player.EnemyKilled(experiencePoints);
			Level.RemoveEnemy(this.gameObject);
			StopCoroutine(moveEnemyCoRoutine);
			StartCoroutine(DestroyMe());
		}
	}

	private IEnumerator DestroyMe() {
		float smoothing = 0.01f;
		while (this.transform.localScale.x < 5.0f) {
			while (GameManager.IsPaused) { //for game pause
				yield return null;
			}
			this.transform.localScale = new Vector3(this.transform.localScale.x + smoothing*Time.deltaTime,
			                                        this.transform.localScale.y,
			                                        this.transform.localScale.z + smoothing*Time.deltaTime);
			smoothing += 0.1f;
			yield return null;
		}
		GameObject.Destroy (this.gameObject);
		yield return null;
	}
}















