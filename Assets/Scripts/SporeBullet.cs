using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SporeBullet : MonoBehaviour {

    public float timeToFall, flyDelay;

    bool fall, grounded, firing, puff;
    float fallx;

    float yvel;

    float timer;

    Vector3 startPos;

    Vector3 groundTarg;

	// Use this for initialization
	void Start () {
        fall = false;
        grounded = false;
        firing = false;
        puff = false;

        timer = 0;

        startPos = transform.position;
	}

    // Update is called once per frame
    void Update()
    {
        if (flyDelay > 0)
        {
            flyDelay -= Time.fixedDeltaTime;
        }
        else
        {
            if (fall)
            {
                if (grounded)
                {
                    if (Vector3.Distance(transform.position, groundTarg) >= 0.125f)
                    {
                        transform.position = Vector3.Lerp(transform.position, groundTarg, 5f * Time.fixedDeltaTime);
                    }
                    else
                    {
                        transform.position = groundTarg;

                        if (!firing)
                        {
                            timer += Time.fixedDeltaTime;
                            if (timer > 3f)
                            {
                                firing = true;
                            }
                            else if (timer > 2.5f && !puff)
                            {
                                GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>("Objects/Effects/RisingSmokePuff"), transform.position + Vector3.up, Quaternion.identity);
                                obj.GetComponent<SpriteRenderer>().sortingOrder = 4;
                                puff = true;
                            }
                        }
                        else
                        {
                            if (timer > 0)
                            {
                                GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>("Objects/Skills/MushBullet"), transform.position + new Vector3(Mathf.Sin(timer * 4) / 2, 0), Quaternion.identity);
                                obj.GetComponent<LinearBulletRaw>().xvel = Random.Range(-1f, 1f);
                                timer -= 2 * Time.fixedDeltaTime;
                            }
                            else
                            {
                                Destroy(this.gameObject);
                            }
                        }
                    }
                }
                else
                {
                    transform.position = new Vector3(fallx, transform.position.y) + new Vector3(2f * Mathf.Sin(timer), -Time.fixedDeltaTime);
                    timer += 2 * Time.fixedDeltaTime;

                    RaycastHit2D[][] rh = { Physics2D.RaycastAll(transform.position + new Vector3(0, 0.25f), Vector2.down, 0.5f),
                                    Physics2D.RaycastAll(transform.position + new Vector3(0, -0.25f), Vector2.up, 0.5f),
                                    Physics2D.RaycastAll(transform.position + new Vector3(0.25f, 0), Vector2.left, 0.5f),
                                    Physics2D.RaycastAll(transform.position + new Vector3(-0.25f, 0), Vector2.right, 0.5f)};

                    if (rh.Length > 0)
                    {
                        for (int i = 0; i < rh.Length; i++)
                        {
                            for (int j = 0; j < rh[i].Length; j++)
                            {

                                if (rh[i][j].collider.transform.tag == "Block")
                                {

                                    grounded = true;
                                    groundTarg = rh[i][j].collider.transform.position;

                                    timer = 0;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                transform.position += Vector3.up * Time.fixedDeltaTime * yvel;
                yvel += 0.5f;
                if (!GetComponent<SpriteRenderer>().isVisible && Vector3.Distance(startPos, transform.position) > 0.5f)
                {
                    timer += Time.fixedDeltaTime;
                    if (timer > timeToFall)
                    {
                        fall = true;
                        timer = 0;
                        if (GameObject.FindGameObjectWithTag("Player"))
                        {
                            transform.position = new Vector3(GameObject.FindGameObjectWithTag("Player").transform.position.x, GameObject.FindGameObjectWithTag("MainCamera").transform.position.y + 8);
                            fallx = transform.position.x;
                        }
                    }
                }
            }
        }
    }
}
