﻿using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	
	//used to determine current spawn location
	private Spawn latestSpawn;

    //Partition Player is Occupying
    public Partition OccupiedPartition { get; set; }

    //CoRoutine which moves player to spawn
    private static Coroutine moveToSpawnCoRoutine;

	//Direction Player is moving
	private static Vector3 playerMovingDirection;
    //Disables Collisions when traveling between levels
	private static bool disablePlayerCollisions;
    //Identifies if WallCollision method is done
    private static bool UpdatingWallCollisions;

    //Wall points calculated by Players chosen path
    public static List<Vector3> PathPoints;

    void Awake () {
		PlayerPathDistanceMax = 200;
		PlayerPathDistance = 0;
		TotalExperiencePoints = 0;
		playerMovingDirection = Vector3.zero;
		disablePlayerCollisions = false;
        UpdatingWallCollisions = false;
    }

	void Start () {
        UI.InitializeUIWithPlayerInfo ();
        PathPoints = new List<Vector3>();
    }

	void Update () {

        if (GameManager.MoveToSpawnState) {
            GameManager.PathLine().enabled = false;
            GameManager.PathChosenLine().enabled = false;
			if (moveToSpawnCoRoutine != null) {
				StopCoroutine(moveToSpawnCoRoutine);
			}
			moveToSpawnCoRoutine = StartCoroutine (MoveToSpawn());
        }
        
        if (GameManager.AimArrowState)
        {
            if (Input.GetMouseButton(0))
            {
                if (!UpdatingWallCollisions && Time.frameCount % 5 == 0) {
                    UpdatingWallCollisions = true;
                    CollisionPath.ClearCollisions();
                    UpdateWallCollisions(
                        transform.position,
                        new Vector3(
                            GameManager.AimArrow().transform.up.x + transform.position.x,
                            transform.position.y,
                            GameManager.AimArrow().transform.up.z + transform.position.z),
                        PlayerPathDistanceMax - PlayerPathDistance,
                        OccupiedPartition,
                        0);
                    GameManager.PathLine().enabled = true;
                    GameManager.PathChosenLine().enabled = false;
                }
            }

            if (Input.GetMouseButtonUp(0)) {
                if (!EventSystem.current.IsPointerOverGameObject()) {
                    GameManager.PlayerMovingState = true;
                    GameManager.PathLine().enabled = false;
                    GameManager.PathChosenLine().enabled = true;
                    playerMovingDirection = GameManager.AimArrow().transform.up;
                    PathPoints = new List<Vector3>(CollisionPath.Collisions); //needed for separate list
                    if (PlayerFollowPathCoRoutine != null)
                    {
                        StopCoroutine(PlayerFollowPathCoRoutine);
                    }
                    PlayerFollowPathCoRoutine = StartCoroutine (PlayerFollowPath ());
                    UpdateChosenPath();
                }
			}
		}
	}

	void OnTriggerEnter(Collider col) {

		if(!disablePlayerCollisions) {
            if (col.gameObject.name.StartsWith("Door")) {
                disablePlayerCollisions = true;
                StopCoroutine(PlayerFollowPathCoRoutine);//stops player movement if Door Hit
                GameManager.DoorHit(col.transform.GetComponent<Door>());
            }
		}
	}

    public static Coroutine PlayerFollowPathCoRoutine { get; private set; }

    public static void UpdateExperiencePoints (int experiencePoints) {
		TotalExperiencePoints += experiencePoints;
		UI.UpdateExperienceText (TotalExperiencePoints);
	}

    public static float PlayerPathDistanceMax { get; set; }

    public static float PlayerPathDistance { get; set; }

    public static float Energy {
		get {return (PlayerPathDistanceMax - PlayerPathDistance) > 0.0f ? (PlayerPathDistanceMax - PlayerPathDistance):0.0f;}
	}

    public static int TotalExperiencePoints { get; set; }

    public static Vector3 PlayerMovingDirection {
		get{return playerMovingDirection;}
	}
	//Determines where player will spawn
	public Spawn LatestSpawn {
		get {return latestSpawn; }
		set { latestSpawn = value;}
	}
	//Resets Path lengths for Player
	public static void LevelStatsReset(float distanceMax) {
		PlayerPathDistanceMax = distanceMax;
		PlayerPathDistance = 0;
		UI.UpdateEnergyText(Mathf.FloorToInt(Energy));
	}
	//Moves Player across Level
	private IEnumerator PlayerFollowPath()
    {
        int index = 0;
        Vector3 prevPosition = transform.position;
        Vector3 nextPosition = PathPoints[index];
        Vector3 lastOccupiedPosition = transform.position;

        while (PlayerPathDistance < PlayerPathDistanceMax)
        {
			while (GameManager.IsPaused) { //for game pause
				yield return null;
			}
            //checks if player has passed nextPosition
            if(Mathf.Abs(Vector3.Distance(transform.position,prevPosition)) >= Mathf.Abs(Vector3.Distance(nextPosition, prevPosition))-0.1)
            {
                transform.position = nextPosition;
                prevPosition = nextPosition;
                index++;
                if (PathPoints.Count > index) {
                    nextPosition = PathPoints[index];
                }
            }
            if (PathPoints.Count > index)
            {
                transform.position = Vector3.MoveTowards(transform.position, nextPosition, GameManager.Speed);
                PlayerPathDistance += Mathf.Abs(Vector3.Distance(transform.position, lastOccupiedPosition));
                lastOccupiedPosition = transform.position;
                UI.UpdateEnergyText(Mathf.FloorToInt(Energy));
            }

			yield return null;
		}
		GameManager.GameOver ();
		yield return null;
	}
	//Moves Player to Spawn Location
	private IEnumerator MoveToSpawn() {
		Vector3 spawn = latestSpawn.transform.position;
		//For Level Changing
		playerMovingDirection = new Vector3(spawn.x,transform.position.y,spawn.z) - transform.position;

		while (Vector3.Distance(gameObject.transform.position,spawn) > 0.1f) {
			while (GameManager.IsPaused) { //for game pause
				yield return null;
			}
			gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, spawn, Time.deltaTime * 3);
			yield return null;
		}
		while (GameManager.IsPaused) { //for game pause
			yield return null;
		}
		gameObject.transform.position = spawn;
		disablePlayerCollisions = false;
        GameManager.MoveToSpawnState = false;
        yield return null;
	}

    //Returns all Wall collisions in order
    private void UpdateWallCollisions(Vector3 pos,Vector3 dir,float dist,Partition currentPartition,int iteration) {

        List<Vector3> collisionPoints = new List<Vector3>();
        
        Vector3 originalPosition = pos;
        Vector3 originalDirection = dir;
        float remainingDistance = dist;
        float partDistance;

        Vector3 finalPosition = new Vector3(pos.x + ((dir.x - pos.x) * dist),pos.y,pos.z + ((dir.z - pos.z) * dist));

        Vector3 nonTranslatedPoint;
        Vector3 translatedPoint;
        Vector3 newDirection;

        //y = (rise/run)x + c
        //ax + by + c = 0
        float rise = originalDirection.z - originalPosition.z;
        float run = originalDirection.x - originalPosition.x;
        float c = originalPosition.z - (System.Math.Abs(run) < Mathf.Epsilon ? 0 : ((rise / run) * originalPosition.x));
        float slope = System.Math.Abs(run) < Mathf.Epsilon ? 0 : rise / run;

        int x, z, i, j;
        float xmin, xmax, zmin, zmax;
        
        if (rise > 0)
        {
            if (run > 0) {
                x = currentPartition.Width;
                z = currentPartition.Depth;
                i = Mathf.RoundToInt(currentPartition.Width / 2.0f) + Mathf.RoundToInt(currentPartition.Origin.x);
                j = Mathf.RoundToInt(currentPartition.Depth / 2.0f) + Mathf.RoundToInt(currentPartition.Origin.z);
                xmin = originalPosition.x;
                xmax = finalPosition.x;
                zmin = originalPosition.z;
                zmax = finalPosition.z;
            }
            else {
                x = -currentPartition.Width;
                z = currentPartition.Depth;
                i = -Mathf.RoundToInt(currentPartition.Width / 2.0f) + Mathf.RoundToInt(currentPartition.Origin.x);
                j = Mathf.RoundToInt(currentPartition.Depth / 2.0f) + Mathf.RoundToInt(currentPartition.Origin.z);
                xmin = finalPosition.x;
                xmax = originalPosition.x;
                zmin = originalPosition.z;
                zmax = finalPosition.z;
            }
        }
        else
        {
            if (run > 0) {
                x = currentPartition.Width;
                z = -currentPartition.Depth;
                i = Mathf.RoundToInt(currentPartition.Width / 2.0f) + Mathf.RoundToInt(currentPartition.Origin.x);
                j = -Mathf.RoundToInt(currentPartition.Depth / 2.0f) + Mathf.RoundToInt(currentPartition.Origin.z);
                xmin = originalPosition.x;
                xmax = finalPosition.x;
                zmin = finalPosition.z;
                zmax = originalPosition.z;
            }
            else {
                x = -currentPartition.Width;
                z = -currentPartition.Depth;
                i = -Mathf.RoundToInt(currentPartition.Width / 2.0f) + Mathf.RoundToInt(currentPartition.Origin.x);
                j = -Mathf.RoundToInt(currentPartition.Depth / 2.0f) + Mathf.RoundToInt(currentPartition.Origin.z);
                xmin = finalPosition.x;
                xmax = originalPosition.x;
                zmin = finalPosition.z;
                zmax = originalPosition.z;
            }
        }
        
        if (System.Math.Abs(rise) > Mathf.Epsilon && System.Math.Abs(run) > Mathf.Epsilon)//flat slopes
        {   
            for (int k = i; xmin <= k && k <= xmax; k += x)
            {
                collisionPoints.Add(new Vector3(k, originalPosition.y, slope * k + c));
            }
            for (int k = j; zmin <= k && k <= zmax; k += z)
            {
                collisionPoints.Add(new Vector3((k - c) / slope, originalPosition.y, k));
            }
        }
        //orders list of positions by distance from player
        collisionPoints.Sort((v1, v2) => (v1 - originalPosition).sqrMagnitude.CompareTo((v2 - originalPosition).sqrMagnitude));

        for (int l = 0; l < collisionPoints.Count; l++)
        {
            nonTranslatedPoint = collisionPoints[l];
            translatedPoint = TranslateCollision(collisionPoints[l], currentPartition);
            CollisionPath.AddCollision(translatedPoint, currentPartition);
            partDistance = remainingDistance - Mathf.Abs(Vector3.Distance(nonTranslatedPoint, originalPosition));//expensive

            if (partDistance > 0)
            {
                if (currentPartition.GetConnection(translatedPoint, out Partition enterPartition))
                {
                    if (CollisionPath.Collisions.Count > 1)
                    {
                        newDirection = Vector3.Normalize(translatedPoint - CollisionPath.Collisions[CollisionPath.Collisions.Count - 2]);
                    }
                    else
                    {
                        newDirection = Vector3.Normalize(translatedPoint - originalPosition);
                    }

                    newDirection = new Vector3(translatedPoint.x + newDirection.x,
                        translatedPoint.y,
                        translatedPoint.z + newDirection.z);

                    //if (originalPosition != translatedPoint)//hax to fix corner collision
                    //{
                    CollisionPath.UpdateCollisionPartition(CollisionPath.Collisions.Count - 1, enterPartition);
                    UpdateWallCollisions(
                            translatedPoint,
                            newDirection,
                            partDistance,
                            enterPartition,
                            iteration + 1);
                    break;
                    //}
                }
            }
        }
        if (!CollisionPath.FinalDestinationFound)
        {
            CollisionPath.AddCollision(TranslateCollision(finalPosition, currentPartition), currentPartition);
            CollisionPath.FinalDestinationFound = true;
        }
        if (iteration == 0)//end of recursive function
        {
            UpdateGuidePath();
            UpdatingWallCollisions = false;
        }
    }
    //Translates Collision point inside a Partition
    private Vector3 TranslateCollision(Vector3 collision, Partition p)
    {
        float xLength, zLength;
        float xResidual, zResidual;
        float xNew, zNew;
        int xPartWidths, zPartDepths;

        int xPartEdgeRight = Mathf.RoundToInt(p.Origin.x + (p.Width / 2.0f));
        int xPartEdgeLeft = Mathf.RoundToInt(p.Origin.x - (p.Width / 2.0f));
        int zPartEdgeBack = Mathf.RoundToInt(p.Origin.z + (p.Depth / 2.0f));
        int zPartEdgeFront = Mathf.RoundToInt(p.Origin.z - (p.Depth / 2.0f));

        if (collision.x > p.Origin.x)
        {
            if (xPartEdgeRight > collision.x)
            {
                xNew = collision.x;
            }
            else
            {
                xLength = collision.x - xPartEdgeRight;
                xPartWidths = Mathf.RoundToInt(Mathf.FloorToInt(xLength) / p.Width);
                xResidual = xLength - (xPartWidths * p.Width);

                if (xPartWidths % 2 == 0)
                {
                    xNew = xPartEdgeRight - xResidual;
                }
                else
                {
                    xNew = xPartEdgeLeft + xResidual;
                }
            }
        }
        else
        {
            if (xPartEdgeLeft < collision.x)
            {
                xNew = collision.x;
            }
            else
            {
                xLength = xPartEdgeLeft - collision.x;
                xPartWidths = Mathf.RoundToInt(Mathf.FloorToInt(xLength) / p.Width);
                xResidual = xLength - (xPartWidths * p.Width);

                if (xPartWidths % 2 == 0)
                {
                    xNew = xPartEdgeLeft + xResidual;
                }
                else
                {
                    xNew = xPartEdgeRight - xResidual;
                }
            }
        }
        if (collision.z > p.Origin.z)
        {
            if (zPartEdgeBack > collision.z)
            {
                zNew = collision.z;
            }
            else
            {
                zLength = collision.z - zPartEdgeBack;
                zPartDepths = Mathf.RoundToInt(Mathf.FloorToInt(zLength) / p.Depth);
                zResidual = zLength - (zPartDepths * p.Depth);

                if (zPartDepths % 2 == 0)
                {
                    zNew = zPartEdgeBack - zResidual;
                }
                else
                {
                    zNew = zPartEdgeFront + zResidual;
                }
            }
        }
        else
        {
            if (zPartEdgeFront < collision.z)
            {
                zNew = collision.z;
            }
            else
            {
                zLength = zPartEdgeFront - collision.z;
                zPartDepths = Mathf.RoundToInt(Mathf.FloorToInt(zLength) / p.Depth);
                zResidual = zLength - (zPartDepths * p.Depth);

                if (zPartDepths % 2 == 0)
                {
                    zNew = zPartEdgeFront + zResidual;
                }
                else
                {
                    zNew = zPartEdgeBack - zResidual;
                }
            }
        }
        return new Vector3(xNew, collision.y, zNew);
    }
    //Plots path following arrow
    private void UpdateGuidePath() {
        if (CollisionPath.Collisions.Count > 1) {
            GameManager.PathLine().positionCount = CollisionPath.Collisions.Count + 1;
            GameManager.PathLine().SetPosition(0, transform.position);
            for (int i = 1; i < GameManager.PathLine().positionCount; i++) {
                GameManager.PathLine().SetPosition(i, CollisionPath.Collisions[i-1]);
            }
        }
    }
    //Plots chosen
    private void UpdateChosenPath() {
        if (PathPoints.Count > 1) {
            GameManager.PathChosenLine().positionCount = PathPoints.Count + 1;
            GameManager.PathChosenLine().SetPosition(0, transform.position);
            for (int i = 1; i < GameManager.PathChosenLine().positionCount; i++) {
                GameManager.PathChosenLine().SetPosition(i, PathPoints[i-1]);
            }
        }
    }
}
//Class which holds Wall Collisions
public class CollisionPath
{
    //Contains list of collision points
    public static List<Vector3> Collisions = new List<Vector3>();
    //Contains Partition of collision point
    public static List<Partition> Partitions = new List<Partition>();
    //Final position of Character Path
    public static Vector3 FinalDestination = Vector3.zero;
    //Final position of Character Path found
    public static bool FinalDestinationFound = false;

    public static void AddCollision(Vector3 loc, Partition p)
    {
        Collisions.Add(loc);
        Partitions.Add(p);
    }

    public static void ClearCollisions()
    {
        Collisions.Clear();
        Partitions.Clear();
        FinalDestinationFound = false;
    }

    public static void UpdateCollisionPartition(int index, Partition p)
    {
        Partitions.Insert(index, p);
    }
}