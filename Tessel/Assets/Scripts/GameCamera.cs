using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
		this.gameObject.transform.position = new Vector3 (LevelManager.FACE_MAGNITUDE,LevelManager.FACE_MAGNITUDE/2.00f,-LevelManager.FACE_MAGNITUDE);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public IEnumerator CameraMapTurn(bool turnHorizon) {

		Vector3 currWallLocalCamPos = LevelManager.walls [LevelManager.currentWallNumber].gameObject.transform.GetChild (0).localPosition;
		currWallLocalCamPos = new Vector3(currWallLocalCamPos.x,currWallLocalCamPos.y,(turnHorizon ? -LevelManager.FACE_MAGNITUDE/16 : -LevelManager.FACE_MAGNITUDE));
		LevelManager.walls [LevelManager.currentWallNumber].gameObject.transform.GetChild (0).localPosition = currWallLocalCamPos;

		Vector3 newCamPos = LevelManager.walls[LevelManager.currentWallNumber].gameObject.transform.GetChild(0).position;
		Quaternion newCamRot = LevelManager.walls[LevelManager.currentWallNumber].gameObject.transform.GetChild(0).rotation;

		bool rotationHit = false;
		bool positionHit = false;
		float smoothing = 1.00f;

		while (Quaternion.Angle(this.gameObject.transform.rotation,newCamRot) > 0.1f ||
		       Vector3.Distance(this.gameObject.transform.position,newCamPos) > 0.1f) {

			smoothing += 0.15f;

			if(!rotationHit) {
				this.gameObject.transform.rotation = Quaternion.Slerp(this.gameObject.transform.rotation, newCamRot, Time.deltaTime*smoothing);
			}
			if(!positionHit) {
				this.gameObject.transform.position = Vector3.Lerp (this.transform.position, newCamPos, Time.deltaTime*smoothing);
			}
			yield return null;
			
			if(Quaternion.Angle(this.gameObject.transform.rotation, newCamRot) <= 0.1f) {
				this.gameObject.transform.rotation = newCamRot;
				rotationHit = true;
			}
			if (Vector3.Distance (this.gameObject.transform.position, newCamPos) <= 0.1f) {
				this.gameObject.transform.position = newCamPos;
				positionHit = true;
			}
		}
	}
}















