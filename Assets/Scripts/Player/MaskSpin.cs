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

    GameObject MaskBlur, Valk;

    InputHandler controls;

    // Use this for initialization
    void Start()
    {
        controls = new InputHandler();

        plyr = GameObject.FindGameObjectWithTag("Player");
        float dx = Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.06125 ? Input.GetAxisRaw("Horizontal"):  plyr.GetComponent<PlayerMove>().facingRight ? 1f : -1f;

        arsc = GetComponent<AudioSource>();

        anim = GetComponent<Animator>();
        ahit = Resources.Load<AudioClip>("SFX/hit");

        if (Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0 && Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.06125f)
        {
            dx = 0;
        }

        transform.position = new Vector3(Mathf.Round(transform.position.x * 16f) / 16f, Mathf.Round(transform.position.y * 16f) / 16f);

        target = new Vector3(dx , Input.GetAxisRaw("Vertical") * 0.75f, 0);
        target = target / target.magnitude;

        RaycastHit2D[] rh = Physics2D.RaycastAll(transform.position, target * 0.75f, 1f);

        target = transform.position + target * 5f;
        stuck = false;
        recall = false;
        poof = false;

        timeOut = 0f;


        if (rh.Length > 0)
        {
            for (int i = 0; i < rh.Length; i++)
            {
                if (!rh[i].collider.transform.tag.Equals("Player"))
                {
                    GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
                    if (!poof)
                    {
                        arsc.PlayOneShot(ahit);
                        GameObject obj = GameObject.Instantiate(LevelManager.SmokePuff, transform.position, Quaternion.identity);
                        obj.GetComponent<SpriteRenderer>().sortingOrder = 4;
                        poof = true;
                    }
                    target = transform.position;

                    if (rh[i].collider.transform.tag.Equals("Block") || rh[i].collider.transform.tag.Equals("FreeBlock"))
                    {
                        stuck = true;
                        recall = true;
                        anim.enabled = false;
                    }
                    else
                    {
                        recall = true;
                    }
                }
            }
        }

        MaskBlur = Resources.Load<GameObject>("Objects/Effects/MaskBlur");
        Valk = Resources.Load<GameObject>("Objects/Valknut");

    }

    // Update is called once per frame
    void Update()
    {

        if (!recall)
        {
            RaycastHit2D[][] rh = { Physics2D.RaycastAll(transform.position + new Vector3(0, 0.5125f), Vector2.down, 1f),
                                    Physics2D.RaycastAll(transform.position + new Vector3(0, -0.5125f), Vector2.up, 1f),
                                    Physics2D.RaycastAll(transform.position + new Vector3(0.5125f, 0), Vector2.left, 1f),
                                    Physics2D.RaycastAll(transform.position + new Vector3(-0.5125f, 0), Vector2.right, 1f)};

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
                                GameObject obj = GameObject.Instantiate(LevelManager.SmokePuff, transform.position, Quaternion.identity);
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
                                recall = true;
                            }
                            else if (hitag.Equals("Card") && rh[i][j].collider.transform.gameObject.GetComponent<TuningCard>())
                            {
                                stuck = false;
                                recall = true;

                                GameObject card = rh[i][j].collider.gameObject;
                                int cardval = card.transform.GetComponent<TuningCard>().value;

                                if (PlayerStats.MajorAttune < 0f)
                                {
                                    PlayerStats.MajorAttune = cardval;
                                }
                                else
                                {
                                    PlayerStats.MinorAttune = cardval;
                                }
                                for (int k = 0; k < 32; k++)
                                {
                                    Valknut obj = GameObject.Instantiate(Valk, card.transform.position, Quaternion.identity).GetComponent<Valknut>();
                                    obj.transform.localScale *= 2;
                                    obj.currency = false;
                                    obj.target = card.transform.position + new Vector3(Mathf.Cos(2 * 3.14f * ((float)k)/32f), Mathf.Sin(2 * 3.14f * ((float)k)/32f)) * 3f;
                                    obj.val = cardval;
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

            transform.position = Vector3.Lerp(transform.position, target, PlayerStats.Speed * 1.5f * Time.fixedDeltaTime);



            if (controls.GetButtonUp("Throw"))
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
                    GameObject.Instantiate(MaskBlur, transform.position, Quaternion.identity);
                }

            }

        }

        if (anim.enabled && Vector3.Distance(target, transform.position) > 0.1f)
        {
            GameObject.Instantiate(MaskBlur, transform.position, Quaternion.identity);
        }

        if (Vector3.Distance(transform.position, plyr.transform.position) < 0.5f && recall)
        {
            GameObject.Destroy(this.gameObject);
        }

    }

}
