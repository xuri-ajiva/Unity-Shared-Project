using UnityEngine;

namespace XTCode.Terrain {
    public static class TextureGenerator {
        public static Texture2D TextureFromColourMap(Color[] colorMap, int width, int height) {
            Texture2D texture = new Texture2D( width, height );
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.SetPixels( colorMap );
            texture.Apply();
            return texture;
        }

        public static Texture2D TextureFromHeightMap(float[,] noiseMap) {
            int width  = noiseMap.GetLength( 0 );
            int height = noiseMap.GetLength( 1 );

            Color[] colorMap = new Color[width * height];

            for ( int y = 0; y < height; y++ ) {
                for ( int x = 0; x < width; x++ ) {
                    colorMap[y * width + x] = Color.Lerp( Color.black, Color.white, noiseMap[x, y] );
                }
            }

            return TextureFromColourMap( colorMap, width, height );
        }
    }
}