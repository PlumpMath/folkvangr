using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskSpin : MonoBehaviour
{

    GameObject plyr;
    Vector3 target;

    AudioSource arsc;
    AudioClip ahit;

    Animator anim;

    public bool recall, stuck, poof;

    float timeOut;

    // Use this for initialization
    void Start()
    {
        plyr = GameObject.FindGameObjectWithTag("Player");
        float dx = plyr.GetComponent<PlayerMove>().facingRight ? 1f : -1f;

        arsc = GetComponent<AudioSource>();

        anim = GetComponent<Animator>();
        ahit = Resources.Load<AudioClip>("SFX/hit");

        if (Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0 && Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.125f)
        {
            dx = 0;
        }

        target = transform.position + new Vector3(dx * (PlayerStats.Speed + PlayerStats.deltaSpeed), Input.GetAxisRaw("Vertical") * (PlayerStats.Speed + PlayerStats.deltaSpeed)* 0.75f, 0);

        stuck = false;
        recall = false;
        poof = false;

        timeOut = 0f;
    }

    // Update is called once per frame
    void Update()
    {

        if (!recall)
        {
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

                        if (!rh[i][j].collider.transform.tag.Equals("Player"))
                        {
                            if (!poof)
                            {
                                arsc.PlayOneShot(ahit);
                                GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>("Objects/Effects/SmokePuff"), transform.position, Quaternion.identity);
                                obj.GetComponent<SpriteRenderer>().sortingOrder = 4;
                                poof = true;
                            }

                            string hitag = rh[i][j].collider.transform.tag;


                            if (hitag.Equals("Block") || hitag.Equals("FreeBlock"))
                            {
                                target = transform.position;
                                anim.enabled = false;
                                stuck = true;
                            }
                            else if (hitag.Equals("Monster") && rh[i][j].collider.transform.gameObject.GetComponent<MonsterStats>())
                            {
                                rh[i][j].collider.transform.gameObject.GetComponent<MonsterStats>().Hit(Mathf.RoundToInt(PlayerStats.Attack*1.5f + PlayerStats.DeltaAttack), transform.position, true);
                            }
                            else if (hitag.Equals("Card") && rh[i][j].collider.transform.gameObject.GetComponent<TuningCard>())
                            {
                                stuck = false;
                                recall = true;

                                if (PlayerStats.MajorAttune < 0f)
                                {
                                    PlayerStats.MajorAttune = rh[i][j].collider.transform.GetComponent<TuningCard>().value;
                                }
                                else
                                {
                                    PlayerStats.MinorAttune = rh[i][j].collider.transform.GetComponent<TuningCard>().value;
                                    
                                }
                            }
                            else if (hitag.Equals("PlayerSkill"))
                            {
                                if (!rh[i][j].collider.transform.GetComponent<PlayerSword>())
                                {
                                    stuck = false;
                                    recall = true;
                                }
                            }
                            else if (hitag.Equals("EnemyShot"))
                            {
                                plyr.GetComponent<PlayerMove>().HitByAttack();
                                recall = true;
                            }
                            else
                            {
                                stuck = false;
                                recall = true;
                            }

                            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
                        }
                    }
                }
            }

            transform.position = Vector3.Lerp(transform.position, target, Time.fixedDeltaTime * 5f);



            if (Input.GetKeyUp(KeyCode.C))
            {
                recall = true;
            }
        }
        else
        {
            if (!plyr.GetComponent<PlayerMove>().recall)
            {
                if (!anim.enabled)
                {
                    anim.enabled = true;
                    GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
                }
                timeOut += Time.fixedDeltaTime;

                transform.position = Vector3.Lerp(transform.position, plyr.transform.position, Time.fixedDeltaTime * 8f * (timeOut < 0.5f ? 1f : 2*timeOut*timeOut));

                if (timeOut > 10f)
                {
                    transform.position = plyr.transform.position;
                    GameObject.Instantiate(Resources.Load<GameObject>("Objects/Effects/MaskBlur"), transform.position, Quaternion.identity);
                }

            }

        }

        if (anim.enabled && Vector3.Distance(target, transform.position) > 0.1f)
        {
            GameObject.Instantiate(Resources.Load<GameObject>("Objects/Effects/MaskBlur"), transform.position, Quaternion.identity);
        }

        if (Vector3.Distance(transform.position, plyr.transform.position) < 0.5f && recall)
        {
            GameObject.Destroy(this.gameObject);
        }

    }

}
