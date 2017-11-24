using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    public bool grounded, facingRight, jumping, recall, repositioning;

    float xvel, yvel;

    public int hp;

    Sprite[] spr;

    GameObject cam;
    Vector3 svel = Vector3.zero;

    Animator anim, transitionHandler;

    Rigidbody2D rb;

    public float flytime, outOfBoundTime;

    private float invuln;

    Vector3 spawn;

    // Use this for initialization
    void Start()
    {
        PlayerStats.SetUp();

        hp = 3;
        spawn = transform.position;

        facingRight = true;

        cam = GameObject.FindGameObjectWithTag("MainCamera");

        anim = GetComponent<Animator>();

        spr = Resources.LoadAll<Sprite>("Spritesheets/Char");

        if (GameObject.Find("UI"))
        {
            transitionHandler = GameObject.Find("UI").transform.GetChild(2).GetComponent<Animator>();
        }

        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        string maskname = hp >= 3 ? "Mask" : hp == 2 ? "Mask_Broken" : "Mask_Peril";

        for (int i = 0; i < spr.Length; i++)
        {
            if (spr[i].name.Equals(maskname))
            {
                transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = spr[i];
                i = spr.Length;
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            print("Major: " + PlayerStats.MajorAttune + "; Minor: " + PlayerStats.MinorAttune);
        }

        if (!repositioning)
        {
            if (!MessageBox.displaying)
            {
                if (GameObject.FindGameObjectWithTag("Mask") && GameObject.FindGameObjectWithTag("Mask").GetComponent<MaskSpin>())
                {
                    MaskSpin msk = GameObject.FindGameObjectWithTag("Mask").GetComponent<MaskSpin>();
                    transform.GetChild(0).gameObject.SetActive(false);

                    
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
                        recall = true;
                        flytime = 0;
                    }

                }
                else
                {
                    transform.GetChild(0).gameObject.SetActive(true);
                    if (Input.GetKeyDown(KeyCode.C))
                    {
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

                    if (grounded)
                    {
                        if (Input.GetKeyDown(KeyCode.X))
                        {
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
        }
        else
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
            invuln = 0f;
        }

        UpdateCamera();
    }

    private void LateUpdate()
    {
        if (xvel == 0)
        {
            transform.position = new Vector3((int)(transform.position.x*16)/16f, transform.position.y);
        }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (invuln <= 0)
        {
            if (collision.transform.tag == "EnemyShot")
            {
                hp--;
                invuln = 1.5f;
                yvel = 5f;
            }
        }
    }


    public bool IsGrounded()
    {
        return grounded;
    }

    public void Halt()
    {
        xvel = 0;
        yvel = 0;
    }

}
