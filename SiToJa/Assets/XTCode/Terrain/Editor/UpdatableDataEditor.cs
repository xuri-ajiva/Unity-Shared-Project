using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UpdatableData), true)]
public class UpdatableDataEditor : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        UpdatableData dada = ( UpdatableData) target;
        if (GUILayout.Button("Updata")) {
            dada.NotifyOfUpdatadVaules();
            EditorUtility.SetDirty(target);
        }
    }

}
