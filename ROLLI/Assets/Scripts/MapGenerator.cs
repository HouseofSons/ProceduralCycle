using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Range(5, 100)]
    public int mapWidth;
    [Range(5, 100)]
    public int mapHeight;

    public int minRoomWidth = 3;
    public int maxRoomWidth = 10;
    public int minRoomHeight = 3;
    public int maxRoomHeight = 10;

    [Range(1, 1000)]
    public int roomAttempts;

    private void Awake()
    {
        Seed seed = new Seed();
        MapGrid mapGrid = new MapGrid(mapWidth, mapHeight);
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateRooms();
        //Why do rooms without doors show up?
        //fix doors ontop of each other
        AddDoorsToRooms();
        GeneratePaths();
        CarveDoorsInPaths();
        RemoveDeadEnds();
        DrawMapGrid(0);
    }

    public void GenerateRooms()
    {
        for (int i = 1; i <= roomAttempts; i++)
        {
            int posX = Mathf.RoundToInt(Seed.Random(0, mapWidth - 1));
            int posY = Mathf.RoundToInt(Seed.Random(0, mapHeight - 1));
            int width = Mathf.RoundToInt(Seed.Random(minRoomWidth, maxRoomWidth));
            int height = Mathf.RoundToInt(Seed.Random(minRoomHeight, maxRoomHeight));

            Room r = new Room(posX, posY, width, height);

            if (r.RoomFitsInMapGrid())
            {
                r.CopyRoomToMapGrid();
            }
        }
    }

    public void AddDoorsToRooms()
    {
        List<Vector3Int> doorOptions;

        int doorNum;

        foreach (Room r in Room.Rooms)
        {
            doorOptions = r.DoorOptions();

            if (doorOptions.Count > 0)
            {
                doorNum = Mathf.RoundToInt(Seed.Random(0, doorOptions.Count - 1));
                Door firstDoorOfRoom = new Door(doorOptions[doorNum], r);
                doorOptions.Remove(doorOptions[doorNum]);

                if (doorOptions.Count > 0)
                {
                    if (Seed.Percent(50))
                    {
                        doorNum = Mathf.RoundToInt(Seed.Random(0, doorOptions.Count - 1));
                        Door secondDoorOfRoom = new Door(doorOptions[doorNum], r);
                    }
                }
            }
            doorOptions.Clear();
        }
    }

    public void GeneratePaths()
    {
        List<Vector2Int> AGC = new List<Vector2Int>();

        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                if (MapGrid.GetGridTile(i, j) == null)
                {
                    AGC.Add(new Vector2Int(i, j));
                }
            }
        }

        Seed.Shuffle(AGC);

        List<Tile> PT = new List<Tile>();

        Tile current_tile = null;

        int pathSingleDirection;

        Path p = new Path();

        bool firstPass = true;

        while (AGC.Count > 0)
        {
            if (current_tile == null)
            {
                if (firstPass)
                {
                    firstPass = false;
                }
                else
                {
                    p = new Path();
                }

                Tile t = new Tile(false, false, false, false, AGC[0], p, null);
                PT.Add(t);
                p.AddPathTile(t);
                current_tile = t;
            }

            while (PT.Count > 0) {

                if (current_tile == null)
                {
                    current_tile = PT[PT.Count - 1];
                }

                pathSingleDirection = Tile.RandomPathBuildDirection(current_tile);

                if (pathSingleDirection >= 0) {

                    current_tile.AddDirection(pathSingleDirection);
                    Tile tnew;
                    switch (pathSingleDirection)
                    {
                        case 0:
                            tnew = new Tile(1, new Vector2Int(current_tile.Coordinates.x, current_tile.Coordinates.y + 1), p, null);
                            break;
                        case 1:
                            tnew = new Tile(0, new Vector2Int(current_tile.Coordinates.x, current_tile.Coordinates.y - 1), p, null);
                            break;
                        case 2:
                            tnew = new Tile(3, new Vector2Int(current_tile.Coordinates.x - 1, current_tile.Coordinates.y), p, null);
                            break;
                        default:
                            tnew = new Tile(2, new Vector2Int(current_tile.Coordinates.x + 1, current_tile.Coordinates.y), p, null);
                            break;
                    }
                        
                    PT.Add(tnew);
                    p.AddPathTile(tnew);
                    current_tile = tnew;

                } else {

                    PT.Remove(current_tile);
                    current_tile = null;
                }
            }

            AGC.Clear();

            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    if (MapGrid.GetGridTile(i, j) == null)
                    {
                        AGC.Add(new Vector2Int(i, j));
                    }
                }
            }
            Seed.Shuffle(AGC);
        }
    }

    public void CarveDoorsInPaths()
    {
        foreach(Door d in Door.Doors)
        {
            if (d.InnerSite.x == d.OuterSite.x)
            {
                if (d.InnerSite.y < d.OuterSite.y)
                {
                    MapGrid.GetGridTile(d.InnerSite.x, d.InnerSite.y).AddDirection(0);
                    MapGrid.GetGridTile(d.OuterSite.x, d.OuterSite.y).AddDirection(1);
                }
                else
                {
                    MapGrid.GetGridTile(d.InnerSite.x, d.InnerSite.y).AddDirection(1);
                    MapGrid.GetGridTile(d.OuterSite.x, d.OuterSite.y).AddDirection(0);
                }
            }
            else
            {
                if (d.InnerSite.x > d.OuterSite.x)
                {
                    MapGrid.GetGridTile(d.InnerSite.x, d.InnerSite.y).AddDirection(2);
                    MapGrid.GetGridTile(d.OuterSite.x, d.OuterSite.y).AddDirection(3);
                }
                else
                {
                    MapGrid.GetGridTile(d.InnerSite.x, d.InnerSite.y).AddDirection(3);
                    MapGrid.GetGridTile(d.OuterSite.x, d.OuterSite.y).AddDirection(2);
                }
            }
        }
    }

    public void RemoveDeadEnds()
    {
        List<Tile> deadEnds = new List<Tile>();
        List<Path> emptyPaths = new List<Path>();

        foreach (Path p in Path.Paths)
        {
            foreach (Tile t in p.PathTiles)
            {
                if (t.HasDirectionCount() == 1)
                {
                    deadEnds.Add(t);
                }
            }

            while (deadEnds.Count > 0)
            {
                int x = deadEnds[0].Coordinates.x;
                int y = deadEnds[0].Coordinates.y;

                List<Tile> tlist = MapGrid.GetGridTile(x, y).CollapseWithNeighbors();

                if (tlist.Count > 0)
                {
                    if (tlist[0].HasDirectionCount() == 1)
                    {
                        deadEnds.Add(tlist[0]);
                    }
                }
                p.PathTiles.Remove(deadEnds[0]);
                deadEnds.Remove(deadEnds[0]);
            }

            if (p.PathTiles.Count == 0)
            {
                emptyPaths.Add(p);
            }
        }

        foreach(Path path in emptyPaths)
        {
            Path.Paths.Remove(path);
        }
    }

    public void DrawMapGrid(int style)
    {
        AddRoomGameObjects();
        AddPathGameObjects();

        for (int i = 0; i < MapGrid.GridWidth; i++)
        {
            for (int j = 0; j < MapGrid.GridHeight; j++)
            {
                if (MapGrid.GetGridTile(i, j) != null)
                {
                    string prefabStringName = string.Concat("MapBlocks\\",MapGrid.GetGridTile(i, j).TileName(), "(", style.ToString(), ")");
                    GameObject tile = Instantiate(Resources.Load(prefabStringName)) as GameObject;
                    tile.transform.position = new Vector3(i * 16, 0, j * 16);

                    for (int k = 0;k<tile.transform.childCount;k++)
                    {
                        GameObject go = tile.transform.GetChild(k).gameObject;
                        if (go.name != "center") {
                            Rigidbody rb = go.AddComponent<Rigidbody>();
                            rb.useGravity = false;
                            rb.isKinematic = true;
                            BoxCollider bc = go.AddComponent<BoxCollider>();
                            bc.isTrigger = true;

                            if (go.name == "ground")
                            {
                                go.layer = 10;
                            }
                            else //wall
                            {
                                go.layer = 9;
                            }
                        }
                    }
                    tile.transform.parent = GameObject.Find(MapGrid.InfoAtGridLocation(i, j)).transform;
                }
            }
        }
    }
    //needed for lack of method string.StartsWith
    public static bool CustomStartsWith(string a, string b)
    {
        int aLen = a.Length;
        int bLen = b.Length;
        int ap = 0; int bp = 0;

        while (ap < aLen && bp < bLen && a[ap] == b[bp])
        {
            ap++;
            bp++;
        }

        return (bp == bLen && aLen >= bLen) ||

                (ap == aLen && bLen >= aLen);
    }

    public static void AddPathGameObjects()
    {
        int count = 0;

        GameObject pathList = new GameObject { name = "PATHS" };

        foreach (Path p in Path.Paths)
        {
            GameObject go = new GameObject();

            if (count < 10)
            {
                go.name = string.Concat("PATH", "00", count.ToString());
                go.transform.parent = pathList.transform;
            }
            else if (count < 100)
            {
                go.name = string.Concat("PATH", "0", count.ToString());
                go.transform.parent = pathList.transform;
            }
            else
            {
                go.name = string.Concat("PATH", count.ToString());
                go.transform.parent = pathList.transform;
            }
            count++;
        }
    }

    public static void AddRoomGameObjects()
    {
        int count = 0;

        GameObject rooms = new GameObject { name = "ROOMS" };

        foreach (Room r in Room.Rooms)
        {
            GameObject go = new GameObject();

            if (count < 10)
            {
                go.name = string.Concat("ROOM", "00", count.ToString());
                go.transform.parent = rooms.transform;
            }
            else if (count < 100)
            {
                go.name = string.Concat("ROOM", "0", count.ToString());
                go.transform.parent = rooms.transform;
            }
            else
            {
                go.name = string.Concat("ROOM", count.ToString());
                go.transform.parent = rooms.transform;
            }
            count++;
        }
    }
}
