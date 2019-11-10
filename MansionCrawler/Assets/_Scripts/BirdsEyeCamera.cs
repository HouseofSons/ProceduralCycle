using UnityEngine;
using System.Collections;

public class BirdsEyeCamera : MonoBehaviour {

	private MainCharacter character;

	void Awake() {
		character = GameObject.FindWithTag ("Player_0" + MainCharacter.PlayerCount ()).GetComponent<MainCharacter>();
		this.gameObject.GetComponent<Camera>().orthographic = true;
		this.gameObject.GetComponent<Camera>().orthographicSize = 6;
	}

	void Start () {
		this.gameObject.transform.position = new Vector3(character.transform.position.x,10,character.transform.position.z);
	}

	void Update () {
		if (character.nextstep != null) {
			this.gameObject.transform.localPosition = Vector3.Lerp(this.gameObject.transform.localPosition,
			    new Vector3(character.nextstep.location.x,10,character.nextstep.location.z),
				3 * Time.deltaTime);
		}
	}
}
