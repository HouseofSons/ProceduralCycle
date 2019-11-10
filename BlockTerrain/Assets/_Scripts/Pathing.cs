using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathing : MonoBehaviour {

	public List<GameBoardHex> pathsFinder(GameBoardHex thisHex, GameBoardHex destCharacterHex) {

		//Debug.Log ("CALCULATING PATH from " + thisHex.toString () + " to char" + destCharacterHex.toString ());//OUTPUT DEBUG STUFF

		List<GameBoardHex> shortestPath = new List<GameBoardHex>();

		bool found = InitOpenCloseLists(thisHex, destCharacterHex);

		GameBoardHex nextNode = null;
		int noPath = -1;
				
		while ((destCharacterHex.GetParent() == null && noPath != 0) && !found) {
			nextNode = null;
			noPath = 0;

			foreach(GameBoardHex hex in GameBoardHexGrid.ColumnOfNeighboringHexs(destCharacterHex.GetXCoord(),
			                                                                     destCharacterHex.GetYCoord(),
			                                                                     destCharacterHex.GetZCoord(),
			                                                                     destCharacterHex.GetOccupant().GetMoveDistance(),
			                                                                     destCharacterHex.GetOccupant().GetMoveDistance())) {
				int i = hex.GetXCoord();
				int j = hex.GetYCoord();
				int k = hex.GetZCoord();

				if (GameBoardHexGrid.GameBoard()[i,j,k].GetState() == 1) {
					if (GameBoardHexGrid.GameBoard()[i,j,k].IsOccupied ()) {
						if (GameBoardHexGrid.GameBoard()[i,j,k].GetOccupant().IsFriendly() == destCharacterHex.GetOccupant().IsFriendly()) {
							if (GameBoardHexGrid.GameBoard()[i,j,k].GetFValue() > 0 && GameBoardHexGrid.GameBoard()[i,j,k].GetFValue() < 99) {
								if(nextNode == null || GameBoardHexGrid.GameBoard()[i,j,k].GetFValue() < nextNode.GetFValue()) {
									nextNode = GameBoardHexGrid.GameBoard()[i,j,k];
									noPath++;
								}
							}
						}
					} else {
						if (GameBoardHexGrid.GameBoard()[i,j,k].GetFValue() > 0 && GameBoardHexGrid.GameBoard()[i,j,k].GetFValue() < 99) {
							if(nextNode == null || GameBoardHexGrid.GameBoard()[i,j,k].GetFValue() < nextNode.GetFValue()) {
								nextNode = GameBoardHexGrid.GameBoard()[i,j,k];
								noPath++;
							}
						}
					}
				}
			}
			if (noPath > 0) {
				//Debug.Log ("LOWEST FVALUE " + nextNode.toString () + " to char" + destCharacterHex.toString ());//OUTPUT DEBUG STUFF
				UpdatePathNeighbors(nextNode,destCharacterHex,false);
			}
		}

		if (destCharacterHex.GetParent () != null) {
			shortestPath.Add (destCharacterHex);
			nextNode = destCharacterHex.GetParent ();

			while (nextNode != thisHex && destCharacterHex.GetParent() != null) {
				shortestPath.Add (nextNode);
				nextNode = nextNode.GetParent();
			}
		}
		shortestPath.Add (thisHex);
		return shortestPath;
	}

	public static int AxialHexDistance(int x1, int z1, int x2, int z2) {
		return Mathf.Max (Mathf.Abs ((x1 - x2)),
		                  Mathf.Abs ((z1 - z2)),
		                  Mathf.Abs ((z1 - z2) - (x1 - x2)));
	}
	
	public static int HexHeuristicDistance(GameBoardHex start, GameBoardHex destination) {
		return AxialHexDistance (start.GetXCoord (),start.GetZCoord (),destination.GetXCoord (),destination.GetZCoord ());
	}
	
	private bool InitOpenCloseLists (GameBoardHex thisHex, GameBoardHex destCharacterHex) {

		if (HexHeuristicDistance(thisHex, destCharacterHex) == 1 && Mathf.Abs(thisHex.GetYCoord() - destCharacterHex.GetYCoord()) <= destCharacterHex.GetOccupant().GetJumpHeight()) {
			destCharacterHex.SetParent(thisHex);
			return true;
		}

		thisHex.SetState (-1);
		destCharacterHex.SetParent(null);

		int h = 100;

		foreach (GameBoardHex hex in GameBoardHexGrid.ColumnOfNeighboringHexs(destCharacterHex.GetXCoord(),
		                                                                     destCharacterHex.GetYCoord(),
		                                                                     destCharacterHex.GetZCoord(),
		                                                                     destCharacterHex.GetOccupant().GetMoveDistance(),
		                                                                     destCharacterHex.GetOccupant().GetMoveDistance())) {
			int i = hex.GetXCoord ();
			int j = hex.GetYCoord ();
			int k = hex.GetZCoord ();

			GameBoardHexGrid.GameBoard () [i, j, k].SetState (0);
			if (GameBoardHexGrid.GameBoard () [i, j, k] != destCharacterHex) {
				h = HexHeuristicDistance (GameBoardHexGrid.GameBoard () [i, j, k], destCharacterHex);

				if (h > destCharacterHex.GetOccupant ().GetMoveDistance ()) {
					GameBoardHexGrid.GameBoard () [i, j, k].SetHeuristic (99);
					GameBoardHexGrid.GameBoard () [i, j, k].SetFValue (99);
					//Debug.Log (terrainBlocks[i,j,k].toString() + " INITIALLY unneeded h & f = 99");//OUTPUT DEBUG STUFF
				} else {
					GameBoardHexGrid.GameBoard () [i, j, k].SetHeuristic (h);				
					GameBoardHexGrid.GameBoard () [i, j, k].SetMovementCost (1);
					GameBoardHexGrid.GameBoard () [i, j, k].SetFValue (-1);
					//Debug.Log (terrainBlocks[i,j,k].toString() + " INITIALLY gets h,m,f: "+h+",1,-1");//OUTPUT DEBUG STUFF
				}
			}

		}
		UpdatePathNeighbors(thisHex,destCharacterHex,true);
		return false;
	}

	public static List<GameBoardHex> GetNeighboringHexs(int i, int j, int k, int jumpHeight) {
		
		List<GameBoardHex> neighbors = new List<GameBoardHex>();

		for (int x=-1;x<=1;x++) {
			for (int y=-jumpHeight;y<=jumpHeight;y++) {
				for (int z=-1;z<=1;z++) {
					if (x+z != 0) {
						if (i + x >= 0 &&
							j + y >= 0 &&
						    k + z >= 0 &&
						    i + x <= GameBoardHexGrid.GameBoard().GetUpperBound (0) &&
						    j + y <= GameBoardHexGrid.GameBoard().GetUpperBound (1) &&
						    k + z <= GameBoardHexGrid.GameBoard().GetUpperBound (2)) {

							if (GameBoardHexGrid.GameBoard()[i+x,j+y,k+z] != null) {
								neighbors.Add (GameBoardHexGrid.GameBoard()[i+x,j+y,k+z]);
							}
						}
					}
				}
			}
		}
		return neighbors;
	}

	private void UpdatePathNeighbors(GameBoardHex hexNode, GameBoardHex charDest, bool start) {
		if (!start) {
			hexNode.SetState(-1);
		}

		foreach(GameBoardHex hex in GetNeighboringHexs(hexNode.GetXCoord(),hexNode.GetYCoord(),hexNode.GetZCoord(),charDest.GetOccupant().GetJumpHeight())) {

			int i = hex.GetXCoord();
			int j = hex.GetYCoord();
			int k = hex.GetZCoord();

			if ( GameBoardHexGrid.GameBoard()[i,j,k] != hexNode && Mathf.Abs(GameBoardHexGrid.GameBoard()[i,j,k].GetYCoord() - hexNode.GetYCoord()) <= charDest.GetOccupant().GetJumpHeight()) {
				if (GameBoardHexGrid.GameBoard()[i,j,k].IsOccupied()) {
					if (GameBoardHexGrid.GameBoard()[i,j,k] == charDest || (GameBoardHexGrid.GameBoard()[i,j,k].GetOccupant().IsFriendly() == charDest.GetOccupant().IsFriendly())) {
						if (start) {
							GameBoardHexGrid.GameBoard()[i,j,k].SetParent(hexNode);
							GameBoardHexGrid.GameBoard()[i,j,k].SetMovementCost(1);
							GameBoardHexGrid.GameBoard()[i,j,k].SetFValue(GameBoardHexGrid.GameBoard()[i,j,k].GetMovementCost() + GameBoardHexGrid.GameBoard()[i,j,k].GetHeurisitic());
							GameBoardHexGrid.GameBoard()[i,j,k].SetState(1);

							//Debug.Log (terrainBlocks[i,j,k].toString() + " START UPDATED Parented to "+hexNode.toString()+" added to openlist gets m,f: 1,"+(terrainBlocks[i,j,k].GetMovementCost() + terrainBlocks[i,j,k].GetHeurisitic()));//OUTPUT DEBUG STUFF
							
						} else {
							if (GameBoardHexGrid.GameBoard()[i,j,k].GetState() != -1) {
								if (GameBoardHexGrid.GameBoard()[i,j,k].GetState() == 1) {
									if (hexNode.GetMovementCost() + 1/* may be variable movement cost in the future*/ < GameBoardHexGrid.GameBoard()[i,j,k].GetMovementCost()) {
										GameBoardHexGrid.GameBoard()[i,j,k].SetParent(hexNode);

										//Debug.Log (terrainBlocks[i,j,k].toString() + " REPARENTED to "+hexNode.toString());//OUTPUT DEBUG STUFF
									}
								} else {
									GameBoardHexGrid.GameBoard()[i,j,k].SetParent(hexNode);
									GameBoardHexGrid.GameBoard()[i,j,k].SetMovementCost(hexNode.GetMovementCost() + 1/* may be variable movement cost in the future*/);
									GameBoardHexGrid.GameBoard()[i,j,k].SetFValue(GameBoardHexGrid.GameBoard()[i,j,k].GetMovementCost() + GameBoardHexGrid.GameBoard()[i,j,k].GetHeurisitic());
									GameBoardHexGrid.GameBoard()[i,j,k].SetState(1);

									//Debug.Log (terrainBlocks[i,j,k].toString() + " UPDATED Parented to "+hexNode.toString()+" added to openlist gets m,f: " + (hexNode.GetMovementCost() + 1) + ","+(terrainBlocks[i,j,k].GetMovementCost() + terrainBlocks[i,j,k].GetHeurisitic()));//OUTPUT DEBUG STUFF
								}
							}
						}
					}
				} else {
					if (start) {
						GameBoardHexGrid.GameBoard()[i,j,k].SetParent(hexNode);
						GameBoardHexGrid.GameBoard()[i,j,k].SetMovementCost(1);
						GameBoardHexGrid.GameBoard()[i,j,k].SetFValue(GameBoardHexGrid.GameBoard()[i,j,k].GetMovementCost() + GameBoardHexGrid.GameBoard()[i,j,k].GetHeurisitic());
						GameBoardHexGrid.GameBoard()[i,j,k].SetState(1);

						//Debug.Log (terrainBlocks[i,j,k].toString() + " START UPDATED Parented to "+hexNode.toString()+" added to openlist gets m,f: 1,"+(terrainBlocks[i,j,k].GetMovementCost() + terrainBlocks[i,j,k].GetHeurisitic()));//OUTPUT DEBUG STUFF
						
					} else {
						if (GameBoardHexGrid.GameBoard()[i,j,k].GetState() != -1) {
							if (GameBoardHexGrid.GameBoard()[i,j,k].GetState() == 1) {
								if (hexNode.GetMovementCost() + 1/* may be variable movement cost in the future*/ < GameBoardHexGrid.GameBoard()[i,j,k].GetMovementCost()) {
									GameBoardHexGrid.GameBoard()[i,j,k].SetParent(hexNode);

									//Debug.Log (terrainBlocks[i,j,k].toString() + " REPARENTED to "+hexNode.toString());//OUTPUT DEBUG STUFF
								}
							} else {
								GameBoardHexGrid.GameBoard()[i,j,k].SetParent(hexNode);
								GameBoardHexGrid.GameBoard()[i,j,k].SetMovementCost(hexNode.GetMovementCost() + 1/* may be variable movement cost in the future*/);
								GameBoardHexGrid.GameBoard()[i,j,k].SetFValue(GameBoardHexGrid.GameBoard()[i,j,k].GetMovementCost() + GameBoardHexGrid.GameBoard()[i,j,k].GetHeurisitic());
								GameBoardHexGrid.GameBoard()[i,j,k].SetState(1);

								//Debug.Log (terrainBlocks[i,j,k].toString() + " UPDATED Parented to "+hexNode.toString()+" added to openlist gets m,f: " + (hexNode.GetMovementCost() + 1) + ","+(terrainBlocks[i,j,k].GetMovementCost() + terrainBlocks[i,j,k].GetHeurisitic()));//OUTPUT DEBUG STUFF
							}
						}
					}
				}
			}
		}
	}
	
	public static void CharacterPaths() {

		foreach (GameBoardHex hex in GameBoardHexGrid.ColumnOfNeighboringHexs(GameBoardHex.GetCTCHex().GetXCoord(),
		                                                                      GameBoardHex.GetCTCHex().GetYCoord(),
		                                                                      GameBoardHex.GetCTCHex().GetZCoord(),
		                                                                      GameBoardHex.GetCTCHex().GetOccupant().GetMoveDistance(),
		                                                                      GameBoardHex.GetCTCHex().GetOccupant().GetMoveDistance())) {
			int i = hex.GetXCoord ();
			int j = hex.GetYCoord ();
			int k = hex.GetZCoord ();

			GameBoardHexGrid.GameBoard () [i, j, k].ReCreatedPathsToCharacters ();
						
			if (GameBoardHexGrid.GameBoard () [i, j, k] != GameBoardHex.GetCTCHex () && GameBoardHex.GetCTCHex ().GetOccupant ().GetMoveDistance () >= Pathing.HexHeuristicDistance (GameBoardHexGrid.GameBoard () [i, j, k], GameBoardHex.GetCTCHex ())) {
							
				GameBoardHexGrid.GameBoard () [i, j, k].GetPathsToCharacters ().Add (GameObject.Find ("LevelManager").GetComponent<Pathing> ().pathsFinder (GameBoardHexGrid.GameBoard () [i, j, k], GameBoardHex.GetCTCHex ()));
			}

		}
	}

	public static void AttackMoveGrid(GameBoardHex hex, bool isOn, bool hasMoved) {
		if (isOn && !hasMoved) {
			foreach (GameBoardHex GameHex in GameBoardHexGrid.ColumnOfNeighboringHexs(hex.GetXCoord(),
			                                                                          hex.GetYCoord(),
			                                                                          hex.GetZCoord(),
			                                                                          hex.GetOccupant().GetMoveDistance(),
			                                                                          hex.GetOccupant().GetMoveDistance())) {
				int i = GameHex.GetXCoord ();
				int j = GameHex.GetYCoord ();
				int k = GameHex.GetZCoord ();

				GameBoardHexGrid.GameBoard()[i,j,k].IsFocusMoveable(GameBoardHexGrid.GameBoard()[i,j,k].HexMoveRange(hex.GetOccupant()));
				if(GameBoardHexGrid.GameBoard()[i,j,k].GetHeurisitic() <= hex.GetOccupant().GetAttackDistance()) {
					if (GameBoardHexGrid.GameBoard()[i,j,k].IsOccupied()) {
						if (GameBoardHexGrid.GameBoard()[i,j,k].GetOccupant().IsFriendly() != GameBoardHex.GetCTCHex().GetOccupant().IsFriendly()) {
							GameBoardHexGrid.GameBoard()[i,j,k].IsFocusAttackable(true);
						}
					} else {
						GameBoardHexGrid.GameBoard()[i,j,k].IsFocusAttackable(true);
					}
				}
				GameBoardHexGrid.GameBoard()[i,j,k].changed = true;
			}
		} else if (isOn && hasMoved) {
			for (int i = hex.GetXCoord() - hex.GetOccupant().GetAttackDistance(); i <= hex.GetXCoord() + hex.GetOccupant().GetAttackDistance(); i++) {
				for (int j = hex.GetYCoord() - hex.GetOccupant().GetAttackDistance(); j <= hex.GetYCoord() + hex.GetOccupant().GetAttackDistance(); j++) {
					for (int k = hex.GetZCoord() - hex.GetOccupant().GetAttackDistance(); k <= hex.GetZCoord() + hex.GetOccupant().GetAttackDistance(); k++) {
						if (i >= 0 && j >= 0 && k >= 0 &&
						    i <= GameBoardHexGrid.GameBoard().GetUpperBound(0) &&
						    j <= GameBoardHexGrid.GameBoard().GetUpperBound(1) &&
						    k <= GameBoardHexGrid.GameBoard().GetUpperBound(2)) {
							if(GameBoardHexGrid.GameBoard()[i,j,k] != null) {
								if (Pathing.AxialHexDistance(hex.GetXCoord(),hex.GetZCoord(),i,k) <= hex.GetOccupant().GetAttackDistance() &&
								    Mathf.Abs (hex.GetYCoord() - j) <= hex.GetOccupant().GetAttackDistance()) {
									GameBoardHexGrid.GameBoard()[i,j,k].IsFocusAttackable(true);
									GameBoardHexGrid.GameBoard()[i,j,k].changed = true;
								}
							}
						}
					}
				}
			}
		} else {
			for (int i = 0; i <= GameBoardHexGrid.GameBoard().GetUpperBound(0); i++) {
				for (int j = 0; j <= GameBoardHexGrid.GameBoard().GetUpperBound(1); j++) {
					for (int k = 0; k <= GameBoardHexGrid.GameBoard().GetUpperBound(2); k++) {
						if (GameBoardHexGrid.GameBoard()[i,j,k] != null) {
							GameBoardHexGrid.GameBoard()[i,j,k].IsFocusMoveable(false);
							GameBoardHexGrid.GameBoard()[i,j,k].IsFocusAttackable(false);
							GameBoardHexGrid.GameBoard()[i,j,k].changed = true;
						}
					}
				}
			}
		}
	}
}
