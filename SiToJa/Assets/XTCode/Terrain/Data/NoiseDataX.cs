using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XTCode.DataStruct {
    [CreateAssetMenu()]
    public class NoiseDataX : UpdatableData {

        public  XTCode.Terrain.Noise.NormalizeMode normalizeMode;
        public float noiseScale;

        public int octave;     //ebene
        public float lacunarity; //flüchtigkeit 

        [Range(0, 1)] public float persistance; //auswirkung

        public int seed;
        public Vector2 offset;

        protected override void OnValidate() {
            if (lacunarity < 1) lacunarity = 1;
            if (octave < 0) octave = 0;

            base.OnValidate();
        }
    }
}