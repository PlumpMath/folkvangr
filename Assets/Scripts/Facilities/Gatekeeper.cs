﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gatekeeper : MonoBehaviour {

    SpeechContainer spc;

    GameObject ply;


    Animator transitionHandler;

    bool fight;
    float timer;

    // Use this for initialization
    void Start()
    {
        fight = false;
        if (PlayerStats.nextBoss.Length <= 0)
        {
            PlayerStats.DetermineSoup();
        }

        if (GameObject.FindGameObjectWithTag("GameController"))
        {
            transitionHandler = GameObject.FindGameObjectWithTag("GameController").transform.GetChild(2).GetComponent<Animator>();
        }

        ply = GameObject.FindGameObjectWithTag("Player");

        spc = SpeechContainer.Load("Data/Dialogue/Facilities/gatekeeper");
 
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

            transitionHandler.Play("blackswipe");
            fight = true;

        }

        if (fight)
        {
            timer += Time.fixedDeltaTime;

            if (timer > 1.5f)
            {
                SceneManager.LoadScene(PlayerStats.nextBoss);
            }
        }


    }
}
