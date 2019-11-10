using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MainCharacter : MonoBehaviour {

	private static int playerCount = 0;
	private int playerNumber;

	private LevelDesignManager ldm;
	private LevelBuildManager lbm;
	private InteractPlatform plat;
	private Tile spawnOrigin;
	private Tile nextStep;
	private Tile prevStep;
	private Tile hiddenStep;
	private bool characterEngaged;
	private bool characterEnroute;
	private Coroutine routeCoroutine;
	private Furniture hidingSpotFurniture;
	private Trap[] traps;
	private Item[] items;
	private bool hasSneakers;
	private bool hasXRayVision;
	private bool hasToolBelt;
	private bool hasChainWallet;
	private bool hasHoodie;
	
	private Furniture trapPlaceHolder;//used for placing traps

	private RaycastHit northHit;
	private RaycastHit eastHit;
	private RaycastHit southHit;
	private RaycastHit westHit;

	private Transform northTarget;
	private Transform eastTarget;
	private Transform southTarget;
	private Transform westTarget;

	private Transform northTargetLatestHit;
	private Transform eastTargetLatestHit;
	private Transform southTargetLatestHit;
	private Transform westTargetLatestHit;
	
	public float smoothing;
	public float reachedThreshold;
	public float maxSpeed;

	void Awake () {
		playerNumber = playerCount++;
		this.gameObject.tag = "Player_0" + playerCount;
		GameObject platform = Instantiate (Resources.Load ("InteractPlatform")) as GameObject;
		plat = platform.GetComponent<InteractPlatform>();
		ldm = GameObject.FindObjectOfType<LevelDesignManager>();
		lbm = GameObject.FindObjectOfType<LevelBuildManager>();
		spawnOrigin = plat.GetPlatform () [Mathf.FloorToInt (ldm.GetSpawnPoints () [playerCount - 1].x), Mathf.FloorToInt (ldm.GetSpawnPoints () [playerCount - 1].z)];
		nextStep = spawnOrigin;
		this.transform.position = new Vector3(nextStep.location.x,0.5f,nextStep.location.z);
		prevStep = nextStep;
		hiddenStep = null;
		nextStep.characterOccupied = true;
		characterEngaged = false;
		traps = new Trap[3];
			//Adjust based on Character Type
			AddTrap ();//Adds random trap to traps array
			AddTrap ();//Adds random trap to traps array
			AddTrap ();//Adds random trap to traps array
		items = new Item[3];
			//Adjust based on Character Type
			AddItem ();//Adds random Item to items array
		GameObject minimap = Instantiate (Resources.Load ("MiniMap")) as GameObject;
		GameObject birdsEyeCamera = Instantiate (Resources.Load ("BirdsEyeCamera")) as GameObject;
		GameObject mousePointer = Instantiate (Resources.Load ("MousePointer")) as GameObject;
		GameObject canvas = Instantiate (Resources.Load ("Canvas")) as GameObject;
		Item.SynchronizeItemBenefits (this);
		InitializeButtons(canvas);
		lbm.InitializeDarkHalls (this);
		lbm.InitializeDarkRooms (this);
		
	}

	void Update() {

		Ray northRay = new Ray (this.transform.position, new Vector3(0,1,2));
		Ray eastRay = new Ray (this.transform.position, new Vector3(2,1,0));
		Ray southRay = new Ray (this.transform.position, new Vector3(0,1,-2));
		Ray westRay = new Ray (this.transform.position, new Vector3(-2,1,0));

		if (Physics.Raycast(northRay, out northHit, 10f)) {
			if (northHit.collider.tag == "Darkness") {
				northTarget = northHit.collider.transform;
			} else {
				northTarget = null;
			}
		} else {
			northTarget = null;
		}

		if (Physics.Raycast(eastRay, out eastHit, 10f)) {
			if (eastHit.collider.tag == "Darkness") {
				eastTarget = eastHit.collider.transform;
			} else {
				eastTarget = null;
			}
		} else {
			eastTarget = null;
		}

		if (Physics.Raycast(southRay, out southHit, 10f)) {
			if (southHit.collider.tag == "Darkness") {
				southTarget = southHit.collider.transform;
			} else {
				southTarget = null;
			}
		} else {
			southTarget = null;
		}

		if (Physics.Raycast(westRay, out westHit, 10f)) {
			if (westHit.collider.tag == "Darkness") {
				westTarget = westHit.collider.transform;
			} else {
				westTarget = null;
			}
		} else {
			westTarget = null;
		}

		if (northTarget == null || northTarget != northTargetLatestHit) {
			if (northTargetLatestHit != null) {
				if (northTargetLatestHit.gameObject.GetComponent<Darkness> () != null) {
					northTargetLatestHit.gameObject.GetComponent<Darkness> ().northHit = false;
					northTargetLatestHit = null;
				} else {
					DarknessPath.northHit = false;
					northTargetLatestHit = null;
				}
			}
		}

		if (northTarget != null && northTarget != northTargetLatestHit) {
			if (northTarget.gameObject.GetComponent<Darkness> () != null) {
				northTarget.gameObject.GetComponent<Darkness> ().northHit = true;
				northTargetLatestHit = northTarget;
			} else {
				DarknessPath.northHit = true;
				northTargetLatestHit = northTarget;
			}
		}

		if (eastTarget == null || eastTarget != eastTargetLatestHit) {
			if (eastTargetLatestHit != null) {
				if (eastTargetLatestHit.gameObject.GetComponent<Darkness> () != null) {
					eastTargetLatestHit.gameObject.GetComponent<Darkness> ().eastHit = false;
					eastTargetLatestHit = null;
				} else {
					DarknessPath.eastHit = false;
					eastTargetLatestHit = null;
				}
			}
		}

		if (eastTarget != null && eastTarget != eastTargetLatestHit) {
			if (eastTarget.gameObject.GetComponent<Darkness> () != null) {
				eastTarget.gameObject.GetComponent<Darkness> ().eastHit = true;
				eastTargetLatestHit = eastTarget;
			} else {
				DarknessPath.eastHit = true;
				eastTargetLatestHit = eastTarget;
			}
		}

		if (southTarget == null || southTarget != southTargetLatestHit) {
			if (southTargetLatestHit != null) {
				if (southTargetLatestHit.gameObject.GetComponent<Darkness> () != null) {
					southTargetLatestHit.gameObject.GetComponent<Darkness> ().southHit = false;
					southTargetLatestHit = null;
				} else {
					DarknessPath.southHit = false;
					southTargetLatestHit = null;
				}
			}
		}

		if (southTarget != null && southTarget != southTargetLatestHit) {
			if (southTarget.gameObject.GetComponent<Darkness> () != null) {
				southTarget.gameObject.GetComponent<Darkness> ().southHit = true;
				southTargetLatestHit = southTarget;
			} else {
				DarknessPath.southHit = true;
				southTargetLatestHit = southTarget;
			}
		}

		if (westTarget == null || westTarget != westTargetLatestHit) {
			if (westTargetLatestHit != null) {
				if (westTargetLatestHit.gameObject.GetComponent<Darkness> () != null) {
					westTargetLatestHit.gameObject.GetComponent<Darkness> ().westHit = false;
					westTargetLatestHit = null;
				} else {
					DarknessPath.westHit = false;
					westTargetLatestHit = null;
				}
			}
		}

		if (westTarget != null && westTarget != westTargetLatestHit) {
			if (westTarget.gameObject.GetComponent<Darkness> () != null) {
				westTarget.gameObject.GetComponent<Darkness> ().westHit = true;
				westTargetLatestHit = westTarget;
			} else {
				DarknessPath.westHit = true;
				westTargetLatestHit = westTarget;
			}
		}
	}

	public static int PlayerCount()
	{
		return playerCount;
	}

	public int PlayerNumber()
	{
		return playerNumber;
	}
	
	public IEnumerator ExecutePath(List<Tile> tiles,Tile actionTile,int type)
	{
		float smooth = smoothing;
		Vector3 location;
		if (tiles != null) {
			
			characterEnroute = true;
			for (int i = 0;i < tiles.Count;i++) {
				if (tiles[i] == actionTile) {
					plat.PerformAction(actionTile,type);
					break;
				}
				if (!tiles[i].isInteractable()) {
					//collision between maincharacter and object/player or directionchange
					break;
				}
				if (hiddenStep != null) {
					hiddenStep.characterOccupied = false;
					hiddenStep = null;
				}
				tiles[i].characterOccupied = true;
				nextStep = tiles[i];
				location = new Vector3(tiles[i].location.x,0.5f,tiles[i].location.z);
				
				while (Vector3.Distance(this.gameObject.transform.position,location) > reachedThreshold) {
					this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position,location,smooth * Time.deltaTime);
					smooth = Mathf.Min (maxSpeed,2 * smooth);
					yield return null;
				}
				this.gameObject.transform.position = location;
				smooth = smoothing;
				
				//*****Section still called when Coroutine Cancelled START
				prevStep.characterOccupied = false;
				prevStep = nextStep;
				//*****Section still called when Coroutine Cancelled END
				
				if (prevStep.isHiddenDoorFloor()) {
					characterEngaged = true;
					location = prevStep.hiddenDoor.connectingDoor.parentRoom.secretDoorOrigin();
					nextStep = plat.GetPlatform()[Mathf.FloorToInt(location.x),Mathf.FloorToInt(location.z)];
					nextStep.characterOccupied = true;
					hiddenStep = prevStep;
					prevStep = nextStep;
					this.gameObject.transform.position = location;
					characterEngaged = false;
					characterEnroute = false;
					break;
				}
			}
			characterEnroute = false;
		}
	}

	public Tile origin
	{
		get {return spawnOrigin; }
		set {spawnOrigin = value; }
	}

	public Tile nextstep
	{
		get {return nextStep; }
		set {nextStep = value; }
	}
	
	public Tile prevstep
	{
		get {return prevStep; }
		set {prevStep = value; }
	}
	
	public void LookAtTile(Vector3 tileLocation)
	{
		this.gameObject.transform.LookAt(tileLocation);
	}
	
	public bool engaged
	{
		get {return characterEngaged; }
		set { characterEngaged = value; }
	}

	public bool enroute
	{
		get {return characterEnroute; }
		set {characterEnroute = value; }
	}
	
	public Coroutine route
	{
		get {return routeCoroutine; }
		set {routeCoroutine = value; }
	}

	public Furniture hidingSpot
	{
		get {return hidingSpotFurniture; }
		set {hidingSpotFurniture = value; }
	}

	public Furniture placeHolder
	{
		get {return trapPlaceHolder; }
		set {trapPlaceHolder = value; }
	}

	public Trap[] GetTraps()
	{
		return traps;
	}

	public bool AddTrap()
	{
		for (int i = 0; i<traps.Length; i++) {
			if (traps[i] == null) {
				traps[i] = Trap.randomTrap(this);
				return true;
			}
		}
		return false;
	}

	public bool AddTrap(Trap trap)
	{
		for (int i = 0; i<traps.Length; i++) {
			if (traps[i] == null) {
				traps[i] = trap;
				trap.character = this;
				trap.location = null;
				return true;
			}
		}
		return false;
	}

	public Item[] GetItems()
	{
		return items;
	}

	public bool AddItem()
	{
		for (int i = 0; i<items.Length; i++) {
			if (items[i] == null) {
				items[i] = Item.randomItem(this);
				return true;
			}
		}
		return false;
	}
	
	public bool AddItem(Item item)
	{
		for (int i = 0; i<items.Length; i++) {
			if (items[i] == null) {
				items[i] = item;
				item.character = this;
				item.location = null;
				return true;
			}
		}
		return false;
	}
	
	public bool sneakers
	{
		get {return hasSneakers; }
		set {hasSneakers = value; }
	}
	
	public bool xRayVision
	{
		get {return hasXRayVision; }
		set {hasXRayVision = value; }
	}
	
	public bool toolBelt
	{
		get {return hasToolBelt; }
		set {hasToolBelt = value; }
	}
	
	public bool chainWallet
	{
		get {return hasChainWallet; }
		set {hasChainWallet = value; }
	}
	
	public bool hoodie
	{
		get {return hasHoodie; }
		set {hasHoodie = value; }
	}
	
	private void InitializeButtons(GameObject UI)
	{
		foreach(Transform child in UI.transform){
			if (child.name == "HideButton") {
				child.GetComponent<Button>().onClick.AddListener(() => { this.Hide(); });
			}
			if (child.name == "UnhideButton") {
				child.GetComponent<Button>().onClick.AddListener(() => { this.Unhide(); });
			}
			if (child.name == "EscapeButton") {
				child.GetComponent<Button>().onClick.AddListener(() => { this.EscapeUI(); });
			}
			if (child.name == "SetTrapButton_01") {
				child.GetComponent<Button>().onClick.AddListener(() => { this.SetTrap(0); });
			}
			if (child.name == "SetTrapButton_02") {
				child.GetComponent<Button>().onClick.AddListener(() => { this.SetTrap(1); });
			}
			if (child.name == "SetTrapButton_03") {
				child.GetComponent<Button>().onClick.AddListener(() => { this.SetTrap(2); });
			}
		}
	}

	public void Hide()
	{
		this.gameObject.transform.position = new Vector3 (hidingSpotFurniture.origin.x + (hidingSpotFurniture.width - 1)/2f,
		                                                  1,
		                                                  hidingSpotFurniture.origin.z + (hidingSpotFurniture.length - 1)/2f);
		prevStep.characterOccupied = false;//allows other characters to occupy spot next to bed
		hidingSpotFurniture.character = this;
		plat.UIOptionHide (null, false);
		plat.UIOptionTrap01(false,null);
		plat.UIOptionTrap02(false,null);
		plat.UIOptionTrap03(false,null);
		plat.UIOptionEscape (false);
		plat.UIOptionUnhide (hidingSpotFurniture, true);
		this.gameObject.transform.localScale = new Vector3 (0.5f,0.5f,0.5f);//makes smaller to avoid clipping
	}

	public void Unhide()
	{
		if (!prevStep.characterOccupied) {
			this.gameObject.transform.position = new Vector3 (prevStep.location.x, 0.5f, prevStep.location.z);
			prevStep.characterOccupied = true;
		} else {
			Tile spot = plat.FindOpenTile(hidingSpotFurniture);
			nextStep = spot;
			prevStep = spot;
			prevStep.characterOccupied = false;
			this.gameObject.transform.position = new Vector3 (spot.location.x, 0.5f, spot.location.z);
		}
		hidingSpotFurniture.character = null;
		plat.UIOptionUnhide (null, false);
		characterEngaged = false;
		this.gameObject.transform.localScale = new Vector3 (0.75f,0.75f,0.75f);
	}
	
	public void EscapeUI()
	{
		plat.UIOptionHide (null, false);
		plat.UIOptionEscape (false);
		plat.UIOptionTrap01(false,null);
		plat.UIOptionTrap02(false,null);
		plat.UIOptionTrap03(false,null);
		characterEngaged = false;
	}

	public void SetTrap(int trapNum)
	{
		trapPlaceHolder.trap = traps [trapNum];
		traps [trapNum].set = true;
		traps [trapNum] = null;
		trapPlaceHolder = null;
		plat.UIOptionHide (null, false);
		plat.UIOptionEscape (false);
		plat.UIOptionTrap01(false,null);
		plat.UIOptionTrap02(false,null);
		plat.UIOptionTrap03(false,null);
		characterEngaged = false;
	}
}
