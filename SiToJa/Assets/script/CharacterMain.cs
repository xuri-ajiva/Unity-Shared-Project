using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Windows.WebCam;

public class CharacterMain : NetworkBehaviour {
    public Camera _playerCam;

    // Start is called before the first frame update
    void Start() {
        if ( hasAuthority ) Camera.SetupCurrent( this._playerCam );
    }

    // Update is called once per frame
    void Update() { }
}