using UnityEngine;

public class MapGrid
{
    public MapGrid(int x, int y)
    {
        mapGrid = new Tile[x, y];
    }

    private static Tile[,] mapGrid;
    public static int GridWidth => mapGrid.GetLength(0);
    public static int GridHeight => mapGrid.GetLength(1);

    public static void AddTileToMapGrid(Tile t, int x, int y)
    {
        mapGrid[x, y] = t;
    }

    public static Tile GetGridTile(int x, int y)
    {
        return mapGrid[x, y];
    }

    public static void RemoveGridTile(int x, int y)
    {
        mapGrid[x, y] = null;
    }

    public static bool InGridTileBounds(int xPos, int yPos)
    {
        if (xPos >= 0 && xPos <= mapGrid.GetLength(0) - 1 && yPos >= 0 && yPos <= mapGrid.GetLength(1) - 1)
        {
            return true;
        }
        return false;
    }

    public static string InfoAtGridLocation(int x, int y)
    {
        Tile tile = GetGridTile(x, y);

        int objectNum;

        if (tile.Path != null)
        {
            objectNum = Path.Paths.IndexOf(tile.Path);
        }
        else
        {
            objectNum = Room.Rooms.IndexOf(tile.Room);
        }

        if (objectNum < 10)
        {
            return string.Concat(tile.TileParentType(), "00", objectNum.ToString());
        }
        if (objectNum < 100)
        {
            return string.Concat(tile.TileParentType(), "0", objectNum.ToString());
        }
        return string.Concat(tile.TileParentType(), objectNum.ToString());
    }
}
