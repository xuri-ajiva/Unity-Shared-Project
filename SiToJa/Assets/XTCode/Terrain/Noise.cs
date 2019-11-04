using UnityEngine;

namespace XTCode.Terrain {
    public static class Noise {
        public static float[,] GenerateNoise(int width, int height, float scale) {
            float[,] noise = new float[width, height];

            if ( scale <= 0 ) scale = 0.001F;
            
            for ( int y = 0; y < width; y++ ) {
                for ( int x = 0; x < height; x++ ) {
                    float sampleX = x/scale;
                    float sampleY = y/scale;

                    float sampleZ = Mathf.PerlinNoise( sampleX, sampleY );

                    noise[x, y] = sampleZ;
                }
                
            }


            return noise;
        }
    }
}