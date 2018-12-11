using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk {

	private int chunkSize;
	private GameObject chunkObject;
	// private Mesh mesh; 
	private MeshFilter meshFilter;
	private MeshCollider meshCollider;
	private MeshRenderer meshRenderer;
	private bool visability;

	public TerrainChunk(Vector2 globalPositionIndex, int chunkSize, Transform parent, Mesh mesh, Texture2D texture)
	{
		chunkObject = new GameObject("Terrain Chunk");

		meshRenderer = chunkObject.AddComponent<MeshRenderer>() as MeshRenderer;
		meshFilter = chunkObject.AddComponent<MeshFilter>() as MeshFilter;
		meshCollider = chunkObject.AddComponent<MeshCollider>() as MeshCollider;

		// this.mesh = mesh;
		meshRenderer.material.mainTexture = texture;

		mesh.RecalculateNormals();
		meshFilter.mesh = mesh;

		mesh.RecalculateBounds(); 
		meshCollider.sharedMesh = mesh;

		chunkObject.transform.position = new Vector3(globalPositionIndex.x * (float)chunkSize, 0f, globalPositionIndex.y * (float)chunkSize);
		chunkObject.transform.parent = parent;
	}

	public void PrepareForDelete()
	{
		/* GameObject.DestroyImmediate(meshRenderer);
		GameObject.DestroyImmediate(meshFilter);
		GameObject.DestroyImmediate(meshCollider);
		GameObject.DestroyImmediate(mesh); */

		chunkObject.transform.parent = null;
		GameObject.DestroyImmediate(chunkObject);
	}

	public bool IsVisable()
	{
		return visability;
	}

	public void SetVisable(bool isVisable)
	{
		visability = isVisable;
		chunkObject.SetActive(isVisable);
	}
}
