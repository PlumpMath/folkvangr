using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearFire : MonoBehaviour
{

    Vector3 vel, prevel;

    public Vector3 preTarget;

    bool fire;

    public float countdown;

    float killTimer;

    private void Start()
    {
        killTimer = 20f;
    }

    private void LateUpdate()
    {


        if (Vector3.Distance(transform.position, preTarget) < 0.25f && !fire)
        {
            fire = true;
            Vector3 target;
            if (GameObject.FindGameObjectWithTag("Player"))
            {
                target = GameObject.FindGameObjectWithTag("Player").transform.position;
            }
            else
            {
                target = Vector3.zero;
            }

            vel = target - transform.position;

        }

        if (killTimer <= 0f)
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (fire)
        {
            if (countdown <= 0)
            {
                transform.position += vel * Time.fixedDeltaTime;
                transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(vel.y, vel.x) * 180 / Mathf.PI);

                killTimer -= Time.fixedDeltaTime;
            }
            else
            {
                countdown -= Time.fixedDeltaTime;
            }
        }
        else
        {
            Vector3 preVel = preTarget - transform.position;
            transform.position += preVel * Time.fixedDeltaTime;
            transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(preVel.y, preVel.x) * 180 / Mathf.PI);
        }

    }
}
