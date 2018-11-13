using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataTextController : MonoBehaviour {
    public string data;
    Text text;
    public static PlayerInfo pinfo;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
        PlayerController pcon = GameObject.Find("Player").GetComponent<PlayerController>();
        pinfo = pcon.pinfo;
	}
	
	// Update is called once per frame
	void Update () {
        SetText();
	}

    void SetText()
    {
        switch(data)
        {
            case "gold":
                text.text = gi.gold.ToString();
                break;

            case "name":
                text.text = pinfo.name + "  Level : "+gi.wave;
                break;

            case "sp":
                text.text = "Skill Points : " +pinfo.skillPoints.ToString();
                break;

            case "hp":
                text.text = "Health : "+ pinfo.stats.hpCur+" / "+pinfo.stats.hp;
                break;

            case "mp":
                text.text = "Mana : " + pinfo.stats.mpCur + " / " + pinfo.stats.mp;
                break;

            case "dmg":
                text.text = "Damage : "+ pinfo.stats.minDmg+" - "+pinfo.stats.damage;
                break;

            case "def":
                text.text = "Armor : " + pinfo.stats.armor;
                break;

            case "idr":
                int reduc = Mathf.RoundToInt(pinfo.stats.incomingDmgReduc * 100);
                text.text = "Damage reduction : " + reduc+" %";
                break;

            case "ap":
                text.text = "Spell Power : " + pinfo.stats.spellPower;
                break;

            case "abil0":
                text.text = pinfo.GetAbilityName(0);
                break;

            case "abil1":
                text.text = pinfo.GetAbilityName(1);
                break;

            case "abil0Lv":
                pinfo.UpdateAbilityData(0);
                text.text = pinfo.GetAbilityLevelAsString(0);
                break;

            case "abil1Lv":
                pinfo.UpdateAbilityData(1);
                text.text = pinfo.GetAbilityLevelAsString(1);
                break;

            case "attLv":
                text.text = pinfo.passiveSkills[0].level.ToString();
                break;

            case "defLv":
                text.text = pinfo.passiveSkills[1].level.ToString();
                break;

            case "apLv":
                text.text = pinfo.passiveSkills[2].level.ToString();
                break;

            case "vitLv":
                text.text = pinfo.passiveSkills[3].level.ToString();
                break;

            case "energyLv":
                text.text = pinfo.passiveSkills[4].level.ToString();
                break;

            case "as":
                string showThis = pinfo.stats.attackSpeed.ToString();
                if(showThis.Length > 3)
                {
                    showThis = showThis[0].ToString() + showThis[1].ToString() + showThis[2].ToString()+ showThis[3].ToString();
                }
                text.text ="Attack Cooldown : "+ showThis+" sec";
                break;

            case "ls":
                text.text = "Life Steal : "+ pinfo.invStats.lifeSteal.ToString()+" %";
                break;
        }
    }
    void shit() { }
}
