using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillsTabController : MonoBehaviour {
    int tab;
    int markedSkill;

    Image[] tabImage = new Image[2];
    GameObject[] tabGroup = new GameObject[2];

    Image[] passiveSkillButtons = new Image[5];
    Image[] SkillButtons = new Image[2];

    GameObject skillInfoBar;
    public Text skillInfoBarTextData;

    public Sprite[] img;
    public Sprite[] skillButtonImage = new Sprite[2];
    PlayerController pcon;
    PlayerInfo pinfo;


    // Use this for initialization
    void Start () {

        pcon = GameObject.Find("Player").GetComponent<PlayerController>();
        pinfo = pcon.pinfo;

        tabImage[0] = gameObject.transform.GetChild(0).gameObject.GetComponent< Image>();
        tabImage[1] = gameObject.transform.GetChild(1).gameObject.GetComponent<Image>();
        tabGroup[0] = gameObject.transform.GetChild(4).gameObject;
        tabGroup[1] = gameObject.transform.GetChild(5).gameObject;


        skillInfoBar = gameObject.transform.GetChild(6).gameObject;
        skillInfoBarTextData = skillInfoBar.transform.GetChild(0).GetComponent<Text>();


        passiveSkillButtons[0] = tabGroup[0].transform.GetChild(1).gameObject.GetComponent<Image>();
        passiveSkillButtons[1] = tabGroup[0].transform.GetChild(2).gameObject.GetComponent<Image>();
        passiveSkillButtons[2] = tabGroup[0].transform.GetChild(3).gameObject.GetComponent<Image>();
        passiveSkillButtons[3] = tabGroup[0].transform.GetChild(4).gameObject.GetComponent<Image>();
        passiveSkillButtons[4] = tabGroup[0].transform.GetChild(5).gameObject.GetComponent<Image>();

        SkillButtons[0] = tabGroup[1].transform.GetChild(1).gameObject.gameObject.GetComponent<Image>(); ;
        SkillButtons[1] = tabGroup[1].transform.GetChild(2).gameObject.gameObject.GetComponent<Image>(); ;
        markedSkill = -1;
        
    }
	
	// Update is called once per frame
	void Update () {
        if(markedSkill != -1)
        {
            MarkSkill(markedSkill);
        }
	}


    string GetAbilityDescription(int slot)
    {
        int id = pinfo.GetAbilityID(slot);
        string desc = "";
        //Debug.Log("SLOT" + slot+"ID"+id);
        switch (id)
        {
            case 1: //frenzy
                Frenzy frenzy = (Frenzy)pinfo.abilities[slot];
                frenzy.CalculateMult();
                desc = frenzy.GetDescription();
                break;

            case 2: //rage
                Rage rage = (Rage)pinfo.abilities[slot];
                rage.CalculateMult();
                desc = rage.GetDescription();
                break;


            case 3: //Mine
                Mine mine = (Mine)pinfo.abilities[slot];
                mine.CalculateDamage();
                desc = mine.GetDescription();
                break;

            case 4: //Mine
                Fireball fireball = (Fireball)pinfo.abilities[slot];
                fireball.CalculateDamage();
                desc = fireball.GetDescription();
                break;
        }
        return desc;
    }

    public void ChangeTab(int tab)
    {
        if(this.tab != tab)
        {
            markedSkill = -1;
            SetDescription("");
            skillInfoBar.SetActive(false);
            UnmarkSkills();
        }

        switch(tab)
        {
            case 0:
                tabImage[tab].sprite = img[1];
                tabImage[1].sprite = img[0];
                tabGroup[tab].SetActive(true);
                tabGroup[1].SetActive(false);
                break;

            case 1:
                tabImage[tab].sprite = img[1];
                tabImage[0].sprite = img[0];
                tabGroup[tab].SetActive(true);
                tabGroup[0].SetActive(false);
                break;
        }

        this.tab = tab;
    }

    public void MarkSkill(int type)
    {
        //pinfo.skillPoints += 10;
        markedSkill = type;
        skillInfoBar.SetActive(true);

        UnmarkSkills();
        if (tab == 0)
        {
            passiveSkillButtons[type].sprite = skillButtonImage[1];
            SetDescription(pinfo.passiveSkills[type].description);
        }

        if (tab == 1)
        {
            SkillButtons[type].sprite = skillButtonImage[1];
            string desc = GetAbilityDescription(type);
            SetDescription(desc);
        }
    }
    public void UnmarkSkills()
    {
        for(int i = 0; i < passiveSkillButtons.Length; i++)
        {
            passiveSkillButtons[i].sprite = skillButtonImage[0];
        }

        for (int i = 0; i < SkillButtons.Length; i++)
        {
            SkillButtons[i].sprite = skillButtonImage[0];
        }
    }

    void SetDescription(string desc)
    {
        skillInfoBarTextData.text = desc;
    }

    public void IncreaseSkill()
    {
        if(pinfo.skillPoints > 0)
        {
            if (tab == 0)
            {
                pinfo.passiveSkills[markedSkill].RaiseLevel();
                pinfo.skillPoints -= 1;
                SetDescription(pinfo.passiveSkills[markedSkill].description);
            }

            if (tab == 1)
            {
                if (pinfo.skillPoints > 0)
                {
                    pinfo.LevelUpSkill(markedSkill);
                    pinfo.skillPoints -= 1;
                    SetDescription(pinfo.GetAbilityDescription(markedSkill));
                }
            }
        }       
    }
}
