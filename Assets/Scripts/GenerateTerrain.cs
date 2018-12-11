using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GenerateTerrain : MonoBehaviour{

	[Range(1,50)]
	public int mapSizeSetting = 1;
	public int seed = 1;
	[Range(1,10)]
	public float scale = 1f;
	[Range(0,10)]
	public int numOctaves = 2;
	[Range(0,1)]
	public float persistance = 0.5f;
	[Range(0,10)]
	public float lacunarity = 0.5f;
	public bool renderMesh = false;
	public bool useTerrainColors = false;
	[Range(25,500)]
	public int maxMapHeight = 50;
	public bool enableWaterCutoff = false;
	private Renderer planeTextureRenderer;

	private PerlinNoise noiseGenerator;
	private int mapSize = 5;
	private TerrainType[] terrains; 
	private ProceduralTerrain terrain;

	GenerateTerrain()
	{
		terrains = new TerrainType[5];

		Color32[] seaColors = new Color32[1];
		seaColors[0] = new Color32(10, 50, 255, 255);

		Color32[] beachColors = new Color32[2];
		beachColors[0] = new Color32(255, 255, 0, 255);
		beachColors[1] = new Color32(255, 255, 51, 255);

		Color32[] landColors = new Color32[4];
		landColors[0] = new Color32(5, 128, 50, 255);
		landColors[1] = new Color32(15, 74, 29, 255);
		landColors[2] = new Color32(11, 150, 87, 255);
		landColors[3] = new Color32(22, 102, 17, 255);

		Color32[] mountainColors = new Color32[2];
		mountainColors[0] = new Color32(103, 110, 93, 255);
		mountainColors[1] = new Color32(79, 87, 71, 255);

		Color32[] mountainCapColors = new Color32[1];
		mountainColors[0] = new Color32(207, 207, 207, 255);

		terrains[0] = new TerrainType(seaColors, 0.35f);
		terrains[1] = new TerrainType(beachColors, 0.4f);
		terrains[2] = new TerrainType(landColors, 0.75f);
		terrains[3] = new TerrainType(mountainColors, 0.9f);
		terrains[4] = new TerrainType(mountainCapColors, 1.0f);
	}


	public void Start()
	{
		drawMap();
	}

	public void ClearMap()
	{
		if(terrain != null)
			terrain.Clear();
		else
			Debug.Log("Terrain not yet set");
	}


	public void drawMap()
	{
		mapSize = mapSizeSetting * 5;
		noiseGenerator = new PerlinNoise(mapSize, mapSize, seed, scale, numOctaves, persistance, lacunarity);
		transform.localScale = new Vector3(5, 1, 5);

		
		ClearMap();
		terrain = new ProceduralTerrain(noiseGenerator, new int[2]{mapSize,mapSize}, terrains);
		terrain.Render();

/*
		float[,] noiseArray = noiseGenerator.GenerateNoiseArr(0, 0);
		Texture2D texture = Generate2DTextureForTerrains(noiseArray, terrains);
		planeTextureRenderer.material.mainTexture = null;
		GetComponent<MeshRenderer>().material.mainTexture = texture;
		GenerateMeshFromNoiseMap(noiseArray, maxMapHeight);
		*/
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
