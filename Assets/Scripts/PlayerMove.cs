using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    public bool grounded, facingRight, jumping, recall, repositioning;

    float xvel, yvel;

    public int hp;

    Sprite[] spr;

    AudioSource arsc;
    AudioClip ajmp, ash, abudu;

    GameObject cam;
    Vector3 svel = Vector3.zero;

    Animator anim, transitionHandler;

    Rigidbody2D rb;

    public float flytime, outOfBoundTime;

    public float invuln, reloadtime, reloadmax;

    Vector3 spawn;

    // Use this for initialization
    void Start()
    {
        PlayerStats.SetUp();
        reloadtime = 0;

        hp = 3;
        spawn = transform.position;

        facingRight = true;

        cam = GameObject.FindGameObjectWithTag("MainCamera");

        anim = GetComponent<Animator>();

        spr = Resources.LoadAll<Sprite>("Spritesheets/Char");

        arsc = GetComponent<AudioSource>();
        ajmp = Resources.Load<AudioClip>("SFX/jump");
        ash = Resources.Load<AudioClip>("SFX/shot");
        abudu = Resources.Load<AudioClip>("SFX/budubu");

        if (GameObject.FindGameObjectWithTag("GameController"))
        {
            transitionHandler = GameObject.FindGameObjectWithTag("GameController").transform.GetChild(2).GetComponent<Animator>();
        }

        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        string maskname = hp >= 3 ? "Mask" : hp == 2 ? "Mask_Broken" : "Mask_Peril";
        float reloadratio = reloadtime / reloadmax;

        for (int i = 0; i < spr.Length; i++)
        {
            if (spr[i].name.Equals(maskname))
            {
                transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = spr[i];
                i = spr.Length;
            }
        }

        if (!repositioning)
        {
            if (!MessageBox.displaying)
            {
                if (Input.GetKeyDown(KeyCode.D))
                {
                    GameObject.Instantiate(Resources.Load<GameObject>("UI/Info"), transform.position, Quaternion.identity);
                }

                if (GameObject.FindGameObjectWithTag("Mask") && GameObject.FindGameObjectWithTag("Mask").GetComponent<MaskSpin>())
                {
                    MaskSpin msk = GameObject.FindGameObjectWithTag("Mask").GetComponent<MaskSpin>();
                    transform.GetChild(0).gameObject.SetActive(false);

                    msk.transform.GetComponent<SpriteRenderer>().color = new Color(1 - reloadratio, 1 - reloadratio, 1f, 1f);
                    msk.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1 - reloadratio, 1 - reloadratio, 1f, 1f);

                    for (int i = 0; i < spr.Length; i++)
                    {
                        if (spr[i].name.Equals(maskname))
                        {
                            msk.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = spr[i];
                            i = spr.Length;
                        }
                    }


                    if (Input.GetKeyDown(KeyCode.A) && msk.recall == false)
                    {
                        arsc.clip = abudu;
                        arsc.Play();
                        recall = true;
                        flytime = 0;
                    }


                }
                else
                {
                    transform.GetChild(0).gameObject.SetActive(true);
                    transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1 - reloadratio, 1 - reloadratio, 1f, 1f);
                    if (Input.GetKeyDown(KeyCode.C))
                    {
                        arsc.clip = ash;
                        arsc.Play();
                        GameObject.Instantiate(Resources.Load<GameObject>("Objects/Skills/Mask"), transform.position, Quaternion.identity);

                    }
                    recall = false;
                }

                if (!recall)
                {
                    xvel = Input.GetAxis("Horizontal") * PlayerStats.Speed;
                    facingRight = xvel > 0 ? true : xvel < 0 ? false : facingRight;
                    transform.localScale = new Vector3(facingRight ? 1f : -1f, 1f, 1f);

                    if (Mathf.Abs(xvel) > 0)
                    {
                        anim.Play("Move");
                    }
                    else
                    {
                        anim.Play("Idle");
                    }

                    FireShot();

                    if (grounded)
                    {
                        if (Input.GetKeyDown(KeyCode.X))
                        {
                            arsc.clip = ajmp;
                            arsc.Play();
                            yvel = 10f;
                            jumping = true;
                        }

                    }
                    else
                    {

                        if (Input.GetKeyUp(KeyCode.X) && yvel > 0)
                        {
                            yvel = Mathf.Sqrt(yvel);
                        }
                    }
                }
                else
                {
                    if (GameObject.FindGameObjectWithTag("Mask") && GameObject.FindGameObjectWithTag("Mask").GetComponent<MaskSpin>())
                    {
                        MaskSpin msk = GameObject.FindGameObjectWithTag("Mask").GetComponent<MaskSpin>();

                        yvel = 0;
                        xvel = 0;

                        grounded = false;
                        jumping = false;

                        anim.Play("Recall");

                        GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>("Objects/Effects/SmokePuffTrail"), transform.position, Quaternion.identity);
                        obj.transform.localScale = transform.localScale;
                        

                        flytime += Time.fixedDeltaTime;

                        transform.position = Vector2.Lerp(transform.position, msk.transform.position, Time.fixedDeltaTime * 8f * (flytime < 0.5f ? PlayerStats.Speed/5f : (2f * PlayerStats.Speed * flytime * flytime) / 5f));
                        if (Vector2.Distance(msk.transform.position, transform.position) <= 0.25f)
                        {
                            recall = false;
                            msk.recall = true;
                        }
                    }

                }
                if (!GetComponent<SpriteRenderer>().isVisible)
                {
                    outOfBoundTime += Time.fixedDeltaTime;

                    if (transitionHandler)
                    {
                        if (outOfBoundTime > 3)
                        {
                            yvel = 0;
                            xvel = 0;
                            transitionHandler.Play("blackswipe");
                            if (GameObject.FindGameObjectWithTag("Mask"))
                            {
                                GameObject.Destroy(GameObject.FindGameObjectWithTag("Mask"));
                            }

                            recall = false;
                            grounded = false;
                            jumping = false;
                            facingRight = true;

                            repositioning = true;
                        }
                    }
                }
                else
                {
                    outOfBoundTime = 0;
                }
            }
        }
        else
        {
            outOfBoundTime -= Time.fixedDeltaTime;
            yvel = 0;
            xvel = 0;
            if (outOfBoundTime < 0)
            {
                outOfBoundTime = 0;
                transform.position = spawn;
                transitionHandler.Play("blackswipeout");
                repositioning = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!recall)
        {
            grounded = false;
            if (yvel <= 0f)
            {
                for (float i = -0.475f; i <= 0.475; i += 0.475f)
                {
                    RaycastHit2D rh = Physics2D.Raycast(new Vector2(transform.position.x + i, transform.position.y - 0.25f), -Vector2.up, 0.30f);
                    if (rh && !rh.collider.isTrigger)
                    {
                        if (!jumping)
                        {
                            grounded = true;
                        }

                        rb.MovePosition(new Vector2(transform.position.x, rh.point.y + 0.5f));

                        if (yvel < 0)
                        {
                            yvel = 0f;
                            jumping = false;
                        }
                    }
                }
            }

            if (!grounded)
            {
                yvel -= 9.8f * Time.fixedDeltaTime;
                for (float i = -0.475f; i <= 0.475; i += 0.475f)
                {
                    RaycastHit2D rh = Physics2D.Raycast(new Vector2(transform.position.x + i, transform.position.y + 0.25f), Vector2.up, 0.30f);
                    if (rh && !rh.collider.isTrigger && !rh.transform.gameObject.GetComponent<PlatformEffector2D>())
                    {

                        rb.MovePosition(new Vector2(transform.position.x, rh.point.y - 0.5f));

                        if (yvel > 0)
                        {
                            yvel = 0f;
                        }
                    }
                }
            }
        }

        rb.MovePosition(new Vector2(transform.position.x, transform.position.y) + new Vector2(xvel, yvel) * Time.fixedDeltaTime);


        if (Input.GetKeyDown(KeyCode.Minus))
        {
            hp--;
        }
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            hp++;
        }

        if (invuln > 0f)
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, Mathf.Sin(100 * invuln));
            invuln -= Time.fixedDeltaTime;

            if (PlayerStats.MajorAttune == 0)
            {
                PlayerStats.DeltaAttack = -1;
                PlayerStats.deltaSpeed = -2;
            }
        }
        else
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
            invuln = 0f;

            if (PlayerStats.MajorAttune == 0)
            {
                PlayerStats.DeltaAttack = 0;
                PlayerStats.deltaSpeed = 0;
            }
        }

        UpdateCamera();
    }

    void UpdateCamera()
    {

        int ratio = Screen.height / 180;
        ratio = ratio < 1 ? 1 : ratio;
        float OrthoSize = Screen.height / 16.0f / (ratio % 2 == 1 ? ratio + 1 : ratio) / 2.0f;
        Camera.main.orthographicSize = OrthoSize;

        if (!MessageBox.displaying)
        {
            float yinp = grounded ? Mathf.RoundToInt(Input.GetAxisRaw("Vertical")) : 0;
            Vector3 target = new Vector3(transform.position.x, transform.position.y + (grounded ? (yinp < 0 ? 0 : yinp > 0 ? 4 : 2) : 0), -10f);
            target.x = (int)(target.x / 0.125f) * 0.125f;
            target.y = (int)(target.y / 0.125f) * 0.125f;

            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, target, ref svel, 0.5f * (grounded ? 1f : 0.5f));

            LevelManager lvl = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>();

            if (cam.transform.position.y < lvl.bB)
            {
                cam.transform.position = new Vector3(cam.transform.position.x, lvl.bB, cam.transform.position.z);
            }
            if (cam.transform.position.x < lvl.lB)
            {
                cam.transform.position = new Vector3(lvl.lB, cam.transform.position.y, cam.transform.position.z);
            }
            if (cam.transform.position.y > lvl.tB)
            {
                cam.transform.position = new Vector3(cam.transform.position.x, lvl.tB, cam.transform.position.z);
            }
            if (cam.transform.position.x > lvl.rB)
            {
                cam.transform.position = new Vector3(lvl.rB, cam.transform.position.y, cam.transform.position.z);
            }

        }
    }

    void FireShot()
    {
        if (reloadtime > 0)
        {
            reloadtime -= Time.fixedDeltaTime;
        }
        else
        {
            reloadtime = 0;
        }

        if (Input.GetKey(KeyCode.S) && PlayerStats.MajorAttune >= 0 && reloadtime <= 0)
        {
            GameObject obj;
            GameObject spawn;

            Vector3 objspawnpos = GameObject.FindGameObjectWithTag("Mask") ? GameObject.FindGameObjectWithTag("Mask").transform.position : transform.position;

            switch (PlayerStats.MajorAttune)
            {
                default:
                    invuln = 5f;
                    break;
                case 1:
                    arsc.clip = ash;
                    arsc.Play();

                    spawn = GameObject.Instantiate(Resources.Load<GameObject>("Objects/Skills/Sword"), objspawnpos + Vector3.right * (facingRight?1:-1), Quaternion.identity);
                    spawn.transform.localScale = transform.localScale;
                    break;
                case 2:
                    arsc.clip = ash;
                    arsc.Play();
                    obj = GameObject.Instantiate(Resources.Load<GameObject>("Objects/Effects/SmokePuff"), objspawnpos + Vector3.right * (facingRight ? 0.5f : -0.5f), Quaternion.identity);
                    obj.GetComponent<SpriteRenderer>().sortingOrder = 5;
                    spawn = GameObject.Instantiate(Resources.Load<GameObject>("Objects/Skills/PlayerBullet"), objspawnpos + Vector3.right * (facingRight ? 1 : -1), Quaternion.identity);

                    if (!GameObject.FindGameObjectWithTag("Mask"))
                    {
                        yvel += 1.5f;
                        xvel += facingRight ? -5f : 5f;
                    }

                    break;
                case 3:
                    break;
            }
            reloadtime = reloadmax;
        }

        switch (PlayerStats.MajorAttune)
        {
            default:
                reloadmax = 10f;
                break;
            case 1:
                reloadmax = 0.25f;
                break;
            case 2:
                reloadmax = 0.15f;
                break;
            case 3:
                reloadmax = 5f;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (invuln <= 0)
        {
            if (collision.transform.tag == "EnemyShot" && transform.GetChild(0).gameObject.activeSelf)
            {
                HitByAttack();
            }
        }
    }

    public void HitByAttack()
    {
        if (invuln <= 0) { 
            hp--;
            invuln = 1.5f;  
            yvel = 5f;
        }
    }


    public bool IsGrounded()
    {
        return grounded;
    }

    public void Halt()
    {
        xvel = 0;

        jumping = false;
        recall = false;

        if (!GameObject.FindGameObjectWithTag("Mask"))
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }

            if (grounded)
        {
            yvel = 0;
        }
    }

}
