using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearBulletRaw : MonoBehaviour {

    public float yvel, xvel;
	
	// Update is called once per frame
	void Update () {
        transform.position += new Vector3(xvel, yvel) * Time.fixedDeltaTime;
	}
}
