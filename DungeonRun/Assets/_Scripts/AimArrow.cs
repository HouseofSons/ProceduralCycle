using UnityEngine;
using System.Collections;

public class AimArrow : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		if (GameManager.AimArrowState) {
			this.gameObject.GetComponent<SpriteRenderer> ().enabled = true;
		} else {
			this.gameObject.GetComponent<SpriteRenderer> ().enabled = false;
		}
	}
}
