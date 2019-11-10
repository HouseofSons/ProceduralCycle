using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class Character : MonoBehaviour {

	private static List<Character> characterList;
	private static Character currentTurnCharacter;
	private static bool friendlyTurn;

	private string characterName;
	private bool isFriendly;

	private int maxHealth;
	private int currentHealth;

	private int moveDistance;
	private int moveCount;
	private bool moved;

	private int attackDistance;
	private bool attacked;
	
	private int statAttack;
	private int statDefense;
	
	private int jumpHeight;
	private int visionDistance;

	private float speed;
	private float currentSpeedStanding;
	
	private int xCoord;
	private int yCoord;
	private int zCoord;
	
	private List<GameBoardHex> nowAttackNeighbors;

	private float blinkStep;
	private bool focusBlink;
	private Color spawnColor;

	private bool pointedAt;
	public static GameBoardHex characterTargetHexLock;
	
	// Use this for initialization
	void Awake () {
		characterList = new List<Character> ();
		blinkStep = 0;
		focusBlink = false;
		attacked = false;
		moved = false;
		pointedAt = false;
		characterTargetHexLock = null;
	}

	void Update() {
		if (ProcessLevel.GetStageAt () >= 3) {
			if (this == currentTurnCharacter || pointedAt) {
				this.gameObject.GetComponent<Renderer> ().material.color = Color.Lerp (spawnColor, Color.gray, blinkStep);
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
			} else {
				if (this.gameObject.GetComponent<Renderer> ().material.color != spawnColor) {
					this.gameObject.GetComponent<Renderer> ().material.color = spawnColor;
				}
			}
		}
	}

	void OnMouseOver() {
		if (GameCamera.DoneMoving) {
			if (characterTargetHexLock == null) {
				if (GetOccupiedHex ().GetIsFocusAttackable()) {
					pointedAt = true;
				}
				GameBoardHex hex = GetOccupiedHex ();
				StartCoroutine (FindObjectOfType<CharacterStatusRight> ().RightStatusPop (hex, true));
			}
		}
	}

	void OnMouseExit() {
		if (characterTargetHexLock == null) {
			pointedAt = false;
			GameBoardHex hex = GetOccupiedHex ();
			StartCoroutine (FindObjectOfType<CharacterStatusRight> ().RightStatusPop (hex, false));
		}
	}
	
	void OnMouseUp () {
		if (!EventSystem.current.IsPointerOverGameObject ()) {
			if (GetOccupiedHex ().GetIsFocusAttackable () && isFriendly != friendlyTurn) {
				GameBoardHex hex = GetOccupiedHex ();
				GameObject.Find ("DialogueBoxAttack").transform.position = new Vector3 (Screen.width * 0.3f, Screen.height * 0.7f, 0);
				GameBoardHex.GameBoardEnabled = false;
				characterTargetHexLock = hex;
				StartCoroutine (FindObjectOfType<CharacterStatusLeft> ().LeftStatusPop (GameBoardHex.GetCTCHex (), true));
				StartCoroutine (FindObjectOfType<CharacterStatusRight> ().RightStatusPop (hex, true));
			}
		}
	}
	
	public GameBoardHex GetOccupiedHex () {
		return GameBoardHexGrid.GameBoard()[xCoord,yCoord,zCoord];
	}

	public static void DestroyCharacter(Character c) {
		Debug.Log ("Destroyed: " + c.ToString());
		characterList.Remove(c);
		GameObject.Destroy(c.gameObject);
	}
	
	//Current Turn Character

	public Character GetCurrentTurnCharacter () {
		return currentTurnCharacter;
	}

	public int GetX () {
		return xCoord;
	}

	public int GetY () {
		return yCoord;
	}
	
	public int GetZ () {
		return zCoord;
	}

	public void SetXYZ (int x, int y, int z) {
		xCoord = x;
		yCoord = y;
		zCoord = z;
	}

	//Friend or Enemy methods
	public bool IsFriendly () {
		return isFriendly;
	}

	public bool GetFriendlyTurn () {
		return friendlyTurn;
	}

	//Character Distance Methods
	public int GetMoveDistance () {
		return moveDistance;
	}
	
	public void SetMoveDistance (int newMoveDistance) {
		moveDistance = newMoveDistance;
	}

	public int GetMoveCount () {
		return moveCount;
	}

	public void SetMoveCount (int moveNum) {
		moveCount = moveNum;
	}

	public bool GetMoved() {
		return moved;
	}

	public void SetMoved(bool move) {
		moved = move;
	}

	//Character Attack Methods
	public int GetAttackDistance () {
		return attackDistance;
	}
	
	public void SetAttackDistance (int newAttackDistance) {
		attackDistance = newAttackDistance;
	}

	public bool GetHasAttacked() {
		return attacked;
	}

	public void SetAttacked(bool attack) {
		attacked = attack;
	}
	
	public void SetJumpHeight(int height) {
		jumpHeight = height;
	}
	
	public int GetJumpHeight() {
		return jumpHeight;
	}

	//Character Name Methods
	public string GetCharacterName () {
		return characterName;
	}

	public void SetCharacterName (string newName) {
		characterName = newName;
	}

	//Character Health Methods
	public int GetMaxHealth () {
		return maxHealth;
	}

	public void SetMaxHealth (int newHealthAmount) {
		maxHealth = newHealthAmount;
	}

	public int GetCurrentHealth () {
		return currentHealth;
	}
	
	public void SetCurrentHealth (int newHealthAmount) {
		currentHealth = newHealthAmount;
	}

	//Character Speed Methods
	public float GetSpeed() {
		return speed;
	}

	public void SetSpeed(float characterSpeed) {
		speed = characterSpeed;
	}

	public float GetCurrentSpeedStanding() {
		return currentSpeedStanding;
	}

	public void SetCurrentSpeedStanding(float currStanding) {
		currentSpeedStanding = currStanding;
	}
	
	public void SetAttackStat (int stat) {
		statAttack = stat;
	}
	
	public int GetAttackStat () {
		return statAttack;
	}
	
	public void SetDefenseStat (int stat) {
		statDefense = stat;
	}
	
	public int GetDefenseStat () {
		return statDefense;
	}

	//Character Visibility Methods
	public int VisionDistance {
		get {return visionDistance;}
		set {visionDistance = value;}
	}

	//Character neighbor Methods
	public List<GameBoardHex> GetNowAttackNeighbors () {
		return nowAttackNeighbors;
	}

	public bool IsNowAattackableNeighbor (GameBoardHex hex) {
		int i = 0;
		do {
			if (nowAttackNeighbors[i] == hex) {
				return true;
			}
			i++;
		} while (i < nowAttackNeighbors.Count);
		return false;
	}
	
	public void NowAttackNeighbors() {
		List<GameBoardHex> nowAttNeighbors = new List<GameBoardHex>();

		foreach(GameBoardHex hex in GameBoardHexGrid.ColumnOfNeighboringHexs(xCoord,yCoord,zCoord,attackDistance,attackDistance)) {
			//if (line of sight to enemy) {
			nowAttNeighbors.Add (hex);
			//}
		}
		nowAttackNeighbors = nowAttNeighbors;
	}

	//Character Turn Order Methods
	public static void CharacterList() {
		GameObject characterObjects = GameObject.Find ("CharacterObjects");
		int count = 0;
		foreach (Transform child in characterObjects.transform) {
			characterList.Add (child.GetComponent<Character>());
			if (count == 0) {
				currentTurnCharacter = child.GetComponent<Character>();
			} else {
				if (child.GetComponent<Character>().GetSpeed() < currentTurnCharacter.GetSpeed()) {
					currentTurnCharacter = child.GetComponent<Character>();
				}
			}
			count++;
		}
		GameBoardHex.SetCTCHex(GameBoardHexGrid.GameBoard()[currentTurnCharacter.GetX(),currentTurnCharacter.GetY(),currentTurnCharacter.GetZ()]);
		friendlyTurn = currentTurnCharacter.isFriendly;
	}

	public void CharacterTurnOver() {
		GameObject characterObjects = GameObject.Find ("CharacterObjects");
		currentTurnCharacter.SetCurrentSpeedStanding(currentTurnCharacter.GetSpeed());
		foreach (Transform child in characterObjects.transform) {
			if (child.GetComponent<Character>() != currentTurnCharacter){
				child.GetComponent<Character>().SetCurrentSpeedStanding(child.GetComponent<Character>().GetCurrentSpeedStanding() - 1);
			}
			if (child.GetComponent<Character>().GetCurrentSpeedStanding() < currentTurnCharacter.GetCurrentSpeedStanding()) {
				currentTurnCharacter = child.GetComponent<Character>();
			}
			child.GetComponent<Character>().moved = false;
		}
		GameBoardHex.SetCTCHex(GameBoardHexGrid.GameBoard()[currentTurnCharacter.GetX(),currentTurnCharacter.GetY(),currentTurnCharacter.GetZ()]);
		friendlyTurn = currentTurnCharacter.GetComponent<Character> ().isFriendly;
	}
	
	//Method for Initializing new Instantiated Characters
	//Needs to be embellished to accomodate Character details

	public void SetCharacterInfo (GameBoardHex HexBlock, int x, int y, int z, bool friendly) {
		HexBlock.SetOccupant(this);
		this.gameObject.transform.position = new Vector3(HexBlock.transform.position.x,HexBlock.transform.position.y + 0.5f + HexWorldTerrain.GetHexScalar()/2.0f, HexBlock.transform.position.z);
		this.gameObject.transform.rotation = Quaternion.identity;
		this.gameObject.transform.parent = FindObjectOfType<PopulateCharacters>().transform;
		
		if (friendly) {
			characterName = "Ally";
		} else {
			characterName = "Enemy";
		}
		
		maxHealth = 100;
		currentHealth = maxHealth;
		moveDistance = 6;
		moveCount = 1;
		attackDistance = 2;
		speed = 1;
		jumpHeight = 4;	
		visionDistance = 6;
		statAttack = 5;
		statDefense = 5;		
		currentSpeedStanding = speed;
		isFriendly = friendly;
		xCoord = x;
		yCoord = y;
		zCoord = z;
		NowAttackNeighbors ();
		spawnColor = this.gameObject.GetComponent<Renderer> ().material.color;
	}
}






















