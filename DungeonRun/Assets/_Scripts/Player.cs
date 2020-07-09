//Player is initiaized by the Game Manager
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Player : MonoBehaviour
{
    //Player Level
    public static int Level { get; set; }
    //Player Experience
    public static int ExperiencePoints { get; set; }

    //used to determine speed of Player
    public static float Speed { get; set; }
    //Maximum path distance Player can travel
    public static float PlayerPathDistanceMax { get; set; }
    //Travelled path distance of Player
    public static float PlayerPathDistance { get; set; }
    //Players experience points
    public static int TotalExperiencePoints { get; set; }
    //Max number of chosen positions allowed
    public static int PlayerManualPositionSize { get; set; }

    //Most recent spawn location of Player
    public static Spawn LatestSpawn { get; set; }
    //Room currently occupied by Player
    public static Room CurrentRoom { get; set; }

    //Returns GameObject of Player
    public static Transform PlayerTransform { get; private set; }
    //Destination Player is heading
    public static Vector3 PlayerDestination { get; private set; }
    //Disables Collisions when traveling between levels
    public static bool DisablePlayerCollisions { get; private set; }
    //Wall points calculated by Players chosen path
    public static List<Vector3> PathPoints { get; private set; }
    //CrossHairs List
    public static List<GameObject> CrossHairs { get; private set; }

    //Coroutine of Player following Path
    public static Coroutine PlayerFollowPathCoRoutine { get; private set; }
    //Coroutine which moves player to spawn
    public static Coroutine MoveToSpawnCoRoutine { get; private set; }

    void Awake ()
    {
        Level = 0;
        ExperiencePoints = 0;

        Speed = GameManager.SpeedMin;
        PlayerPathDistanceMax = GameManager.EnergyDefault;
		PlayerPathDistance = 0;
		TotalExperiencePoints = 0;
        PlayerManualPositionSize = GameManager.ChoiceCount;

        LatestSpawn = GameManager.CurrentLevel.transform.Find("InitialSpawn").GetComponent<Spawn>();
        CurrentRoom = LatestSpawn.GetRoom();

        PlayerTransform = this.transform;
        PlayerDestination = this.transform.position;
        DisablePlayerCollisions = false;
        PathPoints = new List<Vector3>();
        CrossHairs = new List<GameObject>();

        PlayerFollowPathCoRoutine = null;
        MoveToSpawnCoRoutine = null;
    }

	void Start ()
    {
        this.transform.position = LatestSpawn.transform.position;
        UI.InitializeUIWithPlayerInfo ();
    }

	void Update ()
    {
        if (GameManager.IsPaused)
        { //for game pause
            
        }
        else
        {
            Speed = Mathf.SmoothStep(Speed, GameManager.SpeedMin, Time.deltaTime * 5.0f);

            if (GameManager.MoveToSpawnState)
            {
                Speed = GameManager.SpeedMin;
                if (MoveToSpawnCoRoutine != null)
                {
                    StopCoroutine(MoveToSpawnCoRoutine);
                }
                MoveToSpawnCoRoutine = StartCoroutine(MoveToSpawn());
            }

            if (GameManager.PlayerAimingState)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    InputMouseClick(out bool processClick);
                    if (processClick)
                    {
                        if (CollisionPath.Collisions.Count == 1)
                        {//First Chosen Point
                            Speed = GameManager.SpeedMax;
                            if (PlayerFollowPathCoRoutine != null)
                            {
                                StopCoroutine(PlayerFollowPathCoRoutine);
                            }
                        }
                        if (CollisionPath.Collisions.Count == PlayerManualPositionSize)
                        {//Last Chosen Point
                            if (!CollisionPath.UpdatingWallCollisions)
                            {
                                CollisionPath.UpdatingWallCollisions = true;
                                Vector3 direction;
                                Vector3 position;
                                float distance;
                                Partition partition;
                                if (PlayerManualPositionSize == 1)
                                {//Only One Click
                                    position = this.transform.position;
                                    direction = CollisionPath.Collisions[0];
                                    distance = PlayerPathDistanceMax - PlayerPathDistance;
                                    partition = CurrentRoom.GetPartition(position);
                                    CollisionPath.ClearCollisions();
                                }
                                else
                                {
                                    position = CollisionPath.Collisions[CollisionPath.Collisions.Count - 2];
                                    direction = CollisionPath.Collisions[CollisionPath.Collisions.Count - 1];
                                    distance = PlayerPathDistanceMax - PlayerPathDistance;
                                    partition = CurrentRoom.GetPartition(CollisionPath.Collisions[CollisionPath.Collisions.Count - 1]);
                                }
                                UpdateWallCollisions(
                                    position,
                                    direction,
                                    distance,
                                    partition,
                                    0);
                                GameManager.PlayerMovingState = true;
                                PathPoints = new List<Vector3>(CollisionPath.Collisions); //needed for separate list
                                CollisionPath.ClearCollisions();
                                if (PlayerFollowPathCoRoutine != null)
                                {
                                    StopCoroutine(PlayerFollowPathCoRoutine);
                                }
                                PlayerFollowPathCoRoutine = StartCoroutine(PlayerFollowPath());
                            }
                        }
                    }
                }
            }
        }
	}

	void OnTriggerEnter(Collider col)
    {
		if(!DisablePlayerCollisions)
        {
            if (col.gameObject.name.StartsWith("Door"))
            {
                DisablePlayerCollisions = true;
                StopCoroutine(PlayerFollowPathCoRoutine);
                Door door = col.transform.GetComponent<Door>();
                LatestSpawn = door.Destination(this.transform.position);
                CurrentRoom = door.Destination(this.transform.position).GetRoom();
                LevelStatsReset(GameManager.EnergyDefault);
                GameManager.EnterDoor = true;
            }
            if (col.gameObject.name.StartsWith("Enemy"))
            {
                if (col.GetComponent<Enemy>().DeathCoroutine == null)
                {
                    col.GetComponent<Enemy>().DeathCoroutine = StartCoroutine(col.GetComponent<Enemy>().Death());
                }
                if (col.GetComponent<Enemy>().GainExperienceCoroutine == null)
                {
                    UpdateExperiencePoints(col.GetComponent<Enemy>().Experience);
                    col.GetComponent<Enemy>().GainExperienceCoroutine = StartCoroutine(col.GetComponent<Enemy>().GainExperience());
                }
            }
        }
    }

    public void InputMouseClick(out bool validClick)
    {
        Ray ray = GameManager.Camera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        Vector3 location;
        LayerMask layer = ~((1 << LayerMask.NameToLayer("Door")) |
            (1 << LayerMask.NameToLayer("Player")) |
            (1 << LayerMask.NameToLayer("Enemy")));


        if (Physics.Raycast(ray, out RaycastHit hit,100,layer))
        {
            if (hit.transform.parent.GetComponent<Partition>())
            {
                location = Room.TranslateToPlayerView(ray.GetPoint(hit.distance), CurrentRoom, out bool wallHit);
                if (wallHit)
                {
                    validClick = false;
                }
                else
                {
                    CollisionPath.AddCollision(new Vector3(location.x, this.transform.position.y, location.z));
                    validClick = true;

                    if (PlayerManualPositionSize > 1) {
                        if (CollisionPath.Collisions.Count == 1)
                        {
                            foreach (GameObject go in CrossHairs)
                            {
                                GameObject.Destroy(go);
                            }
                            CrossHairs.Clear();
                        }
                        CrossHairs.Add(GameObject.Instantiate(Resources.Load("CrossHair")) as GameObject);
                        CrossHairs[CrossHairs.Count - 1].transform.position =
                            new Vector3(
                                CollisionPath.Collisions[CollisionPath.Collisions.Count - 1].x,
                                PlayerTransform.position.y,
                                CollisionPath.Collisions[CollisionPath.Collisions.Count - 1].z);
                    }
                }
            } else
            {
                validClick = false;
            }
        }
        else
        {
            validClick = false;
        }
    }

    public static void PositionSpriteDirection(bool left)
    {
        PlayerTransform.GetChild(0).GetComponent<SpriteRenderer>().flipX = left;
    }

    public static float GetEnergy()
    {
        if(PlayerPathDistanceMax - PlayerPathDistance > 0.0f)
        {
            return PlayerPathDistanceMax - PlayerPathDistance;
        } else
        {
            return 0.0f;
        }
    }

    public static void UpdateExperiencePoints (int experiencePoints)
    {
		TotalExperiencePoints += experiencePoints;
		UI.UpdateExperienceText (TotalExperiencePoints);
	}
    //Resets path lengths for Player
    public static void LevelStatsReset(float distanceMax)
    {
		PlayerPathDistanceMax = distanceMax;
		PlayerPathDistance = 0;
		UI.UpdateEnergyText(Mathf.FloorToInt(GetEnergy()));
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
            while (GameManager.IsPaused)
            { //for game pause
                yield return null;
            }
            //checks if player has passed nextPosition
            if (Mathf.Abs(Vector3.Distance(transform.position, prevPosition)) >= Mathf.Abs(Vector3.Distance(nextPosition, prevPosition)) - 0.1)
            {
                if (CrossHairs.Count - 1 >= index)
                {
                    GameObject.Destroy(CrossHairs[index]);
                }
                transform.position = nextPosition;
                prevPosition = nextPosition;
                index++;
                if (PathPoints.Count > index)
                {
                    nextPosition = PathPoints[index];
                    //used for Camera tracking
                    PlayerDestination = nextPosition;
                } else
                {   //used for Camera tracking
                    PlayerDestination = transform.position;
                }
            }
            if (PathPoints.Count > index)
            {
                transform.position = Vector3.MoveTowards(transform.position, nextPosition, Speed);
                PlayerPathDistance += Mathf.Abs(Vector3.Distance(transform.position, lastOccupiedPosition));
                lastOccupiedPosition = transform.position;
                UI.UpdateEnergyText(Mathf.FloorToInt(GetEnergy()));
                //used for Sprite
                if (nextPosition.x > this.transform.position.x)
                {
                    PositionSpriteDirection(false);
                }
                else
                {
                    PositionSpriteDirection(true);
                }
            }
            yield return null;
		}
		GameManager.GameOver ();
		yield return null;
	}
	//Moves Player to Spawn Location
	private IEnumerator MoveToSpawn()
    {
		Vector3 spawn = LatestSpawn.transform.position;

		while (Vector3.Distance(gameObject.transform.position,spawn) > 0.1f)
        {
			while (GameManager.IsPaused)
            { //for game pause
				yield return null;
			}
			gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, spawn, Time.deltaTime * 3);
			yield return null;
		}
		while (GameManager.IsPaused)
        { //for game pause
			yield return null;
		}
		gameObject.transform.position = spawn;
		DisablePlayerCollisions = false;
        GameManager.MoveToSpawnState = false;
        yield return null;
	}

    //Returns all Wall collisions in order
    private void UpdateWallCollisions(Vector3 pos,Vector3 dir,float dist,Partition currentPartition,int iteration)
    {
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
            if (run > 0)
            {
                x = currentPartition.Width;
                z = currentPartition.Depth;
                i = Mathf.RoundToInt(currentPartition.Width / 2.0f) + Mathf.RoundToInt(currentPartition.Origin.x);
                j = Mathf.RoundToInt(currentPartition.Depth / 2.0f) + Mathf.RoundToInt(currentPartition.Origin.z);
                xmin = originalPosition.x;
                xmax = finalPosition.x;
                zmin = originalPosition.z;
                zmax = finalPosition.z;
            }
            else
            {
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
            if (run > 0)
            {
                x = currentPartition.Width;
                z = -currentPartition.Depth;
                i = Mathf.RoundToInt(currentPartition.Width / 2.0f) + Mathf.RoundToInt(currentPartition.Origin.x);
                j = -Mathf.RoundToInt(currentPartition.Depth / 2.0f) + Mathf.RoundToInt(currentPartition.Origin.z);
                xmin = originalPosition.x;
                xmax = finalPosition.x;
                zmin = finalPosition.z;
                zmax = originalPosition.z;
            }
            else
            {
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
            translatedPoint = CollisionPath.TranslateCollision(collisionPoints[l], currentPartition);
            CollisionPath.AddCollision(translatedPoint);
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

                    UpdateWallCollisions(
                        translatedPoint,
                        newDirection,
                        partDistance,
                        enterPartition,
                        iteration + 1);
                    break;
                }
            }
        }
        if (!CollisionPath.FinalDestinationFound)
        {
            CollisionPath.AddCollision(CollisionPath.TranslateCollision(finalPosition, currentPartition));
            CollisionPath.FinalDestinationFound = true;
        }
        if (iteration == 0)//end of recursive function
        {
            //UpdateGuidePath();
            CollisionPath.UpdatingWallCollisions = false;
        }
    }
}
//Class which holds Wall Collisions
public class CollisionPath
{
    //Contains list of collision points
    public static List<Vector3> Collisions = new List<Vector3>();
    //Final position of Character Path
    public static Vector3 FinalDestination = Vector3.zero;
    //Final position of Character Path found
    public static bool FinalDestinationFound = false;
    //Identifies if WallCollision method is done
    public static bool UpdatingWallCollisions = false;

    public static void ClearCollisions()
    {
        Collisions.Clear();
        FinalDestinationFound = false;
    }
    public static void AddCollision(Vector3 v)
    {
        Collisions.Add(v);
    }
    //Translates Collision point inside a Partition
    public static Vector3 TranslateCollision(Vector3 collision, Partition p)
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
}