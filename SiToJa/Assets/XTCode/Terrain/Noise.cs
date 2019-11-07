using System;
using UnityEngine;

namespace XTCode.Terrain {
    public static class Noise {
        public static float[,] GenerateNoise(int width, int height, int seed, float scale, int ebene, float auswirkung, float flüchtigkeit, Vector2 offset) {
            float[,] noise = new float[width, height];

            //psyco random number generator
            System.Random prng         = new System.Random( seed );
            Vector2[]     ebenenOddset = new Vector2[ebene];
            for ( int i = 0; i < ebene; i++ ) {
                //not more for best perlinnoise
                float offsetNextX = prng.Next( -100000, 100000 );
                float offsetNextY = prng.Next( -100000, 100000 );
                ebenenOddset[i] = new Vector2( offsetNextX, offsetNextY ) + offset;
            }

            if ( scale <= 0 ) scale = 0.001F;

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            float hefWidth  = width  / 2F;
            float hefHeight = height / 2F;

            for ( int y = 0; y < height; y++ ) {
                for ( int x = 0; x < width; x++ ) {
                    float amplitude   = 1;
                    float frequency   = 1;
                    float noiseHeight = 1;

                    for ( int i = 0; i < ebene; i++ ) {
                        float sampleX = ( x - hefWidth )  / scale * frequency + ebenenOddset[i].x;
                        float sampleY = ( y - hefHeight ) / scale * frequency + ebenenOddset[i].y;

                        //negative values
                        float sampleZ = Mathf.PerlinNoise( sampleX, sampleY ) * 2 - 1;

                        noiseHeight += sampleZ * amplitude;

                        amplitude *= auswirkung;
                        frequency *= flüchtigkeit;
                    }

                    if ( noiseHeight > maxNoiseHeight )
                        maxNoiseHeight                                      = noiseHeight;
                    else if ( noiseHeight < minNoiseHeight ) minNoiseHeight = noiseHeight;

                    noise[x, y] = noiseHeight;
                }
            }

            //normalize
            for ( int y = 0; y < height; y++ ) {
                for ( int x = 0; x < width; x++ ) {
                    noise[x, y] = Mathf.InverseLerp( minNoiseHeight, maxNoiseHeight, noise[x, y] );
                }
            }

            return noise;
        }
    }
}