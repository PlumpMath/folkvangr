using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoReturn : MonoBehaviour {


    SpeechContainer spc;

    bool disp;

    Animator transitionHandler;

    float timer;

    // Use this for initialization
    void Start()
    {
    
        spc = SpeechContainer.Load("Data/Dialogue/Signs/demoend");

        if (GameObject.FindGameObjectWithTag("GameController"))
        {
            transitionHandler = GameObject.FindGameObjectWithTag("GameController").transform.GetChild(2).GetComponent<Animator>();
        }

        disp = false;
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (spc != null && !disp && !GameObject.FindGameObjectWithTag("Monster")) 
        {
            
            MessageBox.LoadIntoBuffer(spc);

        }
        if (MessageBox.displaying)
        {
            disp = true;
        }
        if (disp && !MessageBox.displaying)
        {
            if (timer <= 0)
            {
                transitionHandler.Play("blackswipe");
            }
            timer += Time.fixedDeltaTime;

            if (timer >= 1.5f)
            {
                SceneManager.LoadScene("Vangr");
            }
        }
    }
}
