using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvertMesh : MonoBehaviour
{
    public static void FlipNormals(Mesh mesh)
    {
        Vector3[] newNormals = mesh.normals;
        for(int i=0;i<newNormals.Length;i++)    
        {
            newNormals[i] = -1 * newNormals[i];
        }
        mesh.normals = newNormals;
        for(int i=0;i<mesh.subMeshCount;i++)
        {
            int[] tris = mesh.GetTriangles(i);
            for(int j = 0; j < tris.Length; j+=3)
            {
                int temp = tris[j];
                tris[j] = tris[j + 1];
                tris[j + 1] = temp;
            }
            mesh.SetTriangles(tris, i);
        }
    }
}
