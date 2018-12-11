using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (GenerateTerrain))]
public class GenerateTerrainEditor : Editor {

	public override void OnInspectorGUI() {
		GenerateTerrain terrainGenerator = (GenerateTerrain)target;

		if (DrawDefaultInspector()) {
				// terrainGenerator.drawMap();
		}

		if (GUILayout.Button("Generate")) {
			terrainGenerator.drawMap();
		}
	}
}
