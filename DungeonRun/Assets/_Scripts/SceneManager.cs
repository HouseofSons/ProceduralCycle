using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour {

	void Start () {
		//Splash Screen Only
		if (Application.loadedLevel == 0) {
			Invoke ("LoadNextScene",3);
		}
	}

	public void LoadScene(int SceneIndex) {
		PlayerPrefsManager.UnlockLevel (SceneIndex);
		Application.LoadLevel (SceneIndex);
	}
	
	public void LoadNextScene() {
		PlayerPrefsManager.UnlockLevel (Application.loadedLevel + 1);
		Application.LoadLevel (Application.loadedLevel + 1);
	}
	
	public void LoadCurrentScene() {
		Time.timeScale = 1;
		GameManager.IsPaused = false;
		Application.LoadLevel (Application.loadedLevel );
	}

	public void LoadHighestSceneReached() {
		Application.LoadLevel (PlayerPrefsManager.GetHighestLevelUnlocked ());	
	}
}
