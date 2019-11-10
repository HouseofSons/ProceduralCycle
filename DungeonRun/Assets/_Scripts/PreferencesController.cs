using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PreferencesController : MonoBehaviour {

	public SceneManager sceneManager;
	public Slider volumeSlider;

	void Start () {
		volumeSlider.value = PlayerPrefsManager.GetMasterVolume ();
	}

	public void SaveAndExit () {
		PlayerPrefsManager.SetMasterVolume (volumeSlider.value);
		sceneManager.LoadScene (1);
	}
}
