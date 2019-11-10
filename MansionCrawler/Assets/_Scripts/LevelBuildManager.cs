using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LevelBuildManager : MonoBehaviour {

	private LevelDesignManager ldm;
	//private Sprite[] edgeSprites;
	private Sprite[] floorSprites;
	private GameObject floor;
	private GameObject doors;
	private GameObject walls;
	private GameObject furniture;
	private GameObject dark;
	private GameObject darkPath;
	private static List<Furniture> furniturePieces;
	
	public GameObject floorHall;
	
	void Start () {
		ldm = GameObject.FindObjectOfType<LevelDesignManager>();
		//edgeSprites = Resources.LoadAll<Sprite>("EDGESPRITES");
		floorSprites = Resources.LoadAll<Sprite>("FLOORSPRITES");
		furniturePieces = new List<Furniture> ();
		
		floor = new GameObject();
		floor.gameObject.name = "Floor";
		floor.transform.position = Vector3.zero;
		floor.transform.parent = this.transform;
		doors = new GameObject();
		doors.gameObject.name = "Doors";
		doors.transform.position = Vector3.zero;
		doors.transform.parent = this.transform;
		walls = new GameObject();
		walls.gameObject.name = "Walls";
		walls.transform.position = Vector3.zero;
		walls.transform.parent = this.transform;
		furniture = new GameObject();
		furniture.gameObject.name = "Furniture";
		furniture.transform.position = Vector3.zero;
		furniture.transform.parent = this.transform;
		dark = new GameObject();
		dark.gameObject.name = "Darkness";
		dark.transform.position = Vector3.zero;
		dark.transform.parent = this.transform;
		darkPath = new GameObject();
		darkPath.gameObject.name = "DarknessPath";
		darkPath.transform.position = Vector3.zero;
		darkPath.transform.parent = this.transform;
		
		InitializeDoorsOfFloor ();
		InitializeHallsOfFloor ();
		//InitializeSecretHallsOfFloor();
		InitializeRoomsOfFloor ();
		InitializeDoors ();
		InitializeEdgesOfWalls ();
		InitializeFurnitureOfRooms ();
		
		//Creates Characters
		for(int i = 0; i < ldm.GetCharacterCount();i++) {
			GameObject character = Instantiate (Resources.Load ("MainCharacter")) as GameObject;
			//Should Assign Characters to players
		}
	}
	
	private void InitializeDoorsOfFloor()
	{
		int count = 0;
	
		foreach (Doorway door in ldm.GetDoors()) {
			GameObject hallFloorDoorGameObject = Instantiate (floorHall, Vector3.zero, Quaternion.Euler (Vector3.zero)) as GameObject;
			hallFloorDoorGameObject.gameObject.name = "Hallway_Door_" + count;
			hallFloorDoorGameObject.transform.position = new Vector3(door.x,0,door.z);
			hallFloorDoorGameObject.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
			hallFloorDoorGameObject.transform.parent = floor.transform;
			hallFloorDoorGameObject.GetComponent<MeshRenderer>().material.mainTexture = textureFromSprite(floorSprites[6]);
			door.doorfloor = hallFloorDoorGameObject;//assigns this GameObject to Door Floor Object
			count++;
		}
	}

	private void InitializeHallsOfFloor()
	{
		int count = 0;
		
		foreach (Hall hall in ldm.GetHalls()) {
			GameObject hallFloorGameObject = Instantiate (floorHall, Vector3.zero, Quaternion.Euler (Vector3.zero)) as GameObject;
			hallFloorGameObject.gameObject.name = "Hallway_" + count;
			hallFloorGameObject.transform.position = hall.MapPosition ();
			hallFloorGameObject.transform.position = new Vector3(hallFloorGameObject.transform.position.x,0,hallFloorGameObject.transform.position.z);
			hallFloorGameObject.transform.localScale = hall.MapScale ();
			hallFloorGameObject.transform.parent = floor.transform;
			if (hall.direction == 1 || hall.direction == 3) {
				hallFloorGameObject.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(hall.width,hall.length);
			} else {
				hallFloorGameObject.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(hall.length,hall.width);
			}
			hallFloorGameObject.GetComponent<MeshRenderer>().material.mainTexture = textureFromSprite(floorSprites[31]);
			hall.hall = hallFloorGameObject;//assigns this GameObject to Hall Object
			count++;
		}
	}

	public void InitializeDarkHalls(MainCharacter character)
	{
		int count = 0;

		foreach (Hall hall in ldm.GetHalls()) {
			GameObject darkPlane = GameObject.CreatePrimitive (PrimitiveType.Cube);
			darkPlane.AddComponent<DarknessPath> ();
			darkPlane.GetComponent<DarknessPath>().character = character;
			darkPlane.transform.position = hall.MapPosition ();
			darkPlane.transform.localScale = new Vector3 (hall.MapScale ().x * 10, hall.MapScale ().y / 10, hall.MapScale ().z * 10);
			darkPlane.transform.position = new Vector3 (darkPlane.transform.position.x,
		                                            darkPlane.transform.position.y + 2.01f,
		                                            darkPlane.transform.position.z);
			darkPlane.GetComponent<MeshRenderer> ().material.color = Color.black;
			darkPlane.gameObject.name = "DarkPlanePath_" + count;
			darkPlane.transform.parent = darkPath.transform;
			darkPlane.tag = "Darkness";
			count++;
		}
	}
	
	private void InitializeSecretHallsOfFloor()
	{
		int count = 0;
		
		foreach (Hall hall in ldm.GetSecretHalls()) {
			GameObject secretHallFloorGameObject = Instantiate (floorHall, Vector3.zero, Quaternion.Euler (Vector3.zero)) as GameObject;
			secretHallFloorGameObject.gameObject.name = "Secret_Hallway_" + count;
			secretHallFloorGameObject.transform.position = hall.MapPosition ();
			secretHallFloorGameObject.transform.position = new Vector3(secretHallFloorGameObject.transform.position.x,0,secretHallFloorGameObject.transform.position.z);
			secretHallFloorGameObject.transform.localScale = hall.MapScale ();
			secretHallFloorGameObject.transform.parent = floor.transform;
			if (hall.direction == 1 || hall.direction == 3) {
				secretHallFloorGameObject.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(hall.width,hall.length);
			} else {
				secretHallFloorGameObject.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(hall.length,hall.width);
			}
			secretHallFloorGameObject.GetComponent<MeshRenderer>().material.mainTexture = textureFromSprite(floorSprites[6]);
			hall.hall = secretHallFloorGameObject;//assigns this GameObject to Secret Hall Object
			count++;
		}
	}

	private void InitializeRoomsOfFloor()
	{
		int count = 0;
		
		foreach (Room room in ldm.GetRooms()) {
			GameObject roomFloorGameObject = Instantiate (floorHall, Vector3.zero, Quaternion.Euler (Vector3.zero)) as GameObject;
			roomFloorGameObject.gameObject.name = "Room_" + count;
			roomFloorGameObject.transform.position = room.MapPosition ();
			roomFloorGameObject.transform.localScale = room.MapScale ();
			roomFloorGameObject.transform.parent = floor.transform;
			if (room.direction == 1 || room.direction == 3) {
				roomFloorGameObject.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(room.width,room.length);
			} else {
				roomFloorGameObject.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(room.length,room.width);
			}
			roomFloorGameObject.GetComponent<MeshRenderer>().material.mainTexture = textureFromSprite(floorSprites[18]);
			room.room = roomFloorGameObject;//assigns this GameObject to Room Object

			count++;
		}
	}

	public void InitializeDarkRooms(MainCharacter character)
	{
		int count = 0;
		
		foreach (Room room in ldm.GetRooms()) {
			GameObject darkPlanePath = GameObject.CreatePrimitive(PrimitiveType.Cube);
			darkPlanePath.AddComponent<Darkness>();
			darkPlanePath.GetComponent<Darkness>().character = character;
			darkPlanePath.transform.position = room.MapPosition ();
			darkPlanePath.transform.localScale = new Vector3(room.MapScale().x * 10,room.MapScale().y / 10,room.MapScale().z * 10);
			darkPlanePath.transform.position = new Vector3 (darkPlanePath.transform.position.x,
			                                                darkPlanePath.transform.position.y + 2.01f,
			                                                darkPlanePath.transform.position.z);
			darkPlanePath.GetComponent<MeshRenderer>().material.color = Color.black;
			darkPlanePath.gameObject.name = "DarkPlane_" + count;
			darkPlanePath.transform.parent = dark.transform;
			darkPlanePath.tag = "Darkness";
			count++;
		}
	}
	
	private void InitializeDoors()
	{
		int count = 0;
		
		foreach (Doorway door in ldm.GetDoors()) {
			GameObject doorGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			doorGameObject.gameObject.name = "Door_" + count;
			doorGameObject.transform.position = new Vector3(door.x,1,door.z);
			doorGameObject.transform.localScale = new Vector3 (1, 2, 1);
			doorGameObject.transform.parent = doors.transform;
			if (!door.roomEntrance) {
				doorGameObject.GetComponent<MeshRenderer>().material.mainTexture = textureFromSprite(floorSprites[21]);
			} else {
				doorGameObject.GetComponent<MeshRenderer>().material.mainTexture = textureFromSprite(floorSprites[20]);
				doorGameObject.GetComponent<BoxCollider>().enabled = false;//so it doesn't impede the platform clicks
			}
			doorGameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
			door.door = doorGameObject;//assigns this GameObject to Door Object
			count++;	
		}
	}
	
	private void InitializeEdgesOfWalls ()
	{
		int count = 0;
		
		foreach (Edge edge in ldm.GetEdges()) {
			GameObject edgeGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			edgeGameObject.gameObject.name = "Wall_" + count;
			edgeGameObject.transform.position = new Vector3 (edge.x, 1, edge.z);
			edgeGameObject.transform.localScale = new Vector3 (1, 2, 1);
			edgeGameObject.transform.parent = walls.transform;
			edgeGameObject.GetComponent<MeshRenderer>().material.mainTexture = textureFromSprite(floorSprites[28]);
			edgeGameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
			edge.edge = edgeGameObject;//assigns this GameObject to Edge Object
			count++;	
		}
	}

	private void InitializeFurnitureOfRooms()
	{
		int count = 0;

		foreach (Room room in ldm.GetRooms()) {
			RoomDesigner(room);
			AssignSecretDoorTriggerToFurniture(room);
		}
		foreach (Furniture piece in furniturePieces) {
			GameObject pieceGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			pieceGameObject.gameObject.name = piece.type + "_" + count;
			pieceGameObject.transform.position = new Vector3 (piece.origin.x + (piece.width - 1)/2f, piece.height/2f, piece.origin.z + (piece.length - 1)/2f);
			pieceGameObject.transform.localScale = new Vector3 (piece.width, piece.height, piece.length);
			pieceGameObject.transform.parent = furniture.transform;
			piece.furniture = pieceGameObject;//assigns this GameObject to Furniture Object
			piece.furniture.GetComponent<BoxCollider>().enabled = false;//so it doesn't impede the platform clicks
			if (Random.Range(0,100) < 5) {//adds traps to 5% of furniture items
				piece.AddTrap();
			}
			count++;
		}
	}

	public static Texture2D textureFromSprite(Sprite sprite)
	{
		if(sprite.rect.width != sprite.texture.width){
			Texture2D newText = new Texture2D((int)sprite.rect.width,(int)sprite.rect.height);
			Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x, 
			                                             (int)sprite.textureRect.y, 
			                                             (int)sprite.textureRect.width, 
			                                             (int)sprite.textureRect.height );
			newText.SetPixels(newColors);
			newText.Apply();
			return newText;
		} else
			return sprite.texture;
	}

	public static List<Furniture> GetFurniture()
	{
		return furniturePieces;
	}

	public static void RoomDesigner(Room room)
	{
		int maxFurniturePieces = Mathf.FloorToInt((room.width * room.length)/16);
		int furnitureCount = 0;
		int attempts = 0;
		int wall = 0;
		bool pieceFits = false;
		int roomType = 1;/*Random.Range(1,7);*/
		int[,] roomfloorPlan;
		Furniture testPiece;

		if (room.direction == 2 || room.direction == 4) {
			roomfloorPlan = new int[room.length,room.width];
		} else {
			roomfloorPlan = new int[room.width,room.length];
		}

		for (int i = 0;i<roomfloorPlan.GetLength(0);i++) {
			for (int j = 0;j<roomfloorPlan.GetLength(1);j++) {
				roomfloorPlan[i,j] = 0;
			}
		}

		do {
		
			attempts = 0;

			while (attempts < 10 && !pieceFits)
			{
				wall = Mathf.FloorToInt(Random.Range(1,5));// limits to only pieces on walls
				testPiece = FurnitureInRoomCheck(room,wall,roomType,roomfloorPlan);

				if (testPiece != null) {
					pieceFits = true;
					furniturePieces.Add(testPiece);
				}
				attempts++;
			}

			if (pieceFits) {
				furnitureCount++;
				pieceFits = false;
				attempts = 0;
			}

		} while (furnitureCount < maxFurniturePieces && attempts < 10);
	}

	private static Furniture FurnitureInRoomCheck(Room room, int wall, int roomType, int[,] roomFloorPlan)
	{
		int x = room.StartingX ();
		int z = room.StartingZ ();
		int xend = room.width;
		int zend = room.length;
		Furniture piece;
		Vector3 origin;

		if (room.direction == 2 || room.direction == 4) {
			xend = room.length;
			zend = room.width;
		}

		origin = Vector3.zero;
		piece = null;

		if (wall == 1) {
			origin = new Vector3 (Random.Range (x, x + xend), 0, z + zend - 1);
			piece = ChooseFurniturePiece (roomType, origin, 3, room);
		}
		if (wall == 2) {
			origin = new Vector3 (x + xend - 1, 0, Random.Range (z, z + zend));
			piece = ChooseFurniturePiece (roomType, origin, 4, room);
		}
		if (wall == 3) {
			origin = new Vector3 (Random.Range (x, x + xend), 0, z);
			piece = ChooseFurniturePiece (roomType, origin, 1, room);
		}
		if (wall == 4) {
			origin = new Vector3 (x, 0, Random.Range (z, z + zend));
			piece = ChooseFurniturePiece (roomType, origin, 2, room);
		}

		if (piece == null) {
			return null;
		}

		for(int i = Mathf.FloorToInt(piece.origin.x);i<Mathf.FloorToInt(piece.origin.x) + piece.width;i++) {
			for(int j = Mathf.FloorToInt(piece.origin.z);j<Mathf.FloorToInt(piece.origin.z + piece.length);j++) {
				if (((i - x >= 0 && i - x < roomFloorPlan.GetLength(0)) && (j - z >= 0 && j - z < roomFloorPlan.GetLength(1))) &&
					((i != room.origin().x) || (j != room.origin().z)) &&//Check if furniture blocks doors
				    ((i != room.secretDoorOrigin().x) || (j != room.secretDoorOrigin().z)) &&//Check if furniture blocks secret doors
					!room.BlocksOtherDoorsInRoom(i,j)) {//Check if furniture blocks other doors
					for (int k = i - x - 1;k <= i - x + 1;k++) {
						for (int l = j - z - 1;l <= j - z + 1;l++) {
							if ((k >= 0 && k < roomFloorPlan.GetLength(0) ) && (l >= 0 && l < roomFloorPlan.GetLength(1))) {
								if (roomFloorPlan[k,l] != 0) {
									return null;
								}
							}
						}	
					}
				} else {
					return null;
				}
			}
		}

		for (int i = Mathf.FloorToInt(piece.origin.x - x); i<Mathf.FloorToInt(piece.origin.x - x + piece.width); i++) {
			for (int j = Mathf.FloorToInt(piece.origin.z - z); j<Mathf.FloorToInt(piece.origin.z - z + piece.length); j++) {
				roomFloorPlan[i,j] = 1;
			}
		}
		room.furniture.Add (piece);
		return piece;
	}

	private static Furniture ChooseFurniturePiece(int roomType,Vector3 origin, int direction,Room room)
	{
		int size = 0;

		if (room.width * room.length > 200) {
			size = 1;
			if (room.width * room.length > 400) {
				size = 2;
			}
		}

		if (roomType == 1) { //BedRoom
			if (room.furniture.Count == 0) {
				if (size == 0) {
					return new BedSmall (origin, direction, room);
				}
				if (size == 1) {
					return new BedMedium (origin, direction, room);
				}
				if (size == 2) {
					return new BedLarge (origin, direction, room);
				}
			}
			if (room.furniture.Count == 1) {
				return new Dresser (origin, direction, room);
			}
			if (room.furniture.Count == 2) {
				return new SideTable (origin, direction, room);
			}
			if (room.furniture.Count == 3) {
				return new Vanity (origin, direction, room);
			}
			if (room.furniture.Count == 4) {
				return new Wardrobe (origin, direction, room);
			}
			if (room.furniture.Count == 5) {
				return new SideTable (origin, direction, room);
			}			
			if (room.furniture.Count == 6) {
				return new Mirror (origin, direction, room);
			}			
			if (room.furniture.Count == 7) {
				return new ChairLow (origin, direction, room);
			}			
			if (room.furniture.Count == 8) {
				return new Bookcase (origin, direction, room);
			}			
			if (room.furniture.Count == 9) {
				return new Fireplace (origin, direction, room);
			}
		}

		return null;

//		BedRoom:
//
//		Bed (small, medium, large)//
//		Wardrobe
//		Dresser
//		Bookshelf
//		Side Table
//		Vanity
//		Mirror
//		Chair
//		Fireplace
//		Trunk??
//		Plant??
//
//
//		Bathroom:
//
//		Toilet
//		Shower & Tub
//		Sink w/mirror
//
//
//		Dining Room:
//
//		Table
//		Chairs
//		Buffet Table
//		Hutch
//		Bar
//
//
//		Closet:
//
//		Wardrobe
//		Bookshelf
//		Mirror
//
//
//		Nursery:
//
//		Crib
//		RockingChair
//		Dresser
//
//
//		Parlor:
//
//		Couch
//		Love Seat
//		Chair
//		PoolTable
//		Bookshelf
//		Bar
//		Picture
//		Fireplace
//
//
//		Office:
//
//		Desk
//		Chair
//		Bookshelf
//		Dresser
//		Picture
	}

	private void AssignSecretDoorTriggerToFurniture(Room room)
	{
		if (room.secretDoor != null) {
			int i = Mathf.FloorToInt(Random.Range (0, room.furniture.Count));
			room.furniture [i].hiddenDoorLocation = room.secretDoorOrigin ();
		}
	}
}




























