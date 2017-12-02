using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class TitleScreen : MonoBehaviour {

    public enum MenuState
    {
        titleScreen, optionsMenu, newGame, controlsMenu, setUpControls
    }

    public static InputHandler gameControls;

    MenuState currentState;

    GameObject cam;

    Vector3 startpos;

    Resolution[] resolutions;
    int resIndex;

    bool runfull;

    GameObject resDisplay, winDisplay;

    GameObject jSet;

    float timer;
    bool startmessagedisp;

    bool checkSticks;

    // Use this for initialization
    void Start()
    {

        Cursor.visible = false;

        cam = GameObject.FindGameObjectWithTag("MainCamera");

        startpos = cam.transform.position;

        resolutions = Screen.resolutions;
        resIndex = resolutions.Length - 1;

        resDisplay = GameObject.Find("ResolutionVal");
        winDisplay = GameObject.Find("WindowedVal");
        runfull = Screen.fullScreen;

        startmessagedisp = false;

        jSet = GameObject.Find("JSet");

        currentState = MenuState.titleScreen;

        gameControls = new InputHandler();
        if (!File.Exists(Application.persistentDataPath + "/ControlSet.dat"))
        {
            gameControls.SetDefaultControls();
            gameControls.SaveControls();
            print("Controls Created");
        }
        else
        {
            gameControls.LoadControls();
            print("Controls Loaded: " + Application.persistentDataPath + "/ControlSet.dat");
            foreach (Control c in gameControls.controls)
            {
                print(c.Name + "; Main: " + c.InputMain + "; Alternate: " + c.InputAlt);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (checkSticks)
        {
            if (Input.GetAxisRaw("Vertical") <= -0.95f)
            {
                if (currentState == MenuState.titleScreen)
                {
                    currentState = MenuState.optionsMenu;
                }
                else if (currentState == MenuState.optionsMenu)
                {
                    currentState = MenuState.controlsMenu;
                }
                checkSticks = false;
            }

            if (Input.GetAxisRaw("Vertical") >= 0.95f)
            {
                if (currentState == MenuState.optionsMenu)
                {
                    currentState = MenuState.titleScreen;
                }
                else if (currentState == MenuState.controlsMenu)
                {
                    currentState = MenuState.optionsMenu;
                }
                checkSticks = false;
            }
        }

        if (currentState == MenuState.optionsMenu)
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position, startpos + Vector3.up * -10.5f, Time.fixedDeltaTime * 8f);
            resDisplay.GetComponent<Text>().text = "[<] " + resolutions[resIndex].ToString() + " [>]";
            winDisplay.GetComponent<Text>().text = runfull ? "Definitely" : "Rather Not";

            if (checkSticks)
            {
                if (Input.GetAxisRaw("Horizontal") <= -0.95f)
                {
                    resIndex -= 1;
                    if (resIndex < 0)
                    {
                        resIndex = resolutions.Length - 1;
                    }
                    checkSticks = false;
                }
                if (Input.GetAxisRaw("Horizontal") >= 0.95f)
                {
                    resIndex += 1;
                    if (resIndex >= resolutions.Length)
                    {
                        resIndex = 0;
                    }
                    checkSticks = false;
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
        else if (currentState == MenuState.controlsMenu) {
            cam.transform.position = Vector3.Lerp(cam.transform.position, startpos + Vector3.up * -23f, Time.fixedDeltaTime * 8f);
            

        }
        else
        {
            if (currentState != MenuState.newGame)
            {
                cam.transform.position = Vector3.Lerp(cam.transform.position, startpos, Time.fixedDeltaTime * 8f);
                if (Input.GetAxisRaw("Horizontal") <= -0.95f)
                {
                    currentState = MenuState.newGame;
                    timer = 0;
                }
            }

            if (currentState == MenuState.newGame)
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

        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) <= 0.15f && Mathf.Abs(Input.GetAxisRaw("Vertical")) <= 0.15f)
        {
            checkSticks = true;
        }

        getButton();
    }

    public KeyCode getButton()
    {
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
            {
                print("KeyCode down: " + parseKeyCode(kcode));
                return kcode;
            }
        }
        return KeyCode.None;
    }

    string parseKeyCode(KeyCode k)
    {
        char[] kname = k.ToString().ToCharArray();

        string toret = "";
        if (kname.Length > 4)
        {
            for (int i = 0; i < 3; i++)
            {
                toret += kname[i];
            }

            for (int j = 3; j < kname.Length; j++)
            {
                if (Char.IsDigit(kname[j]))
                {
                    toret += kname[j];
                }
            }
            
        }
        else
        {
            foreach (char c in kname)
            {
                toret += c;
            }
        }
        return toret;
    }


}
