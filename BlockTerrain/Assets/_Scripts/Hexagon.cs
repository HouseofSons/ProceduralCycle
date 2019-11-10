using UnityEngine;
using UnityEditor;
using System.Collections;

public class Hexagon : MonoBehaviour {

	void Awake () {
	
		MeshFilter filter = GetComponent<MeshFilter>();
		Mesh mesh = filter.mesh;
		mesh.Clear();
		
		#region Vertices clockwise top to bottom
		Vector3 p00 = new Vector3(                   0.00f, 0.50f,         1.00f);
		Vector3 p01 = new Vector3( Mathf.Sqrt(3.00f)/2.00f, 0.50f, (1.00f/2.00f));
		Vector3 p02 = new Vector3( Mathf.Sqrt(3.00f)/2.00f, 0.50f,-(1.00f/2.00f));
		Vector3 p03 = new Vector3(                   0.00f, 0.50f,        -1.00f);
		Vector3 p04 = new Vector3(-Mathf.Sqrt(3.00f)/2.00f, 0.50f,-(1.00f/2.00f));
		Vector3 p05 = new Vector3(-Mathf.Sqrt(3.00f)/2.00f, 0.50f, (1.00f/2.00f));
		Vector3 p06 = new Vector3(                   0.00f,-0.50f,         1.00f);
		Vector3 p07 = new Vector3( Mathf.Sqrt(3.00f)/2.00f,-0.50f, (1.00f/2.00f));
		Vector3 p08 = new Vector3( Mathf.Sqrt(3.00f)/2.00f,-0.50f,-(1.00f/2.00f));
		Vector3 p09 = new Vector3(                   0.00f,-0.50f,        -1.00f);
		Vector3 p10 = new Vector3(-Mathf.Sqrt(3.00f)/2.00f,-0.50f,-(1.00f/2.00f));
		Vector3 p11 = new Vector3(-Mathf.Sqrt(3.00f)/2.00f,-0.50f, (1.00f/2.00f));

		Vector3[] vertices = new Vector3[]
		{
			// Top
			p00, p01, p02, p03, p04, p05,
			
			// Bottom
			p06, p07, p08, p09, p10, p11,
			
			// Right
			p01, p02, p07, p08,
			
			// Left
			p04, p05, p10, p11,
			
			// Front Right
			p02, p03, p08, p09,
			
			// Front Left
			p03, p04, p09, p10,
			
			// Back Right
			p00, p01, p06, p07,
			
			// Back Left
			p05, p00, p11, p06
		};
		#endregion
		
		#region Normales
		Vector3 up 			= Vector3.up;
		Vector3 down 		= Vector3.down;
		Vector3 right 		= Vector3.right;
		Vector3 left 		= Vector3.left;
		Vector3 frontRight 	= new Vector3( 0.50f, 0,-((Mathf.Sqrt(3.00f))/2.00f));
		Vector3 frontLeft 	= new Vector3(-0.50f, 0,-((Mathf.Sqrt(3.00f))/2.00f));
		Vector3 backRight	= new Vector3( 0.50f, 0, ((Mathf.Sqrt(3.00f))/2.00f));
		Vector3 backLeft 	= new Vector3(-0.50f, 0, ((Mathf.Sqrt(3.00f))/2.00f));

		Vector3[] normals = new Vector3[]
		{
			// Top
			up, up, up, up, up, up,
			
			// Bottom
			down, down, down, down, down, down,
			
			// Right
			right, right, right, right,
			
			// left
			left, left, left, left,
			
			// Front Right
			frontRight, frontRight, frontRight, frontRight,
			
			// Front Left
			frontLeft, frontLeft, frontLeft, frontLeft,
			
			// Back Right
			backRight, backRight, backRight, backRight,
			
			// Back Left
			backLeft, backLeft, backLeft, backLeft,
		};
		#endregion	
		
		#region UVs
		Vector2 _00 = new Vector2(0.25f,1.00f);
		Vector2 _01 = new Vector2(0.50f,0.75f);
		Vector2 _02 = new Vector2(0.50f,0.25f);
		Vector2 _03 = new Vector2(0.25f,0.00f);
		Vector2 _04 = new Vector2(0.00f,0.25f);
		Vector2 _05 = new Vector2(0.00f,0.75f);
		
		Vector2 _06 = new Vector2(0.50f,0.00f);
		Vector2 _07 = new Vector2(1.00f,0.00f);
		Vector2 _08 = new Vector2(0.50f,1.00f);
		Vector2 _09 = new Vector2(1.00f,1.00f);
	
		
		Vector2[] uvs = new Vector2[]
		{
			// Top
			_00, _01, _02, _03, _04, _05,
			
			// Bottom
			_00, _01, _02, _03, _04, _05,
			
			// Right
			_06, _07, _08, _09,
			
			// Left
			_06, _07, _08, _09,
			
			// Front Right
			_06, _07, _08, _09,
			
			// Front Left
			_06, _07, _08, _09,
			
			// Back Right
			_06, _07, _08, _09,
			
			// Back Left
			_06, _07, _08, _09,
		};
		#endregion
		
		#region Triangles
		int[] triangles = new int[]
		{
			// Top
			0, 1, 3,
			1, 2, 3,	
			0, 3, 4,
			0, 4, 5,		
			
			// Bottom
			9, 7, 6,
			9, 8, 7,	
			10, 9, 6,
			11, 10, 6,
			
			// Right
			14, 13, 12,
			14, 15, 13,
			
			// Left
			18, 17, 16,
			18, 19, 17,
			
			// Front Right
			22, 21, 20,
			22, 23, 21,
			
			// Front Left
			26, 25, 24,
			26, 27, 25,
			
			// Back Right
			30, 29, 28,
			30, 31, 29,
			
			// Back Left
			34, 33, 32,
			34, 35, 33,
			
			
		};
		#endregion
		
		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		
		mesh.RecalculateBounds();
		;

	}

	static void CreateMeshAsset (Mesh mesh) {
		AssetDatabase.CreateAsset(mesh, "Assets/hexMeshTop.asset");
	}

}
