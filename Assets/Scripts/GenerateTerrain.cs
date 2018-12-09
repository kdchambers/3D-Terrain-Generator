using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	private PerlinNoise noiseGenerator;
	private int mapSize = 11;
	public Renderer planeTextureRenderer;
	private TerrainType[] terrains; 
	// public MeshRenderer planeMeshRenderer;

	public PerlinNoise getNoiseGenerator()
	{
		return noiseGenerator;
	}

	GenerateTerrain()
	{
		noiseGenerator = new PerlinNoise();

		terrains = new TerrainType[5];

		terrains[0] = new TerrainType(Color.blue, 0.0f);
		terrains[1] = new TerrainType(Color.yellow, 0.3f);
		terrains[2] = new TerrainType(Color.green, 0.4f);
		terrains[3] = new TerrainType(Color.grey, 0.6f);
		terrains[4] = new TerrainType(Color.white, 0.85f);
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
				for(int i = 0; i < numTerrains - 1; i++)
				{
					if(noiseMap[x,y] >= terrainArr[i].heightFromNormalised && noiseMap[x,y] < terrainArr[i + 1].heightFromNormalised)
					{
						colorMap[x * mapWidth + y] = terrainArr[i].terrainColor;
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
			TestGenerateMeshFromNoiseMap2(noiseArray, 200);
			Debug.Log("Mesh Rendered");
		}else
		{
			planeTextureRenderer.material.mainTexture = texture;
		}
		
	}

	public void TestGenerateMeshFromNoiseMap2(float[,] noiseMap, float maxHeight)
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
				vertices[x * mapSize + y].y = 1 + noiseMap[x,y] * maxHeight;
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
		mesh.triangles = triangles;
		mesh.uv = uvs;
		mesh.normals = normals;

		GetComponent<MeshFilter>().mesh = mesh;

		Debug.Log("Normals array size : " + mesh.normals.Length);

	}

	public void TestGenerateMeshFromNoiseMap(float[,] noiseMap, float maxHeight)
	{
		int noiseArrWidth = noiseMap.GetLength(0);
		int noiseArrHeight = noiseMap.GetLength(1);

		float cellXSize = 10.0f;
		float cellZSize = 10.0f;

		Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

		Vector3[] vertices = new Vector3[(noiseArrWidth + 1) * (noiseArrHeight + 1)];
		int[] triangles = new int[(noiseArrHeight - 1) * (noiseArrWidth - 1) * 6];
		Vector2[] uvs = new Vector2[(noiseArrWidth + 1) * (noiseArrHeight + 1)];

		/* Generate vertices */
		for(int x = 0; x < noiseArrWidth; x++)
		{
			for(int y = 0; y < noiseArrHeight; y++)
			{
				vertices[x * noiseArrWidth + y].x = x;
				vertices[x * noiseArrWidth + y].y = noiseMap[x, y] * maxHeight;
				vertices[x * noiseArrWidth + y].z = y;
			}
		}

		int triangleIndex = 0;
		for(int x = 0; x < noiseArrWidth; x++)
		{
			for(int y = 0; y < noiseArrHeight; y++)
			{
				if (x < noiseArrWidth - 1 && y < noiseArrHeight - 1)
				{
					triangles[triangleIndex] = y;
					triangles[triangleIndex + 1] = y + (x + 1) * noiseArrWidth;
					triangles[triangleIndex + 2] = y + 1 + (x + 1) * noiseArrWidth;

					triangles[triangleIndex + 3] = y;
					triangles[triangleIndex + 4] = y + 1 + (x + 1) * noiseArrWidth;
					triangles[triangleIndex + 5] = y + 1;

					triangleIndex += 6;
				}

				uvs[x * noiseArrWidth + y].x = x / (float)noiseArrWidth;
				uvs[x * noiseArrWidth + y].y = y / (float)noiseArrHeight;
			}
		}

		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;

		mesh.RecalculateNormals();
	}

	public void GenerateMeshFromNoiseMap(float[,] noiseMap, int inverseResolution, float maxHeight)
	{
		if(inverseResolution < 4)
			inverseResolution = 4;

		int noiseArrWidth = noiseMap.GetLength(0);
		int noiseArrHeight = noiseMap.GetLength(1);

		int mapZSize = (noiseArrHeight - 1) / inverseResolution + 1;
		int mapXSize = (noiseArrWidth - 1) / inverseResolution + 1;

		float cellXSize = noiseArrWidth / mapXSize;
		float cellZSize = noiseArrHeight / mapZSize;

		Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
		Vector3[] vertices = new Vector3[mapXSize * mapZSize];
		int[] triangles = new int[(mapXSize - 1) * (mapZSize - 1) * 6];

		/* Generate vertices */
		for(int x = 0; x < mapXSize; x++)
		{
			for(int y = 0; y < mapZSize; y++)
			{
				vertices[x * mapXSize + y].x = x * cellXSize;
				vertices[x * mapXSize + y].y = noiseMap[x * inverseResolution, y * inverseResolution] * maxHeight;
				vertices[x * mapXSize + y].z = y * cellZSize;
			}
		}

		for(int x = 0; x < mapZSize - 1; x++)
		{
			for(int y = 0; y < mapXSize - 1; y++)
			{
				triangles[0] = y;
				triangles[1] = y + (x + 1) * mapXSize;
				triangles[2] = y + 1 + (x + 1) * mapXSize;

				triangles[3] = y;
				triangles[4] = y + 1 + (x + 1) * mapXSize;
				triangles[5] = y + 1;
			}
		}

		mesh.vertices = vertices;
		mesh.triangles = triangles;
	}

	public static Texture2D Texture2DFromNoiseMap(float[,] noiseMap)
	{
		int mapWidth = noiseMap.GetLength(0);
		int mapHeight = noiseMap.GetLength(1);

		Debug.Log("Height: " + mapHeight);
		Debug.Log("Width: " + mapWidth);

		Texture2D resultTexture = new Texture2D(mapWidth, mapHeight);
		Color[] colorMap = new Color[mapWidth * mapHeight];

		/* Generate color pixels */
		for(int x = 0; x < mapWidth; x++)
		{
			for(int y = 0; y < mapHeight; y++)
			{
				colorMap[x * mapWidth + y] = Color.Lerp(Color.black, Color.white, noiseMap[x,y]);
			}
		}

		resultTexture.filterMode = FilterMode.Point;
		resultTexture.wrapMode = TextureWrapMode.Clamp;
		resultTexture.SetPixels(colorMap);
		resultTexture.Apply();

		return resultTexture;
	}

	public static float[,] GenerateNoiseArray(int width, int height)
	{
		/* Validate input */
		if(height < 1)
			height = 1;
		if(width < 1)
			width = 1;

		float offset = 1000.0f;
		float scale = 1.5f; 
		int octaves = 1;
		float persistance = 1.5f;
		float frequency = 1.0f; 
		float amplitude = 1.5f;
		float lacunarity = 1.0f;

		int counter = 0;

		float[,] resultNoiseArray = new float[width, height];

		float noiseResult;
		for(int x = 0; x < width; x++)
		{
			for(int y = 0; y < height; y++)
			{
				noiseResult = 0;
				for(int o = 0; o < octaves; o++)
				{
					noiseResult += Mathf.PerlinNoise(x * frequency, y * frequency) * amplitude;
				}
				/* Ensure result is normalised */
				noiseResult = Mathf.PerlinNoise(x * 0.1f, y * 0.1f);
				resultNoiseArray[x,y] = (noiseResult > 1.0f) ? 1.0f : noiseResult; 
			}
		}

		return resultNoiseArray;
	}
}
