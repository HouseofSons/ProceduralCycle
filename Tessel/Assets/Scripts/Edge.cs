using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Edge {

	private Face edgeFace;
	public Face face {
		get {return edgeFace;}
	}
	
	private Vector3 edgeRawOrigin;
	public Vector3 rawOrigin {
		get {return edgeRawOrigin;}
	}
	
	private Vector3 edgeRawTermination;
	public Vector3 rawTermination {
		get {return edgeRawTermination;}
	}
	
	private int edgeOrder;
	public int order {
		get {return edgeOrder;}
	}
	
	private Vector3 edgeDirection;
	public Vector3 direction {
		get {return edgeDirection;}
	}

	private Vector3 edgeOrigin;
	public Vector3 origin {
		get {return edgeOrigin;}
	}

	private Vector3 edgeTermination;
	public Vector3 termination {
		get {return edgeTermination;}
	}
	
	private List<Vector3> edgeCoordinates;
	public List<Vector3> coordinates {
		get {return edgeCoordinates;}
	}
	
	public Edge (Face face,Vector3 rawOrigin,Vector3 rawTermination,Vector3 direction,int order) {
		edgeFace = face;
		edgeRawOrigin = rawOrigin;
		edgeRawTermination = rawTermination;
		edgeDirection = direction;
		edgeOrder = order;
		edgeCoordinates = new List<Vector3> ();
		WalkEdge ();
		AssignTrueOriginTrueTermination ();
		DedupeEdgeCoordinates ();
	}

	public void WalkEdge() {

		bool terminationReached = false;
		bool validCoordinate = false;
		bool touchedBorder = false;
		bool collision = false;
		bool doStep = false;

		Vector3 insetLocation = edgeRawOrigin;
		Vector3 walkLocation = edgeRawOrigin;
		Vector3 insetBorderCheck = LevelManager.FALSE_VECTOR;
		Vector3 walkStep = LevelManager.FALSE_VECTOR;	
		Vector3 closeEdge = LevelManager.FALSE_VECTOR;

		int walkDistance = 0;

		Vector3 currentPos;
		
		if (edgeOrder == 1) {
			currentPos = edgeRawOrigin;
			doStep = true;
		} else {
			currentPos = edgeFace.edges[edgeOrder-2].termination;
		}

		while (!terminationReached) {

			if(doStep) {
				while (!validCoordinate) {

					insetLocation = ChooseInset(currentPos);
					insetBorderCheck = InsetCollidesBorder(insetLocation);

					if (insetBorderCheck != LevelManager.FALSE_VECTOR) {
						insetLocation = insetBorderCheck;
						touchedBorder = true;
					}

					if(edgeOrder == 4) {
						closeEdge = CloseFaceWithEdge(currentPos,insetLocation);

						if(closeEdge != insetLocation) {
							currentPos = closeEdge;
							edgeCoordinates.Add (currentPos);
							validCoordinate = true;
							collision = true;
							terminationReached = true;
						} else {
							currentPos = insetLocation;
							edgeCoordinates.Add (currentPos);
							validCoordinate = true;
						}
					} else {
						currentPos = insetLocation;
						edgeCoordinates.Add (currentPos);
						validCoordinate = true;
					}
				}
				if (!collision) {
					validCoordinate = false;
				}
			} else {//start of all none initial edges
				edgeCoordinates.Add (currentPos);
				doStep = true;
			}

			while (!validCoordinate) {

				walkLocation = ChooseWalk(currentPos);
				walkDistance = (int) Vector3.Distance(currentPos,walkLocation);
				walkStep = currentPos;

				do {

					walkStep = Vector3.MoveTowards(walkStep,walkLocation,1.00f);

					if (edgeOrder != 4) {
						if (WalkCollidesBorder(walkStep) != LevelManager.FALSE_VECTOR) {
							if (edgeOrder == 1) {
								if (touchedBorder) {
									currentPos = walkStep;
									edgeCoordinates.Add (currentPos);
									collision = true;
									validCoordinate = true;
									terminationReached = true;
								} else {//start entire edge over
									currentPos = edgeRawOrigin;
									edgeCoordinates = new List<Vector3>(); //restart coordinate captures
									collision = true; //stops inner loop
									validCoordinate = true; //not actually true but stops loop as intended
								}
							} else {
								currentPos = walkStep;
								edgeCoordinates.Add (currentPos);
								collision = true;
								validCoordinate = true;
								terminationReached = true;
							}
						}
					} else {
						closeEdge = WalkedIntoCloseEdge(walkStep);

						if (closeEdge != LevelManager.FALSE_VECTOR) {
							currentPos = closeEdge;
							edgeCoordinates.Add (walkStep);
							edgeCoordinates.Add (currentPos);
							collision = true;
							validCoordinate = true;
							terminationReached = true;
						}
					}
				} while (Vector3.Distance(currentPos,walkStep) < walkDistance && !collision);

				if (!terminationReached && !collision) {
					currentPos = walkLocation;
					edgeCoordinates.Add (currentPos);
					validCoordinate = true;
				}

				if (collision) {
					collision = false;
				}
			}
			validCoordinate = false;
		}
	}

	private void AssignTrueOriginTrueTermination() {
		edgeOrigin = edgeCoordinates [0];
		edgeTermination = edgeCoordinates [edgeCoordinates.Count - 1];
	}

	private Vector3 ChooseWalk(Vector3 pos) {

		Vector3 destination;

		int collisionDistance = (int)(LevelManager.FACE_MAGNITUDE_MIN);

		destination = pos + edgeDirection * Random.Range ((int)(LevelManager.FACE_MAGNITUDE_MIN),((int)(LevelManager.FACE_MAGNITUDE_MAX)));

		if (edgeDirection == Vector3.up ||
			edgeDirection == Vector3.down) {
			if(Mathf.Abs(destination.y-edgeRawTermination.y) <= collisionDistance) {
				return new Vector3(destination.x,edgeRawTermination.y,destination.z);
			} else {
				return destination;
			}
		} else if (edgeDirection == Vector3.forward ||
		           edgeDirection == Vector3.back) {
			if(Mathf.Abs(destination.z-edgeRawTermination.z) <= collisionDistance) {
				return new Vector3(destination.x,destination.y,edgeRawTermination.z);
			} else {
				return destination;
			}
		} else {
			if(Mathf.Abs(destination.x-edgeRawTermination.x) <= collisionDistance) {
				return new Vector3(edgeRawTermination.x,destination.y,destination.z);
			} else {
				return destination;
			}
		}
	}

	private Vector3 InsetCollidesBorder(Vector3 insetPos) {

		int faceOriginValue;
		int edgeOriginValue;
		int posValue;

		if (edgeFace.direction == Vector3.up ||
		    edgeFace.direction == Vector3.down) {

			if (edgeDirection == Vector3.forward ||
			    edgeDirection == Vector3.back) {
				faceOriginValue = (int)edgeFace.origin.x;
				edgeOriginValue = (int)edgeRawOrigin.x;
				posValue = (int) insetPos.x;
				if (Mathf.Abs(posValue - faceOriginValue)>=Mathf.Abs(edgeOriginValue - faceOriginValue)) {
					return new Vector3(edgeOriginValue,insetPos.y,insetPos.z);
				} else {
					return LevelManager.FALSE_VECTOR;
				}
			} else { //left or right
				faceOriginValue = (int)edgeFace.origin.z;
				edgeOriginValue = (int)edgeRawOrigin.z;
				posValue = (int) insetPos.z;
				if (Mathf.Abs(posValue - faceOriginValue)>=Mathf.Abs(edgeOriginValue - faceOriginValue)) {
					return new Vector3(insetPos.x,insetPos.y,edgeOriginValue);
				} else {
					return LevelManager.FALSE_VECTOR;
				}
			}

		} else if (	edgeFace.direction == Vector3.forward ||
		           edgeFace.direction == Vector3.back) {
			if (edgeDirection == Vector3.up ||
			    edgeDirection == Vector3.down) {
				faceOriginValue = (int)edgeFace.origin.x;
				edgeOriginValue = (int)edgeRawOrigin.x;
				posValue = (int) insetPos.x;
				if (Mathf.Abs(posValue - faceOriginValue)>=Mathf.Abs(edgeOriginValue - faceOriginValue)) {
					return new Vector3(edgeOriginValue,insetPos.y,insetPos.z);
				} else {
					return LevelManager.FALSE_VECTOR;
				}
			} else { //left or right
				faceOriginValue = (int)edgeFace.origin.y;
				edgeOriginValue = (int)edgeRawOrigin.y;
				posValue = (int) insetPos.y;
				if (Mathf.Abs(posValue - faceOriginValue)>=Mathf.Abs(edgeOriginValue - faceOriginValue)) {
					return new Vector3(insetPos.x,edgeOriginValue,insetPos.z);
				} else {
					return LevelManager.FALSE_VECTOR;
				}
			}
		} else {
			if (edgeDirection == Vector3.up ||
			    edgeDirection == Vector3.down) {
				faceOriginValue = (int)edgeFace.origin.z;
				edgeOriginValue = (int)edgeRawOrigin.z;
				posValue = (int) insetPos.z;
				if (Mathf.Abs(posValue - faceOriginValue)>=Mathf.Abs(edgeOriginValue - faceOriginValue)) {
					return new Vector3(insetPos.x,insetPos.y,edgeOriginValue);
				} else {
					return LevelManager.FALSE_VECTOR;
				}
			} else { //forward or back
				faceOriginValue = (int)edgeFace.origin.y;
				edgeOriginValue = (int)edgeRawOrigin.y;
				posValue = (int) insetPos.y;
				if (Mathf.Abs(posValue - faceOriginValue)>=Mathf.Abs(edgeOriginValue - faceOriginValue)) {
					return new Vector3(insetPos.x,edgeOriginValue,insetPos.z);
				} else {
					return LevelManager.FALSE_VECTOR;
				}
			}
		}
	}

	private Vector3 WalkCollidesBorder(Vector3 pos) {
		
		if (edgeDirection == Vector3.up ||
		    edgeDirection == Vector3.down) {
			if(Mathf.Abs((int)pos.y - (int)edgeRawOrigin.y) >= Mathf.Abs((int)edgeRawTermination.y - (int)edgeRawOrigin.y)) {
				return new Vector3(pos.x,edgeRawTermination.y,pos.z);
			}
		} else if (	edgeDirection == Vector3.forward ||
		           edgeDirection == Vector3.back) {
			if(Mathf.Abs((int)pos.z - (int)edgeRawOrigin.z) >= Mathf.Abs((int)edgeRawTermination.z - (int)edgeRawOrigin.z)) {
				return new Vector3(pos.x,pos.y,edgeRawTermination.z);
			}
		} else {
			if(Mathf.Abs((int)pos.x - (int)edgeRawOrigin.x) >= Mathf.Abs((int)edgeRawTermination.x - (int)edgeRawOrigin.x)) {
				return new Vector3(edgeRawTermination.x,pos.y,pos.z);
			}
		}
		return LevelManager.FALSE_VECTOR;
	}

	private Vector3 ChooseInset(Vector3 pos) {

		int edgeOriginValue;
		int posValue;
		int faceOriginBufferValue;

		int offSet = (Random.Range(0,2)*2-1) * Random.Range((int)(LevelManager.FACE_MAGNITUDE_MIN),(int)(LevelManager.FACE_MAGNITUDE_MAX));

		if (edgeFace.direction == Vector3.up ||
		    edgeFace.direction == Vector3.down) {
			
			if (edgeDirection == Vector3.forward ||
			    edgeDirection == Vector3.back) {
				edgeOriginValue = (int)edgeRawOrigin.x;
				posValue = (int) pos.x+offSet;

				if (edgeFace.origin.x < edgeRawOrigin.x) {
					faceOriginBufferValue = (int)(LevelManager.FACE_ORIGIN_BUFFER+(LevelManager.FACE_MAGNITUDE/2.00f) + edgeFace.origin.x);
				} else {
					faceOriginBufferValue = (int)(edgeFace.origin.x - LevelManager.FACE_ORIGIN_BUFFER-(LevelManager.FACE_MAGNITUDE/2.00f));
				}

				if ((faceOriginBufferValue > posValue && posValue > edgeOriginValue) ||
				    (faceOriginBufferValue < posValue && posValue < edgeOriginValue)) {
					return new Vector3(posValue,pos.y,pos.z);
				} else if ((posValue > faceOriginBufferValue && faceOriginBufferValue > edgeOriginValue) ||
				           (posValue < faceOriginBufferValue && faceOriginBufferValue < edgeOriginValue)) {
					return new Vector3(faceOriginBufferValue,pos.y,pos.z);
				} else {
					return new Vector3(edgeOriginValue,pos.y,pos.z);
				}
			} else { //left or right
				edgeOriginValue = (int)edgeRawOrigin.z;
				posValue = (int) pos.z+offSet;
				
				if (edgeFace.origin.z < edgeRawOrigin.z) {
					faceOriginBufferValue = (int)(LevelManager.FACE_ORIGIN_BUFFER + edgeFace.origin.z);
				} else {
					faceOriginBufferValue = (int)(edgeFace.origin.z - LevelManager.FACE_ORIGIN_BUFFER);
				}

				if ((faceOriginBufferValue > posValue && posValue > edgeOriginValue) ||
				    (faceOriginBufferValue < posValue && posValue < edgeOriginValue)) {
					return new Vector3(pos.x,pos.y,posValue);
				} else if ((posValue > faceOriginBufferValue && faceOriginBufferValue > edgeOriginValue) ||
				           (posValue < faceOriginBufferValue && faceOriginBufferValue < edgeOriginValue)) {
					return new Vector3(pos.x,pos.y,faceOriginBufferValue);
				} else {
					return new Vector3(pos.x,pos.y,edgeOriginValue);
				}
			}
			
		} else if (	edgeFace.direction == Vector3.forward ||
		           edgeFace.direction == Vector3.back) {
			if (edgeDirection == Vector3.up ||
			    edgeDirection == Vector3.down) {
				edgeOriginValue = (int)edgeRawOrigin.x;
				posValue = (int) pos.x+offSet;
				
				if (edgeFace.origin.x < edgeRawOrigin.x) {
					faceOriginBufferValue = (int)(LevelManager.FACE_ORIGIN_BUFFER+(LevelManager.FACE_MAGNITUDE/2.00f) + edgeFace.origin.x);
				} else {
					faceOriginBufferValue = (int)(edgeFace.origin.x - LevelManager.FACE_ORIGIN_BUFFER-(LevelManager.FACE_MAGNITUDE/2.00f));
				}
				
				if ((faceOriginBufferValue > posValue && posValue > edgeOriginValue) ||
				    (faceOriginBufferValue < posValue && posValue < edgeOriginValue)) {
					return new Vector3(posValue,pos.y,pos.z);
				} else if ((posValue > faceOriginBufferValue && faceOriginBufferValue > edgeOriginValue) ||
				           (posValue < faceOriginBufferValue && faceOriginBufferValue < edgeOriginValue)) {
					return new Vector3(faceOriginBufferValue,pos.y,pos.z);
				} else {
					return new Vector3(edgeOriginValue,pos.y,pos.z);
				}
			} else { //left or right
				edgeOriginValue = (int)edgeRawOrigin.y;
				posValue = (int) pos.y+offSet;
				
				if (edgeFace.origin.y < edgeRawOrigin.y) {
					faceOriginBufferValue = (int)(LevelManager.FACE_ORIGIN_BUFFER + edgeFace.origin.y);
				} else {
					faceOriginBufferValue = (int)(edgeFace.origin.y - LevelManager.FACE_ORIGIN_BUFFER);
				}
				
				if ((faceOriginBufferValue > posValue && posValue > edgeOriginValue) ||
				    (faceOriginBufferValue < posValue && posValue < edgeOriginValue)) {
					return new Vector3(pos.x,posValue,pos.z);
				} else if ((posValue > faceOriginBufferValue && faceOriginBufferValue > edgeOriginValue) ||
				           (posValue < faceOriginBufferValue && faceOriginBufferValue < edgeOriginValue)) {
					return new Vector3(pos.x,faceOriginBufferValue,pos.z);
				} else {
					return new Vector3(pos.x,edgeOriginValue,pos.z);
				}
			}
		} else {
			if (edgeDirection == Vector3.up ||
			    edgeDirection == Vector3.down) {
				edgeOriginValue = (int)edgeRawOrigin.z;
				posValue = (int) pos.z+offSet;
				
				if (edgeFace.origin.z < edgeRawOrigin.z) {
					faceOriginBufferValue = (int)(LevelManager.FACE_ORIGIN_BUFFER+(LevelManager.FACE_MAGNITUDE/2.00f) + edgeFace.origin.z);
				} else {
					faceOriginBufferValue = (int)(edgeFace.origin.z - LevelManager.FACE_ORIGIN_BUFFER-(LevelManager.FACE_MAGNITUDE/2.00f));
				}
				
				if ((faceOriginBufferValue > posValue && posValue > edgeOriginValue) ||
				    (faceOriginBufferValue < posValue && posValue < edgeOriginValue)) {
					return new Vector3(pos.x,pos.y,posValue);
				} else if ((posValue > faceOriginBufferValue && faceOriginBufferValue > edgeOriginValue) ||
				           (posValue < faceOriginBufferValue && faceOriginBufferValue < edgeOriginValue)) {
					return new Vector3(pos.x,pos.y,faceOriginBufferValue);
				} else {
					return new Vector3(pos.x,pos.y,edgeOriginValue);
				}
			} else { //forward or back
				edgeOriginValue = (int)edgeRawOrigin.y;
				posValue = (int) pos.y+offSet;
				
				if (edgeFace.origin.y < edgeRawOrigin.y) {
					faceOriginBufferValue = (int)(LevelManager.FACE_ORIGIN_BUFFER + edgeFace.origin.y);
				} else {
					faceOriginBufferValue = (int)(edgeFace.origin.y - LevelManager.FACE_ORIGIN_BUFFER);
				}
				
				if ((faceOriginBufferValue > posValue && posValue > edgeOriginValue) ||
				    (faceOriginBufferValue < posValue && posValue < edgeOriginValue)) {
					return new Vector3(pos.x,posValue,pos.z);
				} else if ((posValue > faceOriginBufferValue && faceOriginBufferValue > edgeOriginValue) ||
				           (posValue < faceOriginBufferValue && faceOriginBufferValue < edgeOriginValue)) {
					return new Vector3(pos.x,faceOriginBufferValue,pos.z);
				} else {
					return new Vector3(pos.x,edgeOriginValue,pos.z);
				}
			}
		}
	}
	
	private void DedupeEdgeCoordinates() {

		Vector3 curr = LevelManager.FALSE_VECTOR;
		Vector3 prev = LevelManager.FALSE_VECTOR;

		List<Vector3> l = new List<Vector3> ();

		foreach (Vector3 v in edgeCoordinates) {
			prev = curr;
			curr = v;

			if (prev == curr) {
				l.Add (curr);
			}
		}

		Vector3 first = LevelManager.FALSE_VECTOR;
		Vector3 second = LevelManager.FALSE_VECTOR;
		Vector3 third = LevelManager.FALSE_VECTOR;

		foreach (Vector3 v in edgeCoordinates) {
			third = second;
			second = first;
			first = v;
			
			if (third != LevelManager.FALSE_VECTOR) {
				if(Vector3.Normalize(second-first) == Vector3.Normalize(third-second)) {
					l.Add (second);
				}
			}
		}
		
		foreach (Vector3 i in l) {
			edgeCoordinates.Remove(i);
		}
	}

	private Vector3 CloseFaceWithEdge(Vector3 pos,Vector3 destination) {

		int walkDistance = (int) Vector3.Distance (pos, destination);

		Vector3 walkStep = pos;
		Vector3 closeEdge = LevelManager.FALSE_VECTOR;

		do {
			closeEdge = WalkedIntoCloseEdge(walkStep);
			if(closeEdge != LevelManager.FALSE_VECTOR) {
				edgeCoordinates.Add (walkStep);//really bad practice but fixes Instep bug where edge doesn't fully connect
				return closeEdge;
			}

			walkStep = Vector3.MoveTowards(walkStep,destination,1.00f);
		} while ((int)Vector3.Distance(pos,walkStep) < walkDistance);

		return destination;
	}

	private Vector3 WalkedIntoCloseEdge(Vector3 pos) {

		Vector3 focus = pos - (edgeFace.origin - edgeFace.GridCenter());

		int x;
		int y;
		
		if ((edgeFace.direction == Vector3.up) ||
		    (edgeFace.direction == Vector3.down)) {
			x = (int)focus.x;
			y = (int)focus.z;
			if (0 < x && x < edgeFace.edgeGrid.GetLength(0) && 0 < y && y < edgeFace.edgeGrid.GetLength(1) && edgeFace.edgeGrid [x, y] == 1) {
				return pos;
			}
			if(0 < x+1 && x+1 < edgeFace.edgeGrid.GetLength(0) && 0 < y && y < edgeFace.edgeGrid.GetLength(1) && edgeFace.edgeGrid [x+1,y] == 1) {
				return new Vector3(pos.x+1,pos.y,pos.z);
			}
			if(0 < x-1 && x-1 < edgeFace.edgeGrid.GetLength(0) && 0 < y && y < edgeFace.edgeGrid.GetLength(1) && edgeFace.edgeGrid [x-1,y] == 1) {
				return new Vector3(pos.x-1,pos.y,pos.z);
			}
			if(0 < x && x < edgeFace.edgeGrid.GetLength(0) && 0 < y+1 && y+1 < edgeFace.edgeGrid.GetLength(1) && edgeFace.edgeGrid [x,y+1] == 1) {
				return new Vector3(pos.x,pos.y,pos.z+1);
			}
			if(0 < x && x < edgeFace.edgeGrid.GetLength(0) && 0 < y-1 && y-1 < edgeFace.edgeGrid.GetLength(1) && edgeFace.edgeGrid [x,y-1] == 1) {
				return new Vector3(pos.x,pos.y,pos.z-1);
			}
			return LevelManager.FALSE_VECTOR;
		} else if ((edgeFace.direction == Vector3.forward) ||
		           (edgeFace.direction == Vector3.back)) {
			x = (int)focus.x;
			y = (int)focus.y;
			if (0 < x && x < edgeFace.edgeGrid.GetLength(0) && 0 < y && y < edgeFace.edgeGrid.GetLength(1) && edgeFace.edgeGrid [x, y] == 1) {
				return pos;
			}
			if(0 < x+1 && x+1 < edgeFace.edgeGrid.GetLength(0) && 0 < y && y < edgeFace.edgeGrid.GetLength(1) && edgeFace.edgeGrid [x+1,y] == 1) {
				return new Vector3(pos.x+1,pos.y,pos.z);
			}
			if(0 < x-1 && x-1 < edgeFace.edgeGrid.GetLength(0) && 0 < y && y < edgeFace.edgeGrid.GetLength(1) && edgeFace.edgeGrid [x-1,y] == 1) {
				return new Vector3(pos.x-1,pos.y,pos.z);
			}
			if(0 < x && x < edgeFace.edgeGrid.GetLength(0) && 0 < y+1 && y+1 < edgeFace.edgeGrid.GetLength(1) && edgeFace.edgeGrid [x,y+1] == 1) {
				return new Vector3(pos.x,pos.y+1,pos.z);
			}
			if(0 < x && x < edgeFace.edgeGrid.GetLength(0) && 0 < y-1 && y-1 < edgeFace.edgeGrid.GetLength(1) && edgeFace.edgeGrid [x,y-1] == 1) {
				return new Vector3(pos.x,pos.y-1,pos.z);
			}
			return LevelManager.FALSE_VECTOR;
		} else {
			x = (int)focus.z;
			y = (int)focus.y;
			if (0 < x && x < edgeFace.edgeGrid.GetLength(0) && 0 < y && y < edgeFace.edgeGrid.GetLength(1) && edgeFace.edgeGrid [x, y] == 1) {
				return pos;
			}
			if(0 < x+1 && x+1 < edgeFace.edgeGrid.GetLength(0) && 0 < y && y < edgeFace.edgeGrid.GetLength(1) && edgeFace.edgeGrid [x+1,y] == 1) {
				return new Vector3(pos.x,pos.y,pos.z+1);
			}
			if(0 < x-1 && x-1 < edgeFace.edgeGrid.GetLength(0) && 0 < y && y < edgeFace.edgeGrid.GetLength(1) && edgeFace.edgeGrid [x-1,y] == 1) {
				return new Vector3(pos.x,pos.y,pos.z-1);
			}
			if(0 < x && x < edgeFace.edgeGrid.GetLength(0) && 0 < y+1 && y+1 < edgeFace.edgeGrid.GetLength(1) && edgeFace.edgeGrid [x,y+1] == 1) {
				return new Vector3(pos.x,pos.y+1,pos.z);
			}
			if(0 < x && x < edgeFace.edgeGrid.GetLength(0) && 0 < y-1 && y-1 < edgeFace.edgeGrid.GetLength(1) && edgeFace.edgeGrid [x,y-1] == 1) {
				return new Vector3(pos.x,pos.y-1,pos.z);
			}
			return LevelManager.FALSE_VECTOR;
		}
	}
}


















