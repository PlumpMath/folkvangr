using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode()]
public class LevelManager : MonoBehaviour {

    public float lB, rB, tB, bB;  // left right top and bottom bounds

    private void Start()
    {
        lB = float.MaxValue;
        rB = float.MinValue;
        bB = lB;
        tB = rB;

        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Block"))
        {
            Vector3 pos = g.transform.position;

            lB = pos.x < lB ? pos.x : lB;

            rB = pos.x > rB ? pos.x : rB;

            bB = pos.y < bB ? pos.y : bB;

            tB = pos.y > tB ? pos.y : tB;
        }

        lB += 5;
        rB -= 5;
        bB += 5;
        tB -= 2;

        if (tB < bB)
        {
            tB = bB;
        }
        if (rB < lB)
        {
            rB = lB;
        }
    }

    // Update is called once per frame
    void Update () {
		if (!Application.isPlaying)
        {
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("Block"))
            {
                Vector3 pos = g.transform.position;

                g.transform.position = new Vector3(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));
            }
        }
	}
}
