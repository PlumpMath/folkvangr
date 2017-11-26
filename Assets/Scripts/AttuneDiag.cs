using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttuneDiag : MonoBehaviour {

    SpeechContainer spc;

    GameObject ply;

    GameObject gcr;

    // Use this for initialization
    void Start()
    {
        ply = GameObject.FindGameObjectWithTag("Player");

        spc = SpeechContainer.Load("Data/Dialogue/Facilities/attune");

        gcr = GameObject.FindGameObjectWithTag("GameController");

        if (PlayerStats.MajorAttune == -1 || PlayerStats.MinorAttune == -1)
        {
            PlayerStats.MajorAttune = 2;
            PlayerStats.MinorAttune = 3;
            PlayerStats.SetUp();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (spc != null)
        {
            if (Input.GetAxisRaw("Vertical") > 0.5f && ply.GetComponent<PlayerMove>().IsGrounded() && Vector3.Distance(ply.transform.position, transform.position + new Vector3(0.25f, 0, 0)) < 0.5f)
            {
                MessageBox.LoadIntoBuffer(spc);
            }
        }
        else
        {
            print("Could not find dialogue");
        }

        if (MessageBox.BufferOwner.Equals("Hemlighet") && MessageBox.AnsBuffer == 0 && !MessageBox.displaying)
        {
            MessageBox.BufferOwner = "";
            MessageBox.AnsBuffer = -1;

            PlayerStats.DeTune();

            

            gcr.GetComponent<LevelManager>().blkEvent = true;
        }

        if (gcr.GetComponent<LevelManager>().blkEvent && PlayerStats.MinorAttune >= 0f)
        {
            gcr.GetComponent<LevelManager>().blkEvent = false;
            PlayerStats.SetUp();
        }
    }
}
