using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using XTCode.DataStruct;

namespace XTCode.Terrain {
    public class MapGenerator : MonoBehaviour {
        public enum DrawMode { NoiseMap, ColorMap, Mesh, FalloffMap }
        public DrawMode drawMode;

        [Range(0, 6)] public int editorPreviewLOD;
        public bool autoUpdate;

        public TerrainDataX terrainData;
        public NoiseDataX noiseData;


        public TerrainType[] regions;
        public float[,] falloffMap;

        static MapGenerator instance;
        public static int MAP_CHUNK_SIZE {
            get {
                if (instance == null)
                    instance = FindObjectOfType<MapGenerator>();

                if (instance.terrainData.useFaltShading)
                    return 95;
                else
                    return 239;
            }
        }

        public void DrawMapInEditor() {
            var mapData = GenerateMapData(Vector2.zero);

            MapDisplay display = FindObjectOfType<MapDisplay>();
            switch (this.drawMode) {
                case DrawMode.ColorMap:
                    display.DrawTexture(TextureGenerator.TextureFromColourMap(mapData.colorMap, MAP_CHUNK_SIZE, MAP_CHUNK_SIZE));
                    break;
                case DrawMode.NoiseMap:
                    display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
                    break;
                case DrawMode.Mesh:
                    display.DrawMesh(MeshGenerrator.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, editorPreviewLOD, terrainData.useFaltShading), TextureGenerator.TextureFromColourMap(mapData.colorMap, MAP_CHUNK_SIZE, MAP_CHUNK_SIZE));
                    break;
                case DrawMode.FalloffMap:
                    display.DrawTexture(TextureGenerator.TextureFromHeightMap(FalloffGenerator.GenerateFalloffMap(MAP_CHUNK_SIZE)));
                    break;
            }
        }

        void OnValuesUpdated() {
            if (!Application.isPlaying) {
                DrawMapInEditor();
            }
        }

        #region MultiThreading

        Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
        Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

        public void RequestMapDate(Vector2 center, Action<MapData> callBack) {
            ThreadStart threadStart = delegate
            {
                MapDataThread(center, callBack);
            };

            new Thread(threadStart).Start();
        }

        void MapDataThread(Vector2 center, Action<MapData> callBack) {
            var mapData = GenerateMapData(center);
            lock (mapDataThreadInfoQueue) {
                mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callBack, mapData));
            }
        }

        public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callBack) {
            ThreadStart threadStart = delegate
            {
                MeshDataThread(mapData, lod, callBack);
            };

            new Thread(threadStart).Start();
        }

        void MeshDataThread(MapData mapData, int lod, Action<MeshData> callBack) {
            var meshData = MeshGenerrator.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, lod, terrainData.useFaltShading);
            lock (meshDataThreadInfoQueue) {
                meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callBack, meshData));
            }
        }

        MapData GenerateMapData(Vector2 center) {
            var noiseMap = Noise.GenerateNoise(MAP_CHUNK_SIZE +2, MAP_CHUNK_SIZE+2, noiseData.seed, noiseData.noiseScale, noiseData.octave, noiseData.persistance, noiseData.lacunarity, center + noiseData.offset, noiseData.normalizeMode);

            var colorMap = new Color[MAP_CHUNK_SIZE * MAP_CHUNK_SIZE];
            for (int y = 0; y < MAP_CHUNK_SIZE; y++) {
                for (int x = 0; x < MAP_CHUNK_SIZE; x++) {
                    if (terrainData.useFalloffMap) {
                        noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                    }
                    float currentHeight = noiseMap[x, y];
                    for (int i = 0; i < this.regions.Length; i++) {
                        if (currentHeight >= this.regions[i].Height) {
                            colorMap[y * MAP_CHUNK_SIZE + x] = this.regions[i].Colour;
                        } else {
                            break;
                        }
                    }
                }
            }

            return new MapData(noiseMap, colorMap);
        }

        struct MapThreadInfo<T> {
            public readonly Action<T> callBack;
            public readonly T parameter;
            public MapThreadInfo(Action<T> _callBack, T _parameter) {
                callBack = _callBack;
                parameter = _parameter;
            }
        }

        #endregion

        private void OnValidate() {

            if (terrainData != null) {
                terrainData.OnVaulesUpdatad -= OnValuesUpdated;
                terrainData.OnVaulesUpdatad += OnValuesUpdated;
            }
            if (noiseData != null) {
                noiseData.OnVaulesUpdatad -= OnValuesUpdated;
                noiseData.OnVaulesUpdatad += OnValuesUpdated;
            }

            falloffMap = FalloffGenerator.GenerateFalloffMap(MAP_CHUNK_SIZE);
        }

        private void Awake() {
            falloffMap = FalloffGenerator.GenerateFalloffMap(MAP_CHUNK_SIZE);
        }

        private void Update() {
            if (mapDataThreadInfoQueue.Count > 0) {
                for (int i = 0; i < mapDataThreadInfoQueue.Count; i++) {
                    var threadInfo = mapDataThreadInfoQueue.Dequeue();
                    threadInfo.callBack(threadInfo.parameter);
                }
            }
            if (meshDataThreadInfoQueue.Count > 0) {
                for (int i = 0; i < meshDataThreadInfoQueue.Count; i++) {
                    var threadInfo = meshDataThreadInfoQueue.Dequeue();
                    threadInfo.callBack(threadInfo.parameter);
                }
            }
        }
    }

    [Serializable]
    public struct TerrainType {
        public string Name;
        public float Height;
        public Color Colour;
    }

    public struct MapData {
        public readonly float[,] heightMap;
        public readonly Color[] colorMap;
        public MapData(float[,] _heightMap, Color[] _colorMap) {
            heightMap = _heightMap;
            colorMap = _colorMap;
        }
    }
}