using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoBlock : MonoBehaviour {

    GameObject ply;

    Sprite[] spr;

    // Use this for initialization
    void Start()
    {
        ply = GameObject.FindGameObjectWithTag("Player");

        spr = Resources.LoadAll<Sprite>("Spritesheets/attunement");

        transform.GetChild(0).GetComponent<Text>().text = PlayerStats.ValkCount.ToString();

        transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 0);
        transform.GetChild(2).GetComponent<Image>().color = new Color(1, 1, 1, 0);
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = ply.transform.position;

        transform.GetChild(0).GetComponent<Text>().text = PlayerStats.ValkCount.ToString();

        if (PlayerStats.MajorAttune >= 0 && PlayerStats.MinorAttune >= 0)
        {
            string maji = getIcon(PlayerStats.MajorAttune);
            string mini = getIcon(PlayerStats.MinorAttune);

            for (int i = 0; i < spr.Length; i++)
            {
                if (spr[i].name.Equals(maji))
                {
                    transform.GetChild(1).GetComponent<Image>().sprite = spr[i];
                    transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                }
                if (spr[i].name.Equals(mini))
                {
                    transform.GetChild(2).GetComponent<Image>().sprite = spr[i];
                    transform.GetChild(2).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                }
            }
        }

        if (!Input.GetKey(KeyCode.D))
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    string getIcon(int num)
    {
        string ico;
        switch (num)
        {
            default:
                ico = "";
                break;
            case 0:
                ico = "HeartIcon";
                break;
            case 1:
                ico = "SpadeIcon";
                break;
            case 2:
                ico = "ClubIcon";
                break;
            case 3:
                ico = "DiamondIcon";
                break;
        }
        return ico;
    }
}
