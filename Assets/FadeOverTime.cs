using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOverTime : MonoBehaviour {

    public float lifetime = 1f;
    public float speed = 5f;

    float remaining;

	// Use this for initialization
	void Start () {
        remaining = lifetime;
	}
	
	// Update is called once per frame
	void Update () {
        remaining -= speed*Time.fixedDeltaTime;

        Color clr = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(clr.r, clr.g, clr.b, remaining / lifetime);
        transform.localScale = new Vector3(remaining / lifetime, remaining / lifetime, 1);

        if (remaining <= 0)
        {
            GameObject.Destroy(this.gameObject);
        }

	}
}
