using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class PassiveSkill
{
    public string name;
    public int level;
    public string affectedStat;
    public string description;
    public float mult;
    public float addPerLv;
    PlayerInfo pinfo;

    public PassiveSkill()
    {

    }

    public PassiveSkill(string name, int level, string affectedStat, float addPerLv)
    {
        this.name = name;
        this.level = level;
        this.affectedStat = affectedStat;
        this.addPerLv = addPerLv;
        SetMult();
    }

    public void SetPlayerInfo(PlayerInfo pinfo)
    {
        this.pinfo = pinfo;
    }
    public void SetDescription(float mult)
    {
        description = name + "      Lv " + level + "\n";
        description += "Increases " + affectedStat + " " + "with " + mult + " %";
        if(affectedStat == "Spell Power")
        {
            description += "\n" +"\n" + "(Does not apply to buff spells)";
        }
    }
    public void SetLevel(int level)
    {
        this.level = level;
        SetMult();
    }

    public void RaiseLevel()
    {
        level++;
        SetMult();
    }

    public void SetMult()
    {
        mult = 1 + (level * addPerLv);
        int a = Mathf.RoundToInt(((mult - 1) * 100));
        SetDescription(a);

        if(pinfo != null)
        {
            pinfo.CalculateAll();
        }
    }
}