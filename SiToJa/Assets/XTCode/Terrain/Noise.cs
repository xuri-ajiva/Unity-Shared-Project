using System;
using UnityEngine;

namespace XTCode.Terrain {
    public static class Noise {

        public enum NormalizeMode {

            Local, Global
        }


        public static float[,] GenerateNoise(int width, int height, int seed, float scale, int ebene, float auswirkung, float flüchtigkeit, Vector2 offset, NormalizeMode normalizeMode) {
            float[,] noise = new float[width, height];

            //psyco random number generator
            System.Random prng         = new System.Random( seed );

            float maxPossibleHeight = 0;
            float amplitude   = 1;
            float frequency   = 1;

            Vector2[]     ebenenOddset = new Vector2[ebene];
            for (int i = 0; i < ebene; i++) {
                //not more for best perlinnoise
                float offsetNextX = prng.Next( -100000, 100000 ) + offset.x;
                float offsetNextY = prng.Next( -100000, 100000 ) - offset.y;
                ebenenOddset[i] = new Vector2(offsetNextX, offsetNextY);

                maxPossibleHeight += amplitude;
                amplitude *= auswirkung;
            }

            if (scale <= 0) scale = 0.001F;

            float maxLocalNoiseHeight = float.MinValue;
            float minLocalNoiseHeight = float.MaxValue;

            float hefWidth  = width  / 2F;
            float hefHeight = height / 2F;

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    amplitude = 1;
                    frequency = 1;
                    float noiseHeight = 1;

                    for (int i = 0; i < ebene; i++) {
                        float sampleX = ( x - hefWidth+ ebenenOddset[i].x )  / scale * frequency ;
                        float sampleY = ( y - hefHeight + ebenenOddset[i].y) / scale * frequency ;

                        //negative values
                        float sampleZ = (Mathf.PerlinNoise( sampleX, sampleY ) * 2) - 1;

                        noiseHeight += sampleZ * amplitude;

                        amplitude *= auswirkung;
                        frequency *= flüchtigkeit;
                    }

                    if (noiseHeight > maxLocalNoiseHeight)
                        maxLocalNoiseHeight = noiseHeight;
                    else if (noiseHeight < minLocalNoiseHeight) minLocalNoiseHeight = noiseHeight;

                    noise[x, y] = noiseHeight;
                }
            }

            //normalize
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (normalizeMode == NormalizeMode.Local)
                        noise[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noise[x, y]);
                    else {
                        float normalizedHeight = (noise [x,y] + 1) / (2F * (maxPossibleHeight / 1F));
                        noise[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                    }
                }
            }

            return noise;
        }
    }
}