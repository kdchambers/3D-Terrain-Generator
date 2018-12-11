using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Terrain : MonoBehaviour {

	private TerrainChunk[,] terrainChunks;
	/* 	How many chunks in each distance to render 
		E.g when 1, will render the players chunk, and 8 others (1 in each direction + diagnals) */
	private int chunkRenderDistance = 0;
	private int chunksPerDimension;
	private PerlinNoise noiseGenerator;
	private int[] chunkDimensions;
	private TerrainType[] terrainTypes;
	private float maxTerrainHeight = 50;
	public GameObject terrainObject;

	private const int XVAL = 0;
	private const int YVAL = 1;

	public Terrain(PerlinNoise noiseGenerator, int[] chunkDimensions, TerrainType[] terrainTypes)
	{
		/* Set object to origin */
		terrainObject.transform.position = new Vector3(0f, 0f, 0f);

		chunksPerDimension = (int) Math.Floor(Math.Pow(chunkRenderDistance + (chunkRenderDistance + 1), 2));
		terrainChunks = new TerrainChunk[chunksPerDimension, chunksPerDimension];
		this.noiseGenerator = noiseGenerator;
		this.chunkDimensions = chunkDimensions;
		this.terrainTypes = terrainTypes;
	}

	public static Texture2D Generate2DTextureForTerrains(float[,] noiseMap, TerrainType[] terrainArr)
	{
		int mapWidth = noiseMap.GetLength(0);
		int mapHeight = noiseMap.GetLength(1);
		int numTerrains = terrainArr.Length;

		Texture2D resultTexture = new Texture2D(mapWidth, mapHeight);
		Color[] colorMap = new Color[mapWidth * mapHeight];

		System.Random random = new System.Random (1332);

		/* Generate color pixels */
		for(int x = 0; x < mapWidth; x++)
		{
			for(int y = 0; y < mapHeight; y++)
			{
				for(int i = 0; i < numTerrains; i++)
				{
					if(noiseMap[x,y] <= terrainArr[i].heightCutoff)
					{
						int numColors = terrainArr[i].terrainColors.Length;
						Color32 currColor = terrainArr[i].terrainColors[ random.Next(1, 1000) % numColors];

						colorMap[y * mapWidth + x] = currColor;
						break;
					}
				}
			}
		}

		resultTexture.filterMode = FilterMode.Point;
		resultTexture.wrapMode = TextureWrapMode.Clamp;
		resultTexture.SetPixels(colorMap);
		resultTexture.Apply();

		return resultTexture;
	}

	/* Vector2 playerPosition */
	public void Render()
	{
		for(int x = 0; x < chunksPerDimension; x++)
		{
			for(int y = 0; y < chunksPerDimension; y++)
			{
				/* Set Noise generation offset to chunkSize * global chunk dimension index */
				float[,] currentNoiseArray = noiseGenerator.GenerateNoiseArr(chunkDimensions[XVAL] * x, chunkDimensions[YVAL] * y);
				Mesh mesh = GenerateMeshFromNoiseMap(currentNoiseArray, maxTerrainHeight, terrainTypes[0].heightCutoff);
				Texture2D texture = Generate2DTextureForTerrains(currentNoiseArray, terrainTypes);

				terrainChunks[x,y] = new TerrainChunk(new Vector2(x, y), chunksPerDimension, this.transform, mesh, texture);
			}
		}
	}

	public static Mesh GenerateMeshFromNoiseMap(float[,] noiseMap, float maxHeight, float minHeightClamp)
	{
		int mapSize = noiseMap.GetLength(0);

		Vector3[] vertices = new Vector3[mapSize * mapSize];
		int[] triangles = new int[(mapSize - 1)*(mapSize - 1) * 6];
		Vector2[] uvs = new Vector2[mapSize * mapSize];
		Vector3[] normals = new Vector3[mapSize * mapSize];

		Mesh mesh = new Mesh();

		for(int x = 0; x < mapSize; x++)
		{
			for(int y = 0; y < mapSize; y++)
			{
				vertices[x * mapSize + y].x = x - 5;

				/* Clamp height at lower bound */
				vertices[x * mapSize + y].y = (noiseMap[x,y] > minHeightClamp) ? (1 + noiseMap[x,y] * maxHeight) : minHeightClamp * maxHeight;

				vertices[x * mapSize + y].z = y - 5;

				normals[x * mapSize + y].x = 0;
				normals[x * mapSize + y].y = 1;
				normals[x * mapSize + y].z = 0;
			}
		}

		int triangleIndex = 0;
		for(int y = 0; y < mapSize; y++)
		{
			for(int x = 0; x < mapSize; x++)
			{
				if (x < (mapSize - 1) && y < (mapSize - 1))
				{
					triangles[triangleIndex] = x + (y * mapSize);							// Top left
					triangles[triangleIndex + 1] = x + mapSize + (y * mapSize);				// Bottom left
					triangles[triangleIndex + 2] = x + mapSize + 1 + (y * mapSize);			// Bottom Right

					triangles[triangleIndex + 3] = x + (y * mapSize);						// Top left
					triangles[triangleIndex + 4] = x + mapSize + 1 + (y * mapSize);			// Bottom Right
					triangles[triangleIndex + 5] = x + 1 + (y * mapSize);					// Top Right

					triangleIndex += 6;
				}

				uvs[x * mapSize + y].x = x / (float)mapSize;
				uvs[x * mapSize + y].y = y / (float)mapSize;
			}
		}

		mesh.vertices = vertices;
		mesh.triangles = triangles.Reverse().ToArray();
		mesh.uv = uvs;
		mesh.normals = normals;

		return mesh;
	}
}
