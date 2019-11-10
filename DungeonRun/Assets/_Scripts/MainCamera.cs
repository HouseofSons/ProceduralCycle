using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {

	//used to determine bounds camera can follow
	private float[] mapBounds = new float[4];

	private float cameraHeightDistance;
	private float cameraWidthDistance;

	private static bool cameraJitter;
	private static bool playerWillHitWall;

	void Start() {

		cameraHeightDistance = this.gameObject.GetComponent<Camera> ().orthographicSize - 5.0f;
		cameraWidthDistance = (this.gameObject.GetComponent<Camera> ().orthographicSize * Screen.width / Screen.height) - 5.0f;
		cameraJitter = false;
		playerWillHitWall = false;
		SetCameraOnPlayer ();
	}
	
	void Update () {
		if (!cameraJitter) {
			CameraLeadPlayer();
		}
	}

	public static bool PlayerWillHitWall {
		get {return playerWillHitWall;}
		set {playerWillHitWall = value;}
	}

	private void SetCameraOnPlayer() {
		Vector3 camPos = this.gameObject.transform.position;
		this.transform.position = new Vector3 (GameManager.GetCurrentPlayer().transform.position.x,camPos.y,GameManager.GetCurrentPlayer().transform.position.z);
	}
	
	private void CameraLeadPlayer() {
		
		Vector3 playerPos = GameManager.GetCurrentPlayer().transform.position;//player position
		Vector3 camPos = this.gameObject.transform.position;//current camera position
		
		Ray playerFacingRay = new Ray(playerPos,Player.PlayerMovingDirection);
		RaycastHit hit;
		LayerMask layerMask = 1 << LayerMask.NameToLayer ("Wall") | 1 << LayerMask.NameToLayer ("InnerWall");
		if (!playerWillHitWall) {
			if (!Physics.SphereCast (playerFacingRay, GameManager.GetCurrentPlayer ().transform.localScale.x, out hit, 15.0f, layerMask)) {
				playerPos = playerFacingRay.GetPoint (10.0f);
			} else {
				playerWillHitWall = true;
			}
		}
		
		SetMapBounds (playerPos);//establishes bounds
		
		float xPos = 0;
		float zPos = 0;
		float zDistance = Mathf.Abs(mapBounds[0] - mapBounds[1]);
		float xDistance = Mathf.Abs(mapBounds[2] - mapBounds[3]);
		
		if (mapBounds[0] != -1 && mapBounds[1] != -1) {
			if (zDistance < cameraHeightDistance * 2.0f) {
				zPos = mapBounds[1] + (zDistance/2.0f);
			} else {
				if (mapBounds[0] - playerPos.z < playerPos.z - mapBounds[1]) {
					if (mapBounds[0] - playerPos.z < cameraHeightDistance) {
						zPos = mapBounds[0] - cameraHeightDistance;
					} else {
						zPos = playerPos.z;
					}
				} else {
					if (playerPos.z - mapBounds[1] < cameraHeightDistance) {
						zPos = mapBounds[1] + cameraHeightDistance;
					} else {
						zPos = playerPos.z;
					}
				}
			}
		} else {
			zPos = playerPos.z;
		}
		
		if (mapBounds[2] != -1 && mapBounds[3] != -1) {
			if (xDistance < cameraWidthDistance * 2.0f) {
				xPos = mapBounds[3] + (xDistance/2.0f);
			} else {
				if (mapBounds[2] - playerPos.x < playerPos.x - mapBounds[3]) {
					if (mapBounds[2] - playerPos.x < cameraWidthDistance) {
						xPos = mapBounds[2] - cameraWidthDistance;
					} else {
						xPos = playerPos.x;
					}
				} else {
					if (playerPos.x - mapBounds[3] < cameraWidthDistance) {
						xPos = mapBounds[3] + cameraWidthDistance;
					} else {
						xPos = playerPos.x;
					}
				}
			}
		} else {
			xPos = playerPos.x;
		}
		this.gameObject.transform.position = Vector3.Lerp (camPos, new Vector3(xPos,GameManager.GetCurrentPlayer().transform.position.y+45.0f,zPos), Time.deltaTime*2);
	}
	
	private void SetMapBounds(Vector3 cameraFollowPosition) {

		LayerMask layerMask = 1<<LayerMask.NameToLayer("Wall");

		Ray zPosRay = new Ray (cameraFollowPosition, new Vector3 (0, 0, 1));
		Ray zNegRay = new Ray (cameraFollowPosition, new Vector3 (0, 0, -1));
		Ray xPosRay = new Ray (cameraFollowPosition, new Vector3 (1, 0, 0));
		Ray xNegRay = new Ray (cameraFollowPosition, new Vector3 (-1, 0, 0));

		RaycastHit zPos;
		RaycastHit zNeg;
		RaycastHit xPos;
		RaycastHit xNeg;

		if (Physics.Raycast (zPosRay,out zPos,5000, layerMask)) {//Z+
			mapBounds[0] = zPos.point.z;
		} else {
			mapBounds[0] = -1;
		}
		if (Physics.Raycast (zNegRay,out zNeg,5000,layerMask)) {//Z-
			mapBounds[1] = zNeg.point.z;
		} else {
			mapBounds[1] = -1;
		}
		if (Physics.Raycast (xPosRay,out xPos,5000,layerMask)) {//X+
			mapBounds[2] = xPos.point.x;
		} else {
			mapBounds[2] = -1;
		}
		if (Physics.Raycast (xNegRay,out xNeg,5000,layerMask)) {//X-
			mapBounds[3] = xNeg.point.x;
		} else {
			mapBounds[3] = -1;
		}
	}

	public static IEnumerator CameraJitter() {
		
		cameraJitter = true;
		
		Vector3 camPosition = GameManager.GetCamera ().transform.position;
		float xRand;
		float zRand;
		
		for (int i=0; i<15; i++) {
			xRand = Random.Range (-0.2f,0.2f);
			zRand = Random.Range (-0.2f,0.2f);
			GameManager.GetCamera ().transform.position = new Vector3(camPosition.x+xRand,camPosition.y,camPosition.z+zRand);
			yield return null;
		}
		GameManager.GetCamera ().transform.position = camPosition;
		cameraJitter = false;
		yield return null;
	}
}








