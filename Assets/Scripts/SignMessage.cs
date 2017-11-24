using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignMessage : MonoBehaviour {

    public string message;

    SpeechContainer spc;

    GameObject ply;

	// Use this for initialization
	void Start () {
        ply = GameObject.FindGameObjectWithTag("Player");

        if (message.Length > 0)
        {
            spc = SpeechContainer.Load(message);
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (spc != null)
        {
            if (Input.GetAxisRaw("Vertical") > 0.5f && ply.GetComponent<PlayerMove>().IsGrounded() && Vector3.Distance(ply.transform.position, transform.position + new Vector3(0.25f,0,0)) < 0.5f)
            {
                MessageBox.LoadIntoBuffer(spc);
            }
        }
	}
}
