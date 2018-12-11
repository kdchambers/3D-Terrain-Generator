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
	public int seededNumberGeneratorMaxVal = DEFAULT_SEEDED_GEN_MAX_VAL;
	public int seededNumberGeneratorMinVal = DEFAULT_SEEDED_GEN_MIN_VAL;

	private ILogger logger = Debug.unityLogger;
	private const string logTag = "TerrainGenerator";

	public PerlinNoise(int arrWidth, int arrHeight, int seed, float scale, int numOctaves, float persistance, float lacunarity)
	{
		this.arrWidth = arrWidth;
		this.arrHeight = arrHeight;
		this.seed = seed;
		this.scale = scale;
		this.numOctaves = numOctaves;
		this.persistance = persistance;
		this.lacunarity = lacunarity;

		ValidateState();
	}

	private static void ValidateInput(int arrWidth, int arrHeight, int seed, float scale, int numOctaves)
	{
		ILogger logger = Debug.unityLogger;

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

		if(scale < 0.0001f)
		{
			scale = 0.0001f;
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

	public static float[,] GenerateNoiseArr(int arrWidth, int arrHeight, int seed, float scale, int numOctaves, float persistance, float lacunarity, int xIndex, int yIndex)
	{
		/* Validations */
		ValidateInput(arrWidth, arrHeight, seed, scale, numOctaves);

		float maxVal = float.MinValue;
		float minVal = float.MaxValue;

		/* Setup Psuedo Random Number Generator with given seed */
		System.Random seededNumberGenerator = new System.Random(seed);
		Vector2[] octaveOffsets = new Vector2[numOctaves];
		float[,] noiseArr = new float[arrWidth, arrWidth];

		float[] offset = new float[2];
		offset[0] = seededNumberGenerator.Next(-5000, 5000);
		offset[1] = seededNumberGenerator.Next(-5000, 5000);
		
		for (int i = 0; i < numOctaves; i++) {
			octaveOffsets[i] = new Vector2 (offset[0], offset[1]);
		}

		float halfWidth = arrWidth / 2f;
		float halfHeight = arrHeight / 2f;

		for (int y = 0; y < arrHeight; y++) {
			for (int x = 0; x < arrWidth; x++) {
		
				float currentVal = 0;

				float amplitude = 1;
				float frequency = 1;

				for (int i = 0; i < numOctaves; i++) {
					float perlinX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x + (xIndex * arrWidth);
					float perlinY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y + (yIndex * arrHeight);

					float perlinValue = Mathf.PerlinNoise(perlinX, perlinY);
					currentVal += perlinValue * amplitude;

					amplitude *= persistance;
					frequency *= lacunarity;
				}

				if(currentVal < minVal)
					minVal = currentVal;
				else if(currentVal > maxVal)
					maxVal = currentVal;

				/* Normalise */
				noiseArr[x, y] = Mathf.InverseLerp(minVal, maxVal, currentVal);
			}
		}

		return noiseArr;
	}

	private void ValidateState()
	{
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

		if(scale < 0.0001f)
		{
			scale = 0.0001f;
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

	public float[,] GenerateNoiseArr(int xIndex, int yIndex)
	{
		/* Validations */
		ValidateState();

		float maxVal = float.MinValue;
		float minVal = float.MaxValue;

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

		float halfWidth = arrWidth / 2f;
		float halfHeight = arrHeight / 2f;

		for (int y = 0; y < arrHeight; y++) {
			for (int x = 0; x < arrWidth; x++) {
		
				float currentVal = 0;

				float amplitude = 1;
				float frequency = 1;

				for (int i = 0; i < numOctaves; i++) {
					float perlinX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x + (xIndex * arrWidth);
					float perlinY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y + (yIndex * arrHeight);

					float perlinValue = Mathf.PerlinNoise(perlinX, perlinY);
					currentVal += perlinValue * amplitude;

					amplitude *= persistance;
					frequency *= lacunarity;
				}

				if(currentVal < minVal)
					minVal = currentVal;
				else if(currentVal > maxVal)
					maxVal = currentVal;

				/* Normalise */
				noiseArr[x, y] = Mathf.InverseLerp(minVal, maxVal, currentVal);
			}
		}

		return noiseArr;
	}
}
