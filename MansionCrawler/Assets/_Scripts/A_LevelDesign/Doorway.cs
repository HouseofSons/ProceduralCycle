using UnityEngine;
using System.Collections;

public class Doorway {

	private int xCoord;
	private int zCoord;
	private int doorDirection;
	private Hall doorwayParentHall;
	private Room doorwayParentRoom;
	private Room roomChild;
	private bool roomDoorEntrance;
	private bool hiddenHallEntrance; /*Legacy Code Usage*/
	private bool hiddenHallExit; /*Legacy Code Usage*/
	private GameObject doorGameObject;
	private GameObject doorFloorGameObject;
	private Doorway secretDoorConnection;
	private bool roomExtension;
	private bool roomDemolish;

	public Doorway(int x, int z, int direction, Hall parentHall, Room parentRoom)
	{
		xCoord = x;
		zCoord = z;
		doorDirection = direction;
		doorwayParentHall = parentHall;
		doorwayParentRoom = parentRoom;
		hiddenHallExit = false;
		roomExtension = false;
		roomDemolish = false;
	}
	
	public GameObject door
	{
		get {return doorGameObject; }
		set {doorGameObject = value; }
	}
	
	public void OpenDoor()
	{
		doorGameObject.transform.position = new Vector3(doorGameObject.transform.position.x,
														doorGameObject.transform.position.y + 2,
														doorGameObject.transform.position.z);
	}
	
	public void CloseDoor()
	{
		doorGameObject.transform.position = new Vector3(doorGameObject.transform.position.x,
		                                                doorGameObject.transform.position.y - 2,
		                                                doorGameObject.transform.position.z);
	}
	
	public GameObject doorfloor
	{
		get {return doorFloorGameObject; }
		set {doorFloorGameObject = value; }
	}
	
	public int x
	{
		get {return xCoord; }
	}
	
	public int z
	{
		get {return zCoord; }
	}
	
	public bool roomEntrance
	{
		get {return roomDoorEntrance; }
		set {roomDoorEntrance = value; }
	}
	/*Legacy Code Usage*/
	public bool hiddenHallDoor
	{
		get {return hiddenHallEntrance; }
		set {hiddenHallEntrance = value; }
	}
	/*Legacy Code Usage*/
	public bool hiddenHallEscape
	{
		get {return hiddenHallExit; }
		set {hiddenHallExit = value; }
	}

	public Doorway connectingDoor
	{
		get {return secretDoorConnection; }
		set {secretDoorConnection = value;}
	}

	public Hall parentHall
	{
		get {return doorwayParentHall; }
	}

	public Room parentRoom
	{
		get {return doorwayParentRoom; }
	}

	public Room child
	{
		get {return roomChild; }
		set {roomChild = value; }
	}
	
	public int direction
	{
		get {return doorDirection; }
		set {doorDirection = value; }
	}

	public bool extension
	{
		get {return roomExtension; }
		set {roomExtension = value; }
	}

	public bool demolish
	{
		get {return roomDemolish; }
		set {roomDemolish = value; }
	}
}
