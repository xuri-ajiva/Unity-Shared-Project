using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XTCode.DataStruct {
    [CreateAssetMenu()]
    public class TerrainDataX : UpdatableData {
        public float uniformScale = 2.5F;

        public bool useFaltShading;

        public bool useFalloffMap;

        public float meshHeightMultiplier;
        public AnimationCurve meshHeightCurve;

        public float minHeight {
            get {
                return uniformScale * meshHeightMultiplier * meshHeightCurve.Evaluate(0);
            }
        }

        public float maxHeight {
            get {
                return uniformScale * meshHeightMultiplier * meshHeightCurve.Evaluate(1);
            }
        }
    }
}