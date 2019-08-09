using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class Wall : MonoBehaviour {

	private MeshFilter filter;
	private MeshCollider coll;

	//List of Edges associated with Face
	private Block[,,] blocks;

	//Face Data associated with Wall
	private Face face;

	public void WallVisible(bool visible) {
		if (visible) {
			this.gameObject.GetComponent<MeshRenderer>().enabled = true;
		} else {
			this.gameObject.GetComponent<MeshRenderer>().enabled = false;
		}
	}

	public void AssignFaceToWall(Face f) {
		face = f;
		blocks = new Block[LevelManager.FACE_MAGNITUDE*2+1,LevelManager.FACE_MAGNITUDE+1,LevelManager.FACE_MAGNITUDE*2+1];
		filter = gameObject.GetComponent<MeshFilter>();
		coll = gameObject.GetComponent<MeshCollider>();
	}

	public Block GetBlock(int x, int y, int z)
	{
		return blocks[x, y, z];
	}

	public bool BlockExistsAt(int x, int y, int z) {

		if (x < 0 || x >= blocks.GetLength (0)) {
			return false;
		}
		if (y < 0 || y >= blocks.GetLength (1)) {
			return false;
		}
		if (z < 0 || z >= blocks.GetLength (2)) {
			return false;
		}
		return true;
	}

	public void BuildGameGrid() {

		for (int i=0;i<LevelManager.FACE_MAGNITUDE*2+1;i++) {
			for (int j=0;j<LevelManager.FACE_MAGNITUDE+1;j++) {
				for (int k=0; k<LevelManager.FACE_MAGNITUDE*2+1; k++) {
					if (k == LevelManager.FACE_MAGNITUDE*2) {
						if (face.edgeGrid[i,j] >=1 && face.edgeGrid[i,j] <=4) {//Border
							blocks[i,j,k] = new Block();
						} else if (face.edgeGrid[i,j] == 6) {//Inside
							blocks[i,j,k] = new BlockBackground();
						} else if (face.edgeGrid[i,j] == 7) {//Ledge
							blocks[i,j,k] = new BlockLedge();
						} else {
							blocks[i,j,k] = new BlockAir();//outside
						}
					} else {
						blocks[i,j,k] = new BlockAir();//foreground
					}
				}
			}
		}
		UpdateGameGrid();
	}

	public void BendGameGrid(int connectionEdge, int[] key) {

		for (int i=0;i<LevelManager.FACE_MAGNITUDE*2+1;i++) {
			for (int j=0;j<LevelManager.FACE_MAGNITUDE+1;j++) {
				for (int k=0;k<LevelManager.FACE_MAGNITUDE*2+1;k++) {

					if (connectionEdge == 0 || connectionEdge == 2) {

						if (k == LevelManager.FACE_MAGNITUDE*2-key[i]) {

							if (face.edgeGrid[i,j] >=1 && face.edgeGrid[i,j] <=4) {//Border
								blocks[i,j,k] = new Block();
							} else if (face.edgeGrid[i,j] == 6) {//Inside
								blocks[i,j,k] = new BlockBackground();
							} else if (face.edgeGrid[i,j] == 7) {//Ledge
								blocks[i,j,k] = new BlockLedge();
							} else {
								blocks[i,j,k] = new BlockAir();//outside
							}
						} else {
							blocks[i,j,k] = new BlockAir();//outside
						}
					
					} else {
						if (k == LevelManager.FACE_MAGNITUDE*2-key[j]) {
							if (face.edgeGrid[i,j] >=1 && face.edgeGrid[i,j] <=4) {//Border
								blocks[i,j,k] = new Block();
							} else if (face.edgeGrid[i,j] == 6) {//Inside
								blocks[i,j,k] = new BlockBackground();
							} else if (face.edgeGrid[i,j] == 7) {//Ledge
								blocks[i,j,k] = new BlockLedge();
							} else {
								blocks[i,j,k] = new BlockAir();//outside
							}
						} else {
							blocks[i,j,k] = new BlockAir();//outside
						}
					}
				}
			}
		}
		UpdateGameGrid();
	}
	
	private void UpdateGameGrid() {

		MeshData meshData = new MeshData();
		for (int i=0;i<LevelManager.FACE_MAGNITUDE*2+1;i++) {
			for (int j=0;j<LevelManager.FACE_MAGNITUDE+1;j++) {
				for (int k=0;k<LevelManager.FACE_MAGNITUDE*2+1;k++) {
					meshData = blocks[i,j,k].Blockdata (this,i,j,k,meshData);
				}
			}
		}
		RenderMesh(meshData);
	}
	
	private void RenderMesh(MeshData meshData)
	{
		filter.mesh.Clear();
		filter.mesh.vertices = meshData.vertices.ToArray();
		filter.mesh.triangles = meshData.triangles.ToArray();
		filter.mesh.uv = meshData.uv.ToArray();
		filter.mesh.RecalculateNormals();
		
		//additions:
		coll.sharedMesh = null;
		Mesh mesh = new Mesh();
		mesh.vertices = meshData.colVertices.ToArray();
		mesh.triangles = meshData.colTriangles.ToArray();
		mesh.RecalculateNormals();
		
		coll.sharedMesh = mesh;
	}
}



















