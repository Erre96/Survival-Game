using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public abstract class Ability
{
    protected int id;
    protected int slot;
    protected string name;
    protected int mpCost;

    protected int addCostPerLv;
    protected int level;
    protected string description;

    public TimerEC coolDown;

    protected ActionButtonController abc;
    protected PlayerInfo pinfo;

    public void SetAttributes(int addPerLv, float sec)
    {
        this.addCostPerLv = addPerLv;
        coolDown = new TimerEC(sec);

        SetCostMP();
    }

    public void UpgradeLevel()
    {
        level++;
        SetCostMP();
    }

    public void SetCostMP()
    {
        float f = 4 + (addCostPerLv * level) * (1 + (level * 0.25f));
        mpCost = Mathf.RoundToInt(f);
    }
    public void SetActionButtonController()
    {
        PlayerController pcon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        this.abc = pcon.abc;
    }
    public void SetDescription(string desc, string extra, float effectValue, string end,bool percentFactor)
    {
        description = name + "      Lv " + level + "\n";

        if (desc != null)
        {
            description += desc + "\n";
        }

        if (extra != null)
        {
            description += extra + "\n";
        }

        if(!percentFactor)
        {
            description += " " + effectValue + " " + end;
        }

        //converts effectValue since it's a percentFactor to the modified value in percent
        if(percentFactor)
        {
            int percent = Mathf.RoundToInt(((effectValue - 1) * 100));
            description += " " + percent + " " + end;
        }

        description += "\n\n" + "Cooldown : " + coolDown.GetMaxTimerInSeconds() + " sec";
        description += "    Mana Cost : " + mpCost;

    }

    public virtual void StartCooldown()
    {
        coolDown.SetActivity(true);
        abc.SetAbilityCooldown(slot, coolDown);
    }

    public string GetDescription()
    {
        return description;
    }

    public void SetID(int id) { this.id = id; }
    public int GetID() { return id; }

    public void SetName(string name) { this.name = name; }
    public string GetName() { return name; }

    public void SetLevel(int level) { this.level = level; }
    public int GetLevel() { return level; }

    public void SetCostMP(int mpCost) { this.mpCost = mpCost; }
    public int GetCostMP() { return mpCost; }

    public void SetSlot(int slot) { this.slot = slot; }
    public int GetSlot() { return slot; }
}

public class Mine : Ability
{
    int dmg;
    int addedDmgPerLv;

    public Mine()
    {
        name = "Mine";
        level = 1;
        id = 3;
        addedDmgPerLv = 3;
        addCostPerLv = 2;
        coolDown = new TimerEC(0);
        CalculateBaseDamage();
    }

    public Mine(int slot, PlayerInfo pinfo, int level)
    {
        name = "Mine";
        this.level = level;
        id = 3;
        addedDmgPerLv = 3;
        addCostPerLv = 2;
        dmg = 10;
        this.slot = slot;
        this.pinfo = pinfo;

        SetActionButtonController();
        coolDown = new TimerEC(0);

        SetDescription();
    }

    public virtual void SetDescription()
    {
        base.SetDescription("Places a magical mine", "that explodes on contact. Deals ", dmg, "damage", false);
    }

    public void Activate()
    {
        AssetsLibrary al = GameObject.FindGameObjectWithTag("Assets").GetComponent<AssetsLibrary>();
        Vector2 placeHere = pinfo.GetPos();
        al.CreateMine(placeHere);
        StartCooldown();
        pinfo.stats.DecreaseMana(mpCost);
    }

    public void CalculateDamage()
    {
        float f = 10 + (((level * addedDmgPerLv) * 2) + (pinfo.stats.spellPower / 3)) * pinfo.passiveSkills[2].mult;
        dmg = Mathf.RoundToInt(f);
        SetCostMP();
        SetDescription();
    }

    void CalculateBaseDamage()
    {
        dmg = 10 + ((level * addedDmgPerLv) * 2);
        SetCostMP();
        SetDescription();
    }

    public int GetDamage()
    {
        return dmg;
    }
}

[Serializable]
public abstract class StatusEffect : Ability
{
    public float mult;
    public TimerEC duration;

    public override void StartCooldown()
    {
        duration.ResetTimer();
        duration.SetActivity(true);
        coolDown.SetActivity(true);
        //Debug.Log("Duration"+abc.abilityDuration.Length+"cooldown"+abc.abilityCooldown.Length);
        abc.SetAbilityCooldown(slot,coolDown);
        abc.SetAbilityDuration(slot, duration);
    }

    public virtual void SetDescription()
    {
        base.SetDescription("Some moron forgot to override the", "description here", 0, "If you are that moron, do it now please,", false);
    }

    public void CalculateMult()
    {
        mult = 1 + (level * 0.07f);
        SetCostMP();
        SetDescription();
        description += "\n" + "Active Time : " + duration.GetMaxTimerInSeconds()+ " sec";
    } 
}

[Serializable]
public class Frenzy : StatusEffect
{
    public Frenzy()
    {
        name = "Frenzy";
        level = 1;
        id = 1;
        addCostPerLv = 4;
        coolDown = new TimerEC(60);
        duration = new TimerEC(30);
        CalculateMult();
    }

    public Frenzy(int slot, PlayerInfo pinfo, int level)
    {
        this.pinfo = pinfo;

        name = "Frenzy";
        this.level = level;
        id = 1;
        addCostPerLv = 4;
        this.slot = slot;

        SetActionButtonController();
        coolDown = new TimerEC(60);
        duration = new TimerEC(30);
    }

    public override void  SetDescription()
    {
        base.SetDescription("Increases attack speed with", null, mult, "percent",true);
    }

    public void Activate()
    {
        pinfo.buffs.SetAttackSpeed(mult);
        pinfo.stats.CalculateAttackSpeed(mult);

        //the duration timer will now be active in the abc class
        StartCooldown();
        pinfo.stats.DecreaseMana(mpCost);
    }

    public void Reset()
    {
        pinfo.buffs.SetAttackSpeed(1);
        pinfo.stats.CalculateAttackSpeed(1);
    }
}

public class Rage : StatusEffect
{
    public Rage()
    {
        name = "Rage";
        level = 1;
        id = 2;
        addCostPerLv = 4;
        coolDown = new TimerEC(60);
        duration = new TimerEC(20);
        CalculateMult();
    }

    public Rage(int slot, PlayerInfo pinfo, int level)
    {
        this.pinfo = pinfo;
        name = "Rage";
        this.level = level;
        id = 2;
        addCostPerLv = 4;
        this.slot = slot;

        SetActionButtonController();
        coolDown = new TimerEC(60);
        duration = new TimerEC(20);
    }

    public void Activate()
    {
        pinfo.buffs.SetDamage(mult);
        pinfo.stats.CalculateDamage(mult);

        //the duration timer will now be active in the abc class
        StartCooldown();
        pinfo.stats.DecreaseMana(mpCost);

    }

    public void Reset()
    {
        pinfo.buffs.SetDamage(1);
        pinfo.stats.CalculateDamage(1);
    }

    public override void SetDescription()
    {
        base.SetDescription("Increases damage by", null, mult, "percent",true);
    }
}

public class Fireball : Ability
{
    int damage;
    int addedDmgPerLv;

    public Fireball()
    {
        name = "Fireball";
        level = 1;
        id = 4;
        addedDmgPerLv = 6;
        addCostPerLv = 2;
        coolDown = new TimerEC(1);
        CalculateBaseDamage();
    }

    public Fireball(int slot, PlayerInfo pinfo, int level)
    {
        this.pinfo = pinfo;
        name = "Fireball";
        this.level = level;
        id = 4;
        addedDmgPerLv = 6;
        addCostPerLv = 2;
        this.slot = slot;

        SetActionButtonController();
        coolDown = new TimerEC(1);
    }

    public int GetDamage()
    {
        return damage;
    }

    public void CalculateDamage()
    {
        float f = 10 + (((addedDmgPerLv * level) * 2) + (pinfo.stats.spellPower / 3)) * pinfo.passiveSkills[2].mult;
        damage = Mathf.RoundToInt(f);
        SetCostMP();
        SetDescription();
    }

    void CalculateBaseDamage()
    {
        damage = 10 +((addedDmgPerLv * level) * 2);
        SetCostMP();
        SetDescription();
    }


    public void Activate()
    {
        AssetsLibrary al = GameObject.FindGameObjectWithTag("Assets").GetComponent<AssetsLibrary>();
        Vector2 placeHere = pinfo.GetPos();
        al.CreateFireball(placeHere);
        StartCooldown();
        pinfo.stats.DecreaseMana(mpCost);
    }

    public void SetDescription()
    {
        base.SetDescription("Shoots a ball of flame", "on the enemy. Deals : ", damage, "damage", false);
    }
}