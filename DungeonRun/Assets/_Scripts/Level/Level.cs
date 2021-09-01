//This script is attached to a Level Game Object
//On Start up will catalogue references to it's Room and Door Child Objects
//The Level Game Objects can be created manually but can also be procedurally generated
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public static List<Vector3Int> CreateRoomLocations()
    {
        List<Vector3Int> roomLocations = new List<Vector3Int>();

        for (int i = 0; i < GameManager.RoomCount; i++)
        {
            Vector3Int v;
            bool tooClose;

            do
            {
                tooClose = false;
                v = new Vector3Int(Seed.Random(0, 99), 0, Seed.Random(0, 99));

                foreach (Vector3Int w in roomLocations)
                {
                    if (Vector3Int.Distance(v, w) <= 2.0f)
                    {
                        tooClose = true;
                        break;
                    }
                }
            } while (tooClose);

            roomLocations.Add(new Vector3Int(Seed.Random(0, 99), 0, Seed.Random(0, 99)));
        }
        return roomLocations;
    }

    public static void CreateRooms(List<Vector3Int> vList)
    {
        List<GameObject> rooms = new List<GameObject>();

        foreach (Vector3Int v in vList)
        {
            GameObject go = new GameObject();
            go.transform.position = new Vector3(v.x * GameManager.Resolution, 0, v.z * GameManager.Resolution);
            go.AddComponent<Room>();
        }

        Vector3 p = new Vector3(10000, 0, 10000);

        GameObject placeHolder = new GameObject();

        for (int i = 0; i < 100 * GameManager.Resolution; i++)
        {
            for (int j = 0; j < 100 * GameManager.Resolution; j++)
            {
                GridCell g = new GridCell(new Vector3Int(i, 0, j),i,j);

                foreach (GameObject go in rooms)
                {
                    if (Vector3.Distance(g.Location, go.transform.position) < Vector3.Distance(g.Location, p))
                    {
                        p = go.transform.position;
                        placeHolder = go;
                    }
                }
                g.SetRoom(placeHolder);
            }
        }

        foreach (GridCell g in GridCell.GridCells)
        {
            if (g.GetX() - 1 >= 0)
            {
                if (GridCell.GridCells[g.GetX() - 1, g.GetZ()].Room != g.Room)
                {
                    g.AddNeighbor(GridCell.GridCells[g.GetX() - 1, g.GetZ()].Room,0);
                }
            }

            if (g.GetX() + 1 < 100 * GameManager.Resolution)
            {
                if (GridCell.GridCells[g.GetX() + 1, g.GetZ()].Room != g.Room)
                {
                    g.AddNeighbor(GridCell.GridCells[g.GetX() + 1, g.GetZ()].Room,1);
                }
            }

            if (g.GetZ() - 1 >= 0)
            {
                if (GridCell.GridCells[g.GetX(), g.GetZ() - 1].Room != g.Room)
                {
                    g.AddNeighbor(GridCell.GridCells[g.GetX(), g.GetZ() - 1].Room,2);
                }
            }

            if (g.GetZ() + 1 < 100 * GameManager.Resolution)
            {
                if (GridCell.GridCells[g.GetX(), g.GetZ() + 1].Room != g.Room)
                {
                    g.AddNeighbor(GridCell.GridCells[g.GetX(), g.GetZ() + 1].Room,3);
                }
            }
        }
    }
}

public class GridCell
{
    public static GridCell[,] GridCells =
            new GridCell[Mathf.FloorToInt(100 * GameManager.Resolution), Mathf.FloorToInt(100 * GameManager.Resolution)];
    public Vector3Int Location { get; private set; }
    public GameObject Room { get; private set; }
    public GameObject[] Neighbors { get; private set; }

    public GridCell(Vector3Int loc, int x, int z)
    {
        Location = loc;
        GridCells[x, z] = this;
        Neighbors = new GameObject[4];
    }

    public void SetRoom(GameObject r)
    {
        Room = r;
        GameObject go = new GameObject();
        go.transform.position = Location;
        go.transform.parent = r.transform;
        go.AddComponent<Partition>();
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.parent = go.transform;
        plane.transform.localPosition = Vector3.zero;
    }

    public void AddNeighbor(GameObject n, int i)
    {
        Neighbors[i] = n;
    }

    public int GetX ()
    {
        return Location.x;
    }

    public int GetZ()
    {
        return Location.z;
    }
}