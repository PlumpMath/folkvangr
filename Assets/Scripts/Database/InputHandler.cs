using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class InputHandler{

    [SerializeField]
    public List<Control> controls;  // list of ingame controls

    public InputHandler()
    {
        controls = new List<Control>();
        if (controls.Count <= 0)
        {
            SetDefaultControls();
        }
    }

    // function to add control to list
    public void AddControl(string name, KeyCode k, KeyCode kalt)
    {
        foreach (Control c in controls)
        {
            if (c.Name.Equals(name) && (c.InputMain == k && c.InputAlt == k))
            {
                return;
            }
        }
        controls.Add(new Control(name, k, kalt));
    }

    // remove a specific control from the controls list
    public void RemoveControl(string name)
    {
        foreach (Control c in controls)
        {
            if (c.Name.Equals(name))
            {
                controls.Remove(c);
                return;
            }
        }
    }

    public bool GetButtonDown(string name)
    {
        Control c = GetControl(name);
        return Input.GetKeyDown(c.InputMain) || Input.GetKeyDown(c.InputAlt);
    }

    public bool GetButton(string name)
    {
        Control c = GetControl(name);
        return Input.GetKey(c.InputMain) || Input.GetKey(c.InputAlt);
    }

    public bool GetButtonUp(string name)
    {
        Control c = GetControl(name);
        return Input.GetKeyUp(c.InputMain) || Input.GetKeyUp(c.InputAlt);
    }

    public Control GetControl(string name)
    {
        foreach (Control c in controls)
        {
            if (c.Name.Equals(name))
            {
                return c;
            }
        }
        return null;
    }

    // set default controls
    public void SetDefaultControls()
    {
        AddControl("Jump", KeyCode.X, KeyCode.Joystick1Button0);
        AddControl("Throw", KeyCode.C, KeyCode.Joystick1Button5);
        AddControl("Warp", KeyCode.A, KeyCode.Joystick1Button4);
        AddControl("Special", KeyCode.S, KeyCode.Joystick1Button2);
    }

    // save controls to a file
    public void SaveControls()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/ControlSet.dat", FileMode.Create);

        bf.Serialize(file, this);
        file.Close();
    }

    // load controls from a file
    public void LoadControls()
    {
        if (File.Exists(Application.persistentDataPath + "/ControlSet.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/ControlSet.dat", FileMode.Open);
            
            InputHandler ih = (InputHandler)bf.Deserialize(file);
            file.Close();

            controls = ih.controls;
        }
    }

}

// Class used for keeping track of game controls.
[System.Serializable]
public class Control
{
    public string Name  // Name of the Control
    {
        get;
        set;
    }

    public KeyCode InputMain    // Main input for the control
    {
        get;
        set;
    }

    public KeyCode InputAlt     // Alternate input for the control
    {
        get;
        set;
    }

    public Control(string nam, KeyCode inp, KeyCode altInp)
    {
        Name = nam;
        InputMain = inp;
        InputAlt = altInp;
    }
}