using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnectUnitScript : NetworkBehaviour
{
    public GameObject PlayerUnitPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
        Debug.Log("Start :: " + isLocalPlayer );
        //my inut ?
        if (!isLocalPlayer) return;
        CmdSpawnPlayerUnitWithClientAuthority();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;
        if (Input.GetKeyDown(KeyCode.S))
        {
            CmdSpawnPlayerUnitWithClientAuthority();
        }

    }

    /////////////////////////// COMMANDS
    /// This is Runing on the server 
    /// 
    [Command]
    public void CmdSpawnPlayerUnitWithClientAuthority( )
    {
        var go = Instantiate(this.PlayerUnitPrefab);
        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
        
        Debug.Log("Spawn :: " + connectionToClient );
    }
}
