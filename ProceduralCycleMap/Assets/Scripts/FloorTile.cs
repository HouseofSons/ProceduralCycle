using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTile
{
	public static List<FloorTile> FloorTiles = new List<FloorTile>();

	public Vector3Int GameGridLocation { get; private set; }
    public bool[,,] FloorGrid { get; private set; }

    public FloorTile(Vector3Int location)
    {
		GameGridLocation = location;
		FloorGrid = new bool[16, 16, 16];
		FloorTiles.Add(this);
	}

    public bool FloorSpace(int xMin, int xMax, int yMin, int yMax, int zMin, int zMax)
	{
        for (int i = xMin - 1; i < xMax; i++)
		{
			for (int j = yMin - 1; j < yMax; j++)
			{
                for (int k = zMin - 1; k < zMax; k++)
                {
                    if (FloorGrid[i, j, k])
                    {
                        return false;
                    }
                }
			}
		}
		return true;
	}

    public void AllocateFloorSpace(int xMin, int xMax, int yMin, int yMax, int zMin, int zMax)
    {
        for (int i = xMin - 1; i < xMax; i++)
        {
            for (int j = yMin - 1; j < yMax; j++)
            {
                for (int k = zMin - 1; k < zMax; k++)
                {
                    FloorGrid[i, j, k] = true;
                }
            }
        }
    }
}
