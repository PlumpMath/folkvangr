using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{

    GameObject plyr;

    float dx, dy;

    float timer;

    // Use this for initialization
    void Start()
    {
        timer = 0;
        plyr = GameObject.FindGameObjectWithTag("Player");

        dx = plyr.transform.localScale.x;
        dy = Random.Range(-4f, 4f);

        transform.localScale = new Vector3(dx, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.fixedDeltaTime;
        transform.position += (Vector3.right * dx * (PlayerStats.Speed * 2 + PlayerStats.Attack / 2) + Vector3.up * dy) * Time.fixedDeltaTime;

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

                    if (!rh[i][j].collider.transform.tag.Equals("Player") && !rh[i][j].collider.transform.tag.Equals("PlayerSkill"))
                    {

                        string hitag = rh[i][j].collider.transform.tag;


                        if (hitag.Equals("Monster") && rh[i][j].collider.transform.gameObject.GetComponent<MonsterStats>())
                        {
                            rh[i][j].collider.transform.gameObject.GetComponent<MonsterStats>().Hit(PlayerStats.Attack, transform.position, false);
                        }

                        GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>("Objects/Effects/RisingSmokePuff"), transform.position, Quaternion.identity);
                        obj.GetComponent<SpriteRenderer>().sortingOrder = 4;
                        GameObject.Destroy(this.transform.gameObject);

                    }
                }
            }
        }
        if (timer > 1f)
        {
            GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>("Objects/Effects/RisingSmokePuff"), transform.position, Quaternion.identity);
            obj.GetComponent<SpriteRenderer>().sortingOrder = 4;
            GameObject.Destroy(this.transform.gameObject);
        }
    }

}
