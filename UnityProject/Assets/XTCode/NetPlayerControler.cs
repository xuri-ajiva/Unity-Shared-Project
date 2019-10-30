using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetPlayerControler : NetworkBehaviour
{

    public GameObject PlayerUnitPrefab;

    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        //is this my unit ?
        if (!hasAuthority) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameObject.transform.Translate(0, 1, 0);
        }
    }



}
