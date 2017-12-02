using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterStats : MonoBehaviour {

    public int maxHP;
    public string title;

    public int CurHP
    {
        get;
        set;
    }

    public bool React
    {
        get;
        set;
    }

    public bool ReactToMask
    {
        get;
        set;
    }

    public int damageTaken
    {
        get;
        set;
    }

    public Vector3 reactPos
    {
        get;
        set;
    }

    public bool isActive
    {
        get;
        set;
    }

    GameObject UIHealthBar;
    Image hpbar, hpghost;

	// Use this for initialization
	void Start () {
        isActive = false;
        React = false;

        UIHealthBar = GameObject.Find("UI").transform.GetChild(3).gameObject;
        UIHealthBar.GetComponent<Animator>().enabled = true;

        UIHealthBar.transform.GetChild(3).GetComponent<Text>().text = title;

        hpbar = UIHealthBar.transform.GetChild(2).GetComponent<Image>();
        hpghost = UIHealthBar.transform.GetChild(1).GetComponent<Image>();
    }

    private void Update()
    {
        if (isActive)
        {
            UIHealthBar.SetActive(true);

            if (UIHealthBar.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("HPBarIdle"))
            {
                UIHealthBar.GetComponent<Animator>().enabled = false;
            }
        }

        hpbar.fillAmount = Mathf.Lerp(hpbar.fillAmount, CurHP / (float)maxHP, Time.fixedDeltaTime * 5f);
        hpghost.fillAmount = Mathf.Lerp(hpghost.fillAmount, CurHP / (float)maxHP, Time.fixedDeltaTime * 2f);

        if (CurHP <= 0)
        {
            UIHealthBar.SetActive(false);
        }
    }

    public void Hit (int dmg, Vector3 pos, bool wasMask)
    {
        damageTaken = dmg;
        if (isActive)
        {
            CurHP -= dmg;
        }
        React = true;
        ReactToMask = wasMask;
        reactPos = pos;
        
    }

}
