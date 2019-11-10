using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room {
	
	private int roomWidth;
	private int roomLength;
	private int roomDirection;
	private Doorway roomParent;
	private GameObject roomGameObject;
	private Doorway secretHiddenDoor;
	private List<Furniture> furniturePieces;
	private List<Doorway> doorwaysInRoom;

	public Room(Doorway parent, int width, int length, int direction)
	{
		roomParent = parent;
		roomWidth = width;
		roomLength = length;
		roomDirection = parent.direction;
		furniturePieces = new List<Furniture> ();
		doorwaysInRoom = new List<Doorway>();
	}
	
	public GameObject room
	{
		get {return roomGameObject; }
		set {roomGameObject = value; }
	}

	public Doorway secretDoor
	{
		get {return secretHiddenDoor; }
		set {secretHiddenDoor = value;}
	}
	
	public int width
	{
		get {return roomWidth; }
		set {roomWidth = value;}
	}
	
	public int length
	{
		get {return roomLength; }
		set {roomLength = value;}
	}
	
	public int direction
	{
		get {return roomDirection; }
	}
	
	public Doorway parent
	{
		get {return roomParent; }
	}

	public List<Furniture> furniture
	{
		get {return furniturePieces; }
	}

	public List<Doorway> GetDoorWayList
	{
		get {return doorwaysInRoom; }
	}

	public bool BlocksOtherDoorsInRoom(int x, int z)
	{
		foreach (Doorway door in doorwaysInRoom) {
			if (door.direction == 1){
				if (door.x == x && door.z-1 == z) {
					return true;
				}
			}
			if (door.direction == 2){
				if (door.x-1 == x && door.z == z) {
					return true;
				}
			}
			if (door.direction == 3){
				if (door.x == x && door.z+1 == z) {
					return true;
				}
			}
			if (door.direction == 4){
				if (door.x+1 == x && door.z == z) {
					return true;
				}
			}
		}
		return false;
	}

	public Vector3 secretDoorOrigin()
	{
		if (secretHiddenDoor.direction == 1){
			return new Vector3(secretHiddenDoor.x,0,secretHiddenDoor.z - 1);
		}
		if (secretHiddenDoor.direction == 2){
			return new Vector3(secretHiddenDoor.x - 1,0,secretHiddenDoor.z);
		}
		if (secretHiddenDoor.direction == 3){
			return new Vector3(secretHiddenDoor.x,0,secretHiddenDoor.z + 1);
		}
		if (secretHiddenDoor.direction == 4){
			return new Vector3(secretHiddenDoor.x + 1,0,secretHiddenDoor.z);
		}
		return Vector3.zero;
	}
	
	public Vector3 origin()
	{
		if (roomDirection == 1) {
			return new Vector3(roomParent.x,0,roomParent.z + 1);
		}
		if (roomDirection == 2) {
			return new Vector3(roomParent.x + 1,0,roomParent.z);
		}
		if (roomDirection == 3) {
			return new Vector3(roomParent.x,0,roomParent.z - 1);
		}
		if (roomDirection == 4) {
			return new Vector3(roomParent.x - 1,0,roomParent.z);
		}
		return Vector3.zero;
	}
	
	public int StartingX()
	{
		if (roomDirection == 1) {
			return Mathf.CeilToInt(origin().x - ((roomWidth - 1f)/2f));
		}
		
		if (roomDirection == 2) {
			return Mathf.CeilToInt(origin().x);
		}
		
		if (roomDirection == 3) {
			return Mathf.FloorToInt(origin().x - ((roomWidth - 1f)/2f));
		}
		
		if (roomDirection == 4) {
			return Mathf.CeilToInt(origin().x - (roomLength - 1));
		}
		return -1;
	}
	
	public int StartingZ()
	{
		if (roomDirection == 1) {
			return Mathf.CeilToInt(origin().z);
		}
		
		if (roomDirection == 2) {
			return Mathf.FloorToInt(origin().z - ((roomWidth - 1f)/2f));
		}
		
		if (roomDirection == 3) {
			return Mathf.CeilToInt(origin().z - (roomLength - 1));
		}
		
		if (roomDirection == 4) {
			return Mathf.CeilToInt(origin().z - ((roomWidth - 1f)/2f));
		}
		return -1;
	}

	public Vector3 MapPosition()
	{
		Vector3 pos = Vector3.zero ;
		
		if(roomDirection == 1){
			if(roomWidth % 2 == 1) {
				pos = new Vector3(origin().x,0,origin().z + ((roomLength - 1)/2f));
				return pos;
			} else {
				pos = new Vector3(origin().x + (1/2f),0,origin().z + ((roomLength - 1)/2f));
				return pos;
			}
		}
		if(roomDirection == 2){
			if(roomWidth % 2 == 1) {
				pos = new Vector3(origin().x + ((roomLength - 1)/2f),0,origin().z);
				return pos;
			} else {
				pos = new Vector3(origin().x + ((roomLength - 1)/2f),0,origin().z - (1/2f));
				return pos;
			}
		}
		if(roomDirection == 3){
			if(roomWidth % 2 == 1) {
				pos = new Vector3(origin().x,0,origin().z - ((roomLength - 1)/2f));
				return pos;
			} else {
				pos = new Vector3(origin().x - (1/2f),0,origin().z - ((roomLength - 1)/2f));
				return pos;
			}
		}
		if(roomDirection == 4){
			if(roomWidth % 2 == 1) {
				pos = new Vector3(origin().x - ((roomLength - 1)/2f),0,origin().z);
				return pos;
			} else {
				pos = new Vector3(origin().x - ((roomLength - 1)/2f),0,origin().z + (1/2f));
				return pos;
			}
		}
		return pos;
	}
	
	public Vector3 MapScale()
	{
		Vector3 scale = Vector3.zero;
		
		if(roomDirection == 1 || roomDirection == 3){
			scale = new Vector3(roomWidth/10f,1,roomLength/10f);
			return scale;
		}
		if(roomDirection == 2 || roomDirection == 4) {
			scale = new Vector3(roomLength/10f,1,roomWidth/10f);
			return scale;
		}
		return scale;
	}
}
