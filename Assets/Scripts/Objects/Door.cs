using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour {

    GameObject ply;

    Animator transitionHandler;

    bool moving;
    float timer;

    public string destination;

    // Use this for initialization
    void Start()
    {
        moving = false;
        if (PlayerStats.nextBoss.Length <= 0)
        {
            PlayerStats.DetermineSoup();
        }

        if (GameObject.FindGameObjectWithTag("GameController"))
        {
            transitionHandler = GameObject.FindGameObjectWithTag("GameController").transform.GetChild(2).GetComponent<Animator>();
        }

        ply = GameObject.FindGameObjectWithTag("Player");

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetAxisRaw("Vertical") > 0.5f && ply.GetComponent<PlayerMove>().IsGrounded() && Vector3.Distance(ply.transform.position, transform.position + new Vector3(1f, 0, 0)) < 1f)
        {
            transitionHandler.Play("blackswipe");
            moving = true;
        }
        


        if (moving)
        {
            timer += Time.fixedDeltaTime;

            if (timer > 1.5f)
            {
                SceneManager.LoadScene(destination);
            }
        }


    }
}
