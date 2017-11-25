using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour {

    public static SpeechContainer dialogue;

    private int MBI;                        // Message Box Index, what message we are printing
    private float MSGInd;                   // Message Index, what character we are printing (It's a float but we round it to an integer to use it)
    int MTP;                                // Message Type. I'll outline that real quick under here:

    /* ##############################################################
     *               MESSAGE TYPES
     * 0    Normal Dialogue
     * 1    Yes/No
     * 2    Choice, history will not be used, answer will be saved for next check.
     * 3    Repeated Choice, history will be used.
     * 4    Meaningless Choice, no branching, no history
     * 5    Ends dialogue altogether
     *  #############################################################
     */

    GameObject msgBox;                      // Physical message box that displays
    Image portrait;                         // Image of speaker to display

    Text msgText, msgName;                  // Text for message and nameplaye

    GameObject chcBox;                      // Physical choice box that may appear
    private int numChoices, selected;    // Number of Choices, Currently Selected Choice, Finalized Choice
    private bool[] choiceHistory;                   // History of choice selections for previous choice.

    static string curBranch, baseBranch, splitBranch;       // Current dialogue branch and baseBranch (In case you want to specify one I guess)
                                                            // Splitbranch is which branch to go towards after a choice

    public static int AnsBuffer;
    public static string BufferOwner;

    int RMBI;                               // Return MBI to return for when a branch that is not the main branch ends.

    GameObject HPBar;
    GameObject Soup;

    public static bool displaying;

    // Use this for initialization
    void Start () {

        // Initialize values
        MBI = 0;
        MSGInd = 0;
        numChoices = 2; // Choices can never be under 2 or over 5
        selected = 0;

        AnsBuffer = -1;
        BufferOwner = "";

        choiceHistory = new bool[5];

        ClearBuffer();

        msgBox = transform.GetChild(0).gameObject;

        msgText = msgBox.transform.GetChild(5).GetComponent<Text>();
        msgName = msgBox.transform.GetChild(1).GetComponent<Text>();
        portrait = msgBox.transform.GetChild(2).GetComponent<Image>();

        chcBox = transform.GetChild(1).gameObject;

        if (transform.childCount > 3)
        {
            HPBar = transform.GetChild(3).gameObject;
            Soup = transform.GetChild(4).gameObject;
        }

        // Hide these bad boys we don't need to see their ugly mugs
        msgBox.SetActive(false);
        chcBox.SetActive(false);

	}
	
	// Update is called once per frame
	void Update () {
        // Update in the updater
        UpdateMessageBox();
        UpdateChoiceBox();

        if (Soup != null && HPBar != null)
        {
            UpdateElements();
        }
        
        // y'all don't need this
        //DebugInputs();
	}

    void UpdateElements()
    {
        if (displaying)
        {
            Soup.SetActive(false);
            HPBar.SetActive(false);
        }
        else
        {
            Soup.SetActive(GameObject.FindGameObjectWithTag("Monster") == null);

            if (GameObject.FindGameObjectWithTag("Monster"))
            {
                HPBar.SetActive(GameObject.FindGameObjectWithTag("Monster").GetComponent<MonsterStats>().isActive);
            }

        }

        if (Soup.activeSelf)
        {
            Soup.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = PlayerStats.nextBoss;

            Soup.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = PlayerStats.currentDay.ToString();
        }
    }

    void UpdateMessageBox()
    {
        // If our dialogue object is not empty:
        if (dialogue.diaBranch.Count > 0)
        {
            displaying = true;
            if (GameObject.FindGameObjectWithTag("Player"))
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMove>().Halt();
            }
            List<Speech> dia = dialogue.diaBranch;  // rip out the dialogue from the db object

            msgBox.SetActive(true);                 // activate the messagebox

            if (dia[MBI].nameOverride != null)      // Put the desired speaker name in the nameplate
            {
                msgName.text = dia[MBI].nameOverride;
            }
            else
            {
                msgName.text = dia[MBI].name;
            }
            

            MTP = dia[MBI].type;                    // Set current message type
            curBranch = dia[MBI].branch;            // Current branch

            portrait.sprite = Resources.LoadAll<Sprite>("Portraits/" + dia[MBI].name)[dia[MBI].portraitID]; // Attach the proper pic

            msgText.text = dia[MBI].content.Substring(0, Mathf.RoundToInt(MSGInd)); // aesthetically start filling the text box

            if (displaying)
            {
                BufferOwner = dia[MBI].name;
            }
            
            if (MSGInd < dia[MBI].content.Length)       // so long as there's still something to print
            {

                MSGInd += Time.fixedDeltaTime * 30f;    // add just a bit to that message index

                if (MSGInd > dia[MBI].content.Length)   // we don't want to overflow
                {

                    MSGInd = dia[MBI].content.Length;

                }

                if (Input.GetKeyDown(KeyCode.X))        // if someone's impatient, let em skip
                {

                    msgText.text = dia[MBI].content;
                    MSGInd = dia[MBI].content.Length;   // this ain't your gramma's cave story
                }

            }
            else                                        // Otherwise if we're done printing
            {
                if (MTP > 0 && MTP < 5 && !chcBox.gameObject.activeSelf)
                {
                    RMBI = MBI + 1;

                    if (MTP == 3)
                    {
                        int ct = 0;
                        for (int i = 0; i < numChoices; i++)
                        {
                            if (choiceHistory[i])
                            {
                                ct += 1;
                            }
                            
                        }
                        if (ct < numChoices - 1)
                        {
                            RMBI = MBI;
                        }

                    }
                        

                    splitBranch = MTP == 4 ? curBranch : dia[MBI].optionbranch;

                    if (MTP == 1)
                    {
                        chcBox.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Yes";
                        chcBox.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "No";
                        selected = 0;
                        numChoices = 2;
                        choiceHistory = new bool[5];

                        OpenChoiceBox();
                    }
                    else
                    {
                        selected = 0;
                        numChoices = 2;
                        chcBox.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = dia[MBI].option1;
                        chcBox.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = dia[MBI].option2;

                        if (MTP != 3)
                        {
                            choiceHistory = new bool[5];
                        }

                        if (dia[MBI].option3 != null)
                        {
                            chcBox.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = dia[MBI].option3;
                            numChoices += 1;
                            if (dia[MBI].option4 != null)
                            {
                                chcBox.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = dia[MBI].option4;
                                numChoices += 1;
                                if (dia[MBI].option5 != null)
                                {
                                    chcBox.transform.GetChild(4).GetChild(0).GetComponent<Text>().text = dia[MBI].option5;
                                    numChoices += 1;
                                }
                            }
                        }

                        OpenChoiceBox();
                    }

                    chcBox.SetActive(true);
                }

                if (Input.GetKeyDown(KeyCode.X) && !chcBox.gameObject.activeSelf)   // If the user presses the confirm button and no choice presented
                {
                    if (MTP == 5)                 // Type 5 dialogues close the message box regardless of any other factor.
                    {
                        ClearBuffer();
                    }
                    else if (MBI >= dia.Count - 1)           // If we reach the end of the tree outright
                    {
                        if (curBranch == baseBranch)
                        {
                            ClearBuffer();
                        }
                        else
                        {
                            MBI = RMBI;
                            ClearMessageBox();
                        }
                    }
                    else if (dia[MBI + 1].branch != curBranch)    // Or if we reach the end of a branch
                    {
                        if (curBranch == baseBranch)            // If we reach the end of the main branch
                        {
                            ClearBuffer();
                        }
                        else                                    // If we reach the end of a sub branch
                        {
                            MBI = RMBI;
                            ClearMessageBox();
                        }
                    }
                    else
                    {
                        MBI += 1;
                        ClearMessageBox();
                    }

                }

            }

        }
        else
        {

            msgBox.SetActive(false);
            MSGInd = 0;
            MBI = 0;

        }

    }

    void ClearMessageBox()
    {
        msgText.text = "";
        MSGInd = 0;
    }

    // Load Dialogue into buffer
    public static void LoadIntoBuffer(string path)
    {
        dialogue = new SpeechContainer();
        dialogue = SpeechContainer.Load(path);

        if (dialogue.diaBranch.Count > 0)
        {
            baseBranch = dialogue.diaBranch[0].branch;
        }
    }

    public static void LoadIntoBuffer(SpeechContainer spc)
    {
        dialogue = new SpeechContainer();
        dialogue = spc;

        if (dialogue.diaBranch.Count > 0)
        {
            baseBranch = dialogue.diaBranch[0].branch;
        }
    }

    // Clear the Dialogue Buffer
    void ClearBuffer()
    {
        dialogue = new SpeechContainer();
        MBI = 0;
        MSGInd = 0;
        displaying = false;
        choiceHistory = new bool[5];
    }

    // Open the choice box
    void OpenChoiceBox()
    {
        chcBox.SetActive(!chcBox.gameObject.activeSelf);

        selected = 0;
        for (int i = 0; i < 5; i++)
        {
            if (!choiceHistory[i])
            {
                selected = i;
                i = 100;
            }
        }
    }

    // Update the choice box
    void UpdateChoiceBox()
    {

        for (int i = 0; i < 5; i++)
        {
            GameObject choice = chcBox.transform.GetChild(i).gameObject;
            choice.SetActive(i >= numChoices ? false : true);
            if (i < numChoices)
            {
                choice.GetComponent<Image>().color = i == selected ? new Color(0.5f,0.5f,0.5f) : 
                    (choiceHistory[i] ? new Color(0.3f, 0.3f, 0.3f) : new Color(1f, 1f, 1f));

                choice.gameObject.transform.GetChild(0).GetComponent<Text>().color = choiceHistory[i] ?
                    new Color(0.25f,0.25f,0.25f) : new Color(1f,1f,1f);
            }
        }

        int amtSelected = 0;
        foreach (bool a in choiceHistory)
        {
            amtSelected += a ? 1 : 0;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && amtSelected < numChoices - 1)
        {
            selected -= 1;
            if (selected < 0) selected = numChoices - 1;
            while (choiceHistory[selected])
            {
                selected -= 1;
                selected = selected < 0 ? numChoices - 1 : selected;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && amtSelected < numChoices - 1)
        {
            selected += 1;
            if (selected >= numChoices) selected = 0;
            while (choiceHistory[selected])
            {
                selected += 1;
                selected = selected >= 5 ? 0 : selected;
            }
        }


        RectTransform BoxBot = chcBox.transform.GetChild(6).GetComponent<RectTransform>();
        BoxBot.localPosition = new Vector2(0, 32 - numChoices*16);

        if (Input.GetKeyDown(KeyCode.X) && chcBox.activeSelf)
        {
            if (MTP == 2 || MTP == 1)
            {
                AnsBuffer = selected;
            }

            if (splitBranch == curBranch)
            {
                MBI += 1;
                ClearMessageBox();
            }
            else
            {
                List<Speech> dia = dialogue.diaBranch;
                string destination = splitBranch + selected;
                
                for(int i = 0; i < dia.Count; i++)
                {
                    if (destination.Equals(dia[i].branch))
                    {
                        MBI = i;
                        i = dia.Count;
                        ClearMessageBox();
                    }
                }
            }

            choiceHistory[selected] = true;
            AnsBuffer = selected;
            selected = 0;

            chcBox.SetActive(false);
        }
    }

    void DebugInputs()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            LoadIntoBuffer("Data/Dialogue/Opening/debugdg");
        }

        if (Input.GetKeyDown(KeyCode.L) && !chcBox.activeSelf)
        {
            LoadIntoBuffer("Data/Dialogue/Opening/debugyn");
        }
        if (Input.GetKeyDown(KeyCode.O) && !chcBox.activeSelf)
        {
            LoadIntoBuffer("Data/Dialogue/Opening/debugchc");
        }
        if (Input.GetKeyDown(KeyCode.P) && !chcBox.activeSelf)
        {
            LoadIntoBuffer("Data/Dialogue/Opening/debugrepchc");
        }
        if (Input.GetKeyDown(KeyCode.M) && !chcBox.activeSelf)
        {
            LoadIntoBuffer("Data/Dialogue/Opening/debugmchc");
        }

    }

}
