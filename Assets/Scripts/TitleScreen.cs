using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour {

    GameObject cam;

    Vector3 startpos;

    bool optionsMenu, newGame;

    Resolution[] resolutions;
    int resIndex;

    bool runfull;

    GameObject resDisplay, winDisplay;

    float timer;
    bool startmessagedisp;

	// Use this for initialization
	void Start () {

        Cursor.visible = false;

        cam = GameObject.FindGameObjectWithTag("MainCamera");

        startpos = cam.transform.position;

        resolutions = Screen.resolutions;
        resIndex = resolutions.Length - 1;

        resDisplay = GameObject.Find("ResolutionVal");
        winDisplay = GameObject.Find("WindowedVal");
        runfull = true;

        startmessagedisp = false;
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.DownArrow) && !optionsMenu)
        {
            optionsMenu = true;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && optionsMenu)
        {
            optionsMenu = false;
        }

        if (optionsMenu)
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position, startpos + Vector3.up * -10.5f, Time.fixedDeltaTime * 8f);
            resDisplay.GetComponent<Text>().text = "[<] " + resolutions[resIndex].ToString() + " [>]";
            winDisplay.GetComponent<Text>().text = runfull ? "Definitely" : "Rather Not";

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                resIndex -= 1;
                if (resIndex < 0)
                {
                    resIndex = resolutions.Length - 1;
                }
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                resIndex += 1;
                if (resIndex >= resolutions.Length)
                {
                    resIndex = 0;
                }
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                runfull = !runfull;
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                Resolution r = resolutions[resIndex];
                Screen.SetResolution(r.width, r.height, runfull, r.refreshRate);
            }
        }
        else
        {
            if (!newGame)
            {
                cam.transform.position = Vector3.Lerp(cam.transform.position, startpos, Time.fixedDeltaTime * 8f);
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    newGame = true;
                    timer = 0;
                }
            }

            if (newGame)
            {
                if (cam.transform.position.x > -24 * Mathf.Pow(Screen.width / Screen.height,2))
                {
                    timer += Time.fixedDeltaTime*(timer+1);
                    cam.transform.position = cam.transform.position + Vector3.left * Time.fixedDeltaTime * timer;
                }
                else 
                {
                    if (!startmessagedisp)
                    {
                        MessageBox.LoadIntoBuffer("Data/Dialogue/Underworld/intro");
                        startmessagedisp = true;
                    }
                    else
                    {
                        if (!MessageBox.displaying)
                        {
                            SceneManager.LoadScene("Underworld");
                        }
                    }
                }
            }


        }

		
	}
}
