using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XTCode.Terrain {
    public class MapGenerator : MonoBehaviour {
        public enum DrawMode {
            NoiseMap,
            ColorMap,
            Mesh
        }

        public DrawMode drawMode;


        [Range( 0, 6 )] public int levelOfDetail;
        public const int MAP_CHUNK_SIZE = 241;

        public float noiseScale;

        public int   octave;     //ebene
        public float lacunarity; //flüchtigkeit

        [Range( 0, 1 )] public float persistance; //auswirkung

        public int            seed;
        public Vector2        offset;
        public float          meshHeightMultiplier;
        public AnimationCurve meshHeightCurve;


        public bool autoUpdate;

        public TerrainType[] regions;

        public void GenerateMap() {
            var noiseMap = Noise.GenerateNoise( MAP_CHUNK_SIZE, MAP_CHUNK_SIZE, this.seed, this.noiseScale, this.octave, this.persistance, this.lacunarity, this.offset );

            var colorMap = new Color[MAP_CHUNK_SIZE * MAP_CHUNK_SIZE];
            for ( int y = 0; y < MAP_CHUNK_SIZE; y++ ) {
                for ( int x = 0; x < MAP_CHUNK_SIZE; x++ ) {
                    float currentHeight = noiseMap[x, y];
                    for ( int i = 0; i < this.regions.Length; i++ ) {
                        if ( currentHeight <= this.regions[i].Height ) {
                            colorMap[y * MAP_CHUNK_SIZE + x] = this.regions[i].Colour;
                            break;
                        }
                    }
                }
            }

            MapDisplay display = FindObjectOfType<MapDisplay>();
            switch (this.drawMode) {
                case DrawMode.ColorMap:
                    display.DrawTexture( TextureGenerator.TextureFromColourMap( colorMap, MAP_CHUNK_SIZE, MAP_CHUNK_SIZE ) );
                    break;
                case DrawMode.NoiseMap:
                    display.DrawTexture( TextureGenerator.TextureFromHeightMap( noiseMap ) );
                    break;
                case DrawMode.Mesh:
                    display.DrawMesh( MashGenerrator.GenerateTerrainMesh( noiseMap, this.meshHeightMultiplier, this.meshHeightCurve, levelOfDetail ), TextureGenerator.TextureFromColourMap( colorMap, MAP_CHUNK_SIZE, MAP_CHUNK_SIZE ) );
                    break;
            }
        }

        private void OnValidate() {
            if ( this.lacunarity < 1 ) this.lacunarity = 1;
            if ( this.octave     < 0 ) this.octave     = 0;
        }

        private void Start() {
            GenerateMap();
        }
    }

    [Serializable]
    public struct TerrainType {
        public string Name;
        public float  Height;
        public Color  Colour;
    }
}