using UnityEngine;

namespace XTCode.Terrain {
    public class EndlessTerrain :MonoBehaviour {

        public const float maxViewDst = 300;
        public Transform viewer;

        public static Vector2 viewerPosition;
        private int chunkSize;
        private int chunksVisableInDst;


        private void Start() {
            this.chunkSize = MapGenerator.MAP_CHUNK_SIZE - 1;

            this.chunksVisableInDst = Mathf.RoundToInt(maxViewDst / this.chunkSize);
        }
    }
}