using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TunnelerApprentice {

	private LevelDesignManager ldm;
	private List<Hall> secretHalls;
	private Doorway parentDoor;
	
	private Vector3 tunnelerPosition;
	private int turnLifeSpan;
	private int turnsMade;
	
	private int tunnelerWidth;
	private int tunnelerDirection;
	private int tunnelerStepLength;
	
	private int turnChance;
	private int changeStepLengthMin;
	private int changeStepLengthMax;
	
	public TunnelerApprentice(int lifeSpan, Doorway door, int width, int direction,
	                        int stepLength, int turn, int StepLengthMin, int StepLengthMax)
	{
		ldm = GameObject.FindObjectOfType<LevelDesignManager>();
		secretHalls = new List<Hall>();
		
		turnLifeSpan = lifeSpan;
		parentDoor = door;
		turnsMade = 0;
		
		tunnelerPosition = GetPosition();
		tunnelerWidth = width;
		tunnelerDirection = direction;
		tunnelerStepLength = stepLength;
		
		turnChance = turn;
		changeStepLengthMin = StepLengthMin;
		changeStepLengthMax = StepLengthMax;
		Tunnel ();
	}
	
	private void Tunnel ()
	{
		Vector3 connection;
		bool foundEdge = false;
		Hall currentHall = new Hall(tunnelerPosition, tunnelerWidth, tunnelerDirection, tunnelerStepLength, null, true);
		Hall prevHall = currentHall;
		bool repeat = false;
		do
		{
			if (turnsMade > 0) {
				getNewAttributes(prevHall,repeat);
				currentHall = new Hall(tunnelerPosition, tunnelerWidth, tunnelerDirection, tunnelerStepLength, prevHall, true);
			}
			connection = ApprenticeHallOK(currentHall);
			if (connection != Vector3.zero) {
				repeat = false;
				secretHalls.Add (currentHall);
				prevHall = currentHall;
				if (connection != Vector3.one) {
					foundEdge = true;
				}
			} else {
				repeat = true;
			}
			turnsMade++;
			
		} while (turnsMade < turnLifeSpan && !foundEdge);
		
		if (foundEdge) {
			parentDoor.hiddenHallDoor = true;
			parentDoor.hiddenHallEscape = true;
			ConnectHall(currentHall,connection);//adjusts last hall to connect with collided structure and adds Doorway
			CopySecretHalls(secretHalls);//copies hidden halls to halls list
		}
	}
	//Connects hidden hall to structure
	private void ConnectHall (Hall hall, Vector3 connection)
	{
		if (hall.direction == 1 || hall.direction == 3) {
			hall.length = Mathf.FloorToInt(Mathf.Abs(hall.origin.z - connection.z));
		}
		if (hall.direction == 2 || hall.direction == 4) {
			hall.length = Mathf.FloorToInt(Mathf.Abs(hall.origin.x - connection.x));
		}
		if (ldm.layoutDesign[Mathf.FloorToInt(connection.x),Mathf.FloorToInt(connection.z)] != 7) {
			ldm.AddSecretDoor(new Doorway(Mathf.FloorToInt(connection.x),Mathf.FloorToInt(connection.z),ldm.findDirection(Mathf.FloorToInt(connection.x),Mathf.FloorToInt(connection.z),false,hall),hall,null));
		}
	}
	//Copies hiddenHall list to the bigger halls list in Level Build Manager
	private void CopySecretHalls(List<Hall> secretHalls)
	{
		foreach (Hall hall in secretHalls) {
			ldm.AddSecretHall(hall);
		}
	}
	//Checks if origin of hidden hall is on edge
	private bool HiddenHallOriginOk(Hall hall)
	{
		int x = 0;
		int z = 0;
	
		if (hall.direction == 1) {
			x = Mathf.FloorToInt(hall.origin.x);
			z = Mathf.FloorToInt(hall.origin.z - 1);
		}
		if (hall.direction == 2) {
			x = Mathf.FloorToInt(hall.origin.x - 1);
			z = Mathf.FloorToInt(hall.origin.z);
		}
		if (hall.direction == 3) {
			x = Mathf.FloorToInt(hall.origin.x);
			z = Mathf.FloorToInt(hall.origin.z + 1);
		}
		if (hall.direction == 4) {
			x = Mathf.FloorToInt(hall.origin.x + 1);
			z = Mathf.FloorToInt(hall.origin.z);
		}
	
		if ((x >= 0 && x <= ldm.mapXCoord) && (z >= 0 && z <= ldm.mapZCoord)) {
			if (ldm.layoutDesign[x,z] == 6) {//equals edge
				return false;
			} else {
				return true;
			}
		} else {
			return false;
		}
	}
	//Checks if hiddenHall is valid and returns true if proper edge is found
	private Vector3 ApprenticeHallOK(Hall hall)
	{
		int xn = hall.StartingX();
		int zn = hall.StartingZ();
		
		if (!HiddenHallOriginOk(hall)) {
			return Vector3.zero;
		}
		
		if (hall.direction == 1) {
			for (int x=xn;x<=(xn + hall.width - 1);x++) {
				for (int z=zn;z<=(zn + hall.length - 1);z++) {
					//false if out of bounds
					if ((x >= 0 && x <= ldm.mapXCoord) && (z >= 0 && z <= ldm.mapZCoord)) {
						if (x == hall.origin.x) {
							if (ldm.layoutDesign[x,z] == 7) {
								return new Vector3(x,0,z);
							}
							if (ldm.layoutDesign[x,z] == 6) {
								if (ldm.findEdge(x,z) != null) {
									if (!ldm.findEdge(x,z).isCorner) {
										return new Vector3(x,0,z);
									} else {
										return Vector3.zero;
									}
								} else {
									return Vector3.zero;
								}
							}
						}
					} else {
						return Vector3.zero;
					}
				}
			}
		}
		if (hall.direction == 3) {
			for (int x=xn;x<=(xn + hall.width - 1);x++) {
				for (int z=(zn + hall.length - 1);z>=zn;z--) {
					//false if out of bounds
					if ((x >= 0 && x <= ldm.mapXCoord) && (z >= 0 && z <= ldm.mapZCoord)) {
						if (x == hall.origin.x) {
							if (ldm.layoutDesign[x,z] == 7) {
									return new Vector3(x,0,z);
							}
							if (ldm.layoutDesign[x,z] == 6) {
								if (ldm.findEdge(x,z) != null) {
									if (!ldm.findEdge(x,z).isCorner) {
										return new Vector3(x,0,z);
									} else {
										return Vector3.zero;
									}
								} else {
									return Vector3.zero;
								}
							}
						}
					} else {
						return Vector3.zero;
					}
				}
			}
		}
		if (hall.direction == 2) {
			for (int x=xn;x<=(xn + hall.length - 1);x++) {
				for (int z=zn;z<=(zn + hall.width - 1);z++) {
					//false if out of bounds
					if ((x >= 0 && x <= ldm.mapXCoord) && (z >= 0 && z <= ldm.mapZCoord)) {
						if (z == hall.origin.z) {
							if (ldm.layoutDesign[x,z] == 7) {
								return new Vector3(x,0,z);
							}
							if (ldm.layoutDesign[x,z] == 6) {
								if (ldm.findEdge(x,z) != null) {
									if (!ldm.findEdge(x,z).isCorner) {
										return new Vector3(x,0,z);
									} else {
										return Vector3.zero;
									}
								} else {
									return Vector3.zero;
								}
							}
						}
					} else {
						return Vector3.zero;
					}
				}
			}
		}
		if (hall.direction == 4) {
			for (int x=(xn + hall.length - 1);x>=xn;x--) {
				for (int z=zn;z<=(zn + hall.width - 1);z++) {
					//false if out of bounds
					if ((x >= 0 && x <= ldm.mapXCoord) && (z >= 0 && z <= ldm.mapZCoord)) {
						if (z == hall.origin.z) {
							if (ldm.layoutDesign[x,z] == 7) {
								return new Vector3(x,0,z);
							}
							if (ldm.layoutDesign[x,z] == 6) {
								if (ldm.findEdge(x,z) != null) {
									if (!ldm.findEdge(x,z).isCorner) {
										return new Vector3(x,0,z);
									} else {
										return Vector3.zero;
									}
								} else {
									return Vector3.zero;
								}
							}
						}
					} else {
						return Vector3.zero;
					}
				}
			}
		}
		return Vector3.one;
	}
	//returns position of hall
	private Vector3 GetPosition()
	{
		if (parentDoor.direction == 1) {
			return new Vector3(parentDoor.x,0,parentDoor.z+1);
		}
		if (parentDoor.direction == 2) {
			return new Vector3(parentDoor.x+1,0,parentDoor.z);
		}
		if (parentDoor.direction == 3) {
			return new Vector3(parentDoor.x,0,parentDoor.z-1);
		}
		if (parentDoor.direction == 4) {
			return new Vector3(parentDoor.x-1,0,parentDoor.z);
		}
		return Vector3.zero;
	}
	
	private void getNewAttributes(Hall lastHall, bool repeatLastHall)
	{
		//adjusts direction of new hall
		if (!repeatLastHall) {
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
		}
		//adjusts length of new hall
		tunnelerStepLength = Mathf.CeilToInt(Random.Range(Mathf.Max(changeStepLengthMin,tunnelerWidth * 2f), Mathf.Min(changeStepLengthMax,tunnelerWidth * 3f)));
		//adjusts starting position of new hall
		if (!repeatLastHall) {
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
}
