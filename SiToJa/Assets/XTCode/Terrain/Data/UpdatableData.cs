using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatableData : ScriptableObject {
    public event System.Action OnVaulesUpdatad;

    public bool autoUpdata;

    public void NotifyOfUpdatadVaules() {
        OnVaulesUpdatad?.Invoke();
    }

    protected virtual void OnValidate() {
        if (autoUpdata)
            NotifyOfUpdatadVaules();
    }
}
