using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyBlock : MonoBehaviour {


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


                        if (hitag.Equals("EnemyShot"))
                        {

                            GameObject obj = GameObject.Instantiate(LevelManager.RisingSmokePuff, transform.position, Quaternion.identity);
                            obj.GetComponent<SpriteRenderer>().sortingOrder = 4;

                            for (int k = 0; k < 2*PlayerStats.Attack*PlayerStats.ValkMult; k++)
                            {
                                GameObject.Instantiate(Resources.Load<GameObject>("Objects/Valknut"), transform.position, Quaternion.identity);
                            }

                            GameObject.Destroy(rh[i][j].collider.gameObject);

                            GameObject.Destroy(this.gameObject);
                            
                            return;
                        }

                    }
                }
            }
        }
    }
}
