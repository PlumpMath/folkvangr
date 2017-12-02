using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TuningCard : MonoBehaviour {

    public int value;
    public bool isOffense;

    Animator anim;

    Vector3 startPos;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();

        startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		
        if (PlayerStats.MajorAttune < 0f && GetComponent<SpriteRenderer>().color.a <= 0.1f)
        {
            anim.Play("CardAppear");
            GetComponent<BoxCollider2D>().enabled = true;
            transform.position = startPos;
        }

        if (isOffense && (PlayerStats.MajorAttune == 1 || PlayerStats.MajorAttune == 2 || PlayerStats.MinorAttune == 1 || PlayerStats.MinorAttune == 2))
        {
            if (GetComponent<SpriteRenderer>().color.a == 1f) {
                anim.Play("CardDisappear");
            }
            GetComponent<BoxCollider2D>().enabled = false;
        }

        if (!isOffense && (PlayerStats.MajorAttune == 0 || PlayerStats.MajorAttune == 3 || PlayerStats.MinorAttune == 0 || PlayerStats.MinorAttune == 3))
        {
            if (GetComponent<SpriteRenderer>().color.a == 1f)
            {
                anim.Play("CardDisappear");
            }
            GetComponent<BoxCollider2D>().enabled = false;
        }

        if (PlayerStats.MajorAttune == value || PlayerStats.MinorAttune == value)
        {
            transform.position += Vector3.up * Time.fixedDeltaTime;
        }

    }
}
