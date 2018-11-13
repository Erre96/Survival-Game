using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDataInfo
{
    public Text dataText;
    public Item item = new Item();

    public ItemDataInfo(Item item, Text dataText)
    {
        this.dataText = dataText;
        this.item = item;
    }

    public void SetItemDataText()
    {
        string mainAttr = item.GetMainAttribute();
        dataText.text = item.name + "\t\t" + "Level " + item.level + "\n"+"\n" + mainAttr + " : ";

        switch (item.itemType)
        {
            case 0:
                dataText.text = "";
                break;

            case 1:
                dataText.text += item.itemDmg.ToString();
                break;

            case 2:
                dataText.text += item.itemArm.ToString();
                break;

            case 3:
                dataText.text += item.itemAP.ToString();
                break;

            case 4:
                dataText.text += item.hpInc.ToString();
                break;

            case 5:
                dataText.text += item.mpInc.ToString();
                break;

            case 6:
                dataText.text += item.lifeSteal.ToString() + " %";
                break;

            case 7:
                dataText.text += item.attSpdInc.ToString() + " %";
                break;
        }
        if(item.itemType != 0)
        {
            dataText.text += "\n" + "\n" + "\n" + "\n" + "Sell Price : " + item.value.ToString();
        }
    }
}
public class SellingController : MonoBehaviour {
    public Text dataText;
    public ItemDataInfo itemDataInfo;
    Image[] itemSpr = new Image[6];
    Image[] marker = new Image[6];
    AssetsLibrary assetsLib;
    public int markedItem;
    PlayerController pcon;

    // Use this for initialization
    void Start () {
        SetRefs();
        RefreshItemImage();
	}
	
	// Update is called once per frame
	void Update () {
	}

    void SetItemImageRefs()
    {
        for (int i = 0; i < itemSpr.Length; i++)
        {
            itemSpr[i] = gameObject.transform.GetChild(1).GetChild(i).GetComponent<Image>();
            marker[i] = gameObject.transform.GetChild(1).GetChild(i).GetChild(0).GetComponent<Image>();
        }
    }

    void SetRefs()
    {
        assetsLib = GameObject.Find("Assets").GetComponent<AssetsLibrary>();
        pcon = GameObject.Find("Player").GetComponent<PlayerController>();
        SetItemImageRefs();
    }

    public void RefreshItemImage()
    {
        for (int i = 0; i < itemSpr.Length; i++)
        {
            itemSpr[i].sprite = assetsLib.itemSpr[pcon.pinfo.items[i].itemType];
            marker[i].enabled = false;
        }
    }

    public void MarkItem(int index)
    {
        RefreshItemImage();
        markedItem = index;

        itemDataInfo = new ItemDataInfo(pcon.pinfo.items[index], dataText);
        itemDataInfo.SetItemDataText();

        if (itemDataInfo.item.itemType != 0)
        {
            marker[index].enabled = true;
        }
    }

    public void SellItem()
    {
        PlayerController pcon = GameObject.Find("Player").GetComponent<PlayerController>();
        PlayerInfo pinfo = pcon.pinfo;
        gi.gold += itemDataInfo.item.value;
        pinfo.items[markedItem].ResetItem();
        pinfo.CalculateAll();
        dataText.text = "";
        RefreshItemImage();
    }
}
