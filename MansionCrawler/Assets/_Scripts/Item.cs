using UnityEngine;
using System.Collections;

public class Item {

	private string itemName;
	private MainCharacter itemOwner;
	private Furniture itemLocation;
	private int itemNumber;
	private static int itemCount = 0;

	public Item (string name, MainCharacter character, Furniture location) {
		itemName = name;
		itemOwner = character;
		itemLocation = location;
		itemNumber = itemCount++;
	}

	public static int TotalItemCount()
	{
		return itemCount;
	}
	
	public int number
	{
		get {return itemNumber; }
	}
	
	public string type
	{
		get {return itemName; }
	}
	
	public MainCharacter character
	{
		get {return itemOwner; }
		set {itemOwner = value; }
	}
	
	public Furniture location
	{
		get {return itemLocation; }
		set {itemLocation = value; }
	}

	public static Item randomItem(MainCharacter character)
	{
		int i = Mathf.FloorToInt(Random.Range (1, 6));
		
		if (i == 1) {
			return new Item("Sneakers",character,null);
		}
		if (i == 2) {
			return new Item("XRayVision",character,null);
		}
		if (i == 3) {
			return new Item("ToolBelt",character,null);
		}
		if (i == 4) {
			return new Item("ChainWallet",character,null);
		}
		if (i == 5) {
			return new Item("Hoodie",character,null);
		}
		
		return null;
	}
	
	public static Item randomItem(Furniture piece)
	{
		int i = Mathf.FloorToInt(Random.Range (1, 6));
		
		if (i == 1) {
			return new Item("Sneakers",null,piece);
		}
		if (i == 2) {
			return new Item("XRayVision",null,piece);
		}
		if (i == 3) {
			return new Item("ToolBelt",null,piece);
		}
		if (i == 4) {
			return new Item("ChainWallet",null,piece);
		}
		if (i == 5) {
			return new Item("Hoodie",null,piece);
		}
		
		return null;
	}

	public static void SynchronizeItemBenefits(MainCharacter character)
	{//Complete logic for each Character
		bool Sneakers = false;
		bool XRayVision = false;
		bool ToolBelt = false;
		bool ChainWallet = false;
		bool Hoodie = false;
	
		for (int i = 0; i < character.GetItems().Length; i++) {
			if(character.GetItems()[i] != null)
			{
				if (character.GetItems()[i].type == "Sneakers") {
					Sneakers = true;
				}
				if (character.GetItems()[i].type == "XRayVision") {
					XRayVision = true;
				}
				if (character.GetItems()[i].type == "ToolBelt") {
					ToolBelt = true;
				}
				if (character.GetItems()[i].type == "ChainWallet") {
					ChainWallet = true;
				}
				if (character.GetItems()[i].type == "Hoodie") {
					Hoodie = true;
				}
			}
		}
		
		if (Sneakers) {
			character.sneakers = true;
		} else {
			character.sneakers = false;
		}
		if (Sneakers) {
			character.xRayVision = true;
		} else {
			character.xRayVision = false;
		}
		if (Sneakers) {
			character.toolBelt = true;
		} else {
			character.toolBelt = false;
		}
		if (Sneakers) {
			character.chainWallet = true;
		} else {
			character.chainWallet = false;
		}
		if (Sneakers) {
			character.hoodie = true;
		} else {
			character.hoodie = false;
		}
	}
}












