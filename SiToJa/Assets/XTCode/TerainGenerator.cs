using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using Random = System.Random;

namespace XTCode {
    public class TerainGenerator : NetworkBehaviour {
        [HideInInspector] private ChunkGenerator _chunkGenerator;

        public CHUNK_SIZE_PREV ChunkSize;
        public float           scala = 2F;

        public float depthDivider = 4;
        public int defaultsY;

        public int seed;

        public GameObject Example;
        public GameObject TerainRoot;


        public                 bool             trackObjects;
        public                 List<GameObject> gameObjectsToTrack = new List<GameObject>();
        [Range( 1, 8 )] public int              trackRange;


        public bool                            spawnObjets;
        public List<ChunkGenerator.ObjectDATA> gameObjectsToSpawn;


        private int delay                 = 0;
        static  int UPDATE_TICKS_TO_CHECK = 128;


        [SyncVar] public static List<GameObject> GameObjectsToTrack = new List<GameObject>();

        // Start is called before the first frame update
        private void Start() {
            int CHUNK_SIZE = int.Parse( this.ChunkSize.ToString().Substring( 1 ) );
            GameObjectsToTrack.AddRange( this.gameObjectsToTrack );

            ChunkGenerator.Init( CHUNK_SIZE, this.scala, this.depthDivider, this.defaultsY, this.Example );

            this._chunkGenerator = new ChunkGenerator( this.TerainRoot, this.spawnObjets, ref this.gameObjectsToSpawn, this.seed );

            GenTeraun();
        }

        private void GenTeraun() {
            for ( int i = 0; i < 4; i++ ) {
                for ( int j = 0; j < 4; j++ ) {
                    _chunkGenerator.requestTerrain( 2 - i, 2 - j );
                }
            }
        }


        private void Update() {
            if ( !this.trackObjects ) return;
            this.delay++;
            if ( this.delay <= UPDATE_TICKS_TO_CHECK ) return;

            foreach ( var o in GameObjectsToTrack ) {
                var pos = o.transform.position / ChunkGenerator.ChunkSize;

                int xChunk = (int) pos.x;
                int zChunk = (int) pos.z;

                for ( int x = -this.trackRange; x < this.trackRange; x++ ) {
                    for ( int z = -this.trackRange; z < this.trackRange; z++ ) {
                        if ( !this._chunkGenerator.GetTerrainAvailable( xChunk + x, zChunk + z ) ) {
                            this._chunkGenerator.requestTerrain( xChunk + x, zChunk + z );
                            Debug.Log( $"Requested: [{xChunk + x},{zChunk + z}]" );
                        }
                    }
                }
            }
        }


        #region Structs

        public enum CHUNK_SIZE_PREV {
            S33,
            S65,
            S129,
            S513,
            S1025,
            S2049
        }

        #endregion
    }
}