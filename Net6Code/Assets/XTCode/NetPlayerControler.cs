#region

using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable 618

#endregion

namespace XTCode {
    public class NetPlayerControler : NetworkBehaviour {
        // Start is called before the first frame update
        private void Start() { }

        // Update is called once per frame
        private void Update() {
            if ( this.hasAuthority ) {
                if ( Input.GetKeyDown( KeyCode.Space ) ) this.gameObject.transform.Translate( 0, 1, 0 );
                if ( Input.GetKeyDown( KeyCode.D ) ) CmdDestroyUnit( this.gameObject );
            }
        }

        [Command]
        public void CmdDestroyUnit(GameObject gameObjectPrefab) {
            Destroy( gameObjectPrefab );
            NetworkServer.Destroy( gameObjectPrefab );
        }
    }
}