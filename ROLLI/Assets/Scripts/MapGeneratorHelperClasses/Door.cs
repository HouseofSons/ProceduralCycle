using System.Collections.Generic;
using UnityEngine;

public class Door
{
    public Door (Vector3Int site, Room room)
    {
        InnerSite = new Vector2Int(site.x,site.y);
        MapGrid.GetGridTile(site.x, site.y).Door = this;
        RoomParent = room;
        OuterSite = FindOuterSiteTile(site.z);
        Doors.Add(this);
        room.Doors.Add(this);
    }

    public static List<Door> Doors = new List<Door>();
    public Vector2Int InnerSite { get; }
    public Vector2Int OuterSite { get; }
    public Room RoomParent { get; }

    private Vector2Int FindOuterSiteTile(int direction)
    {
        if (direction <= 1)
        {
            return new Vector2Int(InnerSite.x, direction == 0 ? InnerSite.y + 1 : InnerSite.y - 1);
        }
        return new Vector2Int(direction == 2 ? InnerSite.x - 1 : InnerSite.x + 1, InnerSite.y);
    }

    public static bool ConnectedToRoom(Vector3Int v)
    {
        Tile t;
        switch (v.z)
        {
            case 0:
                t = MapGrid.GetGridTile(v.x, v.y + 1);
                break;
            case 1:
                t = MapGrid.GetGridTile(v.x, v.y - 1);
                break;
            case 2:
                t = MapGrid.GetGridTile(v.x - 1, v.y);
                break;
            default:
                t = MapGrid.GetGridTile(v.x + 1, v.y);
                break;
        }

        if (t != null)
        {
            if(t.Room != null)
            {
                return true;
            }
        }
        return false;
    }
}
