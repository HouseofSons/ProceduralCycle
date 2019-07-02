using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTile
{
	public static List<FloorTile> FloorTiles = new List<FloorTile>();

	public Vector3Int GameGridLocation { get; private set; }
    public bool[,] FloorGrid { get; private set; }

    public FloorTile(Vector3Int location)
    {
		GameGridLocation = location;
		FloorGrid = new bool[16, 16];
		FloorTiles.Add(this);
	}

    public bool AllocateFloorSpace(int xMin, int xMax, int yMin, int yMax)
	{
		for (int i = xMin; i <= xMax; i++)
		{
			for (int j = yMin; j <= yMax; j++)
			{
				if (FloorGrid[i, j])
				{
					return false;
				} else
				{
					FloorGrid[i, j] = true;
				}
			}
		}
		return true;
	}
}
