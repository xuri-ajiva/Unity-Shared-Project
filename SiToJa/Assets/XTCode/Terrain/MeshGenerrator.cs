using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace XTCode.Terrain {
    public static class MeshGenerrator {
        public static MeshData GenerateTerrainMesh(float[,] heights, float heightMultiplier, AnimationCurve _heightCurve, int levelOfDetail) {
            AnimationCurve heightCurve = new AnimationCurve(_heightCurve.keys);
            int   width    = heights.GetLength( 0 );
            int   height   = heights.GetLength( 1 );
            float topLeftX = ( width  - 1 ) / -2F;
            float topLeftZ = ( height - 1 ) / 2F;

            int meshSimplificationIncrement = ( levelOfDetail == 0 ) ? 1 : levelOfDetail * 2;

            int verticesPerLine = ( width - 1 ) / meshSimplificationIncrement + 1;

            var meshDate    = new MeshData( verticesPerLine, verticesPerLine );
            int vertexIndex = 0;

            for ( int y = 0; y < height; y += meshSimplificationIncrement ) {
                for ( int x = 0; x < width; x += meshSimplificationIncrement ) {
                    
                    meshDate.vertices[vertexIndex] = new Vector3( topLeftX + x, heightCurve.Evaluate( heights[x, y] ) * heightMultiplier, topLeftZ - y );
                    meshDate.uvs[vertexIndex]      = new Vector2( x                                                   / (float) width, y / (float) height );

                    if ( x < width - 1 && y < height - 1 ) {
                        meshDate.AddTriangle( vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine );

                        meshDate.AddTriangle( vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1 );
                    }

                    vertexIndex++;
                }
            }

            return meshDate;
        }
    }

    public class MeshData {
        public Vector3[] vertices;
        public int[]     triangles;
        public Vector2[] uvs;

        private int triangleIndex;

        public MeshData(int meshWidth, int meshHeight) {
            this.vertices  = new Vector3[meshWidth                          * meshHeight];
            this.uvs       = new Vector2[meshHeight                         * meshWidth];
            this.triangles = new int[( meshHeight - 1 ) * ( meshWidth - 1 ) * 6];
        }

        public void AddTriangle(int a, int b, int c) {
            this.triangles[this.triangleIndex]     = a;
            this.triangles[this.triangleIndex + 1] = b;
            this.triangles[this.triangleIndex + 2] = c;

            this.triangleIndex += 3;
        }

        public Mesh CreateMesh() {
            var mesh = new Mesh();
            mesh.vertices  = this.vertices;
            mesh.triangles = this.triangles;
            mesh.uv        = this.uvs;

            mesh.RecalculateNormals();
            return mesh;
        }
    }
}