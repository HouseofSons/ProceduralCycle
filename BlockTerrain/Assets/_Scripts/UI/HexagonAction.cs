using UnityEngine;
using System.Collections;

public class HexagonAction : MonoBehaviour {
	
	HexagonButton hexButton;
	
	[Range(-1,1)]
	public int hexRole;
	
	void Start () {
		GameObject levelManager = GameObject.Find ("LevelManager");
		hexButton = levelManager.GetComponent ("HexagonButton") as HexagonButton;
	}
	
	void Update () {
	
		if (hexButton.GetClickedHex() == 2) {
			if (hexRole == 1) {
				if (hexButton.GetAttackButtonHexFocused()) {
					this.transform.Rotate(Vector3.back * 15);
					this.transform.localScale = new Vector3(100,10,100);
					this.transform.position = new Vector3(0,300,-300);
				} else {
					this.transform.rotation = Quaternion.Lerp (this.transform.rotation,Quaternion.Euler(90, 0, 0),Time.deltaTime * 5);
					this.transform.localScale = Vector3.Lerp (this.transform.localScale,new Vector3(65,5,65),Time.deltaTime * 5);
					this.transform.position = Vector3.Lerp (this.transform.position,new Vector3(0,260,-200),Time.deltaTime * 5);
				}
			} else if (hexRole == -1) {
				if (hexButton.GetDefendButtonHexFocused()) {
					this.transform.Rotate(Vector3.back * 15);
					this.transform.localScale = new Vector3(100,10,100);
					this.transform.position = new Vector3(-90,140,-300);
				} else {
					this.transform.rotation = Quaternion.Lerp (this.transform.rotation,Quaternion.Euler(90, 0, 0),Time.deltaTime * 5);
					this.transform.localScale = Vector3.Lerp (this.transform.localScale,new Vector3(65,5,65),Time.deltaTime * 5);
					this.transform.position = Vector3.Lerp (this.transform.position,new Vector3(-58,160,-200),Time.deltaTime * 5);	
				}
			} else if (hexRole == 0) {
				if (hexButton.GetSustainButtonHexFocused()) {
					this.transform.Rotate(Vector3.back * 15);
					this.transform.localScale = new Vector3(100,10,100);
					this.transform.position = new Vector3(90,140,-300);
				} else {
					this.transform.rotation = Quaternion.Lerp (this.transform.rotation,Quaternion.Euler(90, 0, 0),Time.deltaTime * 5);
					this.transform.localScale = Vector3.Lerp (this.transform.localScale,new Vector3(65,5,65),Time.deltaTime * 5);
					this.transform.position = Vector3.Lerp (this.transform.position,new Vector3(58,160,-200),Time.deltaTime * 5);
				}
			}
		} else {
			if (hexButton.GetClickedHex() == 1) {
				if (hexRole == 1) {
					this.transform.Rotate(Vector3.back * 15);
					this.transform.localScale = Vector3.Lerp (this.transform.localScale,new Vector3(130,10,130),Time.deltaTime * 5);
					this.transform.position = Vector3.Lerp (this.transform.position,new Vector3(GameObject.Find ("BackgroundHex").transform.position.x,GameObject.Find ("BackgroundHex").transform.position.y,-500),Time.deltaTime * 5);
				} else {
					this.transform.position = new Vector3(this.transform.position.x,this.transform.position.y,100);
				}
			} else if (hexButton.GetClickedHex() == -1) {
				if (hexRole == -1) {
					this.transform.Rotate(Vector3.back * 15);
					this.transform.localScale = Vector3.Lerp (this.transform.localScale,new Vector3(130,10,130),Time.deltaTime * 5);
					this.transform.position = Vector3.Lerp (this.transform.position,new Vector3(GameObject.Find ("BackgroundHex").transform.position.x,GameObject.Find ("BackgroundHex").transform.position.y,-500),Time.deltaTime * 5);
				} else {
					this.transform.position = new Vector3(this.transform.position.x,this.transform.position.y,100);
				}
			} else if (hexButton.GetClickedHex() == 0) {
				if (hexRole == 0) {
					this.transform.Rotate(Vector3.back * 15);
					this.transform.localScale = Vector3.Lerp (this.transform.localScale,new Vector3(130,10,130),Time.deltaTime * 5);
					this.transform.position = Vector3.Lerp (this.transform.position,new Vector3(GameObject.Find ("BackgroundHex").transform.position.x,GameObject.Find ("BackgroundHex").transform.position.y,-500),Time.deltaTime * 5);
				} else {
					this.transform.position = new Vector3(this.transform.position.x,this.transform.position.y,100);
				}
			}
		}
	}
}







