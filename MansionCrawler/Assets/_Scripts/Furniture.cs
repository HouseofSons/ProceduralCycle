using UnityEngine;
using System.Collections;

public class Furniture {

	private string furnitureType;
	public Vector3 furnitureOrigin;
	public int furnitureWidth;
	private int furnitureHeight;
	public int furnitureLength;
	private int furnitureDirection;
	private Room furnitureRoom;
	private GameObject furnitureGameObject;
	private bool isHidable;
	private bool isTrappable;

	private Trap furnitureTrap;
	private MainCharacter hiddenCharacter;
	//private Items hiddenItem;
	private Vector3 hiddenDoorTriggerLocation;


	public Furniture (Vector3 origin, int width, int height, int length, int direction, Room room, string type, bool hide, bool trap)
	{
		furnitureType = type;
		furnitureDirection = direction;
		furnitureHeight = height;
		isHidable = hide;
		isTrappable = trap;

		if (furnitureDirection == 2 || furnitureDirection == 4) {
			furnitureWidth = length;
			furnitureLength = width;
		} else {
			furnitureWidth = width;
			furnitureLength = length;
		}
		furnitureOrigin = origin;
		if (direction == 3) {
			furnitureOrigin = new Vector3(origin.x,origin.y,origin.z - furnitureLength + 1);
		}
		if (direction == 4) {
			furnitureOrigin = new Vector3(origin.x - furnitureWidth + 1,origin.y,origin.z);
		}
		furnitureRoom = room;
		hiddenDoorTriggerLocation = Vector3.zero;
		hiddenCharacter = null;
	}

	public string type
	{
		get {return furnitureType; }
		set {furnitureType = value; }
	}

	public GameObject furniture
	{
		get {return furnitureGameObject; }
		set {furnitureGameObject = value; }
	}

	public Vector3 origin
	{
		get {return furnitureOrigin; }
	}

	public int width
	{
		get {return furnitureWidth; }
	}

	public int height
	{
		get {return furnitureHeight; }
	}

	public int length
	{
		get {return furnitureLength; }
	}

	public int direction
	{
		get {return furnitureDirection; }
	}

	public Room room
	{
		get {return furnitureRoom; }
	}

	public Vector3 hiddenDoorLocation
	{
		get {return hiddenDoorTriggerLocation; }
		set {hiddenDoorTriggerLocation = value; }
	}

	public bool hidable
	{
		get {return isHidable; }
	}

	public bool trappable
	{
		get {return isTrappable; }
	}

	//public Item item
	//{
	//	get {return hiddenItem; }
	//	set {hiddenItem = value; }
	//}

	public MainCharacter character
	{
		get {return hiddenCharacter; }
		set {hiddenCharacter = value; }

	}

	public Trap trap
	{
		get {return furnitureTrap; }
		set {furnitureTrap = value; }
	}
	
	public bool AddTrap()
	{
		if (furnitureTrap == null) {
			furnitureTrap = Trap.randomTrap(this);
			return true;
		}
		return false;
	}

	public bool AddTrap(Trap trap)
	{
		if (furnitureTrap == null) {
			furnitureTrap = trap;
			trap.location = this;
			return true;
		}
		return false;
	}
}

public class Bookcase : Furniture
{
	public Bookcase (Vector3 origin, int direction, Room room) : base (origin,4,2,1,direction,room,"Bookcase",false,false){}
}

public class BedLarge : Furniture
{
	public BedLarge (Vector3 origin, int direction, Room room) : base (origin,6,1,5,direction,room,"BedLarge",true,false){}
}

public class BedMedium : Furniture
{
	public BedMedium (Vector3 origin, int direction, Room room) : base (origin,5,1,5,direction,room,"BedMedium",true,false){}
}

public class BedSmall : Furniture
{
	public BedSmall (Vector3 origin, int direction, Room room) : base (origin,3,1,4,direction,room,"BedSmall",true,false){}
}

public class Wardrobe : Furniture
{
	public Wardrobe (Vector3 origin, int direction, Room room) : base (origin,4,2,1,direction,room,"Wardrobe",true,true){}
}

public class Dresser : Furniture
{
	public Dresser (Vector3 origin, int direction, Room room) : base (origin,4,1,1,direction,room,"Dresser",false,true){}
}

public class SideTable : Furniture
{
	public SideTable (Vector3 origin, int direction, Room room) : base (origin,1,1,1,direction,room,"SideTable",false,true){}
}

public class Vanity : Furniture
{
	public Vanity (Vector3 origin, int direction, Room room) : base (origin,3,1,1,direction,room,"Vanity",false,true){}
}

public class Mirror : Furniture
{
	public Mirror (Vector3 origin, int direction, Room room) : base (origin,2,2,1,direction,room,"Mirror",false,false){}
}

public class ChairLow : Furniture
{
	public ChairLow (Vector3 origin, int direction, Room room) : base (origin,2,1,2,direction,room,"ChairLow",false,false){}
}

public class ChairRocking : Furniture
{
	public ChairRocking (Vector3 origin, int direction, Room room) : base (origin,2,1,2,direction,room,"ChairRocking",false,false){}
}

public class ChairLoveseat : Furniture
{
	public ChairLoveseat (Vector3 origin, int direction, Room room) : base (origin,3,1,2,direction,room,"ChairLoveseat",false,true){}
}

public class Fireplace : Furniture
{
	public Fireplace (Vector3 origin, int direction, Room room) : base (origin,4,1,1,direction,room,"Fireplace",true,false){}
}

public class Toilet : Furniture
{
	public Toilet (Vector3 origin, int direction, Room room) : base (origin,1,1,1,direction,room,"Toilet",false,true){}
}

public class ShowerTub : Furniture
{
	public ShowerTub (Vector3 origin, int direction, Room room) : base (origin,4,2,2,direction,room,"ShowerTub",true,false){}
}

public class Sink : Furniture
{
	public Sink (Vector3 origin, int direction, Room room) : base (origin,2,2,1,direction,room,"Sink",false,false){}
}

public class TableAndChairsSmall : Furniture
{
	public TableAndChairsSmall (Vector3 origin, int direction, Room room) : base (origin,4,1,6,direction,room,"TableAndChairsSmall",true,false){}
}

public class TableAndChairsLarge : Furniture
{
	public TableAndChairsLarge (Vector3 origin, int direction, Room room) : base (origin,4,1,8,direction,room,"TableAndChairsLarge",true,false){}
}

public class Buffet : Furniture
{
	public Buffet (Vector3 origin, int direction, Room room) : base (origin,3,1,1,direction,room,"Buffet",false,true){}
}

public class Hutch : Furniture
{
	public Hutch (Vector3 origin, int direction, Room room) : base (origin,3,1,2,direction,room,"Hutch",false,true){}
}

public class Bar : Furniture
{
	public Bar (Vector3 origin, int direction, Room room) : base (origin,4,2,2,direction,room,"Bar",true,true){}
}

public class Crib : Furniture
{
	public Crib (Vector3 origin, int direction, Room room) : base (origin,2,1,1,direction,room,"Crib",false,true){}
}

public class CouchLarge : Furniture
{
	public CouchLarge (Vector3 origin, int direction, Room room) : base (origin,4,1,2,direction,room,"CouchLarge",false,true){}
}

public class CouchMedium : Furniture
{
	public CouchMedium (Vector3 origin, int direction, Room room) : base (origin,5,1,2,direction,room,"CouchMedium",false,true){}
}

public class PoolTableSmall : Furniture
{
	public PoolTableSmall (Vector3 origin, int direction, Room room) : base (origin,4,1,3,direction,room,"PoolTableSmall",true,false){}
}

public class PoolTableLarge : Furniture
{
	public PoolTableLarge (Vector3 origin, int direction, Room room) : base (origin,5,1,4,direction,room,"PoolTableLarge",true,false){}
}

public class Picture : Furniture
{
	public Picture (Vector3 origin, int direction, Room room) : base (origin,2,2,1,direction,room,"Picture",false,false){}
}

public class Desk : Furniture
{
	public Desk (Vector3 origin, int direction, Room room) : base (origin,4,2,3,direction,room,"Desk",true,true){}
}









