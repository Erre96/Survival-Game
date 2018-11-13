using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class Item
{
    public int value;
    public int level;
    public int itemType;
    public string name;
    public string mainAttr;
    public int itemDmg;
    public int itemArm;
    public int itemAP;
    public int hpInc;
    public int mpInc;
    public float lifeSteal;
    public float attSpdInc;

    public Item()
    {

    }

    public string GetMainAttribute()
    {
        return mainAttr;
    }

    public void SetStats(int itemType, int itemLevel, int value, string mainAttr)
    {
        this.value = value / 2;
        this.itemType = itemType;
        this.mainAttr = mainAttr;
        float f = 0;
        switch (itemType)
        {
            case 1:
                f = itemLevel * 2.25f;
                itemDmg = Mathf.RoundToInt(f);
                name = "Sword";
                break;

            case 2:
                f = itemLevel * 1.25f;
                itemArm = Mathf.RoundToInt(f);
                name = "Shield";
                break;

            case 3:
                f = itemLevel * 5f;
                itemAP = Mathf.RoundToInt(f);
                name = "Staff";
                break;

            case 4:
                hpInc = itemLevel * 8;
                name = "Orb of Constituion";
                break;

            case 5:
                mpInc = itemLevel * 4;
                name = "Orb of Energy";
                break;

            case 6:
                lifeSteal = itemLevel * 0.1f;
                name = "Orb of Life";
                break;

            case 7:
                attSpdInc = itemLevel * 0.1f;
                name = "Orb of Speed";
                break;
        }
        level = itemLevel;
    }

    public void SendOverStats(Item item)
    {
        itemType = item.itemType;
        level = item.level;
        name = item.name;
        mainAttr = item.mainAttr;
        value = item.value;

        itemDmg = item.itemDmg;
        itemArm = item.itemArm;
        itemAP = item.itemAP;
        hpInc = item.hpInc;
        mpInc = item.mpInc;
        lifeSteal = item.lifeSteal;
        attSpdInc = item.attSpdInc;
    }

    public void ResetItem()
    {
        itemType = 0;
        level = 0;
        name = null;
        mainAttr = null;
        value = 0;

        itemDmg = 0;
        itemArm = 0;
        itemAP = 0;
        hpInc = 0;
        mpInc = 0;
        lifeSteal = 0;
        attSpdInc = 0;
    }

}