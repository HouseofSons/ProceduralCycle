using UnityEngine;

public class AimArrow : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		if (GameManager.AimArrowState) {
			gameObject.GetComponent<SpriteRenderer> ().enabled = true;
		} else {
			gameObject.GetComponent<SpriteRenderer> ().enabled = false;
		}
	}
}
