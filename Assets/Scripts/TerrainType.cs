using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainType {

	public TerrainType(Color color, float cutoff)
	{
		terrainColor = color;
		heightCutoff = cutoff;
	}

	public Color terrainColor;
	public float heightCutoff;
}
