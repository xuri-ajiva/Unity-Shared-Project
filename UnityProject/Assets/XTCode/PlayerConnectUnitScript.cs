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
        //my inut ?
        if (!isLocalPlayer) return;

        CmdSpawnPlayerUnit();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /////////////////////////// COMMANDS
    /// This is Runing on the server 
    /// 
    [Command]
    void CmdSpawnPlayerUnit()
    {
        var go = Instantiate(PlayerUnitPrefab);

        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);

    }
}
