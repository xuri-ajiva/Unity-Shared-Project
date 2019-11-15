using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XTCode.DataStruct {
    [CreateAssetMenu()]
    public class TextureDataX : UpdatableData {
        float savedMinHeight;
        float savedMaxHeight;

        public Color[] baseColours;
        [Range(0,1)]public float[] baseStartHeight;

        public void ApplyToMaterial(Material material) {
            material.SetInt("baseColourCount", baseColours.Length);
            material.SetColorArray("baseColours", baseColours);
            material.SetFloatArray("baseStartHeight", baseStartHeight);

            UpdataMeshHeight(material, savedMinHeight, savedMaxHeight);
        }

        public void UpdataMeshHeight(Material material, float minHeight, float maxHeight) {
            //Debug.Log("min: " + minHeight + " - max: " + maxHeight);
            savedMaxHeight = maxHeight;
            savedMinHeight = minHeight;

            material.SetFloat("minHeight", minHeight);
            material.SetFloat("maxHeight", maxHeight);
        }

    }
}
