using UnityEngine;
using UnityEngine.Networking;
using static XTCode.XConst;

namespace XTCode {
    public class PlayerConectionScript : NetworkBehaviour {
        public GameObject PlayerCntrolerPrefab;

        //syncs Variable To all Clients
        //hock overrides function
        [SyncVar(hook = nameof(HookUpdateName))]
        public string PlayerName;

        // Start is called before the first frame update
        void Start() {
            if (!this.isLocalPlayer) return;

            CmdSpawnPlayerUnit();
        }

        // Update is called once per frame
        void Update() {
            if (!this.isLocalPlayer) return;

            if (Input.GetKeyDown(KeyCode.N)) {
                var n = Random.Range(0, int.MaxValue);
                CmdChangePlayerName(this.name);
            }
        }

        void ChangePlayerName(string Name) { }

        //////////////////////////////////// COMMANDS
        /// Only Runs On The server
        [Command]
        //must start with Cmd
        void CmdSpawnPlayerUnit() {
            Debug.Log("CmdSpawnPlayerUnit: Spawn Unit for " + this.connectionToClient);
            var obj = Instantiate(this.PlayerCntrolerPrefab);

            //Tell all clients from the new obj and the owner!
            NetworkServer.SpawnWithClientAuthority(obj, this.connectionToClient);
        }

        [Command]
        void CmdChangePlayerName(string name) {
            Debug.Log($"CmdChangePlayerName: Request to change Players name to [{name}] for: " + this.connectionToClient);

            //check plyers Name Length
            if (name.Length < MINIMUM_NAME_LENGTH) {
                Debug.LogError($"CmdChangePlayerName: Players name is to short:[{name.Length}]!");
                return;
            }

            //check plyers Name Length
            if (name.Length > MAXIMUM_NAME_LENGTH) {
                Debug.LogError($"CmdChangePlayerName: Players name is to long:[{name.Length}]!");
                return;
            }

            //TODO: Filter for blacklisted Words

            //because SyncVar Updates On all Clients: Calls HookUpdateName
            this.PlayerName = name;
        }

        ////////////////////////////////// RPC
        /// Only runs on Clients

        /*[ClientRpc]
    void RpcUpdateName(string name)
    {
        Debug.Log($"RpcUpdateName: Request To Chaneg Name From [{PlayerName}] to [{name}].");
        PlayerName = name;
    }
*/


        //////////////////////////////// HOOKS
        /// Custom Hocks
        void HookUpdateName(string name) {
            Debug.Log($"HookUpdateName: Request To change Name from [{this.PlayerName}] to [{name}].");
            //because we override the SyncVar Rcp we need to set the variable self 
            this.PlayerName = name;
        }
    }
}