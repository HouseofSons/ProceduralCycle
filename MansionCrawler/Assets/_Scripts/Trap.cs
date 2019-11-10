using UnityEngine;
using System.Collections;

public class Trap {

	private string trapName;
	private MainCharacter trapOwner;
	private Furniture trapLocation;
	private int trapNumber;
	private static int trapCount = 0;
	private bool trapSet;

	public Trap (string name, MainCharacter character, Furniture location) {
		trapName = name;
		trapOwner = character;
		trapLocation = location;
		trapSet = false;
		trapNumber = trapCount++;
	}

	public static int TotalTrapCount()
	{
		return trapCount;
	}

	public int number
	{
		get {return trapNumber; }
	}

	public bool set
	{
		get {return trapSet; }
		set {trapSet = value; }
	}

	public string type
	{
		get {return trapName; }
	}

	public MainCharacter character
	{
		get {return trapOwner; }
		set {trapOwner = value; }
	}

	public Furniture location
	{
		get {return trapLocation; }
		set {trapLocation = value; }
	}

	public static Trap randomTrap(MainCharacter character)
	{
		int i = Mathf.FloorToInt(Random.Range (1, 4));

		if (i == 1) {
			return new Trap("Zap",character,null);
		}
		if (i == 2) {
			return new Trap("RocketBoard",character,null);
		}
		if (i == 3) {
			return new Trap("SpringBoard",character,null);
		}

		return null;
	}

	public static Trap randomTrap(Furniture piece)
	{
		int i = Mathf.FloorToInt(Random.Range (1, 4));
		
		if (i == 1) {
			return new Trap("Zap",null,piece);
		}
		if (i == 2) {
			return new Trap("RocketBoard",null,piece);
		}
		if (i == 3) {
			return new Trap("SpringBoard",null,piece);
		}

		return null;
	}
}