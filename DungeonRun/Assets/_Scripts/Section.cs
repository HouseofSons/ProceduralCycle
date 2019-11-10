using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Section : MonoBehaviour {

	private static GameObject[] sections;
	private static int mostRecentSectionIndex;
	
	void Update () {
		if (int.Parse(GameManager.CurrentSection.gameObject.name.Substring(7)) != mostRecentSectionIndex) {
			UpdateVisibleSections();
		}
	}
	
	public static int MostRecentSectionIndex {
		get {return mostRecentSectionIndex;}
		set {mostRecentSectionIndex = value;}
	}
	
	public static void InitializeSectionsOfCurrentLevel() {
		
		List<GameObject> tempList = new List<GameObject>();
		
		foreach(Transform child in GameManager.GetCurrentLevel().transform) {
			if (child.name.StartsWith("Section")) {
				tempList.Add(child.gameObject);
			}
		}
		
		if (tempList.Count > 0) {
			sections = new GameObject[tempList.Count];
		}
		
		foreach(GameObject go in tempList) {
			sections[int.Parse(go.name.Substring(7))] = go;
			if (int.Parse(go.name.Substring(7)) == tempList.Count - 1) {
				GameManager.CurrentSection = go;//Initializes the top floor of the level
			}
		}
		
		mostRecentSectionIndex = int.Parse(GameManager.CurrentSection.gameObject.name.Substring(7));
	}
	
	private static void UpdateVisibleSections() {
		mostRecentSectionIndex = int.Parse(GameManager.CurrentSection.gameObject.name.Substring(7));
		EnableRamps(mostRecentSectionIndex);
		for (int i = 0;i<sections.Length;i++) {
			if (mostRecentSectionIndex >= i) {
				sections[i].gameObject.SetActive(true);
			} else {
				sections[i].gameObject.SetActive(false);
			}
		} 
	}
	
	private static void EnableRamps(int sectionIndex) {
		foreach(Transform child in GameManager.GetCurrentLevel().transform) {
			if(child.name.StartsWith("Ramp")) {
				if (int.Parse(child.name.Substring(4,2)) == sectionIndex || int.Parse(child.name.Substring(6,2)) == sectionIndex) {
					child.gameObject.SetActive(true);
				} else {
					child.gameObject.SetActive(false);
				}
			}
		}
	}
	
	public static void LoadSections(GameObject ramp) {
		sections[int.Parse(ramp.name.Substring(4,2))].SetActive(true);
		sections[int.Parse(ramp.name.Substring(6,2))].SetActive(true);
	}
}

























