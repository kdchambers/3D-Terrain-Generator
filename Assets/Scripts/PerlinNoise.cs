using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise {

	private const int DEFAULT_ARR_WIDTH = 100;
	private const int DEFAULT_ARR_HEIGHT = 100;
	private const int DEFAULT_SEED = 1;
	private const float DEFAULT_SCALE = 0.001f;
	private const int DEFAULT_NUM_OCTAVES = 2;
	private const float DEFAULT_PERSISTANCE = 0.2f;
	private const float DEFAULT_LACUNARITY = 0.5f;
	private const int DEFAULT_SEEDED_GEN_MAX_VAL = 5000;
	private const int DEFAULT_SEEDED_GEN_MIN_VAL = -5000;

	public int arrWidth = DEFAULT_ARR_WIDTH;
	public int arrHeight = DEFAULT_ARR_HEIGHT;
	public int seed = DEFAULT_SEED;
	public float scale = DEFAULT_SCALE;
	public int numOctaves = DEFAULT_NUM_OCTAVES;
	public float persistance = DEFAULT_PERSISTANCE;
	public float lacunarity = DEFAULT_LACUNARITY;
	public int seededNumberGeneratorMaxVal = 5000;
	public int seededNumberGeneratorMinVal = -5000;

	private static ILogger logger = Debug.unityLogger;
	private static string logTag = "TerrainGenerator";

	private void ValidateState()
	{
		if(seededNumberGeneratorMaxVal <= seededNumberGeneratorMinVal)
		{
			seededNumberGeneratorMinVal = DEFAULT_SEEDED_GEN_MIN_VAL;
			seededNumberGeneratorMaxVal = DEFAULT_SEEDED_GEN_MAX_VAL;
			logger.LogWarning(logTag, "Invalid seenNumberGenerator MIN or MAX. Setting to default values");
		}

		if(arrWidth < 1)
		{
			arrWidth = DEFAULT_ARR_WIDTH;
			logger.LogWarning(logTag, "Invalid arrWidth value set. Set to default: " + DEFAULT_ARR_WIDTH);
		}

		if(arrHeight < 1)
		{
			arrHeight = DEFAULT_ARR_HEIGHT;
			logger.LogWarning(logTag, "Invalid arrHeight value set. Set to default: " + DEFAULT_ARR_HEIGHT);
		}

		if(scale < 0.00001f)
		{
			scale = 0.00001f;
			logger.LogWarning(logTag, "Scale too low. Clamped to 0.0001f");
		}

		if(seed < 0)
		{
			logger.LogWarning(logTag, "Seed is negative. Absolute value will be used instead");
		}

		if(numOctaves < 1)
		{
			numOctaves = 1;
			logger.LogWarning(logTag, "Invalid numOctaves value set. Clamped up to 1");
		}
	}

	public float[,] GenerateNoiseArr()
	{
		/* Validations */
		ValidateState();

		/* Setup Psuedo Random Number Generator with given seed */
		System.Random seededNumberGenerator = new System.Random(seed);
		Vector2[] octaveOffsets = new Vector2[numOctaves];
		float[,] noiseArr = new float[arrWidth, arrWidth];

		float[] offset = new float[2];
		offset[0] = seededNumberGenerator.Next (seededNumberGeneratorMinVal, seededNumberGeneratorMaxVal);
		offset[1] = seededNumberGenerator.Next (seededNumberGeneratorMinVal, seededNumberGeneratorMaxVal);
		
		for (int i = 0; i < numOctaves; i++) {
			octaveOffsets[i] = new Vector2 (offset[0], offset[1]);
		}

		for (int y = 0; y < arrHeight; y++) {
			for (int x = 0; x < arrWidth; x++) {
		
				float currentVal = 0;

				float amplitude = 1;
				float frequency = 1;

				for (int i = 0; i < numOctaves; i++) {
					float perlinX = x / scale * frequency + octaveOffsets[i].x;
					float perlinY = y / scale * frequency + octaveOffsets[i].y;

					float perlinValue = Mathf.PerlinNoise(perlinX, perlinY) * 2 - 1;
					currentVal += perlinValue * amplitude;

					amplitude *= persistance;
					frequency *= lacunarity;
				}

				noiseArr [x, y] = currentVal;
			}
		}

/*
		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {
				noiseMap [x, y] = Mathf.InverseLerp (minNoiseHeight, maxNoiseHeight, noiseMap [x, y]);
			}
		}
*/

		return noiseArr;
	}
}
