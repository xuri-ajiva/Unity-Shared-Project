using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static XTCode.XConst;

public class PlayerConectionScript : NetworkBehaviour {
    public GameObject PlayerCntrolerPrefab;

    //syncs Variable To all Clients
    //hock overrides function
    [SyncVar(hook = nameof(HookUpdateName))]
    public string PlayerName;

    // Start is called before the first frame update
    void Start() {
        if (!isLocalPlayer) return;

        CmdSpawnPlayerUnit();
    }

    // Update is called once per frame
    void Update() {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.N)) {
            var n = Random.Range(0, int.MaxValue);
            CmdChangePlayerName(name);
        }
    }

    void ChangePlayerName(string Name) { }

    //////////////////////////////////// COMMANDS
    /// Only Runs On The server
    [Command]
    //must start with Cmd
    void CmdSpawnPlayerUnit() {
        Debug.Log("CmdSpawnPlayerUnit: Spawn Unit for " + connectionToClient);
        var obj = Instantiate(PlayerCntrolerPrefab);

        //Tell all clients from the new obj and the owner!
        NetworkServer.SpawnWithClientAuthority(obj, connectionToClient);
    }

    [Command]
    void CmdChangePlayerName(string name) {
        Debug.Log($"CmdChangePlayerName: Request to change Players name to [{name}] for: " + connectionToClient);

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
        PlayerName = name;
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
        Debug.Log($"HookUpdateName: Request To change Name from [{PlayerName}] to [{name}].");
        //because we override the SyncVar Rcp we need to set the variable self 
        PlayerName = name;
    }
}