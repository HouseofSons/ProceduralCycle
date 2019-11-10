using UnityEngine;
using System.Collections;

public class Darkness : MonoBehaviour {

	private bool hitByNorthVector;
	private bool hitByEastVector;
	private bool hitBySouthVector;
	private bool hitByWestVector;
	
	private MainCharacter mainCharacter;

	private GameObject darknessPlane;

	// Use this for initialization
	void Start () {
		hitByNorthVector = false;
		hitByEastVector = false;
		hitBySouthVector = false;
		hitByWestVector = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (hitByNorthVector || hitByEastVector || hitBySouthVector || hitByWestVector) {
			this.gameObject.GetComponent<MeshRenderer>().enabled = false;
			this.gameObject.layer = 8;//Darkness Layer
		} else {
			this.gameObject.GetComponent<MeshRenderer>().enabled = true;
			this.gameObject.layer = 0;
		}
	}
	
	public MainCharacter character
	{
		get {return mainCharacter; }
		set {mainCharacter = value; }
	}

	public bool northHit
	{
		get {return hitByNorthVector; }
		set {hitByNorthVector = value; }
	}

	public bool eastHit
	{
		get {return hitByEastVector; }
		set {hitByEastVector = value; }
	}

	public bool southHit
	{
		get {return hitBySouthVector; }
		set {hitBySouthVector = value; }
	}

	public bool westHit
	{
		get {return hitByWestVector; }
		set {hitByWestVector = value; }
	}

	public GameObject darkPlane
	{
		get {return darknessPlane; }
		set {darknessPlane = value; }
	}
}
