using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameGrid
{
    private static Room[,,] gameGrid;
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
        gameGrid[v.x, v.y, v.z] = r;
        roomsAddedToGrid.Add(r);
        r.GameGridPosition = v;
        return true; //return false
    }
    //Place Room as Close to Origin as Possible
    public static bool FitRoomToGrid(Room r)
    {
        int a,b,c;

        for (int i=0;i<gameGrid.GetLength(0);i++)
        {
            for (int j=0;j<gameGrid.GetLength(1);j++)
            {
                for (int k=0;k<gameGrid.GetLength(2);k++)
                {
                    //Loops around center of grid to avoid index out of bounds of grid array
                    a = (i % 2 == 0 ? GameGridScale / 2 - 1 - Mathf.FloorToInt(i / 2) : GameGridScale / 2 - 1 + Mathf.FloorToInt(i + 1 / 2));
                    b = (j % 2 == 0 ? GameGridScale / 2 - 1 - Mathf.FloorToInt(j / 2) : GameGridScale / 2 - 1 + Mathf.FloorToInt(j + 1 / 2));
                    c = (k % 2 == 0 ? GameGridScale / 2 - 1 - Mathf.FloorToInt(k / 2) : GameGridScale / 2 - 1 + Mathf.FloorToInt(k + 1 / 2));

                    if (gameGrid[a,b,c] == null)
                    {
                        if(BuildRoomOnGrid(r,new Vector3Int(a,b,c)))
                        {
                            return true;
                        }
                    }
                }
            }
        }
        Debug.Log("Couldn't place Room: " + r.Order + ". No More Room in Grid");
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
                coords.z < gameGrid.GetLength(2))
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
            neighborNodes.AddRange(OccupingNodes(r0));
        }
        //Randomizing Neighbors Nodes for more variation
        Seed.Shuffle(neighborNodes);
        //Try all Adjacent Nodes to Neighbor Nodes
        foreach (Vector3Int v in neighborNodes)
        {
            Vector3Int availableSpot = EmptyAdjacentNode(v);
            if (availableSpot != Vector3Int.zero)
            {
                gameGrid[availableSpot.x,availableSpot.y,availableSpot.z] = r;
                roomsAddedToGrid.Add(r);
                r.GameGridPosition = new Vector3Int(availableSpot.x, availableSpot.y, availableSpot.z);
                return true;
            }
        }
        //If no Neighbor Nodes available try to add anywhere
        Debug.Log("Couldn't find Neighbor Adjacent Node for Room: " + r.Order);
        return FitRoomToGrid(r);
    }
    //Extend Room to be adjacent to Neighbors
    public static bool ExtendRoomToNeighbors(Room r)
    {
        return true;
    }
}
