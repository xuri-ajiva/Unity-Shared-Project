using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Random = System.Random;

namespace XTCode {
    public class TerainGenerator : NetworkBehaviour {
        private Noise _noise = new Noise();

        public int depth = 20;

        public float scala = 2F;

        public        List<ObjectDATA> GameObjectsToSpawn;
        public        bool             SpawnObjets;
        private const double           TOLERANCE_SPAWN = .1F;

        public GameObject TerainRoot;

        public int dafaultY;

        public enum CHUNK_SIZE_PREV {
            A33,
            A65,
            A129,
            A513,
            A1025
        }

        [Serializable]
        public struct ObjectDATA {
            public GameObject gameObject;
            public Vector3    offset;
        }

        public        CHUNK_SIZE_PREV ChunkSize;
        public static int             CHUNK_SIZE   = 129;
        public        int             depthDevider = 4;

        public GameObject Example;


        public                  bool             trackObjects;
        [SyncVar] public static List<GameObject> GameObjectsToTrack = new List<GameObject>();

        public const int     CHUNK_GRID_SIZE = 128;
        public       bool[,] TerainMap       = new bool[CHUNK_GRID_SIZE, CHUNK_GRID_SIZE];

        private       int delay             = 0;
        private const int TERRAIN_GEN_RANGE = 4;


        //Terrain Get(int x, int y) { return this.TerainMap[(int) ( x + ( CHUNK_SIZE / 2 ) ), (int) ( y + ( CHUNK_SIZE / 2 ) )]; }

        //void Set(int x, int y, Terrain t) { this.TerainMap[(int) ( x + ( CHUNK_SIZE / 2 ) ), (int) ( y + ( CHUNK_SIZE / 2 ) )] = new Terrain(); }

        private Random rd = new Random();


        // Start is called before the first frame update
        private void Start() {
            for ( int i = 0; i < CHUNK_GRID_SIZE; i++ ) {
                for ( int j = 0; j < CHUNK_GRID_SIZE; j++ ) {
                    this.TerainMap[i, j] = false;
                }
            }

            CHUNK_SIZE = int.Parse( this.ChunkSize.ToString().Substring( 1 ) );

            GenTeraun();
        }

        private void GenTeraun() {
            for ( int i = 0; i < 4; i++ ) {
                for ( int j = 0; j < 4; j++ ) {
                    requestTerrain( 2 - i, 2 - j );
                }
            }
        }

        private float[,] GenerateHeights(int chunkX, int chunkY) {
            float[,] heigts = new float[CHUNK_SIZE, CHUNK_SIZE];
//        var      baseX  = ( CHUNK_SIZE * chunkX );
//        var      baseY  = ( CHUNK_SIZE * chunkY );

            for ( int i = 0; i < CHUNK_SIZE; i++ ) {
                for ( int j = 0; j < CHUNK_SIZE; j++ ) {
                    var k = CalculateHeigth( i, j, chunkX, chunkY ); //Perlinnoide
                    heigts[j, i] = k;
                    SpawnUnit( i + ( CHUNK_SIZE * chunkX ), k * ( CHUNK_SIZE / this.depthDevider ) +dafaultY, j + ( CHUNK_SIZE * chunkY ) );
                }
            }

            return heigts;
        }

        private float CalculateHeigth(int absX, int absY, int chunkX, int chunkY) {
            float xPos = (float) absX / CHUNK_SIZE;
            float yPos = (float) absY / CHUNK_SIZE;

            //float xPos = (float) absX * (1 / scala);
            //float yPos = (float) absY * (1 / scala);

            return Mathf.PerlinNoise( ( xPos + chunkX ) * this.scala, ( yPos + chunkY ) * this.scala );
        }

        private void SpawnUnit(int x, float y, int z) {
            if ( this.SpawnObjets ) {
                //if ( Math.Abs( ( Mathf.PerlinNoise( x / this.scala, y / this.scala ) * this.scala / 2 ) - z ) <= TOLERANCE_SPAWN ) {

                if ( this.rd.NextDouble() > 0.999F ) {
                    var o  = this.rd.Next( 0, this.GameObjectsToSpawn.Count );
                    var go = Instantiate( this.GameObjectsToSpawn[o].gameObject, this.transform, true );

                    go.transform.position = new Vector3( x, y, z ) + this.GameObjectsToSpawn[o].offset;
                }
            }
        }

        private void Update() {
            if ( !this.trackObjects ) return;
            this.delay++;
            if ( this.delay <= CHUNK_SIZE ) return;

            foreach ( var o in GameObjectsToTrack ) {
                var pos = o.transform.position / CHUNK_SIZE;

                int xChunk = (int) pos.x;
                int zChunk = (int) pos.z;

                for ( int x = -TERRAIN_GEN_RANGE; x < TERRAIN_GEN_RANGE; x++ ) {
                    for ( int z = -TERRAIN_GEN_RANGE; z < TERRAIN_GEN_RANGE; z++ ) {
                        if ( !terrainAvailable( xChunk + x, zChunk + z ) ) {
                            requestTerrain( xChunk + x, zChunk + z );
                            Debug.Log( $"Requested: [{xChunk + x},{zChunk + z}]" );
                        }
                    }
                }
            }
        }

        private bool terrainAvailable(int posX, int posY) {
            Debug.Log( posX + " - "    + posY );
            return this.TerainMap[posX + ( CHUNK_GRID_SIZE / 2 ), posY + ( CHUNK_GRID_SIZE / 2 )];
        }

        private void SetTerrainAvailable(int posX, int posY) { this.TerainMap[posX + ( CHUNK_GRID_SIZE / 2 ), posY + ( CHUNK_GRID_SIZE / 2 )] = true; }

        private void requestTerrain(int chunkX, int chunkY) {
            if ( terrainAvailable( chunkX, chunkY ) ) return;

            var go       = Instantiate( this.Example, this.TerainRoot.transform, true );
            var tBase    = go.GetComponent<UnityEngine.Terrain>();
            var tColider = go.GetComponent<TerrainCollider>();

            TerrainData tData = new TerrainData();
            tData.heightmapResolution = CHUNK_SIZE;
            tData.size                = new Vector3( CHUNK_SIZE, CHUNK_SIZE / this.depthDevider, CHUNK_SIZE );
            tData.SetHeights( 0, 0, GenerateHeights( chunkX, chunkY ) );
            //tData.SetHeights( 0, 0, Noise.GenerateNoiseMap( CHUNK_SIZE+1, CHUNK_SIZE+1, 1, this.scala, new Vector2( CHUNK_SIZE * chunkX, CHUNK_SIZE * chunkY ) ) );
            tColider.terrainData  = tData;
            tBase.terrainData     = tData;
            go.transform.position = new Vector3( CHUNK_SIZE * chunkX, dafaultY, CHUNK_SIZE * chunkY );
            go.name               = chunkX + ", " + chunkY;
            go.layer              = 8;
            SetTerrainAvailable( chunkX, chunkY );
        }
    }
}