using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingTextureGen : MonoBehaviour {

	public Renderer planeTextureRenderer;
	public int height = 100;
	public int width = 100;

	void Start () {
		// Generate a noise map
		float[,] noiseArray = GenerateTerrain.GenerateNoiseArray(width, height);
		Texture2D texture = GenerateTerrain.Texture2DFromNoiseMap(noiseArray);

		// planeTextureRenderer.sharedMaterial.mainTexture = texture;
		GetComponent<Renderer>().material.mainTexture = texture;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
