using UnityEngine;
using System.Collections;

public class MiniMap : MonoBehaviour {

	private MainCharacter character;

	void Awake() {
		character = GameObject.FindWithTag ("Player_0" + MainCharacter.PlayerCount ()).GetComponent<MainCharacter>();
	}

	void Update () {
		this.gameObject.transform.position = new Vector3 (character.transform.position.x,10,character.transform.position.z);
	}
}
