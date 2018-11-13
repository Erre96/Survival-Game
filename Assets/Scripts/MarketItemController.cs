using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketItemController : MonoBehaviour {
    public int type; // 1 = sword; 2 = shield
    MarketController mc;
    Image image;

    // Use this for initialization
    void Start () {
        mc = GameObject.Find("Canvas").transform.GetChild(2).GetComponent<MarketController>();
        image = gameObject.transform.GetChild(0).GetComponent<Image>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnPress()
    {
        mc.DeactivateAllItems();

        image.enabled = true;

        switch (type)
        {
            case 1:
                mc.SetMainAttribute("Damage",1);
                break;

            case 2:
                mc.SetMainAttribute("Defense",2);
                break;

            case 3:
                mc.SetMainAttribute("Spell Power",3);
                break;

            case 4:
                mc.SetMainAttribute(null,4);
                break;

            case 5:
                mc.SetDataForHealingPot();
                break;

            case 6:
                mc.SetDataForManaPot();
                break;

            case 7:
                mc.SetMainAttribute("Health bonus", 4);
                break;

            case 8:
                mc.SetMainAttribute("Mana bonus", 5);
                break;

            case 9:
                mc.SetMainAttribute("Life steal", 6);
                break;

            case 10:
                mc.SetMainAttribute("Attack Speed Increase", 7);
                break;
        }
    }

    public void DeactivateItem()
    {
        if(image != null)
        {
            image.enabled = false;
        }
    }
}
