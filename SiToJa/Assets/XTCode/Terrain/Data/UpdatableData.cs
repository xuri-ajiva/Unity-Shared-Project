using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatableData : ScriptableObject {
    public event System.Action OnVaulesUpdatad;

    public bool autoUpdata;

    public void NotifyOfUpdatadVaules() {
        UnityEditor.EditorApplication.update -= NotifyOfUpdatadVaules;
        OnVaulesUpdatad?.Invoke();
    }

    protected virtual void OnValidate() {
        if (autoUpdata) {
            UnityEditor.EditorApplication.update += NotifyOfUpdatadVaules;
        }
    }
}
