using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followObject : MonoBehaviour {
    Vector3 start;

    public static Transform toFolow;
    public bool x;
    public bool y;
    public bool z;
    // Start is called before the first frame update
    void Start() {
        start = transform.position;
        transform.parent = null;
    }

    // Update is called once per frame
    void Update() {
        if (toFolow != null) {
            var newx =new Vector3();

            newx.x = (!x) ? start.x : toFolow.position.x;
            newx.y = (!y) ? start.y : toFolow.position.y;
            newx.z = (!z) ? start.z : toFolow.position.z;

            gameObject.transform.position = newx;

        }
    }
}
