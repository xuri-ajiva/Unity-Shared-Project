using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Windows.WebCam;

public class CharacterMain : NetworkBehaviour {
    public Camera _playerCam;
    
    // Start is called before the first frame update
    void Start() {
        if (hasAuthority) {
            XTCode.Terrain.EndlessTerrain.SetViewer(this.transform);


            Camera.SetupCurrent(this._playerCam);
            //TODO: WY not working ???
        }
    }

    // Update is called once per frame
    void Update() { }
}