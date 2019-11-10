using UnityEngine;
using System.Collections;

public class Hall {

	private Vector3 hallOrigin;
	private int hallWidth;
	private int hallDirection;
	private int hallLength;
	private Hall parentHall;
	private Hall childHall;
	private bool hallAdjusted;
	private bool madeByApprentice;
	private GameObject hallGameObject;
	
	public Hall(Vector3 pos, int width, int direction, int length, Hall parent, bool apprenticeMade)
	{
		hallOrigin = pos;
		hallWidth = width;
		hallDirection = direction;
		hallLength = length;
		parentHall = parent;
		hallAdjusted = false;
		madeByApprentice = apprenticeMade;
	}
	
	public GameObject hall
	{
		get {return hallGameObject; }
		set {hallGameObject = value; }
	}
	
	public Hall parent
	{
		get {return parentHall; }
	}
	
	public Hall child
	{
		get {return childHall; }
		set {childHall = value; }
	}
	
	public Vector3 origin
	{
		get {return hallOrigin; }
		set {hallOrigin = value; }
	}
	
	public int width
	{
		get {return hallWidth; }
		set {hallWidth = value; }
	}
	
	public int direction
	{
		get {return hallDirection; }
	}
	
	public int length
	{
		get {return hallLength; }
		set {hallLength = value; }
	}
	
	public bool adjusted
	{
		get {return hallAdjusted; }
		set {hallAdjusted = value; }
	}
	
	public bool apprenticeMade
	{
		get {return madeByApprentice; }
	}

	public Vector3 MapPosition()
	{
		Vector3 pos = Vector3.zero ;
		
		if(hallDirection == 1){
			if(hallWidth % 2 == 1) {
				pos = new Vector3(hallOrigin.x,0,hallOrigin.z + ((hallLength - 1)/2f));
				return pos;
			} else {
				pos = new Vector3(hallOrigin.x + (1/2f),0,hallOrigin.z + ((hallLength - 1)/2f));
				return pos;
			}
		}
		if(hallDirection == 2){
			if(hallWidth % 2 == 1) {
				pos = new Vector3(hallOrigin.x + ((hallLength - 1)/2f),0,hallOrigin.z);
				return pos;
			} else {
				pos = new Vector3(hallOrigin.x + ((hallLength - 1)/2f),0,hallOrigin.z - (1/2f));
				return pos;
			}
		}
		if(hallDirection == 3){
			if(hallWidth % 2 == 1) {
				pos = new Vector3(hallOrigin.x,0,hallOrigin.z - ((hallLength - 1)/2f));
				return pos;
			} else {
				pos = new Vector3(hallOrigin.x - (1/2f),0,hallOrigin.z - ((hallLength - 1)/2f));
				return pos;
			}
		}
		if(hallDirection == 4){
			if(hallWidth % 2 == 1) {
				pos = new Vector3(hallOrigin.x - ((hallLength - 1)/2f),0,hallOrigin.z);
				return pos;
			} else {
				pos = new Vector3(hallOrigin.x - ((hallLength - 1)/2f),0,hallOrigin.z + (1/2f));
				return pos;
			}
		}
		return pos;
	}
	
	public Vector3 MapScale()
	{
		Vector3 scale = Vector3.zero;
		
		if(hallDirection == 1 || hallDirection == 3){
			scale = new Vector3(hallWidth/10f,1,hallLength/10f);
			return scale;
		}
		if(hallDirection == 2 || hallDirection == 4) {
			scale = new Vector3(hallLength/10f,1,hallWidth/10f);
			return scale;
		}
		return scale;
	}
	
	public int StartingX()
	{
		if (hallDirection == 1) {
			return Mathf.CeilToInt(hallOrigin.x - ((hallWidth - 1f)/2f));
		}
		
		if (hallDirection == 2) {
			return Mathf.CeilToInt(hallOrigin.x);
		}
		
		if (hallDirection == 3) {
			return Mathf.FloorToInt(hallOrigin.x - ((hallWidth - 1f)/2f));
		}
		
		if (hallDirection == 4) {
			return Mathf.CeilToInt(hallOrigin.x - (hallLength - 1));
		}
		return -1;
	}
	
	public int StartingZ()
	{
		if (hallDirection == 1) {
			return Mathf.CeilToInt(hallOrigin.z);
		}
		
		if (hallDirection == 2) {
			return Mathf.FloorToInt(hallOrigin.z - ((hallWidth - 1f)/2f));
		}
		
		if (hallDirection == 3) {
			return Mathf.CeilToInt(hallOrigin.z - (hallLength - 1));
		}
		
		if (hallDirection == 4) {
			return Mathf.CeilToInt(hallOrigin.z - ((hallWidth - 1f)/2f));
		}
		return -1;
	}
}














