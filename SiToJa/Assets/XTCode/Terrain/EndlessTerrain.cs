using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace XTCode.Terrain {
    public class EndlessTerrain : MonoBehaviour {
        const float viewerMoveThresholdBeforUpdate = 25F;
        const float sqrviewerMoveThresholdBeforUpdate = viewerMoveThresholdBeforUpdate * viewerMoveThresholdBeforUpdate;
        
        public LODInfo[] deteilLevels;
        public static float maxViewDst;

        public static void SetViewer(Transform transform) {
            viewer = transform;
        }

        public static Transform viewer; //TODO: Make List
        public Transform localViewer;
        public Material mapMaterial;

        public static Vector2 viewerPosition;
        Vector2 viewerPositionOld;
        public static MapGenerator mapGenerator;
        int chunkSize;
        int chunksVisibleInViewDst;

        public int zOffset;
        public int meshObkectLayer;
        public bool useColider;

        Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
        static   List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

        void Start() {
            viewer = localViewer;

            mapGenerator = FindObjectOfType<MapGenerator>();

            maxViewDst = deteilLevels[deteilLevels.Length - 1].visableDstThreshold;
            chunkSize = MapGenerator.MAP_CHUNK_SIZE - 1;
            chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);

            UpdateVisibleChunks();
        }

        void Update() {
            //if (viewer == null) return;

            viewerPosition = new Vector2(viewer.position.x, viewer.position.z) / mapGenerator.terrainData.uniformScale;
            if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrviewerMoveThresholdBeforUpdate) {
                viewerPositionOld = viewerPosition;
                UpdateVisibleChunks();
            }
        }

        void UpdateVisibleChunks() {

            for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++) {
                terrainChunksVisibleLastUpdate[i].SetVisible(false);
            }
            terrainChunksVisibleLastUpdate.Clear();

            int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
            int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

            for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++) {
                for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++) {
                    Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                    if (terrainChunkDictionary.ContainsKey(viewedChunkCoord)) {
                        terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    } else {
                        terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, zOffset, meshObkectLayer, useColider, chunkSize, deteilLevels, transform, mapMaterial)); ;
                    }

                }
            }
        }

        public class TerrainChunk {

            GameObject meshObject;
            Vector2 position;
            Bounds bounds;

            MeshRenderer meshRenderer;
            MeshFilter meshFilter;
            MeshCollider meshCollider;

            LODInfo[] deteilLevels;
            LODMesh[] lodMeshes;
            LODMesh colisionLODMesh;

            MapData mapData;
            bool mapDataRecieved;
            int previousLODIndex = -1;

            public TerrainChunk(Vector2 coord, int zOffset, int meshObkectLayer, bool useColider, int size, LODInfo[] deteilLevels, Transform parent, Material material) {
                this.deteilLevels = deteilLevels;

                position = coord * size;
                bounds = new Bounds(position, Vector2.one * size);
                Vector3 positionV3 = new Vector3(position.x, zOffset, position.y);

                meshObject = new GameObject("TerainChunk");
                meshRenderer = meshObject.AddComponent<MeshRenderer>();
                meshFilter = meshObject.AddComponent<MeshFilter>();
                meshCollider = meshObject.AddComponent<MeshCollider>();
                meshRenderer.material = material;

                meshObject.transform.position = positionV3 * mapGenerator.terrainData.uniformScale;
                meshObject.transform.parent = parent;
                meshObject.transform.localScale = Vector3.one * mapGenerator.terrainData.uniformScale;
                meshObject.layer = meshObkectLayer;
                SetVisible(false);

                lodMeshes = new LODMesh[deteilLevels.Length];
                for (int i = 0; i < deteilLevels.Length; i++) {
                    lodMeshes[i] = new LODMesh(deteilLevels[i].lod, UpdateTerrainChunk/*, i == 0 && useColider*/);
                    if (deteilLevels[i].useForColider) {
                        colisionLODMesh = lodMeshes[i];
                    }
                }

                mapGenerator.RequestMapDate(position, OnMapDataReceived);
            }

            public void OnMapDataReceived(MapData mapData) {
                this.mapData = mapData;
                mapDataRecieved = true;

                Texture2D texture = TextureGenerator.TextureFromColourMap(mapData.colorMap, MapGenerator.MAP_CHUNK_SIZE, MapGenerator.MAP_CHUNK_SIZE);
                meshRenderer.material.mainTexture = texture;


                UpdateTerrainChunk();
            }


            public void UpdateTerrainChunk() {
                if (!mapDataRecieved) return;

                float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
                bool visible = viewerDstFromNearestEdge <= maxViewDst;

                if (visible) {
                    int lodIndex = 0;

                    for (int i = 0; i < deteilLevels.Length - 1; i++) {
                        if (viewerDstFromNearestEdge > deteilLevels[i].visableDstThreshold) {
                            lodIndex = i + 1;
                        } else {
                            break;
                        }
                    }
                    if (lodIndex != previousLODIndex) {
                        LODMesh lodMesh = lodMeshes[lodIndex];
                        if (lodMesh.hasMesh) {
                            previousLODIndex = lodIndex;
                            meshFilter.mesh = lodMesh.mesh;
                            //if (lodMesh.colider) meshCollider.sharedMesh = lodMesh.mesh;
                            //else meshCollider.sharedMesh = null;
                        } else if (!lodMesh.requested) {
                            lodMesh.RequestMesh(mapData);
                        }
                    }

                    if(lodIndex == 0) {
                        if (colisionLODMesh.hasMesh)
                            meshCollider.sharedMesh = colisionLODMesh.mesh;
                        else if (!colisionLODMesh.requested) {
                            colisionLODMesh.RequestMesh(mapData);
                        }
                    }

                    terrainChunksVisibleLastUpdate.Add(this);
                }

                SetVisible(visible);
            }

            public void SetVisible(bool visible) {
                meshObject.SetActive(visible);
            }

            public bool IsVisible() {
                return meshObject.activeSelf;
            }

        }

        class LODMesh {
            public Mesh mesh;
            public bool requested;
            public bool hasMesh;
            //public bool colider;

            int lod;
            System.Action updataCallBack;

            public LODMesh(int lod, System.Action _updateCallBack/*, bool _colider*/) {
                this.lod = lod;
                updataCallBack = _updateCallBack;
                //colider = _colider;
            }
            void OnMahsDataReceived(MeshData meshData) {
                mesh = meshData.CreateMesh();
                hasMesh = true;

                updataCallBack();
            }


            public void RequestMesh(MapData mapData) {
                requested = true;

                mapGenerator.RequestMeshData(mapData, lod, OnMahsDataReceived);
            }
        }
        [System.Serializable]
        public struct LODInfo {
            public int lod;
            public float visableDstThreshold;
            public bool useForColider;
        }
    }
}