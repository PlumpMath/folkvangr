using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSword : MonoBehaviour {

    GameObject plyr;

    static bool downslash;

    bool poof;

    // Use this for initialization
    void Start()
    {
        poof = false;
        plyr = GameObject.FindGameObjectWithTag("Player");
        transform.localScale = new Vector3(plyr.transform.localScale.x, downslash ? 1 : -1, 1);
        downslash = !downslash;
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit2D[][] rh = { Physics2D.RaycastAll(transform.position + new Vector3(0, 0.5125f), Vector2.down, 1.25f),
                                    Physics2D.RaycastAll(transform.position + new Vector3(0, -0.5125f), Vector2.up, 1.25f),
                                    Physics2D.RaycastAll(transform.position + new Vector3(0.5125f, 0), Vector2.left, 1.25f),
                                    Physics2D.RaycastAll(transform.position + new Vector3(-0.5125f, 0), Vector2.right, 1.25f)};

        if (rh.Length > 0)
        {
            for (int i = 0; i < rh.Length; i++)
            {
                for (int j = 0; j < rh[i].Length; j++)
                {

                    if (!rh[i][j].collider.transform.tag.Equals("Player") && !rh[i][j].collider.transform.tag.Equals("PlayerSkill"))
                    {

                        string hitag = rh[i][j].collider.transform.tag;


                        if (hitag.Equals("Monster") && rh[i][j].collider.transform.gameObject.GetComponent<MonsterStats>())
                        {
                            rh[i][j].collider.transform.gameObject.GetComponent<MonsterStats>().Hit(PlayerStats.Attack, transform.position, false);

                            if (!poof)
                            {
                                GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>("Objects/Effects/RisingSmokePuff"), transform.position, Quaternion.identity);
                                obj.GetComponent<SpriteRenderer>().sortingOrder = 4;
                                poof = true;
                            }
                            GameObject.Destroy(this.gameObject);
                        }

                    }
                }
            }
        }

        if (GetComponent<SpriteRenderer>().color.a <= 0f)
        {
            GameObject.Destroy(this.gameObject);
        }
	}
}
