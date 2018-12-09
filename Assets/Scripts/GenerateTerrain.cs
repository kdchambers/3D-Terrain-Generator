using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GenerateTerrain : MonoBehaviour{

	[Range(1,15)]
	public int mapSizeSetting = 1;
	public int seed = 1;
	[Range(1,10)]
	public float scale = 1f;
	[Range(0,10)]
	public int numOctaves = 2;
	[Range(0,10)]
	public float persistance = 0.5f;
	[Range(0,2)]
	public float lacunarity = 0.5f;
	public bool renderMesh = false;
	public bool useTerrainColors = false;
	[Range(25,500)]
	public int maxMapHeight = 200;
	public bool enableWaterCutoff = false;
	public Renderer planeTextureRenderer;

	private PerlinNoise noiseGenerator;
	private int mapSize = 11;
	private TerrainType[] terrains; 

	public PerlinNoise getNoiseGenerator()
	{
		return noiseGenerator;
	}

	GenerateTerrain()
	{
		noiseGenerator = new PerlinNoise();

		terrains = new TerrainType[5];

		terrains[0] = new TerrainType(Color.blue, 0.3f);
		terrains[1] = new TerrainType(Color.yellow, 0.4f);
		terrains[2] = new TerrainType(Color.green, 0.75f);
		terrains[3] = new TerrainType(Color.grey, 0.9f);
		terrains[4] = new TerrainType(Color.white, 1.0f);
	}

	public static Texture2D Generate2DTextureForTerrains(float[,] noiseMap, TerrainType[] terrainArr)
	{
		int mapWidth = noiseMap.GetLength(0);
		int mapHeight = noiseMap.GetLength(1);
		int numTerrains = terrainArr.Length;

		Texture2D resultTexture = new Texture2D(mapWidth, mapHeight);
		Color[] colorMap = new Color[mapWidth * mapHeight];

		/* Generate color pixels */
		for(int x = 0; x < mapWidth; x++)
		{
			for(int y = 0; y < mapHeight; y++)
			{
				for(int i = 0; i < numTerrains; i++)
				{
					if(noiseMap[x,y] <= terrainArr[i].heightCutoff)
					{
						colorMap[y * mapWidth + x] = terrainArr[i].terrainColor;
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

	public void Start()
	{
		planeTextureRenderer = GetComponent<Renderer>();
		drawMap();
	}

	public void drawMap()
	{

		mapSize = mapSizeSetting * 5;

		noiseGenerator.arrWidth = mapSize;
		noiseGenerator.arrHeight = mapSize;

		noiseGenerator.seed = seed;
		noiseGenerator.scale = scale;
		noiseGenerator.numOctaves = numOctaves;
		noiseGenerator.persistance = persistance;
		noiseGenerator.lacunarity = lacunarity;

		transform.localScale = new Vector3(mapSize, 1, mapSize);

		Debug.Log("Drawing map");

		float[,] noiseArray = noiseGenerator.GenerateNoiseArr();

		Texture2D texture;

		if(useTerrainColors)
		{
			texture = Generate2DTextureForTerrains(noiseArray, terrains);
		}else
		{
			texture = Texture2DFromNoiseMap(noiseArray);
		}

		if(renderMesh)
		{
			/* Reset mesh texture */
			planeTextureRenderer.material.mainTexture = null;
			GetComponent<MeshRenderer>().sharedMaterial.mainTexture = texture;
			GenerateMeshFromNoiseMap(noiseArray, maxMapHeight);
			Debug.Log("Mesh Rendered");
		}else
		{
			planeTextureRenderer.material.mainTexture = texture;
		}
		
	}

	public void GenerateMeshFromNoiseMap(float[,] noiseMap, float maxHeight)
	{
		int mapSize = noiseMap.GetLength(0);

		Vector3[] vertices = new Vector3[mapSize * mapSize];
		int[] triangles = new int[(mapSize - 1)*(mapSize - 1) * 6];
		Vector2[] uvs = new Vector2[mapSize * mapSize];

		Mesh mesh = new Mesh();
		Vector3[] normals = new Vector3[mapSize * mapSize];

		for(int x = 0; x < mapSize; x++)
		{
			for(int y = 0; y < mapSize; y++)
			{
				vertices[x * mapSize + y].x = x - 5;

				if(enableWaterCutoff){
					vertices[x * mapSize + y].y = (noiseMap[x,y] > terrains[0].heightCutoff) ? (1 + noiseMap[x,y] * maxHeight) : terrains[0].heightCutoff * maxHeight;
				} else {
					vertices[x * mapSize + y].y = 1 + noiseMap[x,y] * maxHeight;
				}

				vertices[x * mapSize + y].z = y - 5;

				// Debug.Log( "(" + x + "," + y + ") -> " + noiseMap[x,y] );

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

		GetComponent<MeshFilter>().mesh = mesh;

		Debug.Log("Normals array size : " + mesh.normals.Length);

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
