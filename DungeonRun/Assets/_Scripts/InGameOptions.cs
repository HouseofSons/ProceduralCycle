using UnityEngine;

public class InGameOptions : MonoBehaviour {

	private GameObject OptionsMenu;
	private GameObject OptionsButton;
	private GameObject GameOverMenu;

	// Use this for initialization
	void Start () {
		OptionsMenu = GameObject.Find ("Options Background");
		GameOverMenu = GameObject.Find ("GameOverBackground");
		OptionsButton = GameObject.Find ("Options");
		OptionsMenu.SetActive (false);
		GameOverMenu.SetActive (false);
	}

	public void ShowOptionsMenu() {
		Time.timeScale = 0;
		GameManager.IsPaused = true;
		OptionsMenu.SetActive (true);
		OptionsButton.SetActive (false);
	}
	
	public void HideOptionsMenu() {
		Time.timeScale = 1;
		GameManager.IsPaused = false;
		OptionsMenu.SetActive (false);
		OptionsButton.SetActive (true);
	}

	public void ShowGameOverMenu() {
		Time.timeScale = 0;
		GameManager.IsPaused = true;
		GameOverMenu.SetActive (true);
		OptionsButton.SetActive (false);
	}

	public void HideGameOverMenu()
	{
		Time.timeScale = 1;
		GameManager.IsPaused = false;
		GameOverMenu.SetActive(false);
		OptionsButton.SetActive(true);
	}

	public void RestartLevel()
	{
		GameManager.GameStart();
		HideOptionsMenu();
		HideGameOverMenu();
	}
}
