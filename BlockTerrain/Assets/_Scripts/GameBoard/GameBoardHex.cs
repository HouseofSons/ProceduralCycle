using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class GameBoardHex : MonoBehaviour {
	
	private bool isFocusAttackable;
	private bool isFocusMoveable;

	public static bool GameBoardEnabled = false;

	private bool isOnTop;

	private int xCoord;
	private int yCoord;
	private int zCoord;

	public static GameBoardHex ctcHex;
	public static GameBoardHex focusHex;
	private Character occupant;
	private bool isOccupied;
	
	private Color spawnedColor;
	private Color actionColor;
	private bool focusBlink;
	private float blinkStep;
	private bool pointedAt;
	public bool changed;

	private int h;
	private int m;
	private int f;
	private GameBoardHex p;
	
	private List<List<GameBoardHex>> pathsToCharacters;
	private int pathState;

	private Ray rayToCamera;
	
	// Use this for initialization
	void Start () {
		actionColor = Color.gray;
		focusBlink = false;
		blinkStep = 0;
		pointedAt = false;
		changed = false;
		isOccupied = false;
		isFocusAttackable = false;
		isFocusMoveable = false;
		pathsToCharacters = new List<List<GameBoardHex>>();
		pathState = 0;
	}

	void Update() {

		if ((pointedAt && (isFocusMoveable || isFocusAttackable))|| focusHex == this) {
			if (GameBoardEnabled) {
				Blink ();
			}
		} 
		if (changed) {
			if (isFocusMoveable) {
				changed = false;
				actionColor = new Color(1.0f,0.5f,0.0f,0.5f)/*orange*/;
				this.gameObject.GetComponent<Renderer> ().material.color = Color.Lerp (actionColor, new Color(1.0f,1.0f,1.0f,0.2f)/*white*/,0.1f);
			} else if (isFocusAttackable) {
				changed = false;
				actionColor = new Color(1.0f,0.0f,0.0f,0.5f)/*red*/;
				this.gameObject.GetComponent<Renderer> ().material.color = Color.Lerp (actionColor, new Color(1.0f,1.0f,1.0f,0.2f)/*white*/,0.1f);
			} else {
				changed = false;
				actionColor = new Color(0f,0f,0f,0f);
				this.gameObject.GetComponent<Renderer> ().material.color = spawnedColor;
			}
		}
	}

	public void SetSpawnColor (Color spawned) {
		spawnedColor = spawned;
	}
	
	public void Blink() {
		this.gameObject.GetComponent<Renderer>().material.color = Color.Lerp(actionColor, new Color(1.0f,1.0f,1.0f,0.8f)/*white*/, blinkStep);
		if (!focusBlink) {
			blinkStep += 0.05f;
		} else {
			blinkStep -= 0.05f;
		}
		
		if (blinkStep >= 1) {
			focusBlink = true;
		}
		if (blinkStep <= 0) {
			focusBlink = false;
		}
	}

	public void IsFocusAttackable(bool attack) {
		isFocusAttackable = attack;
	}
	
	public bool GetIsFocusAttackable() {
		return isFocusAttackable;
	}

	public void IsFocusMoveable(bool move) {
		isFocusMoveable = move;
	}

	public bool GetIsFocusMoveable() {
		return isFocusMoveable;
	}

	void OnMouseEnter() {
		if (!GameCamera.ThirdPersonView) {
			if (GameBoardEnabled) {
				pointedAt = true;
			}
		}
	}
	
	void OnMouseExit() {
		if (!GameCamera.ThirdPersonView) {
			pointedAt = false;
			if (!isFocusAttackable && !isFocusMoveable) {
				this.gameObject.GetComponent<Renderer> ().material.color = spawnedColor;
			} else {
				changed = true;
			}
		}
	}
	
	void OnMouseUp () {
	
		if (!GameCamera.ThirdPersonView) {
          if (!EventSystem.current.IsPointerOverGameObject()) {
				if (GameBoardEnabled && GameCamera.DoneMoving) {
					if (isOccupied) {
						if (ctcHex != this) {
							if (occupant.GetComponent<Character>().IsFriendly() == occupant.GetComponent<Character>().GetFriendlyTurn ()) {
								focusHex = this;
								Pathing.AttackMoveGrid(ctcHex, false, occupant.GetMoved());
								StartCoroutine (FindObjectOfType<CharacterStatusLeft>().LeftStatusPop(focusHex,true));
								StartCoroutine (FindObjectOfType<CharacterStatusRight>().RightStatusPop(this,false));
								StartCoroutine (GameCamera.SetCameraToObject (this.gameObject.transform.position));
							} else {
								if (isFocusAttackable) {
									GameObject.Find("DialogueBoxAttack").transform.position = new Vector3(Screen.width * 0.3f,Screen.height * 0.7f,0);
									if (focusHex != null) {
										focusHex.changed = true;
									}
									focusHex = this;
									focusHex.changed = true;
									this.pointedAt = true;
									GameBoardEnabled = false;
									StartCoroutine (FindObjectOfType<CharacterStatusLeft>().LeftStatusPop(ctcHex,true));
									StartCoroutine (FindObjectOfType<CharacterStatusRight>().RightStatusPop(this,true));
								} else {
									Pathing.AttackMoveGrid(ctcHex, false, occupant.GetMoved());
									focusHex = this;
									StartCoroutine (FindObjectOfType<CharacterStatusRight>().RightStatusPop(focusHex,true));
									StartCoroutine (GameCamera.SetCameraToObject (this.gameObject.transform.position));
								}
							}
						} else {
							if (focusHex != ctcHex) {
								Pathing.AttackMoveGrid(ctcHex, true, occupant.GetMoved());
								focusHex = this;
								StartCoroutine (FindObjectOfType<CharacterStatusLeft>().LeftStatusPop(focusHex,true));
								StartCoroutine (FindObjectOfType<CharacterStatusRight>().RightStatusPop(focusHex,false));
								StartCoroutine (GameCamera.SetCameraToObject (this.gameObject.transform.position));
							}
						}
					} else {
						if (isFocusMoveable) {
							if (ctcHex.occupant.GetComponent<Character>().GetMoveCount() > 0) {
								GameObject.Find("DialogueBoxMove").transform.position = new Vector3(Screen.width * 0.3f,Screen.height * 0.7f,0);
								focusHex.changed = true;
								focusHex = this;
								this.pointedAt = true;
								GameBoardEnabled = false;
								StartCoroutine (FindObjectOfType<CharacterStatusLeft>().LeftStatusPop(ctcHex,true));
								StartCoroutine (FindObjectOfType<CharacterStatusRight>().RightStatusPop(ctcHex,false));
							}
						}
					}
				}
			}
		}
	}

	public string toString () {
		string info = "Hex("+xCoord+","+yCoord+","+zCoord+")";
		return info;
	}
	
	public bool HexMoveRange (Character character) {
	
		if(isOccupied){
			return false;
		}
	
		for (int i = 0; i < pathsToCharacters.Count; i++){

			if (pathsToCharacters[i][0].occupant == character) {
				if (pathsToCharacters[i].Count == 0) {
					return false;
				}
				if (pathsToCharacters[i][0].occupant.GetMoveDistance() >= pathsToCharacters[i].Count) {
					return true;
				}
			}
		}
		return false;
	}

	public void SetHeuristic (int heuristic) {
		h = heuristic;
	}
	
	public int GetHeurisitic () {
		return h;
	}
	
	public void SetMovementCost (int cost) {
		m = cost;
	}
	
	public int GetMovementCost () {
		return m;
	}
	
	public void SetFValue (int fValue) {
		f = fValue;
	}
	
	public int GetFValue () {
		return f;
	}

	public int GetState () {
		return pathState;
	}

	public void SetState (int state) {
		pathState = state;
	}
	
	public void SetParent (GameBoardHex parent) {
		p = parent;
	}
	
	public GameBoardHex GetParent () {
		return p;
	}

	public void SetOnTop (bool onTop) {
		isOnTop = onTop;
	}

	public bool IsOnTop () {
		return isOnTop;
	}
	
	public static GameBoardHex GetFocusHex() {
		return focusHex;
	}
	
	public static void SetFocusHex(GameBoardHex hex) {
		focusHex = hex;
	}

	public static void SetCTCHex(GameBoardHex hex) {
		ctcHex = hex;
	}
	
	public static GameBoardHex GetCTCHex() {
		return ctcHex;
	}

	public void CTCHex() {
		foreach (Transform hexBlock in GameObject.Find ("HexTerrain").transform) {
			if (hexBlock.GetComponent<GameBoardHex>().IsOnTop()) {
				if (hexBlock.GetComponent<GameBoardHex>().isOccupied) {
					if (hexBlock.GetComponent<GameBoardHex>() == hexBlock.GetComponent<GameBoardHex>().occupant.GetCurrentTurnCharacter()) {
						ctcHex = hexBlock.GetComponent<GameBoardHex>();
					}
				}
			}
		}
	}

	public void setCoordinates(int x, int y, int z) {
		this.xCoord = x;
		this.yCoord = y;
		this.zCoord = z;
	}

	public int GetXCoord () {
		return xCoord;
	}

	public int GetYCoord () {
		return yCoord;
	}

	public int GetZCoord () {
		return zCoord;
	}

	public List<List<GameBoardHex>> GetPathsToCharacters() {
		return pathsToCharacters;
	}

	public void ReCreatedPathsToCharacters() {
		pathsToCharacters = new List<List<GameBoardHex>>();
	}

	public bool IsOccupied() {
		return isOccupied;
	}

	public void SetOccupant(Character character) {
		occupant = character;
		isOccupied = true;
	}
	
	public Character GetOccupant() {
		return occupant;
	}

	public void NotOccupied() {
		occupant = null;
		isOccupied = false;
	}
	
	public IEnumerator Move (GameBoardHex destHex) {
		GameCamera.DoneMoving = false;
		Character character = ctcHex.occupant;
		character.SetMoved(true);
		Debug.Log ("Moving Character (Animation Input Needed)");
		GameBoardEnabled = false; //Stops mouse up functionality
		yield return StartCoroutine(MoveStep(character, destHex));
		GameBoardEnabled = true; //Continues mouse up functionality
		ctcHex.changed = true;
		destHex.SetOccupant (character);
		ctcHex.NotOccupied ();
		character.SetXYZ (destHex.xCoord, destHex.yCoord, destHex.zCoord);
		focusHex.changed = true;
		focusHex = null;
		if (!character.GetHasAttacked()) { /*switch to ! once incorporating attacking*/
			Pathing.AttackMoveGrid(ctcHex, false, character.GetMoved());
			character.NowAttackNeighbors ();
			ctcHex = destHex;
			Pathing.AttackMoveGrid(ctcHex, true, character.GetMoved());
		} else {
			character.CharacterTurnOver ();
			character.NowAttackNeighbors ();
			Pathing.CharacterPaths(); //changed from destHex.CharacterPaths()
			StartCoroutine (FindObjectOfType<CharacterStatusLeft>().LeftStatusPop(ctcHex,false));
		}
		if (GameCamera.ThirdPersonView) {
			StartCoroutine(GameCamera.ThirdPersonCameraAdjust (ctcHex));
		} else {
			StartCoroutine(GameCamera.SetCameraToObject (destHex.gameObject.transform.position));
		}
	}

	private IEnumerator MoveStep(Character ctc, GameBoardHex destination) {
		float step; //non-smoothed
		float rate; //amount to increase non-smooth step by
		float smoothStep; //smooth step this time

		int moveDirection = -1;

		Vector3 dest = ctc.transform.position;

		for (int i = 1; i < destination.pathsToCharacters[0].Count; i++) {

			step = 0.0f;
			rate = 1.0f;
			smoothStep = 0.0f;

			do {
				if (destination.pathsToCharacters [0] [i-1].transform.position.y < destination.pathsToCharacters [0] [i].transform.position.y) {
					if (moveDirection == -1) {
						dest = new Vector3 (ctc.transform.position.x, destination.pathsToCharacters [0] [i].transform.position.y + 0.75f/*yChange*/, ctc.transform.position.z);
						step = 0;
						rate = 2f;
						moveDirection = 0;
					} else {
						dest = new Vector3 (destination.pathsToCharacters [0] [i].transform.position.x, ctc.transform.position.y, destination.pathsToCharacters [0] [i].transform.position.z);
						step = 0;
						rate = 0.2f;
						moveDirection = -1;
					}
					
				} else if (destination.pathsToCharacters [0] [i-1].transform.position.y > destination.pathsToCharacters [0] [i].transform.position.y) {
					if (moveDirection == -1) {
						dest = new Vector3 (destination.pathsToCharacters [0] [i].transform.position.x, ctc.transform.position.y, destination.pathsToCharacters [0] [i].transform.position.z);
						//step = 0;
						rate = 0.2f;
						moveDirection = 0;
					} else {
						dest = new Vector3 (destination.pathsToCharacters [0] [i].transform.position.x, destination.pathsToCharacters [0] [i].transform.position.y + 0.75f/*yChange*/, destination.pathsToCharacters [0] [i].transform.position.z);
						//step = 0;
						rate = 6;
						moveDirection = -1;
					}
				} else {
					dest = new Vector3 (destination.pathsToCharacters [0] [i].transform.position.x, ctc.transform.position.y, destination.pathsToCharacters [0] [i].transform.position.z);
					//step = 0;
					rate = 0.2f;
					moveDirection = -1;
				}
                ctc.transform.LookAt(new Vector3(destination.transform.position.x, ctc.transform.position.y, destination.transform.position.z));

				while (Vector3.Distance (ctc.transform.position,dest) > 0.05f) {
					step += Time.deltaTime * rate * 10; //increase the step
					smoothStep = Mathf.SmoothStep (0.0f, 1.0f, step); //get the smooth step
					ctc.transform.position = Vector3.Lerp (ctc.transform.position, dest, smoothStep);
					yield return 0;
				}
				ctc.transform.position = dest;
			} while (moveDirection != -1);
		}
	}
}

















