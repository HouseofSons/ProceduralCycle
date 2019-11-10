using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelDesignManager : MonoBehaviour {

	private Vector3 startPosition;
	private Hall spawnHall;
	private Hall exitHall;
	
	public int characterCount;
	public int mapXCoord;
	public int mapZCoord;
	public int tunnelerCount;
	public int startlifeSpan;
	public int startWidth;
	public int startDirection;
	public int startStepLength;
	public int startTurnChance;
	public int startDoorChance;
	public int startSpaceBetweenDoors;
	public int startChangeWidth;
	public int startWidthMin;
	public int startWidthMax;
	public int startStepLengthMin;
	public int startStepLengthMax;
	public int roomWidthMin;
	public int roomWidthMax;
	public int roomLengthMin;
	public int roomLengthMax;
	public int hiddenHallLifeSpan;
	public int hiddenHallWidth;
	public int hiddenHallStepLengthMin;
	public int hiddenHallStepLengthMax;

	private int[,] mapBlocksDirection;
	private int[,] mapLayoutDesign;
	private List<Hall> halls;
	private List<Hall> secretHalls;
	private List<Doorway> doorways;
	private List<Doorway> secretDoorways;
	private List<Edge> edges;
	private List<Room> rooms;
	private List<Vector3> spawnPoints;

	// Use this for initialization
	void Awake ()
	{
		mapBlocksDirection = new int[mapXCoord + 1,mapZCoord + 1];
		mapLayoutDesign = new int[mapXCoord + 1,mapZCoord + 1];
		//initializes mapBlocksDirectionDirections
		for (int i=0;i<=mapXCoord;i++) {
			for (int j=0;j<=mapZCoord;j++) {
				mapBlocksDirection[i,j] = -1;
				mapLayoutDesign[i,j] = -1;
			}
		}
		
		halls = new List<Hall>();
		secretHalls = new List<Hall>();
		doorways = new List<Doorway>();
		secretDoorways = new List<Doorway>();
		edges = new List<Edge>();
		rooms = new List<Room>();
		spawnPoints = new List<Vector3>();
		TunnelerDesigner start;

		int directionCount = 1;
		startPosition = new Vector3 (Mathf.FloorToInt(Random.Range(mapXCoord/4f,mapXCoord*(3/4f))), 0, 5);

		for (int i = 0; i<tunnelerCount; i++) {

			spawnPoints.Add (startPosition);
			start = new TunnelerDesigner (startlifeSpan, startPosition, mapXCoord, mapZCoord, startWidth, directionCount,
		                                              startStepLength, startTurnChance, startChangeWidth, startWidthMin, startWidthMax,
		                                              startStepLengthMin, startStepLengthMax);
			
			if (i==0) {
				startPosition = new Vector3 (5, 0, Mathf.FloorToInt(Random.Range(mapZCoord/4f,mapZCoord*(3/4f))));
			}
			if (i==1) {
				startPosition = new Vector3 (Mathf.FloorToInt(Random.Range(mapXCoord/4f,mapXCoord*(3/4f))), 0, mapZCoord - 5);
			}
			if (i==2) {
				startPosition = new Vector3 (mapXCoord - 5, 0, Mathf.FloorToInt(Random.Range(mapZCoord/4f,mapZCoord*(3/4f))));
			}
			directionCount++;
		}

		//Creates Basic Hall Floor Plan
		Debug.Log ("Creating Floor Plan");
		foreach(Hall hall in halls) {
			MapDesign (hall,true);
		}
		//Places doors along Floor Plan walls
		Debug.Log ("Placing Doors along Hallways");
		foreach(Hall hall in halls) {
			//excludes first hall
			if (hall.parent != null) {
				PlaceDoorsOnHall(hall);
			}
		}
		//shrinks halls to mark out walls
		Debug.Log ("Removing Hall Padding");
		foreach(Hall hall in halls) {
			RemoveHallPaddingLayer(hall);
		}
		//identify adjusted (smaller) halls to distinguish against walls
		Debug.Log ("Creating Halls");
		foreach(Hall hall in halls) {
			MapDesign (hall,false);
		}
		//Doors left on halls instead of walls are removed
		Debug.Log ("Removing Bad Doors");
		RemoveBadDoors();
		//Corners are identified Also, Doors made on corners are removed
		Debug.Log ("Identifying Corners from Walls");
		CreateCorners(false);//Doesn't ignore doorways
		//Turns Walls into edges
		Debug.Log ("Identifying Edges from Walls");
		CreateEdges();
		//Creates rooms from Hall Doors
		Debug.Log ("Creating Floor Plan for Rooms");
		foreach(Doorway door in doorways) {
			if (CreateRoom (door)) {
				door.roomEntrance = true;
			}
		}
		//Removes unused doorways
		Debug.Log ("Removing unused Doors");
		RemoveUnusedDoors();
		//Creates Doors in Rooms
		Debug.Log ("Creates doors on Rooms for other Rooms to connect");
		foreach(Room room in rooms) {
			if (Random.Range (0,100) > 50) { //half the time
				PlaceDoorOnRoom(room,false);
			}
		}
		//Creates rooms from Room Doors and removes bad doors
		Debug.Log ("Creating Rooms from Doors in Room");
		foreach(Doorway door in doorways) {
			if (door.extension) {
				if (CreateRoom (door)) {
					door.roomEntrance = true;
				} else {
					door.demolish = true;
				}
			}
		}
		//Creates secret Doors in Rooms
		Debug.Log ("Creates hidden doors on Rooms");
		foreach(Room room in rooms) {
			PlaceDoorOnRoom(room,true);
		}
		//Pairs secret doors with each other
		Debug.Log ("Pairs up secret doors");
		MatchDoors();
		//shrinks rooms to mark out walls
		Debug.Log ("Removes Room Padding");
		RemoveRoomPaddingLayer();
		//identify adjusted (smaller) rooms to distinguish against walls
		Debug.Log ("Creating Rooms");
		foreach(Room room in rooms) {
			AddRoomToMap (room,false);
		}
		//Removes unused doorways
		Debug.Log ("Removing unused Doors");
		RemoveUnusedDoors();
		//Corners are identified
		Debug.Log ("Identifying Corners from Walls");
		CreateCorners(true);//Ignores doorways
		//Turns Walls into edges
		Debug.Log ("Identifying Edges from Walls");
		CreateEdges();
	}

	public List<Hall> GetHalls() {
		return halls;
	}
	
	public List<Hall> GetSecretHalls() {
		return secretHalls;
	}

	public List<Edge> GetEdges() {
		return edges;
	}

	public List<Doorway> GetDoors() {
		return doorways;
	}

	public List<Room> GetRooms() {
		return rooms;
	}
	
	public List<Vector3> GetSpawnPoints() {
		return spawnPoints;
	}
	
	public int GetCharacterCount() {
		return characterCount;
	}
	
	public int[,] layoutDesign {
		get {return mapLayoutDesign; }
	}
	
	public void SetSpawnHall(Hall hall)
	{
		spawnHall = hall;
	}
	
	public Hall GetSpawnHall()
	{
		return spawnHall;
	}
	
	public void SetExitHall(Hall hall)
	{
		exitHall = hall;
	}
	
	public Hall GetExitHall()
	{
		return exitHall;
	}

	//Checks if suggested hall is legit
	public bool HallBuildOK(Hall hall, int attempts)
	{
		int xn = hall.StartingX();
		int zn = hall.StartingZ();
		
		//Checks if suggested hall doesn't interfere with desired map features
		if (hall.direction == 1 || hall.direction == 3) {
		
			for(int x=xn;x<=(xn + hall.width - 1);x++) {
				for(int z=zn;z<=(zn + hall.length - 1);z++) {
					//false if out of bounds
					if ((x >= 0 && x <= mapXCoord) && (z >= 0 && z <= mapZCoord)) {
						//identifies if section is parallel
						if (mapBlocksDirection[x,z] != -1) {
							if ((Mathf.Abs(mapBlocksDirection[x,z] - hall.direction) == 2) || (mapBlocksDirection[x,z] - hall.direction == 0)) {
								return false;
							}
						}
					} else {
						return false;
					}
				}
			}
			//sets map section as hall
			for(int x=xn;x<=(xn + hall.width - 1);x++) {
				for(int z=zn;z<=(zn + hall.length - 1);z++) {
					//adjusts direction signal in mapBlocksDirection array for future hall creation checks
					//assumes check in previous loop code block would have identified parallel hall overlap and returned false
					if (mapBlocksDirection[x,z] >= 1 && mapBlocksDirection[x,z] <= 4) {
						mapBlocksDirection[x,z] = 0; /*perpendicular signal*/
					} else {
						mapBlocksDirection[x,z] = hall.direction;
					}
				}
			}
		}
		//Checks if suggested hall doesn't interfere with desired map features
		if (hall.direction == 2 || hall.direction == 4) {
		
			for(int x=xn;x<=(xn + hall.length - 1);x++) {
				for(int z=zn;z<=(zn + hall.width - 1);z++) {
					//false if out of bounds
					if ((x >= 0 && x <= mapXCoord) && (z >= 0 && z <= mapZCoord)) {
						//identifies if section is parallel
						if (mapBlocksDirection[x,z] != -1) {
							if ((Mathf.Abs(mapBlocksDirection[x,z] - hall.direction) == 2) || (mapBlocksDirection[x,z] - hall.direction == 0)) {
								return false;
							}
						}
					} else {
						return false;
					}
				}
			}
			//sets map section as hall
			for(int x=xn;x<=(xn + hall.length - 1);x++) {
				for(int z=zn;z<=(zn + hall.width - 1);z++) {
					//adjusts direction signal in mapBlocksDirection array for future hall creation checks
					//assumes check in previous loop code block would have identified parallel hall overlap and returned false
					if (mapBlocksDirection[x,z] >= 1 && mapBlocksDirection[x,z] <= 4) {
						mapBlocksDirection[x,z] = 0; /*perpendicular signal*/
					} else {
						mapBlocksDirection[x,z] = hall.direction;
					}
				}
			}
		}
		AddHall(hall);
		return true;
	}
	//Adds Hall to Hall List and denotes child
	public void AddHall(Hall hall)
	{
		// ignores first hall
		if (hall.parent != null) {
			hall.parent.child = hall;
		}
		halls.Add (hall);
	}
	//Adds Hall to Hall List and denotes child
	public void AddSecretHall(Hall hall)
	{
		secretHalls.Add (hall);
	}
	//Creates Floor Plan with Walls
	public void MapDesign (Hall hall, bool floorPlan)
	{
	
		int xn = hall.StartingX();
		int zn = hall.StartingZ();
		int xDistance = hall.width;
		int zDistance = hall.length;
		
		if (hall.direction == 2 || hall.direction == 4) {
			xDistance = hall.length;
			zDistance = hall.width;
		}
		
		for(int x=xn;x<=(xn + xDistance - 1);x++) {
			for(int z=zn;z<=(zn + zDistance - 1);z++) {
				//copies building plans for initial layout design
				if (floorPlan) {
					if (mapLayoutDesign[x,z] != 7) {
						mapLayoutDesign[x,z] = 5;//Denotes Walls
					}
				}
				else {
					if (mapBlocksDirection[x,z] == 0) {
						mapLayoutDesign[x,z] = 0;//Denotes Overlap
					} else {
						mapLayoutDesign[x,z] = hall.direction;//Denotes Floor Direction
					}
				}
			}
		}
	}
	//Replaces Corners with edges and removes bad doors
	public void CreateCorners(bool ignoreDoorways)
	{
		bool north = false;
		bool east = false;
		bool south = false;
		bool west = false;
		
		int maxDesignType = 7;
		
		if (ignoreDoorways) {
			maxDesignType = 6;
		}
		
		for (int i=0;i<=mapXCoord;i++) {
			for (int j=0;j<=mapZCoord;j++) {
				if (mapLayoutDesign[i,j] >= 5 && mapLayoutDesign[i,j] <= maxDesignType) {
					if (j+1 <= mapZCoord) {
						if (mapLayoutDesign[i,j+1] >= 5 && mapLayoutDesign[i,j+1] <= 7) {
							north = true;
						}
					}
					if (i+1 <= mapXCoord) {
						if (mapLayoutDesign[i+1,j] >= 5 && mapLayoutDesign[i+1,j] <= 7) {
							east = true;
						}
					}
					if (j-1 >= 0) {
						if (mapLayoutDesign[i,j-1] >= 5 && mapLayoutDesign[i,j-1] <= 7) {
							south = true;
						}
					}
					if (i-1 >= 0) {
						if (mapLayoutDesign[i-1,j] >= 5 && mapLayoutDesign[i-1,j] <= 7) {
							west = true;
						}
					}
					if (north || south) {
						if (east || west) {
						
							if (mapLayoutDesign[i,j] == 7) {
								//remove doorway from doorways list
								doorways.Remove(findDoor(i,j));
							}
							mapLayoutDesign[i,j] = 6;//edge
							if (findEdge(i,j) == null) {
								edges.Add (new Edge(i,j,north,east,south,west,true));
							} else {
								edges.Remove (findEdge(i,j));
								edges.Add (new Edge(i,j,north,east,south,west,true));
							}
						}
					}
					north = false;
					east = false;
					south = false;
					west = false;
				}
			}
		}
	}
	//Turns Walls into Edges
	public void CreateEdges()
	{
		bool north = false;
		bool east = false;
		bool south = false;
		bool west = false;
		
		for (int i=0;i<=mapXCoord;i++) {
			for (int j=0;j<=mapZCoord;j++) {
				if (mapLayoutDesign[i,j] == 5) {
					if (j+1 <= mapZCoord) {
						if (mapLayoutDesign[i,j+1] >= 5 && mapLayoutDesign[i,j+1] <= 7) {
							north = true;
						}
					}
					if (i+1 <= mapXCoord) {
						if (mapLayoutDesign[i+1,j] >= 5 && mapLayoutDesign[i+1,j] <= 7) {
							east = true;
						}
					}
					if (j-1 >= 0) {
						if (mapLayoutDesign[i,j-1] >= 5 && mapLayoutDesign[i,j-1] <= 7) {
							south = true;
						}
					}
					if (i-1 >= 0) {
						if (mapLayoutDesign[i-1,j] >= 5 && mapLayoutDesign[i-1,j] <= 7) {
							west = true;
						}
					}
					mapLayoutDesign[i,j] = 6;//edge
					if (findEdge(i,j) == null) {
						edges.Add (new Edge(i,j,north,east,south,west,false));
					} else {
						edges.Remove (findEdge(i,j));
						edges.Add (new Edge(i,j,north,east,south,west,false));
					}
					north = false;
					east = false;
					south = false;
					west = false;
				}
			}
		}
	}
	//Turns Wall piece into Edge
	public void CreateEdgeOnLocation(int x, int z)
	{
		bool north = false;
		bool east = false;
		bool south = false;
		bool west = false;
		
		if (z+1 <= mapZCoord) {
			if (mapLayoutDesign[x,z+1] >= 5 && mapLayoutDesign[x,z+1] <= 7) {
				north = true;
			}
		}
		if (x+1 <= mapXCoord) {
			if (mapLayoutDesign[x+1,z] >= 5 && mapLayoutDesign[x+1,z] <= 7) {
				east = true;
			}
		}
		if (z-1 >= 0) {
			if (mapLayoutDesign[x,z-1] >= 5 && mapLayoutDesign[x,z-1] <= 7) {
				south = true;
			}
		}
		if (x-1 >= 0) {
			if (mapLayoutDesign[x-1,z] >= 5 && mapLayoutDesign[x-1,z] <= 7) {
				west = true;
			}
		}
		
		mapLayoutDesign[x,z] = 6;//edge
		if (findEdge(x,z) == null) {
			edges.Add (new Edge(x,z,north,east,south,west,false));
		} else {
			edges.Remove (findEdge(x,z));
			edges.Add (new Edge(x,z,north,east,south,west,false));
		}
	}	
	//Finds an edge given an x and y coordinate
	public Edge findEdge(int x, int z)
	{
		foreach(Edge edge in edges) {
			if (edge.x == x && edge.z == z) {
				return edge;
			}
		}
		return null;
	}
	//Removes Layer of Padding
	public void RemoveHallPaddingLayer(Hall hall)
	{
		hall.width = hall.width - 2;
	
		if (hall.parent != null) {
			if (hall.parent.direction == 1 && hall.direction == 1) {
				hall.origin = new Vector3(hall.origin.x,hall.origin.y,hall.origin.z - 1);
			}
			if (hall.parent.direction == 1 && hall.direction == 2) {
				hall.origin = new Vector3(hall.origin.x - 1,hall.origin.y,hall.origin.z);
			}
			if (hall.parent.direction == 1 && hall.direction == 4) {
				hall.origin = new Vector3(hall.origin.x + 1,hall.origin.y,hall.origin.z);
			}
			if (hall.parent.direction == 2 && hall.direction == 1) {
				hall.origin = new Vector3(hall.origin.x,hall.origin.y,hall.origin.z - 1);
			}
			if (hall.parent.direction == 2 && hall.direction == 2) {
				hall.origin = new Vector3(hall.origin.x - 1,hall.origin.y,hall.origin.z);
			}
			if (hall.parent.direction == 2 && hall.direction == 3) {
				hall.origin = new Vector3(hall.origin.x,hall.origin.y,hall.origin.z + 1);
			}
			if (hall.parent.direction == 3 && hall.direction == 2) {
				hall.origin = new Vector3(hall.origin.x - 1,hall.origin.y,hall.origin.z);
			}
			if (hall.parent.direction == 3 && hall.direction == 3) {
				hall.origin = new Vector3(hall.origin.x,hall.origin.y,hall.origin.z + 1);
			}
			if (hall.parent.direction == 3 && hall.direction == 4) {
				hall.origin = new Vector3(hall.origin.x + 1,hall.origin.y,hall.origin.z);
			}
			if (hall.parent.direction == 4 && hall.direction == 1) {
				hall.origin = new Vector3(hall.origin.x,hall.origin.y,hall.origin.z - 1);
			}
			if (hall.parent.direction == 4 && hall.direction == 3) {
				hall.origin = new Vector3(hall.origin.x,hall.origin.y,hall.origin.z + 1);
			}
			if (hall.parent.direction == 4 && hall.direction == 4) {
				hall.origin = new Vector3(hall.origin.x + 1,hall.origin.y,hall.origin.z);
			}
		} else {
			//hall.length = hall.length - 2;
			//hall.origin = new Vector3(hall.origin.x,hall.origin.y,hall.origin.z + 1);//HERE@!!!!@#@!@#$#@!@#$#@#$#@

			if (hall.direction == 1) {
				hall.origin = new Vector3(hall.origin.x,hall.origin.y,hall.origin.z - 1);
			}
			if (hall.direction == 2) {
				hall.origin = new Vector3(hall.origin.x - 1,hall.origin.y,hall.origin.z);
			}
			if (hall.direction == 3) {
				hall.origin = new Vector3(hall.origin.x,hall.origin.y,hall.origin.z + 1);
			}
			if (hall.direction == 4) {
				hall.origin = new Vector3(hall.origin.x + 1,hall.origin.y,hall.origin.z);
			}
		}
	}
	//Removes Layer of Hidden Hall Padding
	public void RemoveHiddenHallPaddingLayer(Hall hall)
	{
		hall.width = hall.width - 2;

		if (hall.parent != null) {
			if (hall.parent.direction == 1 && hall.direction == 1) {
				hall.origin = new Vector3(hall.origin.x,hall.origin.y,hall.origin.z - 1);
			}
			if (hall.parent.direction == 1 && hall.direction == 2) {
				hall.origin = new Vector3(hall.origin.x - 1,hall.origin.y,hall.origin.z);
			}
			if (hall.parent.direction == 1 && hall.direction == 4) {
				hall.origin = new Vector3(hall.origin.x + 1,hall.origin.y,hall.origin.z);
			}
			if (hall.parent.direction == 2 && hall.direction == 1) {
				hall.origin = new Vector3(hall.origin.x,hall.origin.y,hall.origin.z - 1);
			}
			if (hall.parent.direction == 2 && hall.direction == 2) {
				hall.origin = new Vector3(hall.origin.x - 1,hall.origin.y,hall.origin.z);
			}
			if (hall.parent.direction == 2 && hall.direction == 3) {
				hall.origin = new Vector3(hall.origin.x,hall.origin.y,hall.origin.z + 1);
			}
			if (hall.parent.direction == 3 && hall.direction == 2) {
				hall.origin = new Vector3(hall.origin.x - 1,hall.origin.y,hall.origin.z);
			}
			if (hall.parent.direction == 3 && hall.direction == 3) {
				hall.origin = new Vector3(hall.origin.x,hall.origin.y,hall.origin.z + 1);
			}
			if (hall.parent.direction == 3 && hall.direction == 4) {
				hall.origin = new Vector3(hall.origin.x + 1,hall.origin.y,hall.origin.z);
			}
			if (hall.parent.direction == 4 && hall.direction == 1) {
				hall.origin = new Vector3(hall.origin.x,hall.origin.y,hall.origin.z - 1);
			}
			if (hall.parent.direction == 4 && hall.direction == 3) {
				hall.origin = new Vector3(hall.origin.x,hall.origin.y,hall.origin.z + 1);
			}
			if (hall.parent.direction == 4 && hall.direction == 4) {
				hall.origin = new Vector3(hall.origin.x + 1,hall.origin.y,hall.origin.z);
			}
		} else {
			hall.length = hall.length - 1;
		}
	}
	//Removes Layer of Padding
	public void RemoveRoomPaddingLayer()
	{
		foreach(Room room in rooms) {
			room.length--;
			room.width = room.width - 2;
		}
	}
	//Establishes Doorway Locations on Hallways
	public void PlaceDoorsOnHall(Hall hall)
	{
		int xn = hall.StartingX();
		int zn = hall.StartingZ();
		int xDistance = hall.width;
		int zDistance = hall.length;
		int zcounter = Mathf.FloorToInt(Random.Range (0,startSpaceBetweenDoors));
		int xcounter = Mathf.FloorToInt(Random.Range (0,startSpaceBetweenDoors));
		int n = 1;
		int m = 2;//more likely to spawn doors at end of halls
		bool side = true;

		if (hall.direction == 2 || hall.direction == 4) {
			xDistance = hall.length;
			zDistance = hall.width;
			n = 2;//more likely to spawn doors at end of halls
			m = 1;
		}
		
		for(int x=xn;x<=(xn + xDistance - 1);x++) {
			for(int z=zn;z<=(zn + zDistance - 1);z++) {
				if(x == xn || x == (xn + xDistance - 1)) {
					if(z > zn && z < (zn + zDistance - 1)) {
						zcounter++;
						if (zcounter > Mathf.FloorToInt(startSpaceBetweenDoors/n)) {
							if (startDoorChance * n > Mathf.FloorToInt(Random.Range (1,100))) {
								
								if (n == 1) {
									side = true;
								} else {
									side = false;
								}
							
								Doorway door = new Doorway(x,z,findDirection(x,z,side,hall),hall,null);
								doorways.Add (door);
								mapLayoutDesign[x,z] = 7;
								zcounter = 0;
							}
						}
					}
				}
			}
		}

		for(int z=zn;z<=(zn + zDistance - 1);z++) {
			for(int x=xn;x<=(xn + xDistance - 1);x++) {
				if(z == zn || z == (zn + zDistance - 1)) {
					if(x > xn && x < (xn + xDistance - 1)) {
						xcounter++;
						if (xcounter > Mathf.FloorToInt(startSpaceBetweenDoors/m)) {
							if (startDoorChance * m > Mathf.FloorToInt(Random.Range (1,100))) {
								
								if (m == 1) {
									side = true;
								} else {
									side = false;
								}
								
								Doorway door = new Doorway(x,z,findDirection(x,z,side,hall),hall,null);
								doorways.Add (door);
								mapLayoutDesign[x,z] = 7;
								xcounter = 0;
							}
						}
					}
				}
			}
		}
	}
	//Establishes Secret Doorway Locations on Rooms
	public void PlaceDoorOnRoom(Room room,bool secret)
	{
		int xn = room.StartingX();
		int zn = room.StartingZ();
		int xDistance = room.width;
		int zDistance = room.length;
		
		if (room.direction == 2 || room.direction == 4) {
			xDistance = room.length;
			zDistance = room.width;
		}
		
		int direction = 0;
		
		int x = 0;
		int z = 0;
		
		bool finished = false;
		
		do
		{
			x = Random.Range(xn,xn + xDistance);
			z = Random.Range(zn,zn + zDistance);
			
			if( ((x == xn || x == (xn + xDistance - 1)) && (z > zn & z < (zn + zDistance - 1))) ||
				((z == zn || z == (zn + zDistance - 1)) && (x > xn & x < (xn + xDistance - 1)))) {
				if( !(room.parent.direction == 1 && z == zn) &&
					!(room.parent.direction == 2 && x == xn) &&
				   	!(room.parent.direction == 3 && z == (zn + zDistance - 1)) &&
				   	!(room.parent.direction == 4 && x == (xn + xDistance - 1)) ) {
				   	if (!AdjacentDoor(x,z)) {
						if (x == xn) {
							direction = 4;
						} 
						if (x == (xn + xDistance - 1)) {
							direction = 2;
						} 
						if (z == zn) {
							direction = 3;
						} 
						if (z == (zn + zDistance - 1)) {
							direction = 1;
						} 
						
						Doorway door = new Doorway(x,z,direction,null,room);
						doorways.Add (door);
						if (secret) {
							door.roomEntrance = false;
							room.secretDoor = door;
						} else {
							room.GetDoorWayList.Add (door);
							door.roomEntrance = true;
							door.extension = true;
						}
						mapLayoutDesign[x,z] = 7;
						finished = true;
					}
				}
			}
		} while (!finished);
	}
	//determines if there is a doorway around a location
	private bool AdjacentDoor(int x,int z)
	{
		for (int i = Mathf.Max(x-1,0);i<=Mathf.Min(x+1,mapXCoord);i++) {
			for (int j = Mathf.Max(z-1,0);j<=Mathf.Min(z+1,mapZCoord);j++) {
				if (i != x && j != z) {
					if (findDoor(x,z) != null) {
						return true;
					}
				}
			}
		}
		return false;
	}
	//Adds a Door to the doorway list
	public void AddDoor(Doorway door)
	{
		doorways.Add(door);
	}
	//Adds a Door to the secret doorway list (Legacy Usage)
	public void AddSecretDoor(Doorway door)
	{
		if (findEdge(door.x,door.z) != null) {
			edges.Remove (findEdge(door.x,door.z));
		}
		secretDoorways.Add(door);
		door.hiddenHallDoor = true;
	}
	//Finds a door given an x and y coordinate
	public Doorway findDoor(int x, int z)
	{
		foreach(Doorway door in doorways) {
			if (door.x == x && door.z == z) {
				return door;
			}
		}
		return null;
	}
	//Removes bad doors
	public void RemoveBadDoors()
	{
		List<Doorway> badDoors = new List<Doorway>();
		foreach(Doorway door in doorways) {
			if (mapLayoutDesign[door.x,door.z] != 7) {
				badDoors.Add(door);
			}
		}
		foreach(Doorway door in badDoors) {
			doorways.Remove(door);
		}
	}
	//Removes Doors which were never used
	public void RemoveUnusedDoors()
	{
		List<Doorway> unusedDoors = new List<Doorway>();
		foreach(Doorway door in doorways) {
			if (door.roomEntrance == false && door.connectingDoor == null) {
				unusedDoors.Add (door);
			}
			if (door.demolish) {
				unusedDoors.Add (door);
			}
			if (door.roomEntrance) {
				if (door.x == 0 || door.x == mapLayoutDesign.GetLength(0) - 1 || door.z == 0 || door.z == mapLayoutDesign.GetLength(1) - 1) {
					unusedDoors.Add (door);
				} else {
					if (door.direction == 1 || door.direction == 3) {
						if (mapLayoutDesign[door.x,door.z-1] == -1 || mapLayoutDesign[door.x,door.z+1] == -1) {
							unusedDoors.Add (door);
						}
					} else {
						if (mapLayoutDesign[door.x-1,door.z] == -1 || mapLayoutDesign[door.x+1,door.z] == -1) {
							unusedDoors.Add (door);
						}
					}
				}
			}
		}
		foreach(Doorway door in unusedDoors) {
			CreateEdgeOnLocation(door.x,door.z);//turns door into edge
			doorways.Remove(door);
		}
	}
	//Establishes direction of door when placing on Hallway
	public int findDirection(int x, int z, bool sideDoorway, Hall parent)
	{
		if (sideDoorway) {
			if (parent.direction == 1 || parent.direction == 3) {
				if (x < parent.origin.x) {
					return 4;
				} else {
					return 2;
				}
			} else {
				if (z > parent.origin.z) {
					return 1;
				} else {
					return 3;
				}
			}
		} else {
			if (parent.direction == 1) {
				if (z == parent.origin.z) {
					return 3;
				} else {
					return 1;
				}
			}
			if (parent.direction == 3) {
				if (z == parent.origin.z) {
					return 1;
				} else {
					return 3;
				}
			}
			if (parent.direction == 2) {
				if (x == parent.origin.x) {
					return 4;
				} else {
					return 2;
				}
			}
			if (parent.direction == 4) {
				if (x == parent.origin.x) {
					return 2;
				} else {
					return 4;
				}
			}
			return 0;
		}
	}
	//Matches up Secret Doors with each other
	public void MatchDoors()  
	{ 
		List<Doorway> secretDoors = new List<Doorway>();
		
		foreach (Doorway door in doorways) {
			if (!door.roomEntrance) {
				secretDoors.Add (door);
			}
		}

		int n = secretDoors.Count;
		Doorway holder;

		while (n > 1) {  
			n--;  
			int k = Random.Range(0,n + 1);  
			holder = secretDoors[k];  
			secretDoors[k] = secretDoors[n];  
			secretDoors[n] = holder;  
		}

		for (int i = 0;i < Mathf.FloorToInt(secretDoors.Count/2f);i++) {
			secretDoors[i].connectingDoor = secretDoors[i + Mathf.FloorToInt(secretDoors.Count/2f)];
			secretDoors[i + Mathf.FloorToInt(secretDoors.Count/2f)].connectingDoor = secretDoors[i];
		}
	}
	//Creates Room from Doorway
	public bool CreateRoom (Doorway door)
	{
		int width = Mathf.FloorToInt(Random.Range (roomWidthMin,roomWidthMax));
		int length = Mathf.FloorToInt(Random.Range (roomLengthMin,roomLengthMax));
		int attempts = 0;
		
		Room room = new Room(door, width, length, door.direction);
		
		while (!RoomBuildOK(room) && attempts < 20)
		{
			width = Mathf.FloorToInt(Random.Range (roomWidthMin,roomWidthMax));
			length = Mathf.FloorToInt(Random.Range (roomLengthMin,roomLengthMax));
			room = new Room(door, width, length, door.direction);
			attempts++;
		}
		if (attempts < 20) {
			rooms.Add (room);
			door.child = room;
			AddRoomToMap(room,true);
			CloseRoomWalls(room);//for closing walls
			return true;
		} else {
			return false;
		}
	}
	//Adds Room to Map
	public void AddRoomToMap(Room room,bool floorPlan)
	{
		int xn = room.StartingX();
		int zn = room.StartingZ();
		int a = room.width;
		int b = room.length;
		
		if (room.direction == 2 || room.direction == 4) {
			a = room.length;
			b = room.width;
		} 
		
		for(int x=xn;x<=(xn + a - 1);x++) {
			for(int z=zn;z<=(zn + b - 1);z++) {
				if (floorPlan) {
					mapLayoutDesign[x,z] = 5;//Denotes walls
				} else {
					mapLayoutDesign[x,z] = 8;//Denotes Room
				}
			}
		}
	}
	//Checks if suggested Room is legit
	public bool RoomBuildOK(Room room)
	{
		int xn = room.StartingX();
		int zn = room.StartingZ();
		int a = room.width;
		int b = room.length;
		
		if (room.direction == 2 || room.direction == 4) {
			a = room.length;
			b = room.width;
		} 

		for(int x=xn;x<=(xn + a - 1);x++) {
			for(int z=zn;z<=(zn + b - 1);z++) {
				if ((x >= 0 && x <= mapXCoord) && (z >= 0 && z <= mapZCoord)) {
					if ((x > xn && x < (xn + a - 1)) && (z > zn && z < (zn + b - 1))) { //walls can overlap other walls
						if (mapLayoutDesign[x,z] != -1) {
							return false;
						}
					}
				} else {
					return false;
				}
			}
		}
		return true;
	}
	//Adds last wall to Room if doesn't exist
	public void CloseRoomWalls(Room room)
	{
		if (room.direction == 1) {
			for (int i=room.StartingX();i<=(room.StartingX() + room.width - 1);i++) {
				if (mapLayoutDesign[i,room.StartingZ() - 1] == -1) {
					mapLayoutDesign[i,room.StartingZ() - 1] = 5;
				}
			}
		}
		if (room.direction == 2) {
			for (int i=room.StartingZ();i<=(room.StartingZ() + room.width - 1);i++) {
				if (mapLayoutDesign[room.StartingX() - 1,i] == -1) {
					mapLayoutDesign[room.StartingX() - 1,i] = 5;
				}
			}
		}
		if (room.direction == 3) {
			for (int i=room.StartingX();i<=(room.StartingX() + room.width - 1);i++) {
				if (mapLayoutDesign[i,room.StartingZ() + room.length] == -1) {
					mapLayoutDesign[i,room.StartingZ() + room.length] = 5;
				}
			}
		}
		if (room.direction == 4) {
			for (int i=room.StartingZ();i<=(room.StartingZ() + room.width - 1);i++) {
				if (mapLayoutDesign[room.StartingX() + room.length,i] == -1) {
					mapLayoutDesign[room.StartingX() + room.length,i] = 5;
				}
			}
		}
	}
	//Draws Map
	public void MapDraw ()
	{
		GameObject mapPieces = new GameObject();
	
		for (int x=0;x<=mapXCoord;x++) {
			for (int z=0;z<=mapZCoord;z++) {
				if (mapLayoutDesign[x,z] >= 0) {
					//Draws map as 1 by 1 plane squares
					GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
					plane.transform.position = new Vector3(x,2.1f,z);
					plane.transform.localScale = new Vector3(0.1f,1,0.1f);
					plane.transform.parent = mapPieces.transform;
					//Adjusts Color for different halls
					if (mapLayoutDesign[x,z] == 0) { //OVERLAPS
						plane.GetComponent<MeshRenderer>().material.color = Color.blue;
					}
					if (mapLayoutDesign[x,z] == 1) { //NORTH PATH
						plane.GetComponent<MeshRenderer>().material.color = Color.yellow;
					}
					if (mapLayoutDesign[x,z] == 2) { //EAST PATH
						plane.GetComponent<MeshRenderer>().material.color = Color.green;
					}
					if (mapLayoutDesign[x,z] == 3) { //SOUTH PATH
						plane.GetComponent<MeshRenderer>().material.color = Color.red;
					}
					if (mapLayoutDesign[x,z] == 4) { //WEST PATH
						plane.GetComponent<MeshRenderer>().material.color = Color.cyan;
					}
					if (mapLayoutDesign[x,z] == 5) { //WALL
						plane.GetComponent<MeshRenderer>().material.color = Color.black;
					}
					if (mapLayoutDesign[x,z] == 6) { //EDGE
						if (findEdge(x,z).isCorner) {
							plane.GetComponent<MeshRenderer>().material.color = Color.gray;
						} else {
							plane.GetComponent<MeshRenderer>().material.color = Color.gray;
						}
					}
					if (mapLayoutDesign[x,z] == 7) { //DOOR
						if (findDoor(x,z).hiddenHallEscape) {
							plane.GetComponent<MeshRenderer>().material.color = Color.magenta;
						} else {
							if (findDoor(x,z).direction == 1) { //NORTH PATH
								plane.GetComponent<MeshRenderer>().material.color = Color.yellow;
							}
							if (findDoor(x,z).direction == 2) { //EAST PATH
								plane.GetComponent<MeshRenderer>().material.color = Color.green;
							}
							if (findDoor(x,z).direction == 3) { //SOUTH PATH
								plane.GetComponent<MeshRenderer>().material.color = Color.red;
							}
							if (findDoor(x,z).direction == 4) { //WEST PATH
								plane.GetComponent<MeshRenderer>().material.color = Color.cyan;
							}
						}
					}
					if (mapLayoutDesign[x,z] == 8) { //ROOM
						plane.GetComponent<MeshRenderer>().material.color = Color.white;
					}
				}
			}
		}
	}
}
