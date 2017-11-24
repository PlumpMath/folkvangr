using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gatekeeper : MonoBehaviour {

    SpeechContainer spc;

    GameObject ply;

    GameObject gcr;

    // Use this for initialization
    void Start()
    {
        ply = GameObject.FindGameObjectWithTag("Player");

        spc = SpeechContainer.Load("Data/Dialogue/Facilities/gatekeeper");

        gcr = GameObject.FindGameObjectWithTag("GameController");
    }

    // Update is called once per frame
    void Update()
    {
        if (spc != null)
        {
            if (Input.GetAxisRaw("Vertical") > 0.5f && ply.GetComponent<PlayerMove>().IsGrounded() && Vector3.Distance(ply.transform.position, transform.position + new Vector3(1f, 0, 0)) < 1f)
            {
                MessageBox.LoadIntoBuffer(spc);
            }
        }
        else
        {
            print("Could not find dialogue");
        }

        if (MessageBox.BufferOwner.Equals("Vaktare") && MessageBox.AnsBuffer == 0 && !MessageBox.displaying)
        {
            MessageBox.BufferOwner = "";
            MessageBox.AnsBuffer = -1;

        }


    }
}
