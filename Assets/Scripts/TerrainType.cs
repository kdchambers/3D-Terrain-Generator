using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainType {

	public TerrainType(Color32[] colors, float cutoff)
	{
		terrainColors = colors;
		heightCutoff = cutoff;
	}

	public Color32[] terrainColors;
	public float heightCutoff;
}
