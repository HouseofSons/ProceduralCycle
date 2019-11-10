using UnityEngine;
using System.Collections;

public class PopulateCharacters: MonoBehaviour {

	public static void PopulateCharactersOnMap (int friendlyCharacterCount, int enemyCharacterCount, GameBoardHex[,,] terrainBlocks) {
		//Logic needs to be added to Populate specific Character Info from a Character data object
		for (int i = 0; i < terrainBlocks.GetUpperBound(0); i++) {
			for (int j = 0; j < terrainBlocks.GetUpperBound(1); j++) {
				for (int k = 0; k < terrainBlocks.GetUpperBound(2); k++) {
					if (terrainBlocks [i,j,k] != null) {
						if (friendlyCharacterCount > 0) {
							GameObject character = Instantiate(Resources.Load("FriendlyCharacter")) as GameObject;
							character.GetComponent<Character>().SetCharacterInfo (terrainBlocks [i,j,k], i, j, k, true);
							friendlyCharacterCount--;
						} else {
							break;
						}
					}
					if (friendlyCharacterCount <= 0) {
						break;
					}
				}
				if (friendlyCharacterCount <= 0) {
					break;
				}
			}
			if (friendlyCharacterCount <= 0) {
				break;
			}
		}

		//Logic needs to be added to Populate specific Character Info from a Character data object
		for (int i = terrainBlocks.GetUpperBound(0); i >= 0; i--) {
			for (int j = 0; j < terrainBlocks.GetUpperBound(1); j++) {
				for (int k = 0; k < terrainBlocks.GetUpperBound(2); k++) {
					if (terrainBlocks [i,j,k] != null) {
						if (enemyCharacterCount > 0) {
							GameObject character = Instantiate(Resources.Load("EnemyCharacter")) as GameObject;
							character.GetComponent<Character>().SetCharacterInfo (terrainBlocks [i,j,k], i, j, k, false);
							enemyCharacterCount--;
						} else {
							break;
						}
						if (enemyCharacterCount <= 0) {
							break;
						}
					}
					if (enemyCharacterCount <= 0) {
						break;
					}
				}
				if (enemyCharacterCount <= 0) {
					break;
				}
			}
			if (enemyCharacterCount <= 0) {
				break;
			}
		}
	}
}
