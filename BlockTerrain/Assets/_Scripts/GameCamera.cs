using UnityEngine;
using System.Collections;	

public class GameCamera : MonoBehaviour {

	//The Camera
	private static Camera sceneCamera;
	//The center position of the terrain map
	private static Vector3 origin;
	//Vector3 to Focus when not focusing origin
	public static Vector3 focusHex;
	//GameBoardHex toggled by AdjacentHexFunction
	public static GameBoardHex focusGameBoardHex;
	//bool which dictates when camera movement complete
	private static bool doneMoving;
	//bool which indicates if thirdperson view is active
	private static bool thirdPersonView;
	//float used to remember camera field of view when using thirdperson camera
	private static float prevCameraFieldOfView;
	//bool which indicates if camera is scounting
	private static bool scouting;

	//Runs once at start of Script
	void Start () {
		doneMoving = true;
		thirdPersonView = false;
		scouting = false;
		prevCameraFieldOfView = 30f;
		//Requires that Terrain already be built
		origin = new Vector3 (0,0,0);
		focusHex = origin;
		sceneCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
		if (sceneCamera.GetComponent<Camera> ().orthographic) {
			sceneCamera.GetComponent<Camera> ().orthographicSize = 14;
		} else {
			sceneCamera.GetComponent<Camera> ().fieldOfView = 30;
		}
	}

	public static bool DoneMoving {
		get{return doneMoving;}
		set{doneMoving = value;}
	}

	public static bool ThirdPersonView {
		get{return thirdPersonView;}
		set{thirdPersonView = value;}
	}

	public static bool Scouting {
		get{return scouting;}
		set{scouting = value;}
	}

	private static Vector3 ConformCameraAngle(float length,float cameraHeight,Quaternion targetInverseAngle,Vector3 targetPosition) {
		float x = targetPosition.x;
		float y = targetPosition.y + cameraHeight;
		float z = targetPosition.z;
		targetInverseAngle *= Quaternion.Euler (0, 180, 0);//creates target angle
		int angleRef = Mathf.RoundToInt (((targetInverseAngle.eulerAngles.y / 60f) + 0.5f) * 10);
		int angleRemainder = angleRef % 5;

		if (angleRemainder < 3) {
			angleRef = angleRef - angleRemainder;
		} else {
			angleRef = angleRef + 5 - angleRemainder;
		}

		switch (angleRef)
		{
		case 5:
			x=x-0f;
			z=z+length;
			break;
		case 10:
			x=x+length*Mathf.Asin(Mathf.PI/6f);
			z=z+length*Mathf.Acos(Mathf.PI/6f);
			break;
		case 15:
			x=x+length*Mathf.Acos(Mathf.PI/6f);
			z=z+length*Mathf.Asin(Mathf.PI/6f);
			break;
		case 20:
			x=x+length;
			z=z+0f;
			break;
		case 25:
			x=x+length*Mathf.Acos(Mathf.PI/6f);
			z=z-length*Mathf.Asin(Mathf.PI/6f);
			break;
		case 30:
			x=x+length*Mathf.Asin(Mathf.PI/6f);
			z=z-length*Mathf.Acos(Mathf.PI/6f);
			break;
		case 35:
			x=x+0f;
			z=z-length;
			break;
		case 40:
			x=x-length*Mathf.Asin(Mathf.PI/6f);
			z=z-length*Mathf.Acos(Mathf.PI/6f);
			break;
		case 45:
			x=x-length*Mathf.Acos(Mathf.PI/6f);
			z=z-length*Mathf.Asin(Mathf.PI/6f);
			break;
		case 50:
			x=x-length;
			z=z+0f;
			break;
		case 55:
			x=x-length*Mathf.Acos(Mathf.PI/6f);
			z=z+length*Mathf.Asin(Mathf.PI/6f);
			break;
		case 60:
			x=x-length*Mathf.Asin(Mathf.PI/6f);
			z=z+length*Mathf.Acos(Mathf.PI/6f);
			break;
		case 65:
			x=x-0f;
			z=z+length;
			break;
		}
		return new Vector3 (x,y,z);
	}

	//Focus Camera on given vector NEEDS ADJUSTING
	public static IEnumerator SetCameraToObject (Vector3 attenionPosition) {
		doneMoving = false;
        //float distanceSmoothing = 0.05f;
        //float rotationSmoothing = 0.05f;
        float smoothing = 0.05f;

		if (!thirdPersonView) {
			prevCameraFieldOfView = sceneCamera.fieldOfView;
		} else {
			sceneCamera.GetComponent<Camera> ().fieldOfView = prevCameraFieldOfView;
		}

		GameObject rotTemp = new GameObject ();
		rotTemp.transform.position = ConformCameraAngle(HexWorldTerrain.GetBoardWidth() * 2, HexWorldTerrain.GetHeight() * 2f, sceneCamera.transform.rotation, attenionPosition);
		rotTemp.transform.LookAt (attenionPosition);
		 
		while (Vector3.Distance(sceneCamera.transform.position,rotTemp.transform.position) > 0.10f ||
		       Quaternion.Angle(sceneCamera.transform.rotation,rotTemp.transform.rotation) > 1f) {
			
			sceneCamera.transform.position = Vector3.Lerp(sceneCamera.transform.position,rotTemp.transform.position,smoothing * Time.deltaTime);
			sceneCamera.transform.rotation = Quaternion.Lerp(sceneCamera.transform.rotation,rotTemp.transform.rotation, smoothing * Time.deltaTime);
            smoothing += 0.25f;
			yield return null;
		}

		Destroy (rotTemp);
		focusHex = attenionPosition;
		thirdPersonView = false;
		scouting = false;
		//CameraOcclusionManager.GameBoardHexVisualPriorityList();//Comment out if not desired
		doneMoving = true;
	}

    //Focus Camera on Origin
    public static IEnumerator SetCameraToObject()
    {
        doneMoving = false;

        Vector3 attenionPosition = new Vector3(
        HexWorldTerrain.xHexToWorldCoordinate(Mathf.FloorToInt(HexWorldTerrain.GetWidth() / 2.0f),
        Mathf.FloorToInt(HexWorldTerrain.GetLength() / 2.0f)), HexWorldTerrain.GetHeight() / 4.0f,
        HexWorldTerrain.zHexToWorldCoordinate(Mathf.FloorToInt(HexWorldTerrain.GetLength() / 2.0f)));

        float smoothing = 0.05f;

        if (!thirdPersonView)
        {
            prevCameraFieldOfView = sceneCamera.fieldOfView;
        }
        else
        {
            sceneCamera.GetComponent<Camera>().fieldOfView = prevCameraFieldOfView;
        }

        GameObject rotTemp = new GameObject();
        rotTemp.transform.position = ConformCameraAngle(HexWorldTerrain.GetBoardWidth() * 2, HexWorldTerrain.GetHeight() * 1.75f, sceneCamera.transform.rotation, attenionPosition);
        rotTemp.transform.LookAt(attenionPosition);

        while (Vector3.Distance(sceneCamera.transform.position, rotTemp.transform.position) > 0.10f ||
               Quaternion.Angle(sceneCamera.transform.rotation, rotTemp.transform.rotation) > 1f)
        {

            sceneCamera.transform.position = Vector3.Lerp(sceneCamera.transform.position, rotTemp.transform.position, smoothing * Time.deltaTime);
            sceneCamera.transform.rotation = Quaternion.Lerp(sceneCamera.transform.rotation, rotTemp.transform.rotation, smoothing * Time.deltaTime);
            smoothing += 0.25f;
            yield return null;
        }

        Destroy(rotTemp);
        focusHex = attenionPosition;
        thirdPersonView = false;
        scouting = false;
        //CameraOcclusionManager.GameBoardHexVisualPriorityList();//Comment out if not desired
        doneMoving = true;
    }
	
	//Function which smoothly zooms camera in and out
	public IEnumerator Zoom(int zoomIn) {
		doneMoving = false;
		if (!thirdPersonView) {
			if (sceneCamera.GetComponent<Camera> ().orthographic) {
				if ((zoomIn > 0 && sceneCamera.GetComponent<Camera> ().orthographicSize > 7.5) || (zoomIn < 0 && sceneCamera.GetComponent<Camera> ().orthographicSize < 30)) {
					float step = 0.0f; //non-smoothed
					float rate = 4.0f; //amount to increase non-smooth step by
					float smoothStep = 0.0f; //smooth step this time
					float lastStep = 0.0f; //smooth step last time
				
					while (step < 1.0) { // until we're done
						step += Time.deltaTime * rate; //increase the step
						smoothStep = Mathf.SmoothStep (0.0f, 1.0f, step); //get the smooth step
						sceneCamera.GetComponent<Camera> ().orthographicSize -= (smoothStep - lastStep) * zoomIn;
						lastStep = smoothStep; //store the smooth step
						yield return new WaitForSeconds (0.005f);
					}
					//finish any left-over
					if (step > 1.0) {
						sceneCamera.GetComponent<Camera> ().orthographicSize -= (1.0f - lastStep) * zoomIn;
					}
					doneMoving = true;
				} else {
					doneMoving = true;
				}
			} else {
				if ((zoomIn > 0 && sceneCamera.GetComponent<Camera> ().fieldOfView > 7.5) || (zoomIn < 0 && sceneCamera.GetComponent<Camera> ().fieldOfView < 70)) {
					float step = 0.0f; //non-smoothed
					float rate = 4.0f; //amount to increase non-smooth step by
					float smoothStep = 0.0f; //smooth step this time
					float lastStep = 0.0f; //smooth step last time
					
					while (step < 1.0) { // until we're done
						step += Time.deltaTime * rate; //increase the step
						smoothStep = Mathf.SmoothStep (0.0f, 1.0f, step); //get the smooth step
						sceneCamera.GetComponent<Camera> ().fieldOfView -= (smoothStep - lastStep) * zoomIn;
						lastStep = smoothStep; //store the smooth step
						yield return new WaitForSeconds (0.005f);
					}
					//finish any left-over
					if (step > 1.0) {
						sceneCamera.GetComponent<Camera> ().fieldOfView -= (1.0f - lastStep) * zoomIn;
					}
					doneMoving = true;
				} else {
					doneMoving = true;
				}
			}
		} else {
			doneMoving = false;
			Vector3 hex = focusHex;
			Vector3 viewPos = sceneCamera.ViewportToWorldPoint(new Vector3(0.5f,0.5f,0.75f));

			Quaternion newRotation = sceneCamera.transform.rotation;
			Vector3 newPosition = sceneCamera.transform.position;

			bool calc = true;

			if (zoomIn > 0) {
				if (20 < sceneCamera.transform.eulerAngles.x && sceneCamera.transform.eulerAngles.x < 40) {
					Debug.Log ("look center" + sceneCamera.transform.eulerAngles);
					newRotation = Quaternion.Euler(new Vector3(0f,sceneCamera.transform.eulerAngles.y,sceneCamera.transform.eulerAngles.z));
					newPosition = new Vector3(hex.x,hex.y+0.5f/*yChange*/,hex.z);
				} else if (320 < sceneCamera.transform.eulerAngles.x && sceneCamera.transform.eulerAngles.x < 340) {
					Debug.Log ("Do Nothing" + sceneCamera.transform.eulerAngles);
					calc = false;
				} else {
					Debug.Log ("look up" + sceneCamera.transform.eulerAngles);
					newRotation = Quaternion.Euler(new Vector3(330f,sceneCamera.transform.eulerAngles.y,sceneCamera.transform.eulerAngles.z));
					viewPos = sceneCamera.ViewportToWorldPoint(new Vector3(0.5f,0.5f,-0.75f));
					newPosition = new Vector3(viewPos.x,hex.y+0.725f/*yChange*/,viewPos.z);
				}
			} else {
				if (20 < sceneCamera.transform.eulerAngles.x && sceneCamera.transform.eulerAngles.x < 40) {
					Debug.Log ("Do Nothing" + sceneCamera.transform.eulerAngles);
					calc = false;
				} else if (320 < sceneCamera.transform.eulerAngles.x && sceneCamera.transform.eulerAngles.x < 340) {
					Debug.Log ("look center" + sceneCamera.transform.eulerAngles);
					newRotation = Quaternion.Euler(new Vector3(0f,sceneCamera.transform.eulerAngles.y,sceneCamera.transform.eulerAngles.z));
					newPosition = new Vector3(hex.x,hex.y+0.5f/*yChange*/,hex.z);
				} else {
					Debug.Log ("look down" + sceneCamera.transform.eulerAngles);
					newRotation = Quaternion.Euler(new Vector3(30f,sceneCamera.transform.eulerAngles.y,sceneCamera.transform.eulerAngles.z));
					newPosition = new Vector3(viewPos.x,hex.y+0.725f/*yChange*/,viewPos.z);
				}
			}
			if (calc) {
				float smoothing = 2f;

				while (Vector3.Distance(sceneCamera.transform.position,newPosition) > 0.05f || Quaternion.Angle(sceneCamera.transform.rotation,newRotation) > 1f) {
					
					sceneCamera.transform.rotation = Quaternion.Lerp(sceneCamera.transform.rotation,newRotation,smoothing * Time.deltaTime);
					sceneCamera.transform.position = Vector3.Lerp(sceneCamera.transform.position,newPosition,smoothing * Time.deltaTime);
					smoothing += 1f;
					yield return null;
				}
			}
			doneMoving = true;
		}
	}

	public static IEnumerator ThirdPersonCameraAdjust (GameBoardHex hex) {
		doneMoving = false;
		thirdPersonView = true;
		float smoothing = 2f;

		if (!thirdPersonView) {
			prevCameraFieldOfView = sceneCamera.fieldOfView;
		}

		Vector3 targetCameraInLine = new Vector3 (sceneCamera.transform.position.x, hex.transform.position.y + 0.5f/*yChange*/,sceneCamera.transform.position.z);
		Vector3 targetLookAt = new Vector3 (hex.transform.position.x,hex.transform.position.y + 0.5f/*yChange*/,hex.transform.position.z);
		Ray ray = new Ray (targetCameraInLine, targetLookAt - targetCameraInLine);
		Vector3 targetPos = ray.GetPoint (Vector3.Distance (targetCameraInLine, targetLookAt) + 0.2f);
		targetLookAt = ray.GetPoint (Vector3.Distance (targetCameraInLine, targetLookAt) + 0.5f/*yChange*/);

		while (Vector3.Distance(sceneCamera.transform.position,targetPos) > 0.05f || sceneCamera.GetComponent<Camera> ().fieldOfView < 60) {

			sceneCamera.transform.position = Vector3.Lerp(sceneCamera.transform.position,targetPos,smoothing * Time.deltaTime);
			sceneCamera.transform.LookAt (targetLookAt);
			if (sceneCamera.GetComponent<Camera> ().fieldOfView < 60) {
				sceneCamera.GetComponent<Camera> ().fieldOfView += smoothing * 0.1f;
			}
			smoothing += 0.25f;
			yield return null;
		}
		sceneCamera.GetComponent<Camera> ().fieldOfView = 60;
		doneMoving = true;
	}

	//Function which smoothly rotates camera around  NEEDS ADJUSTING
	public IEnumerator RotateObject(Vector3 axis) {
		doneMoving = false;
		float step = 0.0f; //non-smoothed
		float rate = 10f; //amount to increase non-smooth step by
		float smoothStep = 0.0f; //smooth step this time
		float lastStep = 0.0f; //smooth step last time
		float rotationStep = 30f;

		Vector3 attnHex = focusHex;
		
		while(step < 1.0) { // until we're done
			step += Time.deltaTime * rate; //increase the step
			smoothStep = Mathf.SmoothStep(0.0f, 1.0f, step); //get the smooth step
			sceneCamera.transform.RotateAround(attnHex, axis, rotationStep * (smoothStep - lastStep));
			lastStep = smoothStep; //store the smooth step
			yield return new WaitForSeconds (0.005f);
		}
		//finish any left-over
		if(step > 1.0) {
			sceneCamera.transform.RotateAround(attnHex, axis, 30f * (1.0f - lastStep));
		}
        //CameraOcclusionManager.GameBoardHexVisualPriorityList();
        doneMoving = true;
	}

	//Function Moves Camera Along Moveable Hexs in direction of Camera when in third person view
	public IEnumerator MoveToHex(Vector3 origin, Vector3 destination) {
		doneMoving = false;
		scouting = true;
		float smoothing = 5f;

		/* Centers Camera Start */
		Vector3 hex = focusHex;
		
		Quaternion newRotation = sceneCamera.transform.rotation;
		Vector3 newPosition = sceneCamera.transform.position;
		
		bool calc = false;
		if (20 < sceneCamera.transform.eulerAngles.x && sceneCamera.transform.eulerAngles.x < 40 ||
		    320 < sceneCamera.transform.eulerAngles.x && sceneCamera.transform.eulerAngles.x < 340) {
			Debug.Log ("look center" + sceneCamera.transform.eulerAngles);
			newRotation = Quaternion.Euler(new Vector3(0f,sceneCamera.transform.eulerAngles.y,sceneCamera.transform.eulerAngles.z));
			newPosition = new Vector3(hex.x,hex.y+0.5f/*yChange*/,hex.z);
			calc = true;
		}
		
		if (calc) {
			while (Vector3.Distance(sceneCamera.transform.position,newPosition) > 0.05f || Quaternion.Angle(sceneCamera.transform.rotation,newRotation) > 1f) {
				
				sceneCamera.transform.rotation = Quaternion.Lerp(sceneCamera.transform.rotation,newRotation,smoothing * Time.deltaTime);
				sceneCamera.transform.position = Vector3.Lerp(sceneCamera.transform.position,newPosition,smoothing * Time.deltaTime);
				smoothing += 1f;
				yield return null;
			}
		}
		/* Centers Camera End */

		Vector3 destHexPos = new Vector3(destination.x,destination.y+0.5f/*yChange*/,destination.z);
		Vector3 origHexPos = new Vector3(origin.x,origin.y+0.5f/*yChange*/,origin.z);
		Vector3 destFirstPos = origHexPos;

		float verticalDistance = destHexPos.y - origHexPos.y; 
		int above = 0;
		smoothing = 1f;
		if (verticalDistance > 0.25f/*yChange*/) {
			above = 1;
			destFirstPos = new Vector3(origHexPos.x,origHexPos.y+verticalDistance,origHexPos.z);
		} else {
			if (verticalDistance < -0.25f/*yChange*/) {
				above = -1;
				destFirstPos = new Vector3(destHexPos.x,origHexPos.y,destHexPos.z);
			}
		}

		if (above == 1 || above == -1) {
			while (Vector3.Distance(sceneCamera.transform.position,destFirstPos) > 0.05f) {
				sceneCamera.transform.position = Vector3.Lerp(sceneCamera.transform.position,destFirstPos,smoothing * Time.deltaTime);
				smoothing += 1f;
				yield return null;
			}
			sceneCamera.transform.position = destFirstPos;
			smoothing = 1f;
			while (Vector3.Distance(sceneCamera.transform.position,destHexPos) > 0.05f) {
				sceneCamera.transform.position = Vector3.Lerp(sceneCamera.transform.position,destHexPos,smoothing * Time.deltaTime);
				smoothing += 0.5f;
				yield return null;
			}
			sceneCamera.transform.position = destHexPos;
		} else {
			while (Vector3.Distance(sceneCamera.transform.position,destHexPos) > 0.05f) {
				sceneCamera.transform.position = Vector3.Lerp(sceneCamera.transform.position,destHexPos,smoothing * Time.deltaTime);
				smoothing += 0.5f;
				yield return null;
			}
			sceneCamera.transform.position = destHexPos;
		}
		focusHex = destination;
		doneMoving = true;
	}

	//Gets Closest Adjacent Moveable Hex in the direction of the Camera
	public static Vector3 AdjacentHex() {
		GameBoardHex focus = GameBoardHexGrid.ClosestGameBoardHex (focusHex);
		float yRot = (sceneCamera.transform.rotation.eulerAngles.y / 60f) + 0.5f;
		int yRotRoundToInt = Mathf.RoundToInt (yRot * 10);
		int x=focus.GetXCoord();
		int z=focus.GetZCoord();
		bool calc = false;
		
		switch (yRotRoundToInt)
		{
			case 10:
				x=focus.GetXCoord()+1;
				z=focus.GetZCoord()+1;
				calc = true;
				break;
			case 20:
				x=focus.GetXCoord()+1;
				z=focus.GetZCoord()+0;
				calc = true;
				break;
			case 30:
				x=focus.GetXCoord()+0;
				z=focus.GetZCoord()-1;
				calc = true;
				break;
			case 40:
				x=focus.GetXCoord()-1;
				z=focus.GetZCoord()-1;
				calc = true;
				break;
			case 50:
				x=focus.GetXCoord()-1;
				z=focus.GetZCoord()+0;
				calc = true;
				break;
			case 60:
				x=focus.GetXCoord()+0;
				z=focus.GetZCoord()+1;
				calc = true;
				break;
		}

		if (calc) {
			Vector3 point = sceneCamera.ViewportToWorldPoint(new Vector3(0.5f,0.5f,1f));

			GameBoardHex[,,] gameBoard = GameBoardHexGrid.GameBoard ();
			float dist = 1000f;

			for (int j=0;j<gameBoard.GetLength(1);j++) {
				if (gameBoard[x,j,z] != null) {
					if (gameBoard[x,j,z].GetIsFocusMoveable()) {
						if (Vector3.Distance(gameBoard[x,j,z].transform.position,point)<dist) {
							dist = Vector3.Distance(gameBoard[x,j,z].transform.position,point);
							focus = gameBoard[x,j,z];
						}
					}
				}
			}
		}
		focusGameBoardHex = focus;
		return focus.transform.position;
	}
}









