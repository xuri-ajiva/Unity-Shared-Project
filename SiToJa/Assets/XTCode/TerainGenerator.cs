﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class TerainGenerator : MonoBehaviour {
    public int depth = 20;

    public int wihide = 512;
    public int heigth = 512;

    public float scala = 2F;


    public Vector2 Offest = new Vector2(0, 0);


    public List<GameObject> GameObjectsToSpawn;
    public bool SpawnObjets;
    private const double TOLERANCE_SPAWN = 1F;


    private Random rd = new Random();

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
        float xCoord = (float) x / wihide * scala + Offest.x;
        float yCoord = (float) y / heigth * scala + Offest.y;

        float zCoord = Mathf.PerlinNoise(xCoord, yCoord);


        if (SpawnObjets) {
            if (Math.Abs((Mathf.PerlinNoise(xCoord / scala, yCoord / scala) * scala / 2) / zCoord) < TOLERANCE_SPAWN) {
                SpawnUnit(x, y, zCoord);
            }
        }

        return zCoord;
    }

    private void SpawnUnit(int x, int y, float z) {
        var o = rd.Next(0, GameObjectsToSpawn.Count);
        var go = Instantiate(GameObjectsToSpawn[o], this.transform, true);
        go.transform.position = new Vector3(x, y - 1, z);
    }


    // Update is called once per frame
    void Update() {
        GenTeraun();
    }
}