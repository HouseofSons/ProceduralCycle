using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {

	private static GameObject experienceText;
	private static GameObject levelText;
	private static GameObject energyText;

	// Use this for initialization
	void Start () {
		energyText = GameObject.Find ("EnergyValue");
		experienceText = GameObject.Find ("ExperienceValue");
		levelText = GameObject.Find ("LevelValue");
	}
	
	public static void InitializeUIWithPlayerInfo() {
		energyText.GetComponent<Text> ().text = Player.Energy.ToString();
		experienceText.GetComponent<Text> ().text = Player.TotalExperiencePoints.ToString();
	}

	public static void UpdateEnergyText(int energy) {
		energyText.GetComponent<Text> ().text = energy.ToString();
	}
	
	public static void UpdateExperienceText(int experience) {
		experienceText.GetComponent<Text> ().text = experience.ToString();
	}
	
	public static void UpdateLevelText(int level) {
		levelText.GetComponent<Text> ().text = level.ToString();
	}
}
