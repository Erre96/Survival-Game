using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml; //Needed for XML functionality

public class ButtonController : MonoBehaviour {
    public static string[] txtInfo = new string[3];
    public Text text;
    public Text descHeader;
    public Text descText;
    public short l; //ability line in text
    public string lastClicked;
    

	// Use this for initialization
	void Start () {
        if(name == "ChoisesBar")
        {
            text = gameObject.transform.GetChild(0).GetComponent<Text>();
        }
    }

    // Update is called once per frame

    public void ResetAll()
    {
        for (int i = 0; i < DataTransferManager.dataHolder.abilId.Length; i++)
        {
            DataTransferManager.dataHolder.abilId[i] = 0;
        }

        for (int j = 0; j < txtInfo.Length; j++)
        {
            txtInfo[j] = "";
        }
        RefreshText();
        descHeader.text = "";
        descText.text = "";
        l = 0;
    }
    void SetWeaponDescription(string header, string mult, string attCooldown, string range)
    {
        descHeader.text = header;
        descText.text = "Damage Multiplier : " + mult + "x";
        descText.text += "\n";

        descText.text += "Attack Cooldown : "+attCooldown+" sec";
        descText.text += "\n";

        if(range != null)
        {
            descText.text += "Range : " + range;
        }
    }

    public void Choose()
    {
        switch (lastClicked)
        {
            case "Long Sword": gi.SetWeaponChoise(1); txtInfo[0] = lastClicked; break;

            case "Dagger": gi.SetWeaponChoise(2); txtInfo[0] = lastClicked; break;

            case "Throwing Knifes": gi.SetWeaponChoise(3); txtInfo[0] = lastClicked; break;

            case "Javelin": gi.SetWeaponChoise(4); txtInfo[0] = lastClicked; break;

            case "Bow": gi.SetWeaponChoise(5); txtInfo[0] = lastClicked; break;

            case "Unarmed": gi.SetWeaponChoise(6); txtInfo[0] = lastClicked; break;

        }
        //before abilities are picked
        if (l < DataTransferManager.dataHolder.abilId.Length)
        {
            switch(lastClicked)
            {
                case "Frenzy": AddAbility(1, lastClicked); break;
                case "Rage": AddAbility(2, lastClicked); break;
                case "Magical Mine": AddAbility(3, lastClicked); break;
                case "Fireball": AddAbility(4, lastClicked); break;
            }

        }
        //after abilities are picked
        if (l > DataTransferManager.dataHolder.abilId.Length)
        {
            return;
        }

        RefreshText();
    }

    public void DoSome(string button)
    {

        lastClicked = button;

        switch(button)
        {
            case "ResetChoises": ResetAll(); break;

            case "Long Sword":
                SetWeaponDescription(button, "3.2", "2", null); break;

            case "Dagger":
                SetWeaponDescription(button, "1.6", "1", null); break;

            case "Throwing Knifes":
                SetWeaponDescription(button, "0.8", "1", "3"); break;

            case "Javelin":
                SetWeaponDescription(button, "2.4", "2.1", "4"); break;

            case "Bow":
                SetWeaponDescription(button, "1.25", "1.8", "5"); break;

            case "Unarmed":
                SetWeaponDescription(button, "0.8", "0.5", null); break;

            case "Frenzy":
                descHeader.text = button; Frenzy frenzy = new Frenzy();
                descText.text = frenzy.GetDescription();
                break;

            case "Rage":
                descHeader.text = button; Rage rage = new Rage();
                descText.text = rage.GetDescription();
                break;

            case "Magical Mine":
                descHeader.text = button; Mine mine = new Mine();
                descText.text = mine.GetDescription();
                break;

            case "Fireball":
                descHeader.text = button; Fireball fireball = new Fireball();
                descText.text = fireball.GetDescription();
                break;
        }
    }

    public void AddAbility(int id, string name)
    {
        for (int i = 0; i < DataTransferManager.dataHolder.abilId.Length; i++)
        {
            if (DataTransferManager.dataHolder.abilId[i] == id)
            {
                break;
            }
            else if (i == DataTransferManager.dataHolder.abilId.Length - 1)
            {
                DataTransferManager.dataHolder.abilId[0+l] = id; txtInfo[1+l] = name; l++;
            }
        }
    }

    public void RefreshText()
    {
        text.text = "You have selected..." + "\n" + "\n" + "Weapon : " + txtInfo[0] + "\n" + "Ability 1 : " + txtInfo[1] + "\n" + "Ability 2 : " + txtInfo[2];
    }
}
