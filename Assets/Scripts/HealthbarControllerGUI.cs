using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarControllerGUI : MonoBehaviour {
    public int id;
    public PlayerController pcon;
    public PlayerInfo pinfo;
    Image image;
    AssetsLibrary assetsLib;

    // Use this for initialization
    void Start () {
        pcon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        assetsLib = GameObject.Find("Assets").GetComponent<AssetsLibrary>();
        image = GetComponent<Image>();
    }
	
	// Update is called once per frame
	void Update () {
        if(id == 1)
        {
            RefreshHpBar();
        }

        if (id == 2)
        {
            RefreshManaBar();
        }
    }

    public void RefreshHpBar()
    {
        pinfo = pcon.pinfo;
        float i = (26f / pinfo.stats.hp) * pinfo.stats.hpCur;

        int j = Mathf.RoundToInt(i);
        if (j < 0) { j = 0; }
        else if (j > 25) { j = 25; }

        image.sprite = assetsLib.hpBar[j];
    }

    public void RefreshManaBar()
    {
        pinfo = pcon.pinfo;
        float i = (26f / pinfo.stats.mp) * pinfo.stats.mpCur;

        int j = Mathf.RoundToInt(i);
        if (j < 0) { j = 0; }
        else if (j > 25) { j = 25; }

        image.sprite = assetsLib.hpBar[j];
    }
}
