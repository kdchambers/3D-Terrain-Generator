using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainType {

	public TerrainType(Color color, float heightFrom)
	{
		terrainColor = color;
		heightFromNormalised = heightFrom;
	}

	public Color terrainColor;
	public float heightFromNormalised;
}
