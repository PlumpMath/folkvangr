using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Valknut : MonoBehaviour {

    GameObject plyr;
    Vector3 target;

    public bool recall;

    float timeOut;


    // Use this for initialization
    void Start()
    {
        plyr = GameObject.FindGameObjectWithTag("Player");
        float dx = Random.Range(-1f,1f);
        float dy = Random.Range(-1f, 1f);

        target = transform.position + new Vector3(dx * 5, dy * 5, 0);

        recall = false;

        timeOut = 0.5f;
        
    }

    // Update is called once per frame
    void Update () {
        transform.Rotate(0,0,360*Time.fixedDeltaTime);

        GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>("Objects/Effects/ValkFade"), transform.position, transform.rotation);
        obj.transform.localScale = transform.localScale;

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
                PlayerStats.ValkCount += 1;
                GameObject.Destroy(this.gameObject);
            }
        }
        
		
	}
}
