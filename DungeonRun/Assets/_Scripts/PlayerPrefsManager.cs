using UnityEngine;
using System.Collections;

public class PlayerPrefsManager : MonoBehaviour {

	const string MASTER_VOLUME_KEY = "master_volume";
	const string LEVEL_KEY = "level_unlocked_";
	const string HIGHEST_LEVEL_KEY = "highest_level_unlocked";

	public static void SetMasterVolume(float volume) {
		if (volume >= 0f || volume <= 1f) {
			PlayerPrefs.SetFloat (MASTER_VOLUME_KEY, volume);
		} else {
			Debug.LogError ("Master Volume out of range");
		}
	}

	public static float GetMasterVolume() {
		return PlayerPrefs.GetFloat (MASTER_VOLUME_KEY);
	}

	public static void UnlockLevel(int level) {
		if (level >= 0 && level <= Application.levelCount - 1) {
			PlayerPrefs.SetInt (LEVEL_KEY + level.ToString (), level);

			if (PlayerPrefs.HasKey(HIGHEST_LEVEL_KEY)) {
				if (GetHighestLevelUnlocked() < level) {
					PlayerPrefs.SetInt (HIGHEST_LEVEL_KEY,level);
				}
			} else {
				PlayerPrefs.SetInt (HIGHEST_LEVEL_KEY,level);
			}
		} else {
			Debug.LogError ("Trying to unlock level which doesn't exist");
		}
	}
	
	public static bool IsLevelUnlocked(int level) {
		if (level >= 0 && level <= Application.levelCount - 1) {
			return PlayerPrefs.HasKey (LEVEL_KEY + level.ToString ());
		} else {
			Debug.LogError ("Trying to unlock level which doesn't exist");
			return false;
		}
	}

	public static int GetHighestLevelUnlocked() {
		if (PlayerPrefs.HasKey (HIGHEST_LEVEL_KEY)) {
			return PlayerPrefs.GetInt (HIGHEST_LEVEL_KEY);
		} else {
			return -1;
		}
	}
}
















