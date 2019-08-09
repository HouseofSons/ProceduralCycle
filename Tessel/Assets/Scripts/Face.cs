using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Face {

	//List of Edges associated with Face
	private List<Edge> faceEdges;
	public List<Edge> edges {
		get {return faceEdges;}
	}
	
	//Vector which is located at center of face
	private Vector3 faceOrigin;
	public Vector3 origin {
		get {return faceOrigin;}
	}
	
	//Vector which points in the direction of the camera from the origin
	private Vector3 faceDirection;
	public Vector3 direction {
		get {return faceDirection;}
	}

	//boolean grid which tells of edge locations
	private int[,] faceEdgeGrid;
	public int[,] edgeGrid {
		get {return faceEdgeGrid;}
	}

	//Hold the lowest Border Vector on the face for each row
	private int[] faceGridRowMin;
	public int[] gridRowMin {
		get {return faceGridRowMin;}
	}
	
	//Hold the highest Border Vector on the face for each row
	private int[] faceGridRowMax;
	public int[] gridRowMax {
		get {return faceGridRowMax;}
	}
	
	//Hold the lowest Border Vector on the face for each column
	private int[] faceGridColMin;
	public int[] gridColMin {
		get {return faceGridColMin;}
	}
	
	//Hold the highest Border Vector on the face for each column
	private int[] faceGridColMax;
	public int[] gridColMax {
		get {return faceGridColMax;}
	}

	//Initializes Face
	public Face (Vector3 origin,Vector3 direction) {
		faceOrigin = origin;
		faceDirection = direction;
		faceEdges = new List<Edge> ();
		faceGridColMin = new int[LevelManager.FACE_MAGNITUDE*2 + 1];
		faceGridColMax = new int[LevelManager.FACE_MAGNITUDE*2 + 1];
		faceGridRowMin = new int[LevelManager.FACE_MAGNITUDE + 1];
		faceGridRowMax = new int[LevelManager.FACE_MAGNITUDE + 1];
		for (int i=0; i<faceGridColMin.Length; i++) {faceGridColMin[i] = -1;}
		for (int i=0; i<faceGridColMax.Length; i++) {faceGridColMax[i] = -1;}
		for (int i=0; i<faceGridRowMin.Length; i++) {faceGridRowMin[i] = -1;}
		for (int i=0; i<faceGridRowMax.Length; i++) {faceGridRowMax[i] = -1;}
		InitializeEdgeGrid ();
		AddEdges (faceOrigin,faceDirection);
		ScrubEdgeOne ();
		InitializeEdgeGrid ();//Clears grid for rewrite after scrubbing
		for (int i=0; i<4; i++) { //Rewrites grid after edge scrubbing
			FillEdgeGrid(faceEdges[i]);
		}
		FindGridMinAndMaxBorderVectors ();
		FillWithinEdgeGridBorder ();
		AddGameGridLedge ();
		//FillEdgeGridCenter ();//for debugging purposes, potentially used for initializing game objects
	}

	public Vector3 GridCenter() {
		if ((faceDirection == Vector3.left ||
		     faceDirection == Vector3.right)) {
			return new Vector3 (faceOrigin.x,(LevelManager.FACE_MAGNITUDE/2.00f),(LevelManager.FACE_MAGNITUDE));
		} else if ((faceDirection == Vector3.up ||
		            faceDirection == Vector3.down)) {
			return new Vector3 ((LevelManager.FACE_MAGNITUDE),faceOrigin.y,(LevelManager.FACE_MAGNITUDE/2.00f));
		} else {
			return new Vector3 ((LevelManager.FACE_MAGNITUDE),(LevelManager.FACE_MAGNITUDE/2.00f),faceOrigin.z);
		}
	}
	
	public bool EdgeOneBorderTouched() {
		//Requires Edge One to always be top of Grid
		for (int i=0; i<faceEdgeGrid.GetLength(0); i++) {
			if (faceEdgeGrid[i,faceEdgeGrid.GetLength(1)-1] != 0) {
				return true;
			}
		}
		return false;
	}

	//Initializes Edges of Face
	private void AddEdges (Vector3 origin,Vector3 direction) {
		if (direction == Vector3.forward || direction == Vector3.back) {
			faceEdges.Add (new Edge(this,	new Vector3(LevelManager.FACE_MAGNITUDE,LevelManager.FACE_MAGNITUDE/2.00f,0.00f) + origin,
			                        		new Vector3(-LevelManager.FACE_MAGNITUDE,LevelManager.FACE_MAGNITUDE/2.00f,0.00f) + origin,
			                       			new Vector3(-1,0,0),
			                        		1));
			FillEdgeGrid(faceEdges[0]);
			faceEdges.Add (new Edge(this,	new Vector3(-LevelManager.FACE_MAGNITUDE,LevelManager.FACE_MAGNITUDE/2.00f,0.00f) + origin,
			                        		new Vector3(-LevelManager.FACE_MAGNITUDE,-LevelManager.FACE_MAGNITUDE/2.00f,0.00f) + origin,
					                        new Vector3(0,-1,0),
			                        		2));
			FillEdgeGrid(faceEdges[1]);
			faceEdges.Add (new Edge(this,	new Vector3(-LevelManager.FACE_MAGNITUDE,-LevelManager.FACE_MAGNITUDE/2.00f,0.00f) + origin,
			                        		new Vector3(LevelManager.FACE_MAGNITUDE,-LevelManager.FACE_MAGNITUDE/2.00f,0.00f) + origin,
			                        		new Vector3(1,0,0),
			                        		3));
			FillEdgeGrid(faceEdges[2]);
			faceEdges.Add (new Edge(this,	new Vector3(LevelManager.FACE_MAGNITUDE,-LevelManager.FACE_MAGNITUDE/2.00f,0.00f) + origin,
			                        		new Vector3(LevelManager.FACE_MAGNITUDE,LevelManager.FACE_MAGNITUDE/2.00f,0.00f) + origin,
			                        		new Vector3(0,1,0),
			                        		4));
			FillEdgeGrid(faceEdges[3]);
		} else if (direction == Vector3.up || direction == Vector3.down) {
			faceEdges.Add (new Edge(this,	new Vector3(LevelManager.FACE_MAGNITUDE,0.00f,LevelManager.FACE_MAGNITUDE/2.00f) + origin,
			                        		new Vector3(-LevelManager.FACE_MAGNITUDE,0.00f,LevelManager.FACE_MAGNITUDE/2.00f) + origin,
			                        		new Vector3(-1,0,0),
			                        		1));
			FillEdgeGrid(faceEdges[0]);
			faceEdges.Add (new Edge(this,	new Vector3(-LevelManager.FACE_MAGNITUDE,0.00f,LevelManager.FACE_MAGNITUDE/2.00f) + origin,
			                        		new Vector3(-LevelManager.FACE_MAGNITUDE,0.00f,-LevelManager.FACE_MAGNITUDE/2.00f) + origin,
			                        		new Vector3(0,0,-1),
			                        		2));
			FillEdgeGrid(faceEdges[1]);
			faceEdges.Add (new Edge(this,	new Vector3(-LevelManager.FACE_MAGNITUDE,0.00f,-LevelManager.FACE_MAGNITUDE/2.00f) + origin,
			                        		new Vector3(LevelManager.FACE_MAGNITUDE,0.00f,-LevelManager.FACE_MAGNITUDE/2.00f) + origin,
			                        		new Vector3(1,0,0),
			                        		3));
			FillEdgeGrid(faceEdges[2]);
			faceEdges.Add (new Edge(this,	new Vector3(LevelManager.FACE_MAGNITUDE,0.00f,-LevelManager.FACE_MAGNITUDE/2.00f) + origin,
			                        		new Vector3(LevelManager.FACE_MAGNITUDE,0.00f,LevelManager.FACE_MAGNITUDE/2.00f) + origin,
			                        		new Vector3(0,0,1),
			                        		4));
			FillEdgeGrid(faceEdges[3]);
		} else {
			faceEdges.Add (new Edge(this,	new Vector3(0.00f,LevelManager.FACE_MAGNITUDE/2.00f,LevelManager.FACE_MAGNITUDE) + origin,
			                        		new Vector3(0.00f,LevelManager.FACE_MAGNITUDE/2.00f,-LevelManager.FACE_MAGNITUDE) + origin,
			                        		new Vector3(0,0,-1),
			                        		1));
			FillEdgeGrid(faceEdges[0]);
			faceEdges.Add (new Edge(this,	new Vector3(0.00f,LevelManager.FACE_MAGNITUDE/2.00f,-LevelManager.FACE_MAGNITUDE) + origin,
			                        		new Vector3(0.00f,-LevelManager.FACE_MAGNITUDE/2.00f,-LevelManager.FACE_MAGNITUDE) + origin,
			                        		new Vector3(0,-1,0),
			                        		2));
			FillEdgeGrid(faceEdges[1]);
			faceEdges.Add (new Edge(this,	new Vector3(0.00f,-LevelManager.FACE_MAGNITUDE/2.00f,-LevelManager.FACE_MAGNITUDE) + origin,
			                        		new Vector3(0.00f,-LevelManager.FACE_MAGNITUDE/2.00f,LevelManager.FACE_MAGNITUDE) + origin,
			                        		new Vector3(0,0,1),
			                        		3));
			FillEdgeGrid(faceEdges[2]);
			faceEdges.Add (new Edge(this,	new Vector3(0.00f,-LevelManager.FACE_MAGNITUDE/2.00f,LevelManager.FACE_MAGNITUDE) + origin,
			                        		new Vector3(0.00f,LevelManager.FACE_MAGNITUDE/2.00f,LevelManager.FACE_MAGNITUDE) + origin,
			                        		new Vector3(0,1,0),
			                        		4));
			FillEdgeGrid(faceEdges[3]);
		}
	}
	
	private void InitializeEdgeGrid() {
		faceEdgeGrid = new int[LevelManager.FACE_MAGNITUDE * 2 + 1, LevelManager.FACE_MAGNITUDE + 1];
		for (int i=0; i<LevelManager.FACE_MAGNITUDE*2; i++) {
			for (int j=0; j<LevelManager.FACE_MAGNITUDE; j++) {
				faceEdgeGrid [i,j] = 0;
			}
		}
	}

	private void FillEdgeGrid(Edge edge) {

		Vector3 current = LevelManager.FALSE_VECTOR;
		Vector3 prev = LevelManager.FALSE_VECTOR;
		Vector3 step = LevelManager.FALSE_VECTOR;

		foreach (Vector3 v in edge.coordinates) {

			prev = current;
			current = v - (faceOrigin - GridCenter());

			if (prev != LevelManager.FALSE_VECTOR) {
				
				step = prev;

				do {
					if((faceDirection == Vector3.up) ||
						(faceDirection == Vector3.down)) {

						faceEdgeGrid[(int)step.x,(int)step.z] = edge.order;
						step = Vector3.MoveTowards(step,current,1.00f);

					} else if ((faceDirection == Vector3.forward) ||
				           		(faceDirection == Vector3.back)) {
					
						faceEdgeGrid[(int)step.x,(int)step.y] = edge.order;
						step = Vector3.MoveTowards(step,current,1.00f);

					} else {
						faceEdgeGrid[(int)step.z,(int)step.y] = edge.order;
						step = Vector3.MoveTowards(step,current,1.00f);
					}
				} while ((int)Vector3.Distance(step,current)>=1.00f);

			}
		}
	}

	private void ScrubEdgeOne() {
		//Method requires no duplicate Vectors in edge coordinates (DedupeEdgeCoordinates() must be run first)
		List<Vector3> remove = new List<Vector3> ();

		Vector3 trueStart = faceEdges [3].termination;
		Vector3 current = LevelManager.FALSE_VECTOR;
		Vector3 prev = LevelManager.FALSE_VECTOR;
		Vector3 step = LevelManager.FALSE_VECTOR;

		foreach(Vector3 v in faceEdges[0].coordinates){

			prev = current;
			current = v;
			step = prev;

			if (prev != LevelManager.FALSE_VECTOR) {

				while(step != current && step != trueStart) {

					step = Vector3.MoveTowards(step,current,1.00f);
				}
				if (step==prev) {
					break;
				} else {
					remove.Add(prev);
					if(step == trueStart && step == current) {
						break;
					}
					if(step == trueStart) {
						faceEdges[0].coordinates.Insert(0,step);
						break;
					}
				}
			}
		}
		foreach(Vector3 v in remove) {
			faceEdges[0].coordinates.Remove(v);
		}
	}

	private void FillEdgeGridCenter() {
		for (int i=0+(int)(LevelManager.FACE_MAGNITUDE/2.00f)-LevelManager.FACE_ORIGIN_BUFFER+1;i<(int)(LevelManager.FACE_MAGNITUDE*3.00f/2.00f)+LevelManager.FACE_ORIGIN_BUFFER;i++) {
			for (int j=0+(int)(LevelManager.FACE_MAGNITUDE/2.00)-LevelManager.FACE_ORIGIN_BUFFER+1;j<(int)(LevelManager.FACE_MAGNITUDE/2.00)+LevelManager.FACE_ORIGIN_BUFFER;j++) {
				faceEdgeGrid[i,j] = 5;
			}
		}
	}
		
	private void AddGameGridLedge() {//Consider adding different ledges
		for (int i=(int)(LevelManager.FACE_MAGNITUDE/2.00f)-LevelManager.FACE_ORIGIN_BUFFER+4;
			i<(int)(LevelManager.FACE_MAGNITUDE*3.00f/2.00f)+LevelManager.FACE_ORIGIN_BUFFER-3;i++) {
			faceEdgeGrid[i,(int)(LevelManager.FACE_MAGNITUDE/2.00)-2] = 7;
		}
	}

	private void FindGridMinAndMaxBorderVectors() {

		for (int i=0; i<faceEdgeGrid.GetLength(0); i++) {
			for (int j=0; j<faceEdgeGrid.GetLength(1); j++) {
				if(edgeGrid[i,j] >= 1 && edgeGrid[i,j] <= 4) {
					faceGridColMax[i] = j;
					if(faceGridColMin[i] == -1) {
						faceGridColMin[i] = j;
					}
					faceGridRowMax[j] = i;
					if(faceGridRowMin[j] == -1) {
						faceGridRowMin[j] = i;
					}
				}
			}
		}
	}
	
	private void FillWithinEdgeGridBorder() {

		for (int i=0; i<faceEdgeGrid.GetLength(0); i++) {
			for (int j=0; j<faceEdgeGrid.GetLength(1); j++) {
				if(!(edgeGrid[i,j] >= 1 && edgeGrid[i,j] <= 4)) {
					if (j<faceGridColMax[i] && j>faceGridColMin[i] &&
					    i<faceGridRowMax[j] && i>faceGridRowMin[j]) {
						edgeGrid[i,j] = 6;
					}
				}
			}
		}
	}

	public void DrawFaceGrid() {//For Debugging Purposes
		
		Vector3 scale = origin - GridCenter();
		
		if((direction == Vector3.up) ||
		   (direction == Vector3.down)) {
			for (int i=0;i<=LevelManager.FACE_MAGNITUDE*2;i++) {
				for (int j=0;j<=LevelManager.FACE_MAGNITUDE;j++) {
					GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
					cube.transform.position = new Vector3(i + scale.x,origin.y,j + scale.z);
					if (edgeGrid[i,j] == 1) {
						cube.GetComponent <Renderer>().material.color = Color.red;
					} else if (edgeGrid[i,j] == 2) {
						cube.GetComponent <Renderer>().material.color = Color.blue;
					} else if (edgeGrid[i,j] == 3) {
						cube.GetComponent <Renderer>().material.color = Color.green;
					} else if (edgeGrid[i,j] == 4) {
						cube.GetComponent <Renderer>().material.color = Color.yellow;
					} else if (edgeGrid[i,j] == 5) {
						cube.GetComponent <Renderer>().material.color = Color.gray;
					} else if (edgeGrid[i,j] == 6) {
						cube.GetComponent <Renderer>().material.color = Color.cyan;
					} else {
						cube.GetComponent <Renderer>().material.color = Color.white;
					}
				}
			}
		} else if ((direction == Vector3.forward) ||
		           (direction == Vector3.back)) {
			for (int i=0;i<=LevelManager.FACE_MAGNITUDE*2;i++) {
				for (int j=0;j<=LevelManager.FACE_MAGNITUDE;j++) {
					GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
					cube.transform.position = new Vector3(i + scale.x,j + scale.y,origin.z);
					if (edgeGrid[i,j] == 1) {
						cube.GetComponent <Renderer>().material.color = Color.red;
					} else if (edgeGrid[i,j] == 2) {
						cube.GetComponent <Renderer>().material.color = Color.blue;
					} else if (edgeGrid[i,j] == 3) {
						cube.GetComponent <Renderer>().material.color = Color.green;
					} else if (edgeGrid[i,j] == 4) {
						cube.GetComponent <Renderer>().material.color = Color.yellow;
					} else if (edgeGrid[i,j] == 5) {
						cube.GetComponent <Renderer>().material.color = Color.gray;
					} else if (edgeGrid[i,j] == 6) {
						cube.GetComponent <Renderer>().material.color = Color.cyan;
					} else {
						cube.GetComponent <Renderer>().material.color = Color.white;
					}
				}
			}
		} else {
			for (int i=0;i<=LevelManager.FACE_MAGNITUDE*2;i++) {
				for (int j=0;j<=LevelManager.FACE_MAGNITUDE;j++) {
					GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
					cube.transform.position = new Vector3(origin.x,j + scale.y,i + scale.z);
					if (edgeGrid[i,j] == 1) {
						cube.GetComponent <Renderer>().material.color = Color.red;
					} else if (edgeGrid[i,j] == 2) {
						cube.GetComponent <Renderer>().material.color = Color.blue;
					} else if (edgeGrid[i,j] == 3) {
						cube.GetComponent <Renderer>().material.color = Color.green;
					} else if (edgeGrid[i,j] == 4) {
						cube.GetComponent <Renderer>().material.color = Color.yellow;
					} else if (edgeGrid[i,j] == 5) {
						cube.GetComponent <Renderer>().material.color = Color.gray;
					} else if (edgeGrid[i,j] == 6) {
						cube.GetComponent <Renderer>().material.color = Color.cyan;
					} else {
						cube.GetComponent <Renderer>().material.color = Color.white;
					}
				}
			}
		}
	}
}




































