using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portobeyo : MonoBehaviour
{

    /*  Portobe-Yo, Ha!
     * Yoyoyo!
     * Even in the final release, I'll be number one, ha!
     * The first boss, yo!
     * 
     * The Strat:
     * I'mma chill in the dirt, yo.
     * You bug me? I'mma throw hands, yo.
     * 
     * Movelist:
     * Yo!
     * Shoot a bullet at the ghost every second
     * 
     * Ha!
     * Throw 3 spore bullets in the air. When they land, they will erupt into an upwards stream of bullets after 3 seconds.
     * 
     * Yoyoyo!
     * Shoot a bullet tri pattern at the ghost every half second
     * 
     * Hahahaha!
     * Throw 5 spore bullets.
     * 
     * Yo Ha!
     * Shoot spore bullets at the ghost. They will drop after going reaching the ghost's x position.
     * 
     */

    Vector3 startPos;

    MonsterStats sts;

    SpriteRenderer face, cap;

    Animator anim;

    int timeshit;

    bool BattleStart;

    float timer, timerDest;

    int Yow, Haw, Yoyow, Hahaw;

    // Use this for initialization
    void Start()
    {
        if (GetComponent<MonsterStats>())
        {
            sts = GetComponent<MonsterStats>();
        }
        else
        {
            sts = gameObject.AddComponent<MonsterStats>();
        }

        face = transform.GetChild(0).GetComponent<SpriteRenderer>();
        cap = transform.GetChild(1).GetComponent<SpriteRenderer>();

        anim = transform.GetComponent<Animator>();

        startPos = transform.position;

        sts.CurHP = sts.maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        if (sts.isActive)
        {
            if (Vector3.Distance(transform.position, startPos + Vector3.up*0.75f) >= 0.125f)
            {
                transform.position = Vector3.Lerp(transform.position, startPos + Vector3.up*0.75f, 10 * Time.fixedDeltaTime);
            }
            else if (!BattleStart)
            {
                transform.position = startPos + Vector3.up*0.75f;
                anim.Play("Yo");
                BattleStart = true;
                timerDest = 4f;
            }

            if (BattleStart)
            {
                timer += Time.fixedDeltaTime;
                if (timer > timerDest)
                {
                    timer = 0f;
                    timerDest = Mathf.Sqrt(Random.Range(16f, 25f));

                    int move = Random.Range(0, 100 + Haw + Yow);

                    if (move < 50 + Yow)
                    {
                        anim.Play("Yo");
                        Haw += 10 + Mathf.RoundToInt(Mathf.Sqrt(Haw));
                        Yow = 0;
                        GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>("Objects/Skills/LinearBullet"), transform.position, Quaternion.identity);
                        obj.GetComponent<LinearFire>().preTarget = transform.position + new Vector3(3, 3);
                        obj.GetComponent<LinearFire>().countdown = 0.66f;

                        obj = GameObject.Instantiate(Resources.Load<GameObject>("Objects/Skills/LinearBullet"), transform.position, Quaternion.identity);
                        obj.GetComponent<LinearFire>().preTarget = transform.position + new Vector3(2, 4);
                        obj.GetComponent<LinearFire>().countdown = 0.33f;

                        obj = GameObject.Instantiate(Resources.Load<GameObject>("Objects/Skills/LinearBullet"), transform.position, Quaternion.identity);
                        obj.GetComponent<LinearFire>().preTarget = transform.position + new Vector3(3, 1);
                        obj.GetComponent<LinearFire>().countdown = 1.33f;
                    }
                    else
                    {
                        anim.Play("Ha");
                        Yow += 10 + Mathf.RoundToInt(Mathf.Sqrt(Yow));
                        Haw = -25;

                        GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>("Objects/Skills/SporeBullet"), transform.position, Quaternion.identity);
                        obj.GetComponent<SporeBullet>().flyDelay = 0f;

                        obj = GameObject.Instantiate(Resources.Load<GameObject>("Objects/Skills/SporeBullet"), transform.position, Quaternion.identity);
                        obj.GetComponent<SporeBullet>().flyDelay = 1f;

                    }
                    
                }
            }
        }

        if (sts.React && sts.CurHP > 0)
        {
            if (sts.ReactToMask && GameObject.FindGameObjectWithTag("Mask"))
            {
                GameObject obj = GameObject.FindGameObjectWithTag("Mask");
                if (obj.GetComponent<MaskSpin>())
                {
                    obj.GetComponent<MaskSpin>().recall = true;
                }
            }

            face.color = new Color(1f, 0f, 0f, 0.5f);

            for (int i = 0; i < sts.damageTaken * PlayerStats.ValkMult; i++)
            {
                GameObject.Instantiate(Resources.Load<GameObject>("Objects/Valknut"), transform.position, Quaternion.identity);
            }

            sts.React = false;

            if (!sts.isActive)
            {
                timeshit += 1;
                transform.Translate(Vector3.down * 0.3f);
                if (timeshit > 3)
                {
                    sts.isActive = true;
                }
            }
            else
            {
                transform.position += (transform.position - sts.reactPos)/2;
            }

        }

        if (sts.CurHP <= 0 && sts.isActive)
        {
            anim.Play("Dying");
            face.color = new Color(1f, 0f, 0f, 0.5f);
            sts.isActive = false;
            timer = 0f;
        }

        if (sts.CurHP < sts.maxHP && sts.CurHP > 0)
        {
            sts.isActive = true;
        }


        if (face.color != new Color(1f, 1f, 1f, 1f))
        {
            face.color = Color.Lerp(face.color, new Color(1f, 1f, 1f, 1f), 10 * Time.fixedDeltaTime);
            cap.color = face.color;
        }

        if (sts.CurHP <= 0 && !sts.isActive)
        {
            if (GameObject.FindGameObjectWithTag("EnemyShot"))
            {
                foreach (GameObject g in GameObject.FindGameObjectsWithTag("EnemyShot"))
                {
                    GameObject.Instantiate(Resources.Load<GameObject>("Objects/Effects/SmokePuff"), g.transform.position, Quaternion.identity);
                    GameObject.Destroy(g);
                }
            }

            if (Vector3.Distance(transform.position, startPos + Vector3.down * 1.85f) > 0.125f)
            {
                transform.position = Vector3.Lerp(transform.position, startPos + Vector3.down * 1.85f, Time.fixedDeltaTime * 0.25f);
                transform.position = new Vector3(startPos.x + Mathf.Sin(timer) * 0.125f, transform.position.y, transform.position.z);
                timer += 100f * Time.fixedDeltaTime;

                GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>("Objects/Effects/SmokePuffTrail"), transform.position + new Vector3(Mathf.Sin(timer / 25f), Random.Range(-0.5f, 1f)), Quaternion.identity);
                obj.GetComponent<SpriteRenderer>().color = new Color(0.5f, Mathf.Sin(timer) * 0.5f, 0);
                face.color = new Color(Mathf.Sin(timer), 0f, 0f, 1f);
            }
            else
            {
                transform.position = transform.position + startPos + Vector3.down * 1.85f;
                GameObject.Destroy(this.gameObject);
            }
        }

    }

}
