using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTerrain : MonoBehaviour{

	public int arrWidth = -1;
	public int arrHeight = -1;
	public int seed = 1;
	public float scale = 0.00f;
	public int numOctaves = 2;
	[Range(0,1)]
	public float persistance = 0.5f;
	[Range(0,1)]
	public float lacunarity = 0.5f;

	private PerlinNoise noiseGenerator;
	public Renderer planeTextureRenderer;

	GenerateTerrain()
	{
		noiseGenerator = new PerlinNoise();
	}

	public void Start()
	{
		planeTextureRenderer = GetComponent<Renderer>();
	}

	public void drawMap()
	{
		noiseGenerator.arrWidth = arrWidth;
		noiseGenerator.arrHeight = arrHeight;
		noiseGenerator.seed = seed;
		noiseGenerator.scale = scale;
		noiseGenerator.numOctaves = numOctaves;
		noiseGenerator.persistance = persistance;
		noiseGenerator.lacunarity = lacunarity;

		Debug.Log("Drawing map");

		// Generate a noise map
		float[,] noiseArray = noiseGenerator.GenerateNoiseArr();
		Texture2D texture = Texture2DFromNoiseMap(noiseArray);
		planeTextureRenderer.sharedMaterial.mainTexture = texture;
		
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
