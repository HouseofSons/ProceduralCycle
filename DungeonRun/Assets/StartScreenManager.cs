using UnityEngine;
using System.Collections;

public class StartScreenManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log (PlayerPrefsManager.GetHighestLevelUnlocked());
		if (PlayerPrefsManager.GetHighestLevelUnlocked() < 4) {
			GameObject.Find ("Continue").SetActive(false);
		}
	}
}
