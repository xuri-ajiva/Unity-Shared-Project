using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetPlayerControler : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (hasAuthority)
        {

            if (Input.GetKeyDown(KeyCode.Space))
            {
                gameObject.transform.Translate(0, 1, 0);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                CmdDestroyUnit(gameObject);
            }
        }
    }

    [Command]
    public void CmdDestroyUnit(GameObject gameObjectPrefab)
    {
        Destroy(gameObjectPrefab);
        NetworkServer.Destroy(gameObjectPrefab);
    }
}
