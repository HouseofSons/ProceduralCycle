using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProcessLevel : MonoBehaviour {
	
	private static int atStageNumber;
	private static int focusAttacksLeft;
	private static int focusMovesLeft;
	private static bool friendlyTurn;
	
	// Use this for initialization
	void Awake () {
		//regulates when updates functions are called
		atStageNumber = 1;
	}

	void Update() {
		//For Debugging DrawRay Example
		//Debug.DrawRay (new Vector3(40.8f,62f,55.6f),transform.TransformDirection(new Vector3(-50.3f,-55f,-27.1f)).normalized*78.9767f);

		if (atStageNumber == 1) {
			if (HexPrismChunk.AllChunksUpdated()){
				Debug.Log ("Hex World Terrain Complete!!");
				atStageNumber = 2;
			}
		}

		if (atStageNumber == 2) {

            Debug.Log("Populating Terrain with Setting");
            //populate GameBoardHexs with Setting choice
            Setting.PopulateSettingOnWorldTerrain(); 
            Debug.Log("Setting Populated!!");

            Debug.Log ("Populating Terrain with Characters");
			//populate players (Logic needs to be added to Populate specific Player Info from a Player data object)!!!!!!!should pass arrays of friendlies and enemies
			PopulateCharacters.PopulateCharactersOnMap (3, 3,GameBoardHexGrid.GameBoard());
			Debug.Log ("Characters Populated!!");

            Debug.Log ("Aligning Order of Characters");
			//Populate CharacterList for gameplay Updates
			Character.CharacterList ();
			Debug.Log ("Characters aligned and first Character Queued Up!!");
			
			Debug.Log ("Setting Up Character Pathing");
			//Populate CharacterList for gameplay Updates
			Pathing.CharacterPaths(); 	
			Debug.Log ("Pathing for all Characters is Set!!");

			GameBoardHex.GameBoardEnabled = true;

			StartCoroutine (GameCamera.SetCameraToObject()/*SetCameraToOrigin()*/);

			atStageNumber = 3;

			//Loop until no players || no enemies left
			
				//choose character by turn
				//character Moves and Attacks
				//spawn fight scene(s) if needed
				//character move done
			
			//if no Players
			
				//Continue Screen
			
			//if no Enemies
			
				//Victory Screen
				//Return to World Map
		}
	}

	public static int GetStageAt () {
		return atStageNumber;
	}

	public static void SetStageAt(int stage) {
		atStageNumber = stage;
	}

	public static void SetFriendlyTurn (bool friendly) {
		friendlyTurn = friendly;
	}

	public static bool GetFriendlyTurn () {
		return friendlyTurn;
	}
}








