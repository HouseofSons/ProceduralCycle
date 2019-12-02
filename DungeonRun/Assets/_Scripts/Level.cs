using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour {
	
	private static List<GameObject> levelEnemies;

	void Start () {
		NewLevelEnemyCheck ();
		Enemy.UpdateEnemiesDifficultyColor ();
	}

	void Update () {
		if (GameManager.ChangeLevel) {
			GameManager.ChangeLevel = false;
			NewLevelEnemyCheck();
			Enemy.UpdateEnemiesDifficultyColor();
		}
	}

	public static List<GameObject> LevelEnemies {
		get {return levelEnemies;}
	}

	public static void RemoveEnemy (GameObject enemy) {
		levelEnemies.Remove (enemy);
	}
	
	private void NewLevelEnemyCheck() {
		levelEnemies = new List<GameObject> ();
		foreach(Transform child in GameManager.GetCurrentLevel().transform) {
			foreach(Transform stepChild in child) {
				if (stepChild.name.StartsWith("Enemy")) {
					levelEnemies.Add(stepChild.gameObject);
				}
			}
		}
	}
}
