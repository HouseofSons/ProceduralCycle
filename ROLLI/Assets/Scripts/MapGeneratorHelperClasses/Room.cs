using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public Room(int x, int y, int w, int h)
    {
        X = x;
        Y = y;
        Width = w;
        Height = h;
    }

    public int X { get; }
    public int Y { get; }
    public int Width { get; }
    public int Height { get; }

    public static List<Room> Rooms { get; } = new List<Room>();
    public List<Door> Doors { get; } = new List<Door>();

    public bool RoomFitsInMapGrid()
    {
        for (int i = X; i < X + Width; i++)
        {
            for (int j = Y; j < Y + Height; j++)
            {
                if (i >= MapGrid.GridWidth || j >= MapGrid.GridHeight)
                {
                    return false;
                }

                if (MapGrid.GetGridTile(i,j) != null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void CopyRoomToMapGrid()
    {
        bool up = true;
        bool down = true;
        bool left = true;
        bool right = true;

        for (int i = X; i < X + Width; i++)
        {
            left &= i != X;
            right &= i != X + Width - 1;

            for (int j = Y; j < Y + Height; j++)
            {
                down &= j != Y;
                up &= j != Y + Height - 1;

                Tile t = new Tile(up, down, left, right, new Vector2Int(i, j), null, this);

                up = true;
                down = true;
            }

            left = true;
            right = true;
        }
        Rooms.Add(this);
    }

    public List<Vector3Int> DoorOptions()
    {
        List<Vector3Int> options = new List<Vector3Int>();

        for (int i = Mathf.Max(1, X); i < Mathf.Min(MapGrid.GridWidth - 1, X + Width); i++)
        {
            for (int j = Mathf.Max(1, Y); j < Mathf.Min(MapGrid.GridHeight - 1, Y + Height); j++)
            {
                if (i == X && j == Y)
                {
                    options.Add(new Vector3Int(i, j, 1));
                    options.Add(new Vector3Int(i, j, 2));
                }
                else if (i == X + Width - 1 && j == Y)
                {
                    options.Add(new Vector3Int(i, j, 1));
                    options.Add(new Vector3Int(i, j, 3));
                }
                else if (i == X && j == Y + Height - 1)
                {
                    options.Add(new Vector3Int(i, j, 0));
                    options.Add(new Vector3Int(i, j, 2));
                }
                else if (i == X + Width - 1 && j == Y + Height - 1)
                {
                    options.Add(new Vector3Int(i, j, 0));
                    options.Add(new Vector3Int(i, j, 3));
                }
                else
                {
                    if (i == X || i == X + Width - 1 || j == Y || j == Y + Height - 1)
                    {
                        if (i == X)
                        {
                            options.Add(new Vector3Int(i, j, 2));
                        }
                        else if (i == X + Width - 1)
                        {
                            options.Add(new Vector3Int(i, j, 3));
                        }
                        else if (j == Y)
                        {
                            options.Add(new Vector3Int(i, j, 1));
                        }
                        else
                        {
                            options.Add(new Vector3Int(i, j, 0));
                        }
                    }
                }
            }
        }
        return options;
    }
}
