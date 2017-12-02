using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OdinIntro : MonoBehaviour {

    SpeechContainer spc;

    GameObject cam;

    GameObject ply;

    bool messageGiven, messageDone;

    // Use this for initialization
    void Start()
    {
        ply = GameObject.FindGameObjectWithTag("Player");

        cam = GameObject.FindGameObjectWithTag("MainCamera");

        spc = SpeechContainer.Load("Data/Dialogue/Underworld/welcome");

        messageGiven = false;
    }

    // Update is called once per frame
    void Update () {

        if (ply.transform.position.x >= (transform.position.x - 3f) && ply.transform.position.y >= transform.position.y && !messageGiven)
        {
            MessageBox.LoadIntoBuffer(spc);
            messageGiven = true;
        }

        if (MessageBox.displaying && MessageBox.BufferOwner.Equals("Odin"))
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position, (transform.position + ply.transform.position) / 2f, 2 * Time.fixedDeltaTime);
            cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, -10f);
        }
        
        if (!MessageBox.displaying && MessageBox.BufferOwner.Equals("Odin") && messageGiven)
        {
            MessageBox.BufferOwner = "";
            messageDone = true;
        }

        if (messageDone)
        {
            transform.position += Vector3.up * Time.fixedDeltaTime;
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, GetComponent<SpriteRenderer>().color.a - Time.fixedDeltaTime);
        }
		
	}
}
