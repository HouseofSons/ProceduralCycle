//MeshData.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshData {
	public List<Vector3> vertices = new List<Vector3>();
	public List<Vector3> colVertices = new List<Vector3>();
	public List<int> triangles = new List<int>();
	public List<int> colTriangles = new List<int>();
	public List<Vector2> uv = new List<Vector2>();
	public MeshData() { }

	public bool useRenderDataForHex;
	
	public void AddVertex(Vector3 vertex)
	{
		colVertices.Add(vertex);
		if (useRenderDataForHex) {
			vertices.Add (vertex);
		}
	}

	public void AddQuadTriangles()
	{
		
		colTriangles.Add (colVertices.Count - 4);
		colTriangles.Add (colVertices.Count - 3);
		colTriangles.Add (colVertices.Count - 2);
		colTriangles.Add (colVertices.Count - 4);
		colTriangles.Add (colVertices.Count - 2);
		colTriangles.Add (colVertices.Count - 1);

		if (useRenderDataForHex) {
			triangles.Add(vertices.Count - 4);
			triangles.Add(vertices.Count - 2);
			triangles.Add(vertices.Count - 3);
			triangles.Add(vertices.Count - 1);
			triangles.Add(vertices.Count - 3);
			triangles.Add(vertices.Count - 2);
		}
	}
	
	public void AddHexTriangles()
	{
		colTriangles.Add (colVertices.Count - 3);
		colTriangles.Add (colVertices.Count - 2);
		colTriangles.Add (colVertices.Count - 1);
		colTriangles.Add (colVertices.Count - 3);
		colTriangles.Add (colVertices.Count - 1);
		colTriangles.Add (colVertices.Count - 6);
		colTriangles.Add (colVertices.Count - 3);
		colTriangles.Add (colVertices.Count - 6);
		colTriangles.Add (colVertices.Count - 4);
		colTriangles.Add (colVertices.Count - 5);
		colTriangles.Add (colVertices.Count - 4);
		colTriangles.Add (colVertices.Count - 6);

		if (useRenderDataForHex) {
			triangles.Add(vertices.Count - 3);
			triangles.Add(vertices.Count - 2);
			triangles.Add(vertices.Count - 1);
			triangles.Add(vertices.Count - 3);
			triangles.Add(vertices.Count - 1);
			triangles.Add(vertices.Count - 6);
			triangles.Add(vertices.Count - 3);
			triangles.Add(vertices.Count - 6);
			triangles.Add(vertices.Count - 4);
			triangles.Add(vertices.Count - 5);
			triangles.Add(vertices.Count - 4);
			triangles.Add(vertices.Count - 6);
		}
	}
}
