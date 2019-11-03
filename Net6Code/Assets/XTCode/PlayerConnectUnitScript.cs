#region

using UnityEngine;
using UnityEngine.Networking;

#endregion

namespace XTCode {
    public class PlayerConnectUnitScript : NetworkBehaviour {
        public XCDataHolder defaults = new XCDataHolder();

        //[SyncVar] // on server change changes alsow on clients
        [SyncVar( hook = nameof(OnPlayerNameChanged) )] // WARNING! Hook Overrides Auto Update
        public string PlayerName = XCDataHolder.DefaultPlayerName;

        public GameObject PlayerUnitPrefab;


        // Start is called before the first frame update
        private void Start() {
            Debug.Log( "Start :: " + this.isLocalPlayer );
            //my inut ?
            if ( !this.isLocalPlayer ) return;
            CmdSpawnPlayerUnitWithClientAuthority();
        }

        // Update is called once per frame
        private void Update() {
            if ( !this.isLocalPlayer ) return;
            if ( Input.GetKeyDown( KeyCode.S ) ) CmdSpawnPlayerUnitWithClientAuthority();

            if ( Input.GetKeyDown( KeyCode.Q ) ) {
                var n = Random.Range( 0, 2000 ).ToString();
                CmdChangePlayerName( n );
            }
        }

        private void OnPlayerNameChanged(string newName) { // is an Rpc
            Debug.Log( "OnPlayerNameChanged:  Old name: " + this.PlayerName + "    :-:    New name: " + newName );
            this.gameObject.name = nameof(PlayerConnectUnitScript) + $" [{newName}] ";

            // update Playername //because hook
            this.PlayerName = newName;
        }

        /////////////////////////// COMMANDS
        /// This is Runing on the server
        [Command]
        public void CmdSpawnPlayerUnitWithClientAuthority() {
            Debug.Log( "CmdSpawnPlayerUnitWithClientAuthority: " + this.connectionToClient );

            //instantiate object
            var go = Instantiate( this.PlayerUnitPrefab );

            // tell all client from the new object :>
            NetworkServer.SpawnWithClientAuthority( go, this.connectionToClient );
        }

        [Command]
        private void CmdChangePlayerName(string n) {
            //Check playername for length of 4

            if ( n.Length < 4 ) {
                Debug.LogError( "CmdChangePlayerName: Name Must be at least 4 characters long, client: { '" + this.connectionToClient + "' } !" );
                return;
            }

            Debug.Log( "CmdChangePlayerName: " + n );

            this.PlayerName = n;

            // tell all the clients the name 
            //RpcChangePlayerName( n ); // - not needet because [SyncVar]
        }

        /////////////////////////// RPC
        /// This is Runing ONLY on the Clients
        [ClientRpc]
        private void RpcChangePlayerName(string n) { this.PlayerName = n; } // same as hook
    }
}