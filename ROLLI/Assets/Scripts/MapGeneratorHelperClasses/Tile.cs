using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public Tile(bool upDirection, bool downDirection, bool leftDirection, bool rightDirection, Vector2Int coordinates, Path p, Room r)
    {
        Directions = TileDirectionsToString(upDirection, downDirection, leftDirection, rightDirection);
        Coordinates = coordinates;
        MapGrid.AddTileToMapGrid(this,coordinates.x,coordinates.y);
        Path = p;
        Room = r;
    }

    public Tile(int direction, Vector2Int coordinates, Path p, Room r)
    {
        switch (direction)
        {
            case 0:
                Directions = TileDirectionsToString(true, false, false, false);
                break;
            case 1:
                Directions = TileDirectionsToString(false, true, false, false);
                break;
            case 2:
                Directions = TileDirectionsToString(false, false, true, false);
                break;
            default:
                Directions = TileDirectionsToString(false, false, false, true);
                break;
        }

        Coordinates = coordinates;
        MapGrid.AddTileToMapGrid(this, coordinates.x,coordinates.y);
        Path = p;
        Room = r;
    }

    public string Directions { get; set; }
    public Vector2Int Coordinates { get; }
    public Path Path { get; }
    public Room Room { get; }
    public Door Door { get; set; }

    public static string TileDirectionsToString(bool up, bool down, bool left, bool right)
    {
        char upDirection = '0';
        char downDirection = '0';
        char leftDirection = '0';
        char rightDirection = '0';

        if (up) { upDirection = '1'; }
        if (down) { downDirection = '1'; }
        if (left) { leftDirection = '1'; }
        if (right) { rightDirection = '1'; }

        char[] chars = { upDirection, downDirection, leftDirection, rightDirection };
        return new string(chars);
    }
    public string TileParentType()
    {
        if (Path != null)
        {
            return "PATH";
        }
        return "ROOM";
    }

    public string TileName()
    {
        string tileParentType;

        if (Path != null)
        {
            tileParentType = "PATH";
        }
        else
        {
            tileParentType = "ROOM";
        }
        return string.Concat(tileParentType, "_", Directions);
    }

    public void AddDirection(int direction)
    {
        switch (direction)
        {
            case 0:
                Directions = string.Concat('1', Directions[1], Directions[2], Directions[3]);
                break;
            case 1:
                Directions = string.Concat(Directions[0], '1', Directions[2], Directions[3]);
                break;
            case 2:
                Directions = string.Concat(Directions[0], Directions[1], '1', Directions[3]);
                break;
            default:
                Directions = string.Concat(Directions[0], Directions[1], Directions[2], '1');
                break;
        }
    }

    public void RemoveDirection(int direction)
    {
        switch (direction)
        {
            case 0:
                Directions = string.Concat('0', Directions[1], Directions[2], Directions[3]);
                break;
            case 1:
                Directions = string.Concat(Directions[0], '0', Directions[2], Directions[3]);
                break;
            case 2:
                Directions = string.Concat(Directions[0], Directions[1], '0', Directions[3]);
                break;
            default:
                Directions = string.Concat(Directions[0], Directions[1], Directions[2], '0');
                break;
        }
    }

    public bool HasUpDirection() { return Directions[0] == '1'; }
    public bool HasDownDirection() { return Directions[1] == '1'; }
    public bool HasLeftDirection() { return Directions[2] == '1'; }
    public bool HasRightDirection() { return Directions[3] == '1'; }

    public int HasDirectionCount()
    {
        int i = 0;
        if (HasUpDirection()) { i++; }
        if (HasDownDirection()) { i++; }
        if (HasLeftDirection()) { i++; }
        if (HasRightDirection()) { i++; }
        return i;
    }

    public static int RandomPathBuildDirection(Tile t)
    {
        List<int> directions = new List<int>();

        if (MapGrid.InGridTileBounds(t.Coordinates.x, t.Coordinates.y + 1))
        {
            if (MapGrid.GetGridTile(t.Coordinates.x, t.Coordinates.y + 1) == null)
            {
                directions.Add(0);
            }
        }
    
        if (MapGrid.InGridTileBounds(t.Coordinates.x, t.Coordinates.y - 1))
        {
            if (MapGrid.GetGridTile(t.Coordinates.x, t.Coordinates.y - 1) == null)
            {
                directions.Add(1);
            }
        }
   
        if (MapGrid.InGridTileBounds(t.Coordinates.x - 1, t.Coordinates.y))
        {
            if (MapGrid.GetGridTile(t.Coordinates.x - 1, t.Coordinates.y) == null)
            {
                directions.Add(2);
            }
        }
   
        if (MapGrid.InGridTileBounds(t.Coordinates.x + 1, t.Coordinates.y))
        {
            if (MapGrid.GetGridTile(t.Coordinates.x + 1, t.Coordinates.y) == null)
            {
                directions.Add(3);
            }
        }

        if (directions.Count > 0)
        {
            return directions[Mathf.RoundToInt(Seed.Random(0, directions.Count - 1))];
        }
        else
        {
            return -1;
        }
    }

    public List<Tile> AdjacentTiles(string tType)
    {
        List<Tile> tileList = new List<Tile>();

        if (MapGrid.InGridTileBounds(Coordinates.x, Coordinates.y + 1))
        {
            if (MapGrid.GetGridTile(Coordinates.x, Coordinates.y + 1).TileParentType() == tType)
            {
                tileList.Add(MapGrid.GetGridTile(Coordinates.x, Coordinates.y + 1));
            }
        }

        if (MapGrid.InGridTileBounds(Coordinates.x, Coordinates.y - 1))
        {
            if (MapGrid.GetGridTile(Coordinates.x, Coordinates.y - 1).TileParentType() == tType)
            {
                tileList.Add(MapGrid.GetGridTile(Coordinates.x, Coordinates.y - 1));
            }
        }

        if (MapGrid.InGridTileBounds(Coordinates.x - 1, Coordinates.y))
        {
            if (MapGrid.GetGridTile(Coordinates.x - 1, Coordinates.y).TileParentType() == tType)
            {
                tileList.Add(MapGrid.GetGridTile(Coordinates.x - 1, Coordinates.y));
            }
        }

        if (MapGrid.InGridTileBounds(Coordinates.x + 1, Coordinates.y))
        {
            if (MapGrid.GetGridTile(Coordinates.x + 1, Coordinates.y).TileParentType() == tType)
            {
                tileList.Add(MapGrid.GetGridTile(Coordinates.x + 1, Coordinates.y));
            }
        }

        return tileList;
    }

    public List<Tile> CollapseWithNeighbors()
    {
        List<Tile> neighbors = new List<Tile>();

        if (MapGrid.InGridTileBounds(Coordinates.x, Coordinates.y + 1))
        {
            Tile t = MapGrid.GetGridTile(Coordinates.x, Coordinates.y + 1);
            if (t != null)
            {
                if (t.HasDownDirection())
                {
                    t.RemoveDirection(1);
                    neighbors.Add(t);
                }
            }
        }

        if (MapGrid.InGridTileBounds(Coordinates.x, Coordinates.y - 1))
        {
            Tile t = MapGrid.GetGridTile(Coordinates.x, Coordinates.y - 1);
            if (t != null)
            {
                if (t.HasUpDirection())
                {
                    t.RemoveDirection(0);
                    neighbors.Add(t);
                }
            }
        }

        if (MapGrid.InGridTileBounds(Coordinates.x - 1, Coordinates.y))
        {
            Tile t = MapGrid.GetGridTile(Coordinates.x - 1, Coordinates.y);
            if (t != null)
            {
                if (t.HasRightDirection())
                {
                    t.RemoveDirection(3);
                    neighbors.Add(t);
                }
            }
        }

        if (MapGrid.InGridTileBounds(Coordinates.x + 1, Coordinates.y))
        {
            Tile t = MapGrid.GetGridTile(Coordinates.x + 1, Coordinates.y);
            if (t != null)
            {
                if (t.HasLeftDirection())
                {
                    t.RemoveDirection(2);
                    neighbors.Add(t);
                }
            }
        }

        MapGrid.RemoveGridTile(Coordinates.x, Coordinates.y);
        return neighbors;
    }
}