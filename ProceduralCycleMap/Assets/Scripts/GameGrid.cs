using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameGrid
{
    public static Room[,,] gameGrid;
    private static List<Room> roomsAddedToGrid;
    public const int GameGridScale = 64;

    public GameGrid()
    {
        gameGrid = new Room[GameGridScale,GameGridScale,GameGridScale];
        roomsAddedToGrid = new List<Room>();
    }

    public static Room RoomAtGridLocation(Vector3Int v)
    {
        return gameGrid[v.x, v.y, v.z];
    }
    //Tries to Fit room on Grid
    public static bool BuildRoomOnGrid(Room r, Vector3Int v)
    {
        //Establish Size of Room based on Room Neighbors

        //1-2   Neighbors = room size 1,2,4
        //3-4   Neighbors = room size 4,6
        //5-6   Neighbors = room hall size 1 x 4 to 6
        //7-12  Neighbors = room size 9

        int width;
        int length;

        if(r.GetNeighbors().Count <= 2)
        {
            width = Seed.Random(1, 2);
            length = Seed.Random(1, 2);
        } else if(r.GetNeighbors().Count <= 4)
        {
            width = Seed.Random(2, 3);
            if(width == 2)
            {
                length = Seed.Random(2, 3);
            } else
            {
                length = 2;
            }
        } else if(r.GetNeighbors().Count <= 6)
        {
            width = Seed.Random(1, 6);
            if (width <= 3)
            {
                width = 1;
                length = Seed.Random(4, 6);
            }
            else
            {
                length = 1;
            }
        } else
        {
            width = 3;
            length = 3;
        }

        //Look for available size on grid to fit room at location v
        List<Vector3Int> roomPartitions;

        for (int swap = 0; swap < 2; swap++)
        {
            roomPartitions = SpaceAvailableOnGrid(v, width, length);
            //if space available on gamegrid set locations to r, add r to roomAddedToGrid List, v to r.GameGridPosition
            if (roomPartitions.Count > 0)
            {
                foreach (Vector3Int rp in roomPartitions)
                {
                    gameGrid[rp.x, rp.y, rp.z] = r;
                }
                roomsAddedToGrid.Add(r);
                r.GameGridPosition = roomPartitions;
                return true;
            }
            int temp = width;
            width = length;
            length = temp;
        }
        return false;
    }
    //Checks if Space Available on Game Grid
    public static List<Vector3Int> SpaceAvailableOnGrid(Vector3Int v,int width,int length)
    {
        Vector3Int origin;
        List<Vector3Int> roomSiteCoords;
        bool roomAvailable;

        for (int x = 0; x > -width; x--)
        {
            for (int z = 0; z > -length; z--)
            {
                origin = new Vector3Int(v.x + x, v.y, v.z + z);
                roomSiteCoords = new List<Vector3Int>();
                roomAvailable = true;

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < length; j++)
                    {
                        if (origin.x + i >= 0 && origin.x + i < GameGridScale &&
                            origin.z + j >= 0 && origin.z + j < GameGridScale)
                        {
                            if (gameGrid[origin.x + i, origin.y, origin.z + j] == null)
                            {
                                roomSiteCoords.Add(new Vector3Int(origin.x + i, origin.y, origin.z + j));
                            } else
                            {
                                roomAvailable = false;
                            }
                        }
                    }
                }
                if(roomAvailable)
                {
                    return roomSiteCoords;
                }
            }
        }
        return new List<Vector3Int>();
    }
    //Place Room as Close to Target as Possible
    public static bool FitRoomToGrid(Room r, Vector3Int t)
    {
        List<Vector3Int> temp = new List<Vector3Int>();
        List<Vector3Int> final = new List<Vector3Int>();
        Vector3Int target = t == Vector3Int.zero ? new Vector3Int(GameGridScale / 2 - 1, GameGridScale / 2 - 1, GameGridScale / 2 - 1) : t;

        int targetBound = Mathf.Min(GameGridScale - 1 - Mathf.Max(target.x, target.y, target.z),
            Mathf.Min(target.x, target.y, target.z));

        for (int k = 0; k < 2; k++)
        {
            for (int layer = 0; layer <= targetBound; layer++)
            {
                int level = 0;

                while (Mathf.Abs(level) <= layer)
                {
                    for (int i = -layer; i <= layer; i++)
                    {
                        for (int j = -layer; j <= layer; j++)
                        {
                            temp.Add(new Vector3Int(target.x + i, target.y + level, target.z + j));
                        }
                    }
                    Seed.Shuffle(temp);
                    final.AddRange(temp);
                    temp = new List<Vector3Int>();
                    level = level > 0 ? level * -1 : level * -1 + 1;
                }
                foreach (Vector3Int v in final)
                {
                    if (gameGrid[v.x, v.y, v.z] == null)
                    {
                        if (BuildRoomOnGrid(r, v))
                        {
                            return true;
                        }
                    }
                }
                final = new List<Vector3Int>();
            }
            targetBound = GameGridScale / 2 - 1;
        }
        Debug.Log("ERROR!!!! Couldn't place Room: " + r.Order + ". No More Room on Grid");
        return false;
    }
    //Find all Locations on Grid of Room
    public static List<Vector3Int> OccupingNodes(Room r)
    {
        List<Vector3Int> occupiedNodes = new List<Vector3Int>();

        for (int i = 0; i < gameGrid.GetLength(0); i++)
        {
            for (int j = 0; j < gameGrid.GetLength(1); j++)
            {
                for (int k = 0; k < gameGrid.GetLength(2); k++)
                {
                    if (gameGrid[i, j, k] == r)
                    {
                        occupiedNodes.Add(new Vector3Int(i,j,k));
                    }
                }
            }
        }
        Seed.Shuffle(occupiedNodes);
        return occupiedNodes;
    }
    //Returns an empty Node next to Node v in the GameGrid
    public static Vector3Int EmptyAdjacentNode(Vector3Int v)
    {
        int a,b,c;
        List<int> random = new List<int> { 0, 1, 2, 3 };
        List<int> randomY = new List<int> { 4, 5 };

        Seed.Shuffle(random);
        Seed.Shuffle(randomY);

        random.AddRange(randomY);

        Vector3Int coords;

        foreach(int i in random)
        {
            a = (i == 0 ? -1 : (i == 1 ? 1 : 0));
            b = (i == 4 ? -1 : (i == 5 ? 1 : 0));
            c = (i == 2 ? -1 : (i == 3 ? 1 : 0));

            coords = new Vector3Int(v.x + a, v.y + b, v.z + c);

            if (coords.x < gameGrid.GetLength(0) &&
                coords.y < gameGrid.GetLength(1) &&
                coords.z < gameGrid.GetLength(2) &&
                coords.x >= 0 && coords.y >= 0 && coords.z >= 0)
            {
                if(gameGrid[coords.x, coords.y, coords.z] == null)
                {
                    return new Vector3Int(coords.x, coords.y, coords.z);
                }
            } else
            {
                Debug.Log("Warning, Room at Grid Edge location:" + new Vector3Int(coords.x, coords.y, coords.z));
            }
        }
        return Vector3Int.zero;
    }
    //Add Door Locations to Doors
    public static void AddDoorLocations(Room r, Vector3Int rDoorLocation, Room r0, Vector3Int r0DoorLocation)
    {
        foreach(Door d in r.Doors)  
        {
            if(d.RoomFirst == r && d.RoomSecond == r0)
            {
                d.RoomFirstLocation = rDoorLocation;
                d.RoomSecondLocation = r0DoorLocation;
            }
            if (d.RoomSecond == r && d.RoomFirst == r0)
            {
                d.RoomSecondLocation = rDoorLocation;
                d.RoomFirstLocation = r0DoorLocation;
            }
        }
    }
    //Place Room close to Neighbor
    public static bool AddRoomNextToNeighbor(Room r)
    {
        //Intersection of Room Neighbors and Rooms Placed on Grid
        IEnumerable<Room> placedNeighbors = roomsAddedToGrid.AsQueryable().Intersect(r.GetNeighbors());
        //List of Neighbor Nodes to help place Room
        List<Vector3Int> neighborNodes = new List<Vector3Int>();
        //Identify all Neighbor Nodes
        foreach (Room r0 in placedNeighbors)
        {
            neighborNodes = OccupingNodes(r0);
            foreach (Vector3Int v in neighborNodes)
            {
                Vector3Int availableSpot = EmptyAdjacentNode(v);
                if (availableSpot != Vector3Int.zero)
                {
                    if (BuildRoomOnGrid(r, availableSpot))
                    {
                        AddDoorLocations(r, availableSpot, r0, v);
                        return true;
                    }
                }
            }
        }
        //If no Neighbor Nodes available try to add anywhere
        Debug.Log("Couldn't find Neighbor Adjacent Node for Room: " + r.Order);
        return FitRoomToGrid(r, neighborNodes.Count == 0 ? Vector3Int.zero : neighborNodes[0]);
    }
    //Extend Room to be adjacent to Neighbors
    public static void ExtendRoomToNeighbors(Room r)
    {
        //find all neighbors not touching r
        List<Room> remoteNeighbors = r.GetNeighbors();
        Room temp;
        int a, b, c;
        foreach (Vector3Int v in r.GameGridPosition)
        {
            for (int i = 0; i < 6; i++)
            {
                a = (i == 0 ? -1 : (i == 1 ? 1 : 0));
                b = (i == 2 ? -1 : (i == 3 ? 1 : 0));
                c = (i == 4 ? -1 : (i == 5 ? 1 : 0));

                if (v.x + a >= 0 && v.x + a < GameGridScale &&
                    v.y + b >= 0 && v.y + b < GameGridScale &&
                    v.z + c >= 0 && v.z + c < GameGridScale)
                {

                    temp = gameGrid[v.x + a, v.y + b, v.z + c];

                    if (temp != null && temp != r)
                    {
                        if (remoteNeighbors.Contains(temp))
                        {
                            remoteNeighbors.Remove(temp);
                        }
                    }
                }
            }
        }
        foreach (Room r0 in remoteNeighbors)
        {
            List<Vector3Int> path = FindPath(r, r0);

            if (!BuildPath(r, r0, path))
            {
                Debug.Log("Couldn't Build Brige between Room: " + r.Order + " and Room: " + r0.Order);
            } else
            {
                Vector3Int pathStart = path[path.Count - 1];
                Vector3Int pathEnd = path[0];
                Vector3Int rDoorLocation;
                Vector3Int r0DoorLocation;
                bool startFound = false;

                for (int i = 0; i < 6; i++)
                {
                    a = (i == 0 ? -1 : (i == 1 ? 1 : 0));
                    b = (i == 2 ? -1 : (i == 3 ? 1 : 0));
                    c = (i == 4 ? -1 : (i == 5 ? 1 : 0));

                    if (pathStart.x + a >= 0 && pathStart.x + a < GameGridScale &&
                        pathStart.y + b >= 0 && pathStart.y + b < GameGridScale &&
                        pathStart.z + c >= 0 && pathStart.z + c < GameGridScale && !startFound)
                    {
                        if (gameGrid[pathStart.x + a, pathStart.y + b, pathStart.z + c] == r)
                        {
                            rDoorLocation = new Vector3Int(pathStart.x + a, pathStart.y + b, pathStart.z + c);
                            AddDoorLocations(r, rDoorLocation, r, pathStart);//might not be needed
                            startFound = true;
                        }
                    }

                    if (pathEnd.x + a >= 0 && pathEnd.x + a < GameGridScale &&
                        pathEnd.y + b >= 0 && pathEnd.y + b < GameGridScale &&
                        pathEnd.z + c >= 0 && pathEnd.z + c < GameGridScale)
                    {
                        if (gameGrid[pathEnd.x + a, pathEnd.y + b, pathEnd.z + c] == r0)
                        {
                            r0DoorLocation = new Vector3Int(pathEnd.x + a, pathEnd.y + b, pathEnd.z + c);
                            AddDoorLocations(r0, r0DoorLocation, r, pathEnd);
                            return;
                        }
                    }
                }
            }
        }
    }

    public static bool BuildPath(Room start,Room end,List<Vector3Int> path)
    {
        if (path.Count == 0)
        {
            return false;
        }
        foreach (Vector3Int p in path)
        {
            GameObject pathStep = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pathStep.transform.localScale = new Vector3(
                ProceduralMapController.ROOM_SCALE,
                ProceduralMapController.ROOM_SCALE,
                ProceduralMapController.ROOM_SCALE);
            pathStep.transform.position = new Vector3Int(
                p.x * ProceduralMapController.ROOM_SCALE,
                p.y * ProceduralMapController.ROOM_SCALE,
                p.z * ProceduralMapController.ROOM_SCALE);
            pathStep.transform.GetComponent<Renderer>().material.color = start.GetComponent<Renderer>().material.color;
            pathStep.transform.name = "Path: " + start.Order + " to " + end.Order;
            pathStep.transform.parent = start.transform;
            gameGrid[p.x, p.y, p.z] = start;
        }
        start.GameGridPosition.AddRange(path);
        return true;
    }

    public static List<Vector3Int> FindPath(Room r, Room r0)
    {
        List<Vector3Int> PathStarts = new List<Vector3Int>();
        int a, b, c;

        foreach (Vector3Int v in r.GameGridPosition)
        {
            for (int i = 0; i < 6; i++)
            {
                a = (i == 0 ? -1 : (i == 1 ? 1 : 0));
                b = (i == 2 ? -1 : (i == 3 ? 1 : 0));
                c = (i == 4 ? -1 : (i == 5 ? 1 : 0));

                if (v.x + a >= 0 && v.x + a < GameGridScale &&
                    v.y + b >= 0 && v.y + b < GameGridScale &&
                    v.z + c >= 0 && v.z + c < GameGridScale)
                {
                    if (gameGrid[v.x + a, v.y + b, v.z + c] == null)
                    {
                        PathStarts.Add(new Vector3Int(v.x + a, v.y + b, v.z + c));
                    }
                }
            }
        }

        if(PathStarts.Count == 0)
        {
            return new List<Vector3Int>();
        }

        List<Vector3Int> PathEnds = new List<Vector3Int>();

        foreach (Vector3Int v in r0.GameGridPosition)
        {
            for (int i = 0; i < 6; i++)
            {
                a = (i == 0 ? -1 : (i == 1 ? 1 : 0));
                b = (i == 2 ? -1 : (i == 3 ? 1 : 0));
                c = (i == 4 ? -1 : (i == 5 ? 1 : 0));

                if (v.x + a >= 0 && v.x + a < GameGridScale &&
                    v.y + b >= 0 && v.y + b < GameGridScale &&
                    v.z + c >= 0 && v.z + c < GameGridScale)
                {
                    if (gameGrid[v.x + a, v.y + b, v.z + c] == null)
                    {
                        PathEnds.Add(new Vector3Int(v.x + a, v.y + b, v.z + c));
                    }
                }
            }
        }

        if (PathEnds.Count == 0)
        {
            return new List<Vector3Int>();
        }

        Seed.Shuffle(PathStarts);
        Seed.Shuffle(PathEnds);

        foreach(Vector3Int start in PathStarts)
        {
            foreach (Vector3Int end in PathEnds)
            {
                List<Vector3Int> path = Pathing.AstarPath(start,end, gameGrid);
                if (path.Count > 0)
                {
                    return path;
                }
            }
        }
        return new List<Vector3Int>();
    }
}