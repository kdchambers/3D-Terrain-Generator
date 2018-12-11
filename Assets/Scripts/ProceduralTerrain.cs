using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ProceduralTerrain {

	private TerrainChunk[,] terrainChunks;
	/* 	How many chunks in each distance to render 
		E.g when 1, will render the players chunk, and 8 others (1 in each direction + diagnals) */
	private int chunkRenderDistance = 0;
	private int chunksPerDimension;
	private PerlinNoise noiseGenerator;
	private int[] chunkDimensions;
	private TerrainType[] terrainTypes;
	private float maxTerrainHeight = 20;
	public const string OBJECTTYPENAME = "Terrain Map";
	public GameObject terrainObject;

	private const int XVAL = 0;
	private const int YVAL = 1;

	public ProceduralTerrain(PerlinNoise noiseGenerator, int chunkRenderDistance, TerrainType[] terrainTypes, float maxMapHeight)
	{
		terrainObject = new GameObject(OBJECTTYPENAME);
		/* Set object to origin */
		terrainObject.transform.position = new Vector3(0f, 0f, 0f);

		maxTerrainHeight = maxMapHeight;

		this.noiseGenerator = noiseGenerator;
		this.chunkDimensions = noiseGenerator.GetDimensions();
		this.terrainTypes = terrainTypes;

		this.chunkRenderDistance = chunkRenderDistance;

		chunksPerDimension = chunkRenderDistance + (chunkRenderDistance + 1);
		Debug.Log("Chunks per Dimension : " + chunksPerDimension);
		terrainChunks = new TerrainChunk[chunksPerDimension, chunksPerDimension];

		terrainChunks[0,0] = null;
	}

	public void Clear()
	{
		if(terrainChunks == null || terrainChunks.GetLength(0) == 0 || terrainChunks[0,0] == null)
		{
			Debug.Log("No terrain map in scene to clear");
			return;
		}

		int terrainChunksWidth = terrainChunks.GetLength(0);
		int terrainChunksHeight = terrainChunks.GetLength(1);

		for(int x = terrainChunksWidth - 1; x >= 0; x--)
        {
        	for(int y = terrainChunksHeight - 1; y >= 0; y--)
        	{
        		terrainChunks[x,y].SetVisable(false);
        		terrainChunks[x,y].PrepareForDelete();
        		terrainChunks[x,y] = null;
        	}
        }

    	object[] allRootObjects = GameObject.FindObjectsOfType(typeof (GameObject));
    	GameObject[] terrainsToDelete = new GameObject[terrainChunksWidth * terrainChunksHeight];

    	int terrainsFound = 0;

		foreach (object currentObject in allRootObjects)
		{
		    GameObject currentGameObject = (GameObject) currentObject;

		    if(currentGameObject.name == OBJECTTYPENAME)
		    {
		    	terrainsToDelete[terrainsFound] = currentGameObject;
		    	terrainsFound++;
		    }
		}

		allRootObjects = null;

		for(int i = terrainsFound - 1; i >= 0; i--)
			GameObject.DestroyImmediate(terrainsToDelete[i]);

		terrainsToDelete = null;

		Debug.Log(terrainsFound + " " + OBJECTTYPENAME + "'s deleted from scene");
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
					if(noiseMap[x, y] <= terrainArr[i].heightCutoff)
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

	public void UpdateGenerationParameters(PerlinNoise noiseGenerator, int[] chunkDimensions, TerrainType[] terrainTypes)
	{
		if(noiseGenerator != null)
			this.noiseGenerator = noiseGenerator;

		if(chunkDimensions != null)
			this.chunkDimensions = chunkDimensions;

		if(terrainTypes != null)
			this.terrainTypes = terrainTypes;

		chunksPerDimension = chunkRenderDistance + (chunkRenderDistance + 1);
		terrainChunks = new TerrainChunk[chunksPerDimension, chunksPerDimension];
	}

	/* Vector2 playerPosition */
	public void Render(bool useTerrainColors)
	{
		if(terrainChunks[0,0] != null)
			this.Clear();

		for(int x = 0; x < chunksPerDimension; x++)
		{
			int relativeX = x - (chunksPerDimension - 1) / 2;

			for(int y = 0; y < chunksPerDimension; y++)
			{
				int relativeY = y - (chunksPerDimension - 1) / 2;

				/* Set Noise generation offset to chunkSize * global chunk dimension index */
				float[,] currentNoiseArray = noiseGenerator.GenerateNoiseArr(relativeX, relativeY);

				Mesh mesh = GenerateMeshFromNoiseMap(currentNoiseArray, maxTerrainHeight, terrainTypes[0].heightCutoff);
				Texture2D texture;

				if(useTerrainColors)
					texture = Generate2DTextureForTerrains(currentNoiseArray, terrainTypes);
				else
					texture = Texture2DFromNoiseMap(currentNoiseArray);

				terrainChunks[x,y] = new TerrainChunk(	new Vector2(relativeX, relativeY), 
														new Vector2(currentNoiseArray.GetLength(0), currentNoiseArray.GetLength(1)), 
														terrainObject.transform, mesh, texture);
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
				vertices[x * mapSize + y].x = x;
				/* Clamp height at lower bound */
				vertices[x * mapSize + y].y = (noiseMap[x,y] > minHeightClamp) ? (noiseMap[x,y] * maxHeight) : minHeightClamp * maxHeight;
				vertices[x * mapSize + y].z = y;

				// Debug.Log("(" + x + "," + y + ") -> " + noiseMap[x,y]);

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

		Debug.Assert(triangles.Length == ((mapSize - 1)) * (mapSize - 1) * 6);

		mesh.vertices = vertices;
		mesh.triangles = triangles.Reverse().ToArray();
		mesh.uv = uvs;
		mesh.normals = normals;

		return mesh;
	}

	public static Texture2D Texture2DFromNoiseMap(float[,] noiseMap)
	{
		int mapWidth = noiseMap.GetLength(0);
		int mapHeight = noiseMap.GetLength(1);

		Texture2D resultTexture = new Texture2D(mapWidth, mapHeight);
		Color[] colorMap = new Color[mapWidth * mapHeight];

		/* Generate color pixels */
		for(int x = 0; x < mapWidth; x++)
		{
			for(int y = 0; y < mapHeight; y++)
			{
				colorMap[y * mapWidth + x] = Color.Lerp(Color.black, Color.white, noiseMap[x,y]);
			}
		}

		resultTexture.filterMode = FilterMode.Point;
		resultTexture.wrapMode = TextureWrapMode.Clamp;
		resultTexture.SetPixels(colorMap);
		resultTexture.Apply();

		return resultTexture;
	}
}
