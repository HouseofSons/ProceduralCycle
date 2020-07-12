using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTiler
{
    public static void CreateWallMesh(Room r)
    {
        int n = 0;
        int s = 0;
        int e = 0;
        int w = 0;
        int x, y;

        foreach (Partition p in r.Partitions)
        {
            if (p.Nedge > n) { n = p.Nedge; }
            if (p.Sedge > s) { s = p.Sedge; }
            if (p.Eedge > e) { e = p.Eedge; }
            if (p.Wedge > w) { w = p.Wedge; }
        }

        x = ((e - w) / 2) + 2;
        y = ((n - s) / 2) + 2;

        Cell[,] grid = new Cell[x, y];

        foreach (Partition p in r.Partitions)
        {
            for (int i = p.Eedge; i < p.Wedge; i++)
            {
                for (int j = p.Sedge; j < p.Nedge; j++)
                {
                    grid[i - e + 2, j - s + 2] = new Cell(i, j, 1);
                }
            }
        }

        bool edge = false;

        for (int i = 1; i < x - 1; i++)
        {
            for (int j = 1; j < y - 1; j++)
            {
                if (grid[i, j].Type != 1)
                {
                    Cell c = new Cell(i, j, 2);

                    if (grid[i - 1, j - 1].Type == 1) { c.SW = true; edge = true; }
                    if (grid[    i, j - 1].Type == 1) { c.S  = true; edge = true; }
                    if (grid[i + 1, j - 1].Type == 1) { c.SE = true; edge = true; }
                    if (grid[i - 1, j    ].Type == 1) { c.W  = true; edge = true; }
                    if (grid[i + 1, j    ].Type == 1) { c.E  = true; edge = true; }
                    if (grid[i - 1, j + 1].Type == 1) { c.NW = true; edge = true; }
                    if (grid[    i, j + 1].Type == 1) { c.N  = true; edge = true; }
                    if (grid[i + 1, j + 1].Type == 1) { c.NE = true; edge = true; }

                    if(edge)
                    {
                        edge = false;
                        grid[i, j] = c;
                    }
                }
            }
        }
    }

    public static GameObject CreateFloorMesh(float planeWidth, float planeHeight, string theme)
    {
        //width and height reflect X and Z of Plane Template
        //Tiles should be 2 x 2 so we divide by 2
        int width = Mathf.FloorToInt(planeWidth / 2);
        int height = Mathf.FloorToInt(planeHeight / 2);

        GameObject go = new GameObject("Floor");
        MeshFilter mf = go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
        go.GetComponent<MeshRenderer>().material =
            GameObject.Instantiate(Resources.Load("FloorSpriteSheetMaterial")) as Material;
        go.GetComponent<MeshRenderer>().material.SetFloat("_Glossiness", 0.0f);

        Mesh m = new Mesh();

        List<Vector3> verts = new List<Vector3>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                verts.Add(new Vector3(2 * i,        0, 2 * j        ));
                verts.Add(new Vector3(2 * i,        0, 2 * (j + 1)  ));
                verts.Add(new Vector3(2 * (i + 1),  0, 2 * (j + 1)  ));
                verts.Add(new Vector3(2 * (i + 1),  0, 2 * j        ));
            }
        }
        
        m.vertices = verts.ToArray();

        List<int> tris = new List<int>();

        for (int i = 0; i < (width * height); i++)
        {
            tris.Add(0 + (4 * i));
            tris.Add(1 + (4 * i));
            tris.Add(3 + (4 * i));
            tris.Add(1 + (4 * i));
            tris.Add(2 + (4 * i));
            tris.Add(3 + (4 * i));
        }

        m.triangles = tris.ToArray();

        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; i < (width * height); i++)
        {
            uvs.AddRange(SpriteTiles.RandomTileUVs(theme));
        }

        m.uv = uvs.ToArray();

        mf.mesh = m;    
        mf.mesh.RecalculateBounds();
        mf.mesh.RecalculateNormals();

        return go;
    }
}

//Class which holds all needed Connection information
public class Cell
{
    public Vector2Int Location { get; private set; }
    public bool SW { get; set; }
    public bool S { get; set; }
    public bool SE { get; set; }
    public bool W { get; set; }
    public bool E { get; set; }
    public bool NW { get; set; }
    public bool N { get; set; }
    public bool NE { get; set; }

    public int Type { get; private set; }

    public Cell(int x, int y, int type)
    {
        Location = new Vector2Int(x, y);
        Type = type;
    }
}