using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketController : MonoBehaviour{

    public GameObject[] tabs;

    public int itemType;
    public Text ItemDataText;

    Item item;

    public Text itemLevelText;
    public Text itemCost;

    MarketItemController[] itemCon = new MarketItemController[7];
    MarketItemController[] potCon = new MarketItemController[2];

    public ActionButtonController abc;


    public int itemLevelTot;
    public int[] itemLevelEach = new int[3];

    public bool superior;
    public string mainAttribute;

    public int cost;

    bool isPotion;
    int potId;
    int currentTab;

	// Use this for initialization
	void Start () {
        SetallRefs();
        item = new Item();
        itemLevelEach[0] = 0;
        itemLevelEach[1] = 0;
        itemLevelEach[2] = 1;
        SetTotItemLevel();
        SetTotalCost();
        RefreshItemLevelText();
        RefreshItemCost();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetMainAttribute(string attr, int itemType)
    {
        isPotion = false;
        mainAttribute = attr;
        this.itemType = itemType;
        SetItemData();
        RefreshItemCost();

    }

    public void SetSuperior(bool state)
    {
        superior = state;
        RefreshItemCost();
    }

    public void ChangeValue(int change) //for item level
    {
        if(itemType != 0 && isPotion == false)
        {
            switch (change)
            {
                case 1:
                    ChangeValueIfPossible(1, 2);
                    break;

                case -1:
                    ChangeValueIfPossible(-1, 2);
                    break;

                case 10:
                    ChangeValueIfPossible(1, 1);
                    break;

                case -10:
                    ChangeValueIfPossible(-1, 1);
                    break;

                case 100:
                    ChangeValueIfPossible(1, 0);
                    break;

                case -100:
                    ChangeValueIfPossible(-1, 0);
                    break;
            }
        }
    }

    void ChangeValueIfPossible(int change, int slot)
    {
        itemLevelEach[slot] += change;
      
        if (itemLevelEach[slot] < 0)
        {
            itemLevelEach[slot] = 9;
        }

        if (itemLevelEach[slot] > 9)
        {
            itemLevelEach[slot] = 0;
        }

        SetTotItemLevel();
        RefreshItemLevelText();
        SetItemData();
        RefreshItemCost();
    }

    void SetTotItemLevel()
    {
        string tot = itemLevelEach[0].ToString() + itemLevelEach[1].ToString() + itemLevelEach[2].ToString();
        //print(tot);
        itemLevelTot = int.Parse(tot);
    }
    void RefreshItemLevelText()
    {
        itemLevelText.text = itemLevelEach[0] +"      "+ itemLevelEach[1]+ "      "+
            itemLevelEach[2];
    }

    void RefreshItemCost()
    {
        if(!isPotion)
        {
            SetTotalCost();
        }

        if(itemType != 0 && itemLevelTot != 0)
        {
            itemCost.text = cost.ToString();
            ItemDataText.text += "\n" + "\n" + "\n" + "\n" + "Cost : " + cost;
        }
        else
        {
            itemCost.text = "";
        }
    }

    public void SetTotalCost()
    {

        float value = (itemLevelTot * 4) * (1 + (1.2f + (itemLevelTot / 300f)));
        if (itemType == 6)
        {
            value = (itemLevelTot * 10) * (1+(1.25f+(itemLevelTot/250f)));
        }

        cost = Mathf.RoundToInt(value);
    }

    public void DeactivateAllItems()
    {
        for(int i = 0; i < itemCon.Length; i++)
        {
            itemCon[i].DeactivateItem();
        }

        for (int i = 0; i < potCon.Length; i++)
        {
            potCon[i].DeactivateItem();
        }
        ResetItemChoise();
    }

    void ResetItemChoise()
    {
        itemType = -1;
        mainAttribute = null;
    }

    void SetItemData()
    {
        if(itemLevelTot == 0)
        {
            ItemDataText.text = "";
        }
        else
        {
            item.ResetItem();
            item.SetStats(this.itemType, itemLevelTot, cost, mainAttribute);
            ItemDataText.text = item.name + "\t\t" + "Level " + itemLevelTot + "\n" + "\n" + mainAttribute + " : ";

            switch (item.itemType)
            {
                case 1:
                    ItemDataText.text += item.itemDmg.ToString();
                    break;

                case 2:
                    ItemDataText.text += item.itemArm.ToString();
                    break;

                case 3:
                    ItemDataText.text += item.itemAP.ToString();
                    break;

                case 4:
                    ItemDataText.text += item.hpInc.ToString();
                    break;

                case 5:
                    ItemDataText.text += item.mpInc.ToString();
                    break;

                case 6:
                    ItemDataText.text += item.lifeSteal.ToString() + " %";
                    break;

                case 7:
                    ItemDataText.text += item.attSpdInc.ToString() + " %";
                    break;
            }
        }
    }

    public void BuyItem()
    {
        if(gi.gold >= cost)
        {
            PlayerController pcon = GameObject.Find("Player").gameObject.GetComponent<PlayerController>();
            PlayerInfo pinfo = pcon.pinfo;

            if (!isPotion && itemType > 0 && itemLevelTot > 0)
            {

                for (int i = 0; i < pinfo.items.Length; i++)
                {
                    if (pinfo.items[i].name == null)
                    {
                        gi.gold -= cost;

                        pinfo.items[i].SendOverStats(item);
                        pinfo.CalculateAll();
                        break;
                    }
                }
            }
            if(isPotion)
            {
                switch(potId)
                {
                    case 1:
                        pinfo.AddHealingPots(1);
                        break;

                    case 2:
                        pinfo.AddManaPots(1);
                        break;
                }
                abc.RefreshPotsAmountText();
                gi.gold -= cost;
            }
        }
    }
    void SetallRefs()
    {
        //text
        ItemDataText = gameObject.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<Text>();
        itemLevelText = gameObject.transform.GetChild(1).GetChild(2).GetChild(2).GetComponent<Text>();
        itemCost = gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();

        //equipment
        for(int i = 0; i < itemCon.Length; i++)
        {
            itemCon[i] = gameObject.transform.GetChild(1).GetChild(1).GetChild(i).GetComponent<MarketItemController>();
        }

        for (int i = 0; i < potCon.Length; i++)
        {
            potCon[i] = gameObject.transform.GetChild(2).GetChild(0).GetChild(i).GetComponent<MarketItemController>();
        }
    }

    public void SetDataForHealingPot()
    {
        potId = 1;
        isPotion = true;
        cost = gi.wave * 3;
        ItemDataText.text = "Health potion     Restores 25 %"+"\n"+"of maximum hitpoints";
        RefreshItemCost();
    }

    public void SetDataForManaPot()
    {
        potId = 2;
        isPotion = true;
        cost = gi.wave * 3;
        ItemDataText.text = "Mana Potion     Restores 25 %" + "\n" +"of maximum mana";
        RefreshItemCost();
    }

    public void SwitchTab(int tab)
    {
        if (tab != currentTab)
        {
            currentTab = tab;
            ItemDataText.text = "";
            ResetItemChoise();
            DeactivateAllItems();

            for (int i = 0; i < tabs.Length; i++)
            {
                if (i == tab)
                {
                    tabs[i].SetActive(true);
                }

                if (i != tab)
                {
                    tabs[i].SetActive(false);
                }
            }
        }
    }
}
