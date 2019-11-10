using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

	private InteractPlatform platform;
	private MainCharacter mainCharacter;

	private Tile pathDestinationTile;
	private int heuristic;
	private float gValue;
	private float fValue;
	private Tile parentTile;
	private Tile mainCharacterNextStepTile;
	private List<Tile> path;

	private Vector3 tileLocation;	
	private Doorway door;
	private Furniture furniturePiece;
	public bool isOccupiedByCharacter;
	public bool isOccupiedByWidget;
	private bool isDoorTileClosed;
	private bool closeDoorTrigger;
	private bool isHiddenDoorWallTile;
	private Doorway hiddenDoorway;
	
	private bool isFocused;
	
	private bool focusPulse;
	private float pulseStep;
	private bool queueStepSoundViz;
	
	void Awake() {
		mainCharacter = GameObject.FindWithTag ("Player_0" + MainCharacter.PlayerCount ()).GetComponent<MainCharacter>();
	}

	void Start ()
	{
		platform = GameObject.FindObjectOfType<InteractPlatform>();

		isFocused = false;
		closeDoorTrigger = false;
		
		this.gameObject.GetComponent<Renderer>().material.color = Color.clear;
		focusPulse = false;
		pulseStep = 0.5f;
		queueStepSoundViz = true;
	}
	
	void Update ()
	{
		if (isFocused) {
			if (isInteractable() || (isDoorTileClosed && !isHiddenDoorWall) || isOccupiedByWidget) {
				if (!mainCharacter.engaged && !isDoorTileClosed) {
					Pulse();
				}
			}	
		}
		
		if (isOccupiedByCharacter && queueStepSoundViz) {
			StartCoroutine(FootSteps());
			queueStepSoundViz = false;
		}
		if (!isOccupiedByCharacter && !queueStepSoundViz) {
			queueStepSoundViz = true;
		}
		
		//ReThink logic for opening and closing door since this might not be the best location for logic START
		if (!isDoorTileClosed && isOccupiedByCharacter) {
			closeDoorTrigger = true;
		}
		if (closeDoorTrigger && !isOccupiedByCharacter && isDoor()) {
			closeDoorTrigger = false;
			platform.PerformAction(this,2);
		}
		//ReThink logic for opening and closing door since this might not be the best location for logic END
	}
	
	public void MouseEnter()
	{
		if (isInteractable() || (isDoorTileClosed && !isHiddenDoorWall) || isOccupiedByWidget) {
			if (isOccupiedByWidget) {
				//WidgetGlow();//Occupying widget should glow when focused
			}
			isFocused = true;
		}
	}
	
	public void MouseExit()
	{
		isFocused = false;
		StopPulse();
		if (isOccupiedByWidget) {
			//WidgetStopGlow();//Occupying widget should stop glowing when no longer focused
		}
	}
	
	public void MouseDown()
	{
		if (isInteractable() || (isDoorTileClosed && !isHiddenDoorWall) || isOccupiedByWidget) {
		
			pathDestinationTile = this.gameObject.GetComponent<Tile>();
			platform.CalculateHeuristics(mainCharacter.nextstep,pathDestinationTile);
			if (mainCharacter.nextstep != mainCharacterNextStepTile) {
				path = platform.AStarPath(mainCharacter.nextstep,pathDestinationTile);
				mainCharacterNextStepTile = mainCharacter.nextstep;
			}
			
			if (!mainCharacter.engaged) {
				if (isDoorTileClosed || isOccupiedByWidget) {
					if (mainCharacter.enroute) {
						if (mainCharacter.route != null) {
							StopCoroutine(mainCharacter.route);
							mainCharacter.prevstep.characterOccupied = false;
							mainCharacter.prevstep = mainCharacter.nextstep;
						}
					}
					if (!isDoorTileClosed || isHiddenDoorWall) {
						mainCharacter.route = StartCoroutine(mainCharacter.ExecutePath(path,this.gameObject.GetComponent<Tile>(),3));
					} else {
						mainCharacter.route = StartCoroutine(mainCharacter.ExecutePath(path,this.gameObject.GetComponent<Tile>(),1));
					}
				} else {
					if (mainCharacter.enroute) {
						if (mainCharacter.route != null) {
							StopCoroutine(mainCharacter.route);
							mainCharacter.prevstep.characterOccupied = false;
							mainCharacter.prevstep = mainCharacter.nextstep;
						}
					}
					mainCharacter.route = StartCoroutine(mainCharacter.ExecutePath(path,null,0));
				}
			}
		}
	}
	
	public bool isInteractable()
	{
		if((!isOccupiedByCharacter || mainCharacter.prevstep == this) && !isOccupiedByWidget && !isHiddenDoorWall) {
			return true;
		} else {
			return false;
		}
	}
	
	private void StopPulse()
	{
		if (isHiddenDoorFloor()) {
			this.gameObject.GetComponent<Renderer>().material.color = Color.white;
		} else {
			this.gameObject.GetComponent<Renderer>().material.color = Color.clear;
		}
	}
	
	private void Pulse()
	{
		this.gameObject.GetComponent<Renderer>().material.color = Color.Lerp(Color.clear,Color.green, pulseStep);

		if (!focusPulse) {
			pulseStep += 0.05f;
		} else {
			pulseStep -= 0.05f;
		}
		
		if (pulseStep >= 1) {
			focusPulse = true;
		}
		if (pulseStep <= .25f) {
			focusPulse = false;
		}
	}
	
	public Vector3 location
	{
		get {return tileLocation; }
		set {tileLocation = value;}
	}
	
	public Doorway GetDoor()
	{
		return door;
	}
	
	public void SetDoor(Doorway d)
	{
		door = d;
	}
	
	public bool isDoorClosed
	{
		get {return isDoorTileClosed; }
		set {isDoorTileClosed = value; }
	}
	
	private bool isDoor()
	{
		if (door != null) {
			return true;
		} else {
			return false;
		}
	}
	
	public bool isHiddenDoorWall
	{
		get {return isHiddenDoorWallTile; }
		set {isHiddenDoorWallTile = value; }
	}

	public Doorway hiddenDoor
	{
		get {return hiddenDoorway; }
		set {hiddenDoorway = value; }
	}

	public bool isHiddenDoorFloor()
	{
		if (hiddenDoorway != null) {
			return true;
		} else {
			return false;
		}
	}
	
	public bool widgetOccupied
	{
		get {return isOccupiedByWidget; }
		set {isOccupiedByWidget = value;}
	}

	public bool characterOccupied
	{
		get {return isOccupiedByCharacter; }
		set {isOccupiedByCharacter = value;}
	}

	public Furniture furniture
	{
		get {return furniturePiece; }
		set {furniturePiece = value;}
	}
	
	private IEnumerator FootSteps()
	{
		if (mainCharacter.nextstep != this && !mainCharacter.sneakers) {
			GameObject creeky00 = new GameObject();
			creeky00.AddComponent<SpriteRenderer>();
			creeky00.GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("CREEKY")[0];
			creeky00.gameObject.transform.position = new Vector3(tileLocation.x,2.11f,tileLocation.z);
			creeky00.gameObject.transform.rotation = Quaternion.Euler (new Vector3(90,0,0));
			yield return new WaitForSeconds(0.2f);
	
			GameObject creeky01 = new GameObject();
			creeky01.AddComponent<SpriteRenderer>();
			creeky01.GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("CREEKY")[1];
			creeky01.transform.position = new Vector3(tileLocation.x,2.11f,tileLocation.z);
			creeky01.gameObject.transform.rotation = Quaternion.Euler (new Vector3(90,0,0));
			yield return new WaitForSeconds(0.5f);
			Destroy(creeky00);
			yield return new WaitForSeconds(0.5f);
			Destroy(creeky01);
		}
	}
	
	public void ColorDebug()
	{
		this.gameObject.GetComponent<Renderer>().material.color = Color.red;
	}
	
	/*ASTAR PATHING METHODS BEGIN*/
	public int GetHeuristic()
	{
		return heuristic;
	}
	
	public void SetHeuristic(int h)
	{
		heuristic = h;
	}
	
	public float GetG()
	{
		return gValue;
	}
	
	public void SetG(float g)
	{
		gValue = g;
	}
	
	public float GetF()
	{
		return fValue;
	}
	
	public void SetF(float f)
	{
		fValue = f;
	}
	
	public Tile GetParent()
	{
		return parentTile;
	}
	
	public void SetParent(Tile parent)
	{
		parentTile = parent;
	}
	/*ASTAR PATHING METHODS END*/
}
