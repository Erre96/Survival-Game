using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionButtonController : MonoBehaviour {
    public Image attackButton;
    public Image[] skillButtons;
    public PlayerController pcon;
    public PlayerInfo pinfo;

    AssetsLibrary assetsLib;

    TimerEC[] abilityDuration = new TimerEC[2];
    TimerEC[] abilityCooldown = new TimerEC[2];
    TimerEC[] potionCooldown = new TimerEC[2];

    Text[] cooldownText = new Text[2];
    Image[] cooldownImg = new Image[2];

    Text[] potCooldownText = new Text[2];
    Image[] potBlockImg = new Image[2];
    Text[] potAmountText = new Text[2];

    public void SetAbilityDuration(int slot, TimerEC timer) {abilityDuration[slot] = timer; }
    public void SetAbilityCooldown(int slot, TimerEC timer) { abilityCooldown[slot] = timer; }

    // Use this for initialization
    void Start () {
        //PlayerController pcon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        assetsLib = GameObject.FindGameObjectWithTag("Assets").GetComponent<AssetsLibrary>();

        cooldownText[0] = gameObject.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Text>();
        cooldownText[1] = gameObject.transform.GetChild(1).GetChild(1).gameObject.GetComponent<Text>();
        cooldownImg[0] = gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>();
        cooldownImg[1] = gameObject.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Image>();

        potCooldownText[0] = gameObject.transform.GetChild(2).GetChild(2).gameObject.GetComponent<Text>();
        potCooldownText[1] = gameObject.transform.GetChild(3).GetChild(2).gameObject.GetComponent<Text>();
        potBlockImg[0] = gameObject.transform.GetChild(2).GetChild(1).gameObject.GetComponent<Image>();
        potBlockImg[1] = gameObject.transform.GetChild(3).GetChild(1).gameObject.GetComponent<Image>();

        potAmountText[0] = gameObject.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Text>();
        potAmountText[1] = gameObject.transform.GetChild(3).GetChild(0).gameObject.GetComponent<Text>();

        potionCooldown[0] = new TimerEC(3);
        potionCooldown[1] = new TimerEC(3);

        RefreshPotsAmountText();

    }

    // Update is called once per frame
    void Update()
    {
        PotCooldownCountdown();
        CooldownCountdown();
        DurationCountdown();
        PotCooldownCountdown();
        RefreshCooldownText();
    }

    void RefreshCooldownText()
    {
        for(int i = 0; i < 2; i++)
        {
            if (abilityCooldown[i] != null)
            {
                int value = Mathf.RoundToInt(abilityCooldown[i].GetCurTimerInSeconds());
                bool active = abilityCooldown[i].GetActivity();
                if (active)
                {
                    cooldownText[i].text = value.ToString();
                }

                if (!active)
                {
                    cooldownText[i].text = "";
                    cooldownImg[i].gameObject.SetActive(false);
                    abilityCooldown[i] = null;
                    abilityDuration[i] = null;
                }
            }
        }
    }

    void CooldownCountdown()
    {
        for (int i = 0; i < abilityCooldown.Length; i++)
        {
            if(abilityCooldown[i] != null)
            {
                bool active = abilityCooldown[i].GetActivity();
                if (active)
                {
                    abilityCooldown[i].Countdown();

                    float f = abilityCooldown[i].GetCurTimerInSeconds();
                    if (f < 0)
                    {
                        abilityCooldown[i].ResetTimer();
                    }
                }
            }         
        }
    }

    void DurationCountdown () {
        for(int i = 0; i <abilityDuration.Length; i++)
        {
            if (abilityDuration[i] != null)
            {
                bool active = abilityDuration[i].GetActivity();
                if (active)
                {
                    abilityDuration[i].Countdown();

                    float f = abilityDuration[i].GetCurTimerInSeconds();
                    if (f < 0)
                    {
                        abilityDuration[i].ResetTimer();
                        DeactivateBuff(i);
                    }
                }
            }
        }
	}


    public void AbilityButtonPress(int slot)
    {
        UtilizeAbility(slot);
    }

    void UtilizeAbility(int slot)
    {
        bool cooldownActive = pinfo.abilities[slot].coolDown.GetActivity();
        int mpCost = pinfo.abilities[slot].GetCostMP();

        if (!cooldownActive && pinfo.stats.mpCur > mpCost)
        {
            cooldownImg[slot].gameObject.SetActive(true);
            int id = pinfo.abilities[slot].GetID();
            switch (id)
            {
                case 1:
                    Frenzy frenzy = (Frenzy)pinfo.abilities[slot];
                    frenzy.Activate();
                    break;

                case 2:
                    Rage rage = (Rage)pinfo.abilities[slot];
                    rage.Activate();
                    break;

                case 3:
                    Mine mine = (Mine)pinfo.abilities[slot];
                    mine.Activate();
                    break;

                case 4:
                    Fireball fireball = (Fireball)pinfo.abilities[slot];
                    PlayerInfo target = Target.GetTargetUnit();
                    if(target != null)
                    {
                        fireball.Activate();
                    }
                    break;
            }
        }      
    }

    void DeactivateBuff(int slot)
    {
        int id = pinfo.abilities[slot].GetID();
        switch (id)
        {
            case 1:
                Frenzy frenzy = (Frenzy)pinfo.abilities[slot];
                frenzy.Reset();
                break;

            case 2:
                Rage rage = (Rage)pinfo.abilities[slot];
                rage.Reset();
                break;
        }
    }

    public void UsePotion(int type)
    {
        switch(type)
        {
            case 1:
                UtilizeHealingPotion();
                break;

            case 2:
                UtilizeManaPotion();
                break;
        }
    }

    void UtilizeHealingPotion()
    {
        bool mustWait = false;
        mustWait = potionCooldown[0].GetActivity();
        if (mustWait == false)
        {
            int healingPots = pinfo.GetHealingPots();
            if (healingPots > 0)
            {
                float maxHP = Mathf.RoundToInt(pinfo.stats.hp);
                int healValue = Mathf.RoundToInt(maxHP * 0.25f);

                pinfo.stats.hpCur += healValue;
                pinfo.AddHealingPots(-1);
                RefreshPotsAmountText();
                potionCooldown[0].SetActivity(true);

                potBlockImg[0].gameObject.SetActive(true);
                potCooldownText[0].gameObject.SetActive(true);
            }
        }
    }

    void UtilizeManaPotion()
    {
        bool mustWait = false;
        mustWait = potionCooldown[1].GetActivity();
        if (mustWait == false)
        {
            int healingPots = pinfo.GetManaPots();
            if (healingPots > 0)
            {
                float maxMP = Mathf.RoundToInt(pinfo.stats.mp);
                int healValue = Mathf.RoundToInt(maxMP * 0.25f);

                pinfo.stats.mpCur += healValue;
                pinfo.AddManaPots(-1);
                RefreshPotsAmountText();
                potionCooldown[1].SetActivity(true);

                potBlockImg[1].gameObject.SetActive(true);
                potCooldownText[1].gameObject.SetActive(true);
            }
        }
    }

    public void RefreshPotsAmountText()
    {
        int healingPots = pinfo.GetHealingPots();
        potAmountText[0].text = healingPots + " x";

        int manaPots = pinfo.GetManaPots();
        potAmountText[1].text = manaPots + " x";

        if(pinfo.hpCon != null)
        {
            pinfo.hpCon.RefreshHpBar();
        }
    }

    void PotCooldownCountdown()
    {
        for(int i = 0; i < potionCooldown.Length; i++)
        {
            bool cooldownActive = potionCooldown[i].GetActivity();
            if(cooldownActive == true)
            {
                potionCooldown[i].Countdown();
                potCooldownText[i].text = Mathf.RoundToInt(potionCooldown[i].GetCurTimerInSeconds()).ToString();
                float tl = potionCooldown[i].GetCurTimerInSeconds();
                if (tl < 0)
                {
                    potBlockImg[i].gameObject.SetActive(false);
                    potCooldownText[i].gameObject.SetActive(false);
                    potionCooldown[i].ResetTimer();
                }
            }
        }
    }

    void RefreshPotCooldownText()
    {
        for(int i = 0; i < potionCooldown.Length; i++)
        {
            float tl = potionCooldown[i].GetCurTimerInSeconds();
            potCooldownText[0].text = Mathf.RoundToInt(tl).ToString();
        }
    }

    public void SetIcons()
    {
        if (pinfo.abilities.Length > 0)
        {
            for (int i = 0; i < pinfo.abilities.Length; i++)
            {
                int id = pinfo.abilities[i].GetID();
                skillButtons[i].sprite = assetsLib.abilityIcons[id];

            }
        }
    }

    public void ResetTimers()
    {
        for (int i = 0; i < abilityCooldown.Length; i++)
        {

            if (abilityCooldown[i] != null)
            {
                abilityCooldown[i].ResetTimer();
            }

            if (abilityDuration[i] != null)
            {
                abilityDuration[i].ResetTimer();
            }

            RefreshCooldownText();
        }
    }
}
