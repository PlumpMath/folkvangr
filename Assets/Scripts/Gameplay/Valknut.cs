using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Valknut : MonoBehaviour {

    GameObject plyr;
    SpriteRenderer spr;

    public Vector3 target;

    public int val;

    public bool recall;
    public bool currency;

    float timeOut;

    GameObject fadeSprite;

    // Use this for initialization
    void Start()
    {
        plyr = GameObject.FindGameObjectWithTag("Player");

        if (target.Equals(Vector3.zero))
        {
            float dx = Random.Range(-1f, 1f);
            float dy = Random.Range(-1f, 1f);

            target = transform.position + new Vector3(dx * 5, dy * 5, 0);
        }

        recall = false;

        fadeSprite = Resources.Load<GameObject>("Objects/Effects/ValkFade");
        spr = GetComponent<SpriteRenderer>();

        if (!currency)
        {
            int truVal = val % 4;
            Sprite[] icSpr = Resources.LoadAll<Sprite>("Spritesheets/attunement");
            string ico = InfoBlock.getIcon(truVal);

            for (int i = 0; i < icSpr.Length; i++)
            {
                if (icSpr[i].name.Equals(ico))
                {
                    spr.sprite = icSpr[i];
                }
            }
        }

        timeOut = 0.5f;
        
    }

    // Update is called once per frame
    void Update () {
        transform.Rotate(0,0,360*Time.fixedDeltaTime);

        GameObject obj = GameObject.Instantiate(fadeSprite, transform.position, transform.rotation);
        obj.transform.localScale = transform.localScale;
        obj.GetComponent<SpriteRenderer>().sprite = spr.sprite;

        if (recall)
        {
            target = plyr.transform.position;
            timeOut += 2 * Time.fixedDeltaTime;
        }
        transform.position = Vector3.Lerp(transform.position, target, Time.fixedDeltaTime * 5 * (recall ? timeOut : 1f));

        if (transform.localScale.x > 0.1)
        {
            transform.localScale = transform.localScale - transform.localScale * Time.fixedDeltaTime;
        }


        if (Vector3.Distance(transform.position,target) < 0.5f)
        {
            if (!recall)
            {
                timeOut -= Time.fixedDeltaTime;
                if (timeOut <= 0)
                {
                    timeOut = 1;
                    recall = true;
                }
            }
            else
            {
                if (currency)
                {
                    PlayerStats.ValkCount += 1;
                }
                GameObject.Destroy(this.gameObject);
            }
        }
        
		
	}
}
