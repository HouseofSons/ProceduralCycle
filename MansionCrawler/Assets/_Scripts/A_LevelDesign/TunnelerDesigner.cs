using UnityEngine;
using System.Collections;

public class TunnelerDesigner {
	
	private LevelDesignManager ldm;

	private Vector3 tunnelerPosition;
	private int turnLifeSpan;
	private int turnsMade;

	private int tunnelerWidth;
	private int tunnelerDirection;
	private int tunnelerStepLength;
	
	private int turnChance;
	private int changeWidthChance;
	private int changeWidthMin;
	private int changeWidthMax;
	private int changeStepLengthMin;
	private int changeStepLengthMax;
	
	public TunnelerDesigner(int lifeSpan, Vector3 pos, int x, int z, int width, int direction,
		int stepLength, int turn, int changeWidth, int widthMin, int widthMax,
	    int StepLengthMin, int StepLengthMax)
	{
		ldm = GameObject.FindObjectOfType<LevelDesignManager>();
		
		turnLifeSpan = lifeSpan;
		turnsMade = 0;
		
		tunnelerPosition = pos;
		tunnelerWidth = width;
		tunnelerDirection = direction;
		tunnelerStepLength = stepLength;
		
		turnChance = turn;
		changeWidthChance = changeWidth;
		changeWidthMin = widthMin;
		changeWidthMax = widthMax;
		changeStepLengthMin = StepLengthMin;
		changeStepLengthMax = StepLengthMax;
		Tunnel ();
	}
	
	private void Tunnel ()
	{
		int attempts = 0;
		
		Hall currentHall = new Hall(tunnelerPosition, tunnelerWidth, tunnelerDirection, tunnelerStepLength, null, false);
		ldm.SetSpawnHall(currentHall);
		Hall prevHall = currentHall;
		
		ldm.HallBuildOK(currentHall,0);
	
		do
		{
			attempts = 0;
		
			getNewAttributes(prevHall);
			currentHall = new Hall(tunnelerPosition, tunnelerWidth, tunnelerDirection, tunnelerStepLength, prevHall, false);
			
			while (!ldm.HallBuildOK(currentHall,attempts) && attempts < 20)
			{
				if (attempts < 10) {
					getNewAttributes(prevHall);
					currentHall = new Hall(tunnelerPosition, tunnelerWidth, tunnelerDirection, tunnelerStepLength, prevHall, false);
				} else {
					if (attempts > 15) {
						getNewAttributes(prevHall.parent.parent);
						currentHall = new Hall(tunnelerPosition, tunnelerWidth, tunnelerDirection, tunnelerStepLength, prevHall.parent.parent, false);
					} else {
						getNewAttributes(prevHall.parent);
						currentHall = new Hall(tunnelerPosition, tunnelerWidth, tunnelerDirection, tunnelerStepLength, prevHall.parent, false);
					}
				}
				attempts++;
			}
			prevHall = currentHall;
			turnsMade++;
			
		} while ((attempts < 20) && (turnsMade < turnLifeSpan));
		ldm.SetExitHall(currentHall);
		Debug.Log ("attempts: " + attempts + " turns: " + turnsMade);
	}
	
	private void getNewAttributes(Hall lastHall)
	{
		//adjusts direction of new hall
		if (turnChance >= Random.Range (1, 100)) {
			//coin flip for which turn to make
			if (Random.Range (1, 100) <= 50) {
				if (lastHall.direction - 1 < 1) {
					tunnelerDirection = 4;
				} else {
					tunnelerDirection = lastHall.direction - 1;
				}
			} else {
				if (lastHall.direction + 1 > 4) {
					tunnelerDirection = 1;
				} else {
					tunnelerDirection = lastHall.direction + 1;
				}
			}
		} else {
			tunnelerDirection = lastHall.direction;
		}
		//adjusts width of new hall only when hall turns
		if(changeWidthChance >= Random.Range(1,100)) {
			//checks if hall is turning and limits width max
			if (lastHall.direction != tunnelerDirection) {
				tunnelerWidth = Mathf.CeilToInt(Random.Range(changeWidthMin, Mathf.Min(changeWidthMax,lastHall.length - 1)));
			} else {
				tunnelerWidth = lastHall.width;
			}
		} else {
			tunnelerWidth = lastHall.width;
		}
		//adjusts length of new hall
		tunnelerStepLength = Mathf.CeilToInt(Random.Range(Mathf.Max(changeStepLengthMin,tunnelerWidth * 2f), Mathf.Min(changeStepLengthMax,tunnelerWidth * 3f)));
		//adjusts starting position of new hall
		if (lastHall.direction == tunnelerDirection) {
			if (tunnelerDirection == 1) {
				tunnelerPosition = new Vector3(lastHall.origin.x,lastHall.origin.y,lastHall.origin.z + lastHall.length);	
			}
			if (tunnelerDirection == 3) {
				tunnelerPosition = new Vector3(lastHall.origin.x,lastHall.origin.y,lastHall.origin.z - lastHall.length);	
			}
			if (tunnelerDirection == 2) {
				tunnelerPosition = new Vector3(lastHall.origin.x + lastHall.length,lastHall.origin.y,lastHall.origin.z);
			}
			if (tunnelerDirection == 4) {
				tunnelerPosition = new Vector3(lastHall.origin.x - lastHall.length,lastHall.origin.y,lastHall.origin.z);
			}
		} else {
			tunnelerPosition = getTurnOrigin(lastHall);
		}
	}
	//Method for returning new hall origin when a turn occurs
	private Vector3 getTurnOrigin(Hall lastHall)
	{
		int xn = -1;
		int zn = -1;
		
		if(lastHall.direction == 1 && tunnelerDirection == 2){
			xn = (int)lastHall.origin.x + Mathf.FloorToInt((lastHall.width/2f) + 1);
			zn = (int)lastHall.origin.z + lastHall.length - 1 - Mathf.FloorToInt((tunnelerWidth - 1)/2f);
		}
		if(lastHall.direction == 1 && tunnelerDirection == 4){
			xn = (int)lastHall.origin.x - Mathf.CeilToInt(lastHall.width/2f);
			zn = (int)lastHall.origin.z + lastHall.length - 1 - Mathf.CeilToInt((tunnelerWidth - 1)/2f);
		}
		if(lastHall.direction == 3 && tunnelerDirection == 2){
			xn = (int)lastHall.origin.x + Mathf.CeilToInt(lastHall.width/2f);
			zn = (int)lastHall.origin.z - lastHall.length + 1 + Mathf.CeilToInt((tunnelerWidth - 1)/2f);
		}
		if(lastHall.direction == 3 && tunnelerDirection == 4){
			xn = (int)lastHall.origin.x - Mathf.FloorToInt((lastHall.width/2f) + 1);
			zn = (int)lastHall.origin.z - lastHall.length + 1 + Mathf.FloorToInt((tunnelerWidth - 1)/2f);
		}
		if(lastHall.direction == 2 && tunnelerDirection == 1){
			xn = (int)lastHall.origin.x + lastHall.length - 1 - Mathf.CeilToInt((tunnelerWidth - 1)/2f);
			zn = (int)lastHall.origin.z + Mathf.CeilToInt(lastHall.width/2f);
		}
		if(lastHall.direction == 2 && tunnelerDirection == 3){
			xn = (int)lastHall.origin.x + lastHall.length - 1 - Mathf.FloorToInt((tunnelerWidth - 1)/2f);
			zn = (int)lastHall.origin.z - Mathf.FloorToInt((lastHall.width/2f) + 1);
		}
		if(lastHall.direction == 4 && tunnelerDirection == 1){
			xn = (int)lastHall.origin.x - lastHall.length + 1 + Mathf.FloorToInt((tunnelerWidth - 1)/2f);
			zn = (int)lastHall.origin.z + Mathf.FloorToInt((lastHall.width/2f) + 1);
		}
		if(lastHall.direction == 4 && tunnelerDirection == 3){
			xn = (int)lastHall.origin.x - lastHall.length + 1 + Mathf.CeilToInt((tunnelerWidth - 1)/2f);
			zn = (int)lastHall.origin.z - Mathf.CeilToInt(lastHall.width/2f);
		}
		Vector3 position = new Vector3(xn,tunnelerPosition.y,zn);
		return position;
	}
	
	public Vector3 GetSpawnPoint()
	{
		return tunnelerPosition;
	}
}




















