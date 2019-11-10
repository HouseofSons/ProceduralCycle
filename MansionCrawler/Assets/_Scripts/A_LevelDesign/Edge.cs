using UnityEngine;
using System.Collections;

public class Edge {

	private int xCoord;
	private int zCoord;
	private bool isNorthRunning;
	private bool isEastRunning;
	private bool isSouthRunning;
	private bool isWestRunning;
	private bool isCornerEdge;
	private GameObject edgeGameObject;

	public Edge(int x, int z, bool north, bool east, bool south, bool west, bool isCorner)
	{
		xCoord = x;
		zCoord = z;
		isNorthRunning = north;
		isEastRunning = east;
		isSouthRunning = south;
		isWestRunning = west;
		isCornerEdge = isCorner;
	}
	
	public GameObject edge
	{
		get {return edgeGameObject; }
		set {edgeGameObject = value; }
	}
	
	public int x
	{
		get {return xCoord; }
	}
	
	public int z
	{
		get {return zCoord; }
	}
	
	public bool north
	{
		get {return isNorthRunning; }
	}
	
	public bool east
	{
		get {return isEastRunning; }
	}
	
	public bool south
	{
		get {return isSouthRunning; }
	}
	
	public bool west
	{
		get {return isWestRunning; }
	}
	
	public bool isCorner
	{
		get {return isCornerEdge; }
	}

	public float edgeType()
	{
		int connectCount = 0;
		bool opposing = false;
		
		if (isNorthRunning == true) {
			connectCount++;
			if (isSouthRunning == true) {
				opposing = true;
			}
		}
		if (isEastRunning == true) {
			connectCount++;
			if (isWestRunning == true) {
				opposing = true;
			}
		}
		if (isSouthRunning == true) {
			connectCount++;
		}
		if (isWestRunning == true) {
			connectCount++;
		}
		if (connectCount == 1) {
			return 1;
		}
		if (connectCount == 2) {
			if (opposing) {
				return 2.5f;
			} else {
				return 2.25f;
			}
		}
		if (connectCount == 3) {
			return 3;
		}
		if (connectCount == 4) {
			return 4;
		} else {
			return 0;
		}
	}

	public int direction()
	{
		if (edgeType () == 1) {
			if (isNorthRunning == true) {
				return 1;
			}
			if (isEastRunning == true) {
				return 2;
			}
			if (isSouthRunning == true) {
				return 3;
			}
			if (isWestRunning == true) {
				return 4;
			}
		}
		if (edgeType () == 2.25f) {
			if (isNorthRunning == true) {
				if (isEastRunning == true) {
					return 1;
				} else {
					return 4;
				}
			}
			if (isEastRunning == true) {
				return 2;
			}
			if (isSouthRunning == true) {
				return 3;
			}
		}
		if (edgeType () == 2.5f) {
			if (isNorthRunning == true) {
				return 1;
			} else {
				return 2;
			}
		}
		if (edgeType () == 3) {
			if (isNorthRunning != true) {
				return 2;
			}
			if (isEastRunning != true) {
				return 3;
			}
			if (isSouthRunning != true) {
				return 4;
			}
			return 1;
		}
		return 1;
	}
}
