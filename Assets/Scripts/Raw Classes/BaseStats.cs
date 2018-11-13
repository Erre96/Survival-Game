using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public abstract class Stats {
    public int hp;
    public int mp;

    public int damage;
    public int armor;
    public int spellPower;

    public int movementSpeed;
    public float attackSpeed;
}

[Serializable]
public class CompleteStats : Stats
{
    public TimerEC attackCooldown;
    int weapon;
    public float dmgMult;
    public float range;

    public int minDmg;

    public int hpCur;
    public int mpCur;

    public float incomingDmgReduc;

    BaseStats baseStats;
    EquipmentStats invStats;
    PassiveSkill[] passiveSkills;


    public void SetWeapon(int weapon)
    {
        this.weapon = weapon;
    }

    public int GetWeapon()
    {
        return weapon;
    }
    public CompleteStats()
    {

    }

    public CompleteStats(BaseStats baseStats, EquipmentStats invStats, PassiveSkill[] passiveSkills)
    {
        this.baseStats = baseStats;
        this.invStats = invStats;
        this.passiveSkills = passiveSkills;
        SetClassAttributes();
        attackCooldown = new TimerEC(attackSpeed);
    }

    public void DecreaseMana(int mpCost)
    {
        mpCur -= mpCost;
    }

    //Uses the choises you made during character creation to set some base values
    public void SetClassAttributes() 
    {
        if (baseStats.type == 1)
        {
            weapon = gi.GetWeaponChoise();
            SetClassAttributes2(weapon);
        }

        if (baseStats.type == 2)
        {
            System.Random rnd = new System.Random();
            weapon = rnd.Next(1,7);
            SetClassAttributes2(weapon);
            dmgMult /= 2.25f;
        }
    }

    public void SetClassAttributes2(int weapon)
    {
         switch (weapon)
            {
                case 0:
                    dmgMult = 0.8f; baseStats.attackSpeed = 0.5f; range = 1.45f; baseStats.specType = "Melee"; break;

                case 1:
                    dmgMult = 3.2f; baseStats.attackSpeed = 2; range = 1.45f; baseStats.specType = "Melee"; break; //long sword

                case 2:
                    dmgMult = 1.6f; baseStats.attackSpeed = 1; range = 1.45f; baseStats.specType = "Melee"; break; //dagger

                case 3:
                    dmgMult = 0.8f; baseStats.attackSpeed = 1; range = 3f; baseStats.specType = "Range"; break; //Throwing knifes

                case 4:
                    dmgMult = 2.4f; baseStats.attackSpeed = 2.1f; range = 4f; baseStats.specType = "Range"; break; //Javelin

                case 5:
                    dmgMult = 1.25f; baseStats.attackSpeed = 1.8f; range = 5f; baseStats.specType = "Range"; break; //Short Bow

                case 6:
                    dmgMult = 0.8f; baseStats.attackSpeed = 0.5f; range = 1.45f; baseStats.specType = "Melee"; break; //Unarmed
            }
    }

    public int GetRandomDamage()
    {
        System.Random rnd = new System.Random();
        int dmg = rnd.Next(minDmg, damage + 1);
        return dmg;
    }

    public void CalculateAttackSpeed(float mult)
    {
        attackSpeed = (baseStats.attackSpeed / (1 + (invStats.attackSpeed/100))) / mult;
        attackCooldown.SetNewMaxTimerInSeconds(attackSpeed);
    }
    public void CalculateArmor()
    {

        float f = (baseStats.armor + invStats.armor) * passiveSkills[1].mult;
        armor = Mathf.RoundToInt(f);
        incomingDmgReduc = ((armor) * 0.002f) / (1 + 0.002f * (armor));
    }

    public void CalculateDamage(float mult)
    {
        float holder = (((baseStats.damage + (invStats.damage)) * dmgMult) * passiveSkills[0].mult) * mult; //last mult is for buff
        damage = Mathf.RoundToInt(holder);

        float holder2 = damage * 0.6f;
        minDmg = Mathf.RoundToInt(holder2);
    }

    public void CalculateHealth()
    {
        float f = (baseStats.hp) * passiveSkills[3].mult + invStats.hp;
        hp = Mathf.RoundToInt(f);

        if (gi.canContinue)
        {
            RestoreHealth();
        }
    }
    public void RestoreHealth()
    {
        hpCur = hp;
    }
    public void CalculateMana()
    {
        float f = (baseStats.mp + invStats.mp) * passiveSkills[4].mult;
        mp = Mathf.RoundToInt(f);
        mpCur = mp;
    }

    public void CalculateSpellPower()
    {
        float holder = ((baseStats.spellPower) + (invStats.spellPower)) * passiveSkills[2].mult;
        spellPower = Mathf.RoundToInt(holder);

    }
}

[Serializable]
public class EquipmentStats : Stats
{
    public float lifeSteal;

    private Item[] items;

    public EquipmentStats()
    {

    }

    public EquipmentStats(Item[] items)
    {
        this.items = items;
        CalculateItemstats();
    }

    //For npc
    public void SetRandomItemStats()
    {
        if(gi.wave >= 7)
        {
            System.Random random = new System.Random();
            int itemDmg = random.Next(gi.wave * 3, gi.wave * 6);
            int itemHP = random.Next(gi.wave * 7, gi.wave * 14);
            int itemArm = random.Next(gi.wave * 4, gi.wave * 8);

            damage = itemDmg;
            armor = itemArm;
            hp = itemHP;
        }
    }

    public void CalculateItemstats()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (i == 0)
            {
                spellPower = 0; armor = 0; damage = 0; hp = 0; mp = 0; lifeSteal = 0; attackSpeed = 0;
            }

            spellPower += items[i].itemAP;
            armor += items[i].itemArm;
            damage += items[i].itemDmg;
            hp += items[i].hpInc;
            mp += items[i].mpInc;
            lifeSteal += items[i].lifeSteal;
            attackSpeed += items[i].attSpdInc;
        }

    }
}

[Serializable]
public class BaseStats : Stats
{
    public int type;
    public int spec;
    public string specType;

    int hpPerLv;
    int mpPerLv;
    int dmgPerLv;
    int armPerLv;
    int spPerLv;

    public float stepSpeed;
    int bonusHp;

    public BaseStats()
    {

    }

    public BaseStats(int type)
    {
        this.type = type;

        if(type == 1)
        {
            spec = 1;
            bonusHp = 400;
        }

        if (type == 2)
        {
            while (true)
            {
                spec = UnityEngine.Random.Range(1, 5);
                if(spec != 1)
                {
                    break;
                }
            }
        }

        SetSpecialization();
        CalculateStats();
    }

    public void CalculateStats()
    {
        hp = 50 + (gi.wave * hpPerLv) + bonusHp;
        mp = 20 + (gi.wave * mpPerLv);
        damage = 5 + (gi.wave * dmgPerLv);
        armor = 2 + (gi.wave * armPerLv);
        spellPower = 5 + (gi.wave * spPerLv);
        CalculateStepSpeed();
    }

    void SetSpecialization()
    {
        switch(spec)
        {
            case 1: //players specialization
                hpPerLv = 15;
                mpPerLv = 15;
                dmgPerLv = 5;
                armPerLv = 5;
                spPerLv = 5;
                movementSpeed = 130 * MapDataController.mult;
                break;

            case 2: //tank spec
                hpPerLv = 25;
                mpPerLv = 0;
                dmgPerLv = 1;
                armPerLv = 5;
                spPerLv = 0;
                movementSpeed = 60  * MapDataController.mult;
                break;

            case 3: //warrior spec
                hpPerLv = 10;
                mpPerLv = 0;
                dmgPerLv = 3;
                armPerLv = 2;
                spPerLv = 0;
                movementSpeed = 80 * MapDataController.mult;
                break;

            case 4: //rouge spec
                hpPerLv = 6;
                mpPerLv = 0;
                dmgPerLv = 6;
                armPerLv = 1;
                spPerLv = 0;
                movementSpeed = 100 * MapDataController.mult;
                break;
        }
    }

    void CalculateStepSpeed()
    {
        stepSpeed = movementSpeed  / 1500f;
    }
}
