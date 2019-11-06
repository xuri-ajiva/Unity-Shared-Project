using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

namespace XTCode {
    public class ChunkGenerator : MonoBehaviour {
        private static int   _chunkSize;
        private static float _scale;

        private static int _depthDivider;
        private static int _defaultsY;

        private static GameObject _example;

        private readonly GameObject       _terrainRoot;
        private readonly bool             _spawnObjects;
        private readonly List<ObjectDATA> _gameObjectsToSpawn;

        private readonly int _seed;

        /// <inheritdoc />
        public ChunkGenerator(GameObject terrainRoot, bool spawnObjects, ref List<ObjectDATA> gameObjectsToSpawn, int seed) {
            for ( int i = 0; i < CHUNK_GRID_SIZE; i++ ) {
                for ( int j = 0; j < CHUNK_GRID_SIZE; j++ ) {
                    this.terainMap[i, j] = false;
                }
            }

            this._terrainRoot        = terrainRoot;
            this._spawnObjects       = spawnObjects;
            this._gameObjectsToSpawn = gameObjectsToSpawn;
            this._seed               = seed;
        }

        public static void Init(int chunkSize, float scale, int depthDivider, int defaultsY, GameObject example) {
            _chunkSize    = chunkSize;
            _scale        = scale;
            _depthDivider = depthDivider;
            _defaultsY    = defaultsY;
            _example      = example;
        }

        public void requestTerrain(int chunkX, int chunkY) {
            if ( GetTerrainAvailable( chunkX, chunkY ) ) return;

            var go        = Instantiate( _example, this._terrainRoot.transform, true );
            var tBase     = go.GetComponent<UnityEngine.Terrain>();
            var tCollider = go.GetComponent<TerrainCollider>();

            var tData = new TerrainData { heightmapResolution = _chunkSize, size = new Vector3( _chunkSize, _chunkSize / _depthDivider, _chunkSize ) };

            tData.SetHeights( 0, 0, GenerateHeights( chunkX, chunkY ) );
            tCollider.terrainData   =  tData;
            tBase.terrainData       =  tData;
            go.transform.position   =  new Vector3( _chunkSize * chunkX, _defaultsY, _chunkSize * chunkY );
            go.name                 =  chunkX + ", " + chunkY;
            go.layer                =  8;
            go.transform.localScale += new Vector3( .2F, .2F, .2F );
            SetTerrainAvailable( chunkX, chunkY );
        }

        private float[,] GenerateHeights(int chunkX, int chunkY) {
            System.Random rd = null;
            if ( this._spawnObjects ) {
                rd = new System.Random( int.Parse( "" + chunkX + this._seed + chunkY ) );
            }

            var heights = new float[_chunkSize, _chunkSize];
            int baseX   = ( _chunkSize * chunkX );
            int baseY   = ( _chunkSize * chunkY );

            for ( int i = 0; i < _chunkSize; i++ ) {
                for ( int j = 0; j < _chunkSize; j++ ) {
                    float k = CalculateHeigth( i, j, chunkX, chunkY ); //Perlinnoide
                    heights[j, i] = k;
                    if ( this._spawnObjects ) SpawnUnit( i + baseX, k * ( _chunkSize / _depthDivider ) + _defaultsY, j + baseY, rd );
                }
            }

            return heights;
        }

        private static float CalculateHeigth(int absX, int absY, int chunkX, int chunkY) {
            float _const = .5F / _chunkSize;

            float xPos = (float) absX / _chunkSize;
            float yPos = (float) absY / _chunkSize;

            //if ( absX == 0 ) xPos          -= _const;
            //if ( absX == _chunkSize ) xPos += _const;
//
            //if ( absY == 0 ) yPos          -= _const;
            //if ( absY == _chunkSize ) yPos += _const;

            return Mathf.PerlinNoise( ( xPos + chunkX ) * _scale, ( yPos + chunkY ) * _scale );
        }

        private void SpawnUnit(int x, float y, int z, System.Random rd) {
            //if ( Math.Abs( ( Mathf.PerlinNoise( x / this.scala, y / this.scala ) * this.scala / 2 ) - z ) <= TOLERANCE_SPAWN ) {

            if ( !( rd.NextDouble() > 0.999F ) ) return;

            int o  = rd.Next( 0, this._gameObjectsToSpawn.Count );
            var go = Instantiate( this._gameObjectsToSpawn[o].gameObject, this.transform, true );

            go.transform.position = new Vector3( x, y, z ) + this._gameObjectsToSpawn[o].offset;
        }

        #region Memory

        //maby redo with dictionary
        public const int     CHUNK_GRID_SIZE = 128;
        public       bool[,] terainMap       = new bool[CHUNK_GRID_SIZE, CHUNK_GRID_SIZE];

        public bool GetTerrainAvailable(int posX, int posY) {
            //Debug.Log( posX + " - "    + posY );
            return this.terainMap[posX + ( CHUNK_GRID_SIZE / 2 ), posY + ( CHUNK_GRID_SIZE / 2 )];
        }

        private void SetTerrainAvailable(int posX, int posY) {
            Debug.Log( $"NewTerain: [{posX}, {posY}]" );
            this.terainMap[posX + ( CHUNK_GRID_SIZE / 2 ), posY + ( CHUNK_GRID_SIZE / 2 )] = true;
        }

        #endregion

        #region Pref

        [Serializable]
        public struct ObjectDATA {
            public GameObject gameObject;
            public Vector3    offset;
        }

        #endregion

        #region Getter

        public static int ChunkSize {
            [DebuggerStepThrough] get => _chunkSize;
        }

        public static float Scale {
            [DebuggerStepThrough] get => _scale;
        }

        public static int DepthDivider {
            [DebuggerStepThrough] get => _depthDivider;
        }

        public static int DefaultsY {
            [DebuggerStepThrough] get => _defaultsY;
        }

        public static GameObject Example {
            [DebuggerStepThrough] get => _example;
        }

        public GameObject TerrainRoot {
            [DebuggerStepThrough] get => this._terrainRoot;
        }

        public int Seed {
            [DebuggerStepThrough] get => this._seed;
        }

        public bool[,] TerainMap {
            [DebuggerStepThrough] get => this.terainMap;
        }

        #endregion
    }
}