using UnityEngine;
using System.Collections;

public class Controls : MonoBehaviour {

	private GameCamera cameraScript;
	private bool cameraZoomAndRotate;

	// Use this for initialization
	void Awake () {
		GameObject camera = GameObject.Find ("Main Camera");
		cameraScript = camera.GetComponent("GameCamera") as GameCamera;
		cameraZoomAndRotate = true;
	}

	
	// Update is called once per frame
	void Update () {
		if (cameraZoomAndRotate) {
			if (Input.GetKeyDown (KeyCode.A)) {
				if (GameCamera.DoneMoving) {
					StartCoroutine (cameraScript.RotateObject (Vector3.down));
				}
			}
		
			if (Input.GetKeyDown (KeyCode.D)) {
				if (GameCamera.DoneMoving) {
					StartCoroutine (cameraScript.RotateObject (Vector3.up));
				}
			}
		
			if (Input.GetKeyDown (KeyCode.W)) {
				if (GameCamera.DoneMoving) {
					if (!GameCamera.Scouting) {
						StartCoroutine (cameraScript.Zoom (2));
					} else {
						StartCoroutine (cameraScript.MoveToHex(GameCamera.focusHex,GameCamera.AdjacentHex()));
						if (!GameBoardHex.GetCTCHex().GetOccupant().GetMoved()) {//only if character hasn't moved yet
							GameObject.Find("DialogueBoxMove").transform.position = new Vector3(Screen.width * 0.3f,Screen.height * 0.7f,0);
							//GameBoardHex.focusHex.changed = true;//is this necessary?
							GameBoardHex.focusHex = GameCamera.focusGameBoardHex;
							GameBoardHex.GameBoardEnabled = false;
							StartCoroutine (FindObjectOfType<CharacterStatusLeft>().LeftStatusPop(GameBoardHex.GetCTCHex(),true));
							StartCoroutine (FindObjectOfType<CharacterStatusRight>().RightStatusPop(GameBoardHex.GetCTCHex(),false));
						}
					}
				}
			}
		
			if (Input.GetKeyDown (KeyCode.S)) {
				if (GameCamera.DoneMoving) {
					if (!GameCamera.Scouting) {
						StartCoroutine (cameraScript.Zoom (-2));
					}
				}
			}

			if (Input.GetKeyDown (KeyCode.Z)) {
				if (GameCamera.DoneMoving) {
                    if (GameCamera.ThirdPersonView)
                    {
                        if (GameCamera.Scouting)
                        {
                            GameCamera.Scouting = false;
                        }
                        else
                        {
                            GameCamera.Scouting = true;
                        }
                    }
				}
			}

			if (Input.GetKeyDown (KeyCode.Q)) {
				if (GameCamera.DoneMoving) {
					if (!GameCamera.Scouting) {
                        StartCoroutine(FindObjectOfType<CharacterStatusLeft>().LeftStatusPop(GameBoardHex.ctcHex, false));
                        StartCoroutine(FindObjectOfType<CharacterStatusRight>().RightStatusPop(GameBoardHex.ctcHex, false));
                        StartCoroutine (GameCamera.SetCameraToObject());
					}
				}
			}

			if (Input.GetKeyDown (KeyCode.LeftShift)) {
				if (GameCamera.DoneMoving) {
					GameObject.Find ("LevelManager").GetComponent<PlayerAction>().PlayerChoseStay();//to reset move option if visible
					GameObject.Find ("LevelManager").GetComponent<PlayerAction>().PlayerChoseSustain();//to reset attack option if visible
					if (GameCamera.focusHex != GameBoardHex.ctcHex.transform.position) { //positions camera at focus hex
						Pathing.AttackMoveGrid(GameBoardHex.ctcHex, true, GameBoardHex.ctcHex.GetOccupant().GetMoved());
						GameBoardHex.focusHex = GameBoardHex.ctcHex;
						StartCoroutine (FindObjectOfType<CharacterStatusLeft>().LeftStatusPop(GameBoardHex.ctcHex,true));
						StartCoroutine (FindObjectOfType<CharacterStatusRight>().RightStatusPop(GameBoardHex.ctcHex,false));
						StartCoroutine (GameCamera.SetCameraToObject (GameBoardHex.ctcHex.transform.position));
					} else {
						if (GameCamera.ThirdPersonView) {
							StartCoroutine (FindObjectOfType<CharacterStatusRight>().RightStatusPop(GameBoardHex.ctcHex,false));//deselect previously select target
							StartCoroutine(GameCamera.SetCameraToObject (GameBoardHex.GetCTCHex().gameObject.transform.position));
						} else { //zooms in third person
							//Pathing.AttackMoveGrid(GameBoardHex.ctcHex, true, GameBoardHex.ctcHex.GetOccupant().GetMoved());
							//GameBoardHex.focusHex = GameBoardHex.ctcHex;
							//StartCoroutine (FindObjectOfType<CharacterStatusLeft>().LeftStatusPop(GameBoardHex.ctcHex,true));
							//StartCoroutine (FindObjectOfType<CharacterStatusRight>().RightStatusPop(GameBoardHex.ctcHex,false));
							//StartCoroutine (GameCamera.ThirdPersonCameraAdjust (GameBoardHex.GetCTCHex()));
						}
					}
				}
			}
		}
	}

	public void setCameraZandR (bool switchOn) {
		if (switchOn) {
			Debug.Log ("Camera Zoom and Rotate is ON");
		} else {
			Debug.Log ("Camera Zoom and Rotate is OFF");
		}
		cameraZoomAndRotate = switchOn;
	}
}