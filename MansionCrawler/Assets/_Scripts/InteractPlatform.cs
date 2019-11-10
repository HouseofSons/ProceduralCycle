using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InteractPlatform : MonoBehaviour {

	private LevelDesignManager ldm;
	private MainCharacter mc;
	private Tile[,] tilePlatform;
	public Tile tilePrefab;
	public bool tryme;
	
	void Awake() {
		mc = GameObject.FindWithTag ("Player_0" + MainCharacter.PlayerCount ()).GetComponent<MainCharacter>();
		ldm = GameObject.FindObjectOfType<LevelDesignManager>();	
		tilePlatform = new Tile[ldm.mapXCoord+1,ldm.mapZCoord+1];
		InitializePlatformWithTiles ();
		InitializePlatformWithFurniture();
	}
	
	private void InitializePlatformWithTiles ()
	{
		int count = 0;
	
		for (int i=0;i<=ldm.mapXCoord;i++) {
			for (int j=0;j<=ldm.mapZCoord;j++) {
				if ((ldm.layoutDesign[i,j] >= 0 && ldm.layoutDesign[i,j] <= 4) || ldm.layoutDesign[i,j] == 7 || ldm.layoutDesign[i,j] == 8) {
					Tile tile = Instantiate (tilePrefab, new Vector3(i,0.01f,j), Quaternion.identity) as Tile;
					tile.gameObject.name = "Tile_" + ldm.layoutDesign[i,j] + "_" + count;
					tile.GetComponent<MeshRenderer>().receiveShadows = false;
					tile.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
					tile.transform.parent = this.gameObject.transform;
					tile.location = new Vector3(i,0,j);
					tile.characterOccupied = false;
					tile.gameObject.layer = 9;//platform layer
					tilePlatform[i,j] = tile;
					
					if ((ldm.layoutDesign[i,j] >= 0 && ldm.layoutDesign[i,j] <= 4)) {
						tile.isDoorClosed = false;
						tile.widgetOccupied = false;
						tile.SetDoor(null);
						tile.isHiddenDoorWall = false;
					}
					if (ldm.layoutDesign[i,j] == 7) {
						tile.isDoorClosed = true;
						tile.widgetOccupied = true;
						tile.SetDoor(ldm.findDoor(i,j));
						
						if (!ldm.findDoor(i,j).roomEntrance) {
							tile.isHiddenDoorWall = true;
						} else {
							tile.GetComponent<MeshCollider>().enabled = false;
							tile.gameObject.AddComponent<BoxCollider>();
							tile.gameObject.GetComponent<BoxCollider>().center = new Vector3(0,10,0);
							tile.gameObject.GetComponent<BoxCollider>().size = new Vector3(10,20,10);
							tile.isHiddenDoorWall = false;
						}
					}
					if (ldm.layoutDesign[i,j] == 8) {
						tile.isDoorClosed = false;
						tile.widgetOccupied = false;
						tile.SetDoor(null);
						tile.isHiddenDoorWall = false;
					}
					count++;
				}
			}
		}
	}
	
	public Tile[,] GetPlatform()
	{
		return tilePlatform;
	}

	public Tile FindOpenTile(Furniture piece)
	{
		for(int i = Mathf.FloorToInt(piece.origin.x) - 1;i <= Mathf.FloorToInt(piece.origin.x) + piece.width;i++) {
			for(int j = Mathf.FloorToInt(piece.origin.z) - 1;j <= Mathf.FloorToInt(piece.origin.z) + piece.length;j++) {
				if(tilePlatform[i,j] != null) {
					if (tilePlatform[i,j].isInteractable()) {
						return tilePlatform[i,j];
					}
				}
			}
		}
		return null;
	}
	
	public void PerformAction(Tile interactingTile, int actionType)
	{
		if (actionType == 1) { //Open Door
			interactingTile.GetDoor().OpenDoor();//physically adjusts door
			interactingTile.widgetOccupied = false;//considers tile a viable path
			interactingTile.isDoorClosed = false;//identifies door is open
			interactingTile.GetComponent<BoxCollider>().enabled = false;//allows rays to pass through once open
			interactingTile.GetComponent<MeshCollider>().enabled = true;
		}
		if (actionType == 2) { //Close Door
			interactingTile.GetDoor().CloseDoor();//physically adjusts door
			interactingTile.widgetOccupied = true;//considers tile a viable path
			interactingTile.isDoorClosed = true;//identifies door is open
			interactingTile.GetComponent<BoxCollider>().enabled = true;
			interactingTile.GetComponent<MeshCollider>().enabled = false;
		}
		if (actionType == 3) { //Furniture Interaction
			Debug.Log ("Furniture Clicked");
			bool foundCharacter = false;
			bool trapSprung = false;
			bool noRoomForTrap = false;

			if (interactingTile.furniture.hiddenDoorLocation != Vector3.zero) {
				Debug.Log ("Hidden Door Opened");//Add hidden door trigger logic
				Tile tile = tilePlatform[Mathf.FloorToInt(interactingTile.furniture.hiddenDoorLocation.x),Mathf.FloorToInt(interactingTile.furniture.hiddenDoorLocation.z)];
				tile.hiddenDoor = interactingTile.furniture.room.secretDoor;
				tile.GetComponent<Renderer>().material.color = Color.white;
				tile.GetComponent<MeshRenderer>().receiveShadows = true;
				tile.GetComponent<MeshRenderer>().material.mainTexture = LevelBuildManager.textureFromSprite(Resources.LoadAll<Sprite>("FLOORSPRITES")[23]);
			}

			if (interactingTile.furniture.hidable) {
				if (interactingTile.furniture.character != null) {
					Debug.Log ("Found Character");
					foundCharacter = true;
					//Stun Character and take item(s)
				}
			}

			if (interactingTile.furniture.trappable) {
				if (interactingTile.furniture.trap != null && interactingTile.furniture.trap.set) {
					Debug.Log ("Trap Sprung!");
					SpringTrap(interactingTile.furniture.trap);
					interactingTile.furniture.trap = null;
					trapSprung = true;
				}
			}

			if (interactingTile.furniture.hidable && !foundCharacter && !trapSprung) {
				mc.engaged = true;//engaged when UI options are available
				UIOptionHide(interactingTile.furniture,true);
				UIOptionEscape(true);
			}

			if (interactingTile.furniture.trappable && !foundCharacter && !trapSprung) {
				if (interactingTile.furniture.trap != null) {
					Debug.Log ("Found a Trap Item");
					if (mc.AddTrap(interactingTile.furniture.trap)) {
						Debug.Log ("Added Trap to inventory");
						interactingTile.furniture.trap = null;
					} else {
						Debug.Log ("No room in inventory");
						noRoomForTrap = true;
					}
				}

				if (!noRoomForTrap) {
					mc.placeHolder = interactingTile.furniture;
					if (mc.GetTraps ()[0] != null) {
						UIOptionTrap01(true,mc.GetTraps ()[0]);
						UIOptionEscape(true);
						mc.engaged = true;//engaged when UI options are available
					}
					if (mc.GetTraps ()[1] != null) {
						UIOptionTrap02(true,mc.GetTraps ()[1]);
						UIOptionEscape(true);
						mc.engaged = true;//engaged when UI options are available
					}
					if (mc.GetTraps ()[2] != null) {
						UIOptionTrap03(true,mc.GetTraps ()[2]);
						UIOptionEscape(true);
						mc.engaged = true;//engaged when UI options are available
					}
				}
			}
		}
	}

	public void UIOptionHide(Furniture piece,bool enable)
	{
		if (enable) {
			mc.hidingSpot = piece;
			GameObject.Find ("HideButton").GetComponent<Image> ().enabled = true;
			GameObject.Find ("HideButton").GetComponent<Button> ().enabled = true;
			GameObject.Find ("HideButton").GetComponentInChildren<Text>().enabled = true;
		} else {
			GameObject.Find ("HideButton").GetComponent<Image> ().enabled = false;
			GameObject.Find ("HideButton").GetComponent<Button> ().enabled = false;
			GameObject.Find ("HideButton").GetComponentInChildren<Text>().enabled = false;
		}
	}

	public void UIOptionUnhide(Furniture piece,bool enable)
	{
		if (enable) {
			mc.hidingSpot = piece;
			GameObject.Find ("UnhideButton").GetComponent<Image> ().enabled = true;
			GameObject.Find ("UnhideButton").GetComponent<Button> ().enabled = true;
			GameObject.Find ("UnhideButton").GetComponentInChildren<Text>().enabled = true;
		} else {
			mc.hidingSpot = null;
			GameObject.Find ("UnhideButton").GetComponent<Image> ().enabled = false;
			GameObject.Find ("UnhideButton").GetComponent<Button> ().enabled = false;
			GameObject.Find ("UnhideButton").GetComponentInChildren<Text>().enabled = false;
		}
	}

	public void UIOptionTrap01(bool enable,Trap trap)
	{
		if (enable) {
			GameObject.Find ("SetTrapButton_01").GetComponent<Image> ().enabled = true;
			GameObject.Find ("SetTrapButton_01").GetComponent<Button> ().enabled = true;
			GameObject.Find ("SetTrapButton_01").GetComponentInChildren<Text>().text = trap.type;
			GameObject.Find ("SetTrapButton_01").GetComponentInChildren<Text>().enabled = true;
		} else {
			GameObject.Find ("SetTrapButton_01").GetComponent<Image> ().enabled = false;
			GameObject.Find ("SetTrapButton_01").GetComponent<Button> ().enabled = false;
			GameObject.Find ("SetTrapButton_01").GetComponentInChildren<Text>().enabled = false;
		}
	}

	public void UIOptionTrap02(bool enable,Trap trap)
	{
		if (enable) {
			GameObject.Find ("SetTrapButton_02").GetComponent<Image> ().enabled = true;
			GameObject.Find ("SetTrapButton_02").GetComponent<Button> ().enabled = true;
			GameObject.Find ("SetTrapButton_02").GetComponentInChildren<Text>().text = trap.type;
			GameObject.Find ("SetTrapButton_02").GetComponentInChildren<Text>().enabled = true;
		} else {
			GameObject.Find ("SetTrapButton_02").GetComponent<Image> ().enabled = false;
			GameObject.Find ("SetTrapButton_02").GetComponent<Button> ().enabled = false;
			GameObject.Find ("SetTrapButton_02").GetComponentInChildren<Text>().enabled = false;
		}
	}

	public void UIOptionTrap03(bool enable,Trap trap)
	{
		if (enable) {
			GameObject.Find ("SetTrapButton_03").GetComponent<Image> ().enabled = true;
			GameObject.Find ("SetTrapButton_03").GetComponent<Button> ().enabled = true;
			GameObject.Find ("SetTrapButton_03").GetComponentInChildren<Text>().text = trap.type;
			GameObject.Find ("SetTrapButton_03").GetComponentInChildren<Text>().enabled = true;
		} else {
			GameObject.Find ("SetTrapButton_03").GetComponent<Image> ().enabled = false;
			GameObject.Find ("SetTrapButton_03").GetComponent<Button> ().enabled = false;
			GameObject.Find ("SetTrapButton_03").GetComponentInChildren<Text>().enabled = false;
		}
	}

	public void UIOptionEscape(bool enable)
	{
		if (enable) {
			GameObject.Find ("EscapeButton").GetComponent<Image> ().enabled = true;
			GameObject.Find ("EscapeButton").GetComponent<Button> ().enabled = true;
			GameObject.Find ("EscapeButton").GetComponentInChildren<Text>().enabled = true;
		} else {
			GameObject.Find ("EscapeButton").GetComponent<Image> ().enabled = false;
			GameObject.Find ("EscapeButton").GetComponent<Button> ().enabled = false;
			GameObject.Find ("EscapeButton").GetComponentInChildren<Text>().enabled = false;
		}
	}

	public void InitializePlatformWithFurniture()
	{
		foreach (Furniture piece in LevelBuildManager.GetFurniture()) {
			for (int i = Mathf.FloorToInt(piece.origin.x);i < Mathf.FloorToInt(piece.origin.x) + piece.width;i++) {
				for (int j = Mathf.FloorToInt(piece.origin.z);j < Mathf.FloorToInt(piece.origin.z) + piece.length;j++) {
					tilePlatform[i,j].GetComponent<MeshCollider>().enabled = false;
					tilePlatform[i,j].gameObject.AddComponent<BoxCollider>();
					if (piece.height == 1) {
						tilePlatform[i,j].gameObject.GetComponent<BoxCollider>().center = new Vector3(0,5,0);
						tilePlatform[i,j].gameObject.GetComponent<BoxCollider>().size = new Vector3(10,10,10);
					} else {//height of 2
						tilePlatform[i,j].gameObject.GetComponent<BoxCollider>().center = new Vector3(0,10,0);
						tilePlatform[i,j].gameObject.GetComponent<BoxCollider>().size = new Vector3(10,20,10);
					}
					tilePlatform[i,j].furniture = piece;
					tilePlatform[i,j].widgetOccupied = true;
				}
			}
		}
	}

	public void SpringTrap(Trap trap)
	{
		if (trap.type == "Zap") {
			//coroutine with zap animation
			//character drops item(s)
			//mc.BackToOrigin();
		}
		if (trap.type == "RocketBoard") {
			//coroutine with RocketBoard animation and movement
			//character drops item(s) at final location
		}
		if (trap.type == "SpringBoard") {
			//coroutine with SpringBoard animation
			//character drops item(s) random locations
			//mc.RandomMapLocation();
		}
	}
	
	/*ASTAR PATHING SECTION BEGIN*/
	
	public void CalculateHeuristics(Tile characterTile, Tile destination)
	{
		for (int i = Mathf.Max(Mathf.FloorToInt(characterTile.location.x) - 20,0);i <= Mathf.Min (Mathf.FloorToInt(characterTile.location.x) + 20,ldm.mapXCoord);i++) {
			for (int j = Mathf.Max(Mathf.FloorToInt(characterTile.location.z) - 15,0);j <= Mathf.Min (Mathf.FloorToInt(characterTile.location.z) + 15,ldm.mapZCoord);j++) {
					if (tilePlatform[i,j] != null) {
					tilePlatform[i,j].SetHeuristic(Mathf.FloorToInt(Mathf.Abs(tilePlatform[i,j].location.x - destination.location.x)
						+ Mathf.Abs(tilePlatform[i,j].location.z - destination.location.z)));
				}
			}
		}
	}
	
	public List<Tile> AStarPath(Tile character, Tile destination)
	{
		List<Tile> closedList = new List<Tile>();
		List<Tile> openList = new List<Tile>();
	
		character.SetG(0);
		character.SetParent(null);
		
		Tile objectTile = character;
		
		do {
		
			closedList.Add (objectTile);
			
			if (openList.Count > 0) {
				openList.Remove (objectTile);
			}

			if (objectTile.GetHeuristic() == 1) {
				return TraceTile(objectTile, destination);
			}
			
			for (int i = Mathf.FloorToInt(objectTile.location.x) - 1; i <= Mathf.FloorToInt(objectTile.location.x) + 1; i++) {
				for (int j = Mathf.FloorToInt(objectTile.location.z) - 1; j <= Mathf.FloorToInt(objectTile.location.z) + 1; j++) {
					if (Mathf.FloorToInt(objectTile.location.x) == i || Mathf.FloorToInt(objectTile.location.z) == j) { //removes diagonal movement
						if (tilePlatform[i,j] != null) {
							if (tilePlatform[i,j].isInteractable()) {
								if (!closedList.Contains(tilePlatform[i,j])) {
									if (openList.Contains(tilePlatform[i,j])) {
										if ((objectTile.GetG() + MovementCost(objectTile,tilePlatform[i,j])) < tilePlatform[i,j].GetG()) {
											tilePlatform[i,j].SetParent(objectTile);
										}
									} else {
										tilePlatform[i,j].SetParent(objectTile);
										openList.Add (tilePlatform[i,j]);
										tilePlatform[i,j].SetG(tilePlatform[i,j].GetParent().GetG() + MovementCost(objectTile,tilePlatform[i,j]));
										tilePlatform[i,j].SetF(tilePlatform[i,j].GetG () + tilePlatform[i,j].GetHeuristic());
									}
								}
							}
						}
					}
				}
			}
		
			if (openList.Count == 0) {
				objectTile = null;
			} else {
				objectTile = SmallestF(openList);
			}
		
		} while (objectTile != null);
		return null;
	}
	
	private List<Tile> TraceTile(Tile objectTile, Tile destination)
	{
		Tile start = objectTile;
		List<Tile> path = new List<Tile>();
		
		path.Add(destination);
		
		while (start.GetParent() != null)
		{
			path.Add (start);
			start = start.GetParent();
		}
		
		path.Reverse();
		return path;
	}
	
	private float MovementCost(Tile center, Tile edge)
	{
		if (center.location.x != edge.location.x && center.location.z != edge.location.z) {
			return 14;
		} else {
			return 10;
		}
	}
	
	private Tile SmallestF(List<Tile> openList)
	{
		Tile smallest = openList[openList.Count-1];
		
		foreach(Tile tile in openList)
		{
			if (tile.GetF() < smallest.GetF()) {
				smallest = tile;
			}
		}
		
		return smallest;
	}
	
	/*ASTAR PATHING SECTION END*/
}





