using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBlock : MonoBehaviour {

    Sprite[] spr;

    GameObject gcr;

    bool poof;

	// Use this for initialization
	void Start () {
        spr = Resources.LoadAll<Sprite>("Spritesheets/Field");

        gcr = GameObject.FindGameObjectWithTag("GameController");
    }
	
	// Update is called once per frame
	void Update () {

        if (Application.isPlaying)
        {
            string str = "EventEmpt";
            if (gcr && gcr.GetComponent<LevelManager>())
            {
                bool blkEvent = gcr.GetComponent<LevelManager>().blkEvent;
                str = blkEvent ? "EventFilled" : "EventEmpt";
                GetComponent<BoxCollider2D>().enabled = blkEvent;

                if (blkEvent && !poof)
                {
                    GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>("Objects/Effects/RisingSmokePuff"), transform.position, Quaternion.identity);
                    obj.GetComponent<SpriteRenderer>().sortingOrder = 4;
                    poof = true;
                }
                if (poof && !blkEvent)
                {
                    poof = false;
                }
            }

            for (int i = 0; i < spr.Length; i++)
            {
                if (spr[i].name.Equals(str))
                {
                    GetComponent<SpriteRenderer>().sprite = spr[i];
                    i = spr.Length;
                }
            }
        }

    }
}
