using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerainGenerator : MonoBehaviour {
    public int depth = 20;


    public int wihide = 512;
    public int heigth = 512;

    public float scala = 20F;


    // Start is called before the first frame update
    void Start() {
        GenTeraun();
    }

    private void GenTeraun() {        
        Terrain t = GetComponent<Terrain>();
        t.terrainData = GenerateTerrain(t.terrainData);
    }

    private TerrainData GenerateTerrain(TerrainData terrainData) {
        terrainData.size = new Vector3(wihide, depth, heigth);
        terrainData.SetHeights(0, 0, GenerateHeights());

        return terrainData;
    }

    private float[,] GenerateHeights() {
        float[,] heigts = new float[wihide, heigth];

        for (int i = 0; i < wihide; i++) {
            for (int j = 0; j < heigth; j++) {
                heigts[i, j] = CalculateHeigth(i, j); //Perlinnoide
            }
        }

        return heigts;
    }

    private float CalculateHeigth(int x, int y) {
        float xCoord = (float) x / wihide * scala;
        float yCoord = (float) y / heigth * scala;

        return Mathf.PerlinNoise(xCoord, yCoord);
    }


    // Update is called once per frame
    void Update() {
        
    }
}