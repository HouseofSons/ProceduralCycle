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

        foreach(Partition p in r.Partitions)
        {
            if (p.Nedge > n) { n = p.Nedge; }
            if (p.Nedge > s) { s = p.Sedge; }
            if (p.Nedge > e) { e = p.Eedge; }
            if (p.Nedge > w) { w = p.Wedge; }
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