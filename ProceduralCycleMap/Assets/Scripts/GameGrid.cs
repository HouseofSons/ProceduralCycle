using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameGrid
{
    private static Room[,,] gameGrid;
    private static List<Room> roomsAddedToGrid;

    public GameGrid()
    {
        gameGrid = new Room[ProceduralMapController.ROOM_SCALE * 16,
            ProceduralMapController.ROOM_SCALE * 16,
            ProceduralMapController.ROOM_SCALE * 16];
        roomsAddedToGrid = new List<Room>();
    }

    public static Room RoomAtGridLocation(Vector3Int v)
    {
        return gameGrid[v.x, v.y, v.z];
    }
    //Randomly Place Room as Close to Origin as Possible
    public static bool AddRoomToGrid(Room r)
    {
        for(int i=0;i<gameGrid.GetLength(0);i++)
        {
            for (int j=0;j<gameGrid.GetLength(1);j++)
            {
                for (int k=0;k<gameGrid.GetLength(2);k++)
                {
                    if(gameGrid[i,j,k] == null)
                    {
                        gameGrid[i,j,k] = r;
                        roomsAddedToGrid.Add(r);
                        return true;
                    }
                }
            }
        }
        Debug.Log("Couldn't place Room: " + r.Order);
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
    //Returns first found empty adjacent Node
    public static Vector3Int EmptyAdjacentNode(Vector3Int v)
    {
        int a,b,c;

        for(int i=0;i<6;i++)
        {
            a = (i == 0 ? -1 : (i == 1 ? 1 : 0));
            b = (i == 2 ? -1 : (i == 3 ? 1 : 0));
            c = (i == 4 ? -1 : (i == 5 ? 1 : 0));

            if (v.x+a < gameGrid.GetLength(0) &&
                v.y+b < gameGrid.GetLength(1) &&
                v.z+c < gameGrid.GetLength(2))
            {
                if(gameGrid[v.x+a,v.y+b,v.z+c] == null)
                {
                    return new Vector3Int(v.x+a,v.y+b,v.z+c);
                }
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
            }
        }
        //If no Neighbor Nodes available try to add anywhere
        Debug.Log("Couldn't find Neighbor Adjacent Node for Room: " + r.Order);
        return AddRoomToGrid(r);
    }
    //Extend Room to be adjacent to Neighbors
    public static bool ExtendRoomToNeighbors(Room r)
    {
        return true;
    }
}
