using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTile
{
	public static List<FloorTile> FloorTiles = new List<FloorTile>();

	public Vector3Int GameGridLocation { get; private set; }
    public int[,,] FloorGrid { get; private set; }
    // 0:OPEN 1:HOLE 2:OCCUPIED
    public Room Room { get; private set; }

    public FloorTile(Room r,Vector3Int location)
    {
		GameGridLocation = location;
		FloorGrid = new int[16, 16, 16];
        for(int i = 0;i < FloorGrid.GetLength(0);i++) {
            for (int j = 0; j < FloorGrid.GetLength(1); j++) {
                for (int k = 0; k < FloorGrid.GetLength(2); k++) {
                    FloorGrid[i, j, k] = 0;
                }
            }
        }
		FloorTiles.Add(this);
        Room = r;
	}

    public bool FloorSpace(int xMin, int xMax, int yMin, int yMax, int zMin, int zMax)
	{
        for (int i = xMin - 1; i < xMax; i++)
		{
			for (int j = yMin - 1; j < yMax; j++)
			{
                for (int k = zMin - 1; k < zMax; k++)
                {
                    if (FloorGrid[i, j, k] != 0)
                    {
                        return false;
                    }
                }
			}
		}
		return true;
    }

    public void AllocateFloorHole(int xMin, int xMax, int yMin, int yMax, int zMin, int zMax)
    {
        for (int i = xMin - 1; i < xMax; i++)
        {
            for (int j = yMin - 1; j < yMax; j++)
            {
                for (int k = zMin - 1; k < zMax; k++)
                {
                    FloorGrid[i, j, k] = 1;
                }
            }
        }
    }

    public void AllocateFloorOccupied(int xMin, int xMax, int yMin, int yMax, int zMin, int zMax)
    {
        for (int i = xMin - 1; i < xMax; i++)
        {
            for (int j = yMin - 1; j < yMax; j++)
            {
                for (int k = zMin - 1; k < zMax; k++)
                {
                    FloorGrid[i, j, k] = 2;
                }
            }
        }
    }
}
