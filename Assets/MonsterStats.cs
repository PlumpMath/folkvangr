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

    public bool isActive
    {
        get;
        set;
    }

    GameObject UIHealthBar;

	// Use this for initialization
	void Start () {
        isActive = false;
        React = false;

        UIHealthBar = GameObject.Find("UI").transform.GetChild(3).gameObject;
        UIHealthBar.GetComponent<Animator>().enabled = true;

        UIHealthBar.transform.GetChild(3).GetComponent<Text>().text = title;
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

        Image hpbar = UIHealthBar.transform.GetChild(2).GetComponent<Image>();
        Image hpghost = UIHealthBar.transform.GetChild(1).GetComponent<Image>();

        hpbar.fillAmount = Mathf.Lerp(hpbar.fillAmount, CurHP / (float)maxHP, Time.fixedDeltaTime * 5f);
        hpghost.fillAmount = Mathf.Lerp(hpghost.fillAmount, CurHP / (float)maxHP, Time.fixedDeltaTime * 2f);

        if (CurHP <= 0)
        {
            UIHealthBar.SetActive(false);
        }
    }

    public void Hit (int dmg)
    {
        if (isActive)
        {
            CurHP -= dmg;
        }
        React = true;
    }

}
