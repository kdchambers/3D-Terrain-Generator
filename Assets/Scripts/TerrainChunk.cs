using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk {

	private int chunkSize;
	private GameObject chunkObject;
	private Mesh mesh; 
	private MeshFilter meshFilter;
	private MeshCollider meshCollider;
	private MeshRenderer meshRenderer;
	private bool visability;

	public TerrainChunk(Vector2 globalPositionIndex, int chunkSize, Transform parent, Mesh mesh, Texture2D texture)
	{
		chunkObject = new GameObject("Terrain Chunk");

		meshRenderer = chunkObject.AddComponent<MeshRenderer>() as MeshRenderer;
		meshFilter = chunkObject.AddComponent<MeshFilter>() as MeshFiler;
		meshCollider = chunkObject.AddComponent<MeshCollider>() as MeshCollider;

		meshRenderer.material.mainTexture = texture;

		mesh.RecalculateNormals();
		meshFilter.mesh = mesh;

		mesh.RecalculateBounds(); 
		meshCollider.sharedMesh = mesh;

		chunkObject.transform.position = new Vector3(globalPositionIndex.x * chunkSize, 0, globalPositionIndex.y * globalPositionIndex);
		chunkObject.transform.parent = parent;
	}

	public bool isVisable()
	{
		return visability;
	}

	public setVisable(bool isVisable)
	{
		visability = isVisable;
		chunkObject.SetActive(isVisable);
	}
}
