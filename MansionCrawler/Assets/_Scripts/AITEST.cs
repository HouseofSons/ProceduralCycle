using UnityEngine;
using System.Collections;

public class AITEST : MonoBehaviour {

	private InteractPlatform platform;
	private Tile occupiedTile;
	private bool readyForMove;
	private int lastMoveTime;
	
	public int timeInSeconds;
	

	void Start() {
	
		platform = GameObject.FindObjectOfType<InteractPlatform>();
		occupiedTile = GameObject.FindObjectOfType<Tile>();
		readyForMove = true;
		timeInSeconds = 1;
	}
	
	// Update is called once per frame
	void Update () {
	
		if (Time.fixedTime % timeInSeconds == 0 && readyForMove)
		{
			readyForMove = false;
			lastMoveTime = Mathf.FloorToInt(Time.fixedTime);
			
			Tile nextPosition = AImovement();
			
			if (occupiedTile != nextPosition) {
				nextPosition.characterOccupied = true;
				occupiedTile.characterOccupied = false;
				occupiedTile = nextPosition;
				this.transform.position = nextPosition.location;
			}
		}
		if (Mathf.FloorToInt(Time.fixedTime) == lastMoveTime + 1) {
			readyForMove = true;
		}
	}
	
	private Tile AImovement()
	{
		for (int i = Mathf.FloorToInt(this.transform.position.x)-1;i<=Mathf.FloorToInt(this.transform.position.x)+1;i++) {
			for (int j = Mathf.FloorToInt(this.transform.position.z)-1;j<=Mathf.FloorToInt(this.transform.position.z)+1;j++) {
				if (Mathf.FloorToInt(this.transform.position.x) == i || Mathf.FloorToInt(this.transform.position.z) == j) {
					if (platform.GetPlatform()[i,j] != null) {
						if (platform.GetPlatform()[i,j].isInteractable()) {
							return platform.GetPlatform()[i,j];
						}
					}
				}
			}
		}
		return occupiedTile;
	}
}
