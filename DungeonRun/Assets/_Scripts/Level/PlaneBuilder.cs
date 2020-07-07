using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTiler
{
    public static GameObject CreateFloor(float planeWidth, float planeHeight, string theme)
    {
        //width and height reflect X and Z of Plane Template
        //Tiles should be 2 x 2 so we divide by 2
        int width = Mathf.FloorToInt(planeWidth / 2);
        int height = Mathf.FloorToInt(planeHeight / 2);

        GameObject floor = new GameObject();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject go = CreateTile(theme);
                go.transform.parent = floor.transform;
                go.transform.localPosition = new Vector3(i * 2, 0, j * 2);
                go.GetComponent<MeshRenderer>().material = GameObject.Instantiate(Resources.Load("FloorSpriteSheetMaterial")) as Material;
                go.GetComponent<MeshRenderer>().material.SetFloat("_Glossiness", 0.0f);
            }
        }
        return floor;
    }

    public static GameObject CreateTile(string theme)
    {
        GameObject go = new GameObject("FloorTile");
        MeshFilter mf = go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
        Mesh m = new Mesh();

        Vector3[] verts = new Vector3[4]
        {
            new Vector3(0,0,0),
            new Vector3(0,0,2),
            new Vector3(2,0,2),
            new Vector3(2,0,0)
        };

        m.vertices = verts;

        int[] tris = new int[6] {0,1,3,1,2,3};

        m.triangles = tris;

        Vector2[] uvs = SpriteTiles.RandomTileUVs(theme);
        m.uv = uvs;

        mf.mesh = m;    
        mf.mesh.RecalculateBounds();
        mf.mesh.RecalculateNormals();

        return go;
    }
}