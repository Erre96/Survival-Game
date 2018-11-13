using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class PlayerInfo
{
    public int skillPoints;
    private bool attacking;
    AssetsLibrary assetsLib;
    private int team;

    public Item[] items = new Item[6];

    //ability skills
    public Ability[] abilities = new Ability[2];

    //passive skills
    public PassiveSkill[] passiveSkills = new PassiveSkill[5];

    //Stat classes
    public BaseStats baseStats;
    public EquipmentStats invStats;
    public CompleteStats stats;

    //status effects
    public StatusEffects buffs = new StatusEffects();
    public StatusEffects nerfs = new StatusEffects();

    //public Stats fullStats;
    public string dir;
    public string name;

    //pots
    private int healingPots;
    private int manaPots;

    public HealthbarController hpCon;

    private Vector2Int pos;


    public PlayerInfo(int team, HealthbarController hpCon)
    {
        if(team == 1)
        {
            if (DataTransferManager.gameLoaded)
            {
                for (int i = 0; i < DataTransferManager.inv.items.Length; i++)
                {
                    items[i] = DataTransferManager.inv.items[i];
                    skillPoints = DataTransferManager.dataHolder.skillPoints;
                }
            }
            if (!DataTransferManager.gameLoaded)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    items[i] = new Item();
                }
            }
        }

        if(team == 2)
        {
            for(int i = 0; i < items.Length; i++)
            {
                items[i] = new Item();
            }
        }

        InstantiateAbilities();
        this.team = team;
        this.hpCon = hpCon;
        this.name = DataTransferManager.dataHolder.name;
        baseStats = new BaseStats(team);
        PlayerInfo pinfo = this;
        hpCon.SetPlayerInfoRef(pinfo);

        InstantiatePassiveSkills();
        invStats = new EquipmentStats(items);
        stats = new CompleteStats(baseStats, invStats, passiveSkills);
        SetStartingStats();
        InstantiateAbilities();
        AddHealingPots(5);
        AddManaPots(5);
        stats.RestoreHealth();
    }

    public int GetHealingPots()
    {
        return healingPots;
    }

    public void AddHealingPots(int added)
    {
        healingPots += added;
    }

    public int GetManaPots()
    {
        return manaPots;
    }

    public void AddManaPots(int added)
    {
        manaPots += added;
    }

    public void SetDir(string dir)
    {
        this.dir = dir;
    }

    public string GetDir()
    {
        return dir;
    }

    public int GetAbilityID(int slot)
    {
        int id = abilities[slot].GetID();
        return id;
    }

    public void InstantiateAbilities()
    {
        for(int i = 0; i < DataTransferManager.dataHolder.abilId.Length; i++)
        {
            switch(DataTransferManager.dataHolder.abilId[i])
            {
                case 1:
                    abilities[i] = new Frenzy(i,this, DataTransferManager.dataHolder.abilLv[i]);
                    break;

                case 2:
                    abilities[i] = new Rage(i,this, DataTransferManager.dataHolder.abilLv[i]);
                    break;

                case 3:
                    abilities[i] = new Mine(i, this, DataTransferManager.dataHolder.abilLv[i]);
                    break;

                case 4:
                    abilities[i] = new Fireball(i, this, DataTransferManager.dataHolder.abilLv[i]);
                    break;
            }
        }
    }

    public void UpdateAbilityData(int slot)
    {
        int id = abilities[slot].GetID();
        switch (id)
        {
            case 1:
                Frenzy frenzy = (Frenzy)abilities[slot];
                frenzy.CalculateMult();
                break;

            case 2:
                Rage rage = (Rage)abilities[slot];
                rage.CalculateMult();
                break;

            case 3:
                Mine mine = (Mine)abilities[slot];
                mine.CalculateDamage();
                break;

            case 4:
                Fireball fireball = (Fireball)abilities[slot];
                fireball.CalculateDamage();
                break;
        }
    }
    
    public void LevelUpSkill(int slot)
    {
        abilities[slot].UpgradeLevel();
        UpdateAbilityData(slot);
    }

    public string GetAbilityDescription(int slot)
    {
        string ret = abilities[slot].GetDescription();
        return ret;
    }

    public string GetAbilityName(int slot)
    {
        string ret = abilities[slot].GetName();
        return ret;
    }


    public string GetAbilityLevelAsString(int slot)
    {
        string ret = "0";
        ret = abilities[slot].GetLevel().ToString();
        return ret;
    }

    public void SetPos(Vector2Int pos)
    {
        this.pos = pos;

        if(team == 1)
        {
            gi.plrPos = pos;
        }
    }

    public Vector2Int GetPos()
    {
        return pos;
    }
    public void AddHP(int value)
    {
        stats.hpCur += value;
        if(stats.hpCur > stats.hp)
        {
            stats.hpCur = stats.hp;
        }

        hpCon.RefreshHpBar();
    }
    public void DecreaseHP(int incomingDmg, PlayerInfo attacker)
    {
        float f = incomingDmg;
        f -= (incomingDmg * stats.incomingDmgReduc);
        incomingDmg = Mathf.RoundToInt(f);

        //only for lifesteal
        if(attacker != null)
        {
            float healMult = (attacker.invStats.lifeSteal / 100f);
            int healValue = Mathf.RoundToInt(f * healMult);
            //Debug.Log("LIFESTEAL" + healValue);
            attacker.AddHP(healValue);
        }
       stats.hpCur -= incomingDmg;
        if(stats.hpCur < 0)
        {
            stats.hpCur = 0;
        }

        hpCon.RefreshHpBar();
        hpCon.CheckIfDead();
        Vector3 placeHere = new Vector3(pos.x, pos.y, -1);
        assetsLib.CreateDamageText(placeHere, incomingDmg);
    }

    public void SetAttackState(bool status)
    {
        attacking = status;
    }

    public bool GetAttackState()
    {
        return attacking;
    }

    public PlayerInfo()
    {

    }

    public int GetTeam()
    {
        return team;
    }

    public float GetAttackTimer()
    {
        return stats.attackSpeed;
    }

    void InstantiatePassiveSkills()
    {
        passiveSkills[0] = new PassiveSkill("Attack", DataTransferManager.dataHolder.passiveLv[0], "Damage", 0.03f);
        passiveSkills[1] = new PassiveSkill("Defense", DataTransferManager.dataHolder.passiveLv[1], "Armor", 0.03f);
        passiveSkills[2] = new PassiveSkill("Magic", DataTransferManager.dataHolder.passiveLv[2], "Spell Power", 0.03f);
        passiveSkills[3] = new PassiveSkill("Vitality", DataTransferManager.dataHolder.passiveLv[3], "Health", 0.06f);
        passiveSkills[4] = new PassiveSkill("Energy", DataTransferManager.dataHolder.passiveLv[4], "Mana", 0.06f);
        SetPlayerInfoToSkills();
}
    void SetPlayerInfoToSkills()
    {
        passiveSkills[0].SetPlayerInfo(this);
        passiveSkills[1].SetPlayerInfo(this);
        passiveSkills[2].SetPlayerInfo(this);
        passiveSkills[3].SetPlayerInfo(this);
        passiveSkills[4].SetPlayerInfo(this);
    }
    public void SetStartingStats()
    {
        assetsLib = GameObject.Find("Assets").GetComponent<AssetsLibrary>();
        //for npc
        if (team == 2)
        {
            float f = gi.wave;

            System.Random random = new System.Random();
            int attMult = random.Next(10, 25);
            int hpMult = random.Next(10, 25);
            int defMult = random.Next(10, 25);

            float attLv = (1+(gi.wave * attMult) / 8) * 1.4f;
            float hpLv = (1 + (gi.wave * hpMult) / 8) * 1.4f;
            float defLv = (1 + (gi.wave * defMult) / 12) * 1.4f;

            int lv = 0;
            lv = Mathf.RoundToInt(attLv);
            passiveSkills[0].SetLevel(lv);

            lv = Mathf.RoundToInt(hpLv);
            passiveSkills[3].SetLevel(lv);

            lv = Mathf.RoundToInt(defLv);
            passiveSkills[1].SetLevel(lv);

            invStats.SetRandomItemStats();
        }

        CalculateAll();
    }

    public void ActionButtonPress(int slot)
    {
        int mpCost = abilities[slot].GetCostMP();
        bool active = abilities[slot].coolDown.GetActivity();

        if(!active)
        {
            if (stats.mpCur > mpCost)
            {
                //UtilizeAbility(slot);
                stats.mpCur -= mpCost;
            }
        }
    }

    public void CalculateAll()
    {
        if (team == 1)
        {

            invStats.CalculateItemstats(); //npc only sets total item attributes directly
        }

        float buffMult = 1;

        stats.SetClassAttributes();
        baseStats.CalculateStats();

        stats.CalculateHealth();
        stats.CalculateMana();

        buffMult = buffs.GetDamage();
        stats.CalculateDamage(buffMult);

        stats.CalculateArmor();
        stats.CalculateSpellPower();

        buffMult = buffs.GetAttackSpeed();
        stats.CalculateAttackSpeed(buffMult);

        if (team ==  1)
        {
            for (int i = 0; i < DataTransferManager.dataHolder.abilId.Length; i++)
            {
                UpdateAbilityData(i);
            }
        }
    }
}