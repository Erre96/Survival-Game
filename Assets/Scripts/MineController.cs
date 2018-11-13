using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineController : MonoBehaviour {
    public static Mine mine;

    PlayerController pcon;
 
    AssetsLibrary AssetsLib;
    Animator anim;
    bool blasted;
    int timer = 100;

    // Use this for initialization
    void Start()
    {
        pcon = GameObject.Find("Player").GetComponent <PlayerController>();
        //AssetsLib = GameObject.Find("Assets").GetComponent<AssetsLibrary>();
        anim = GetComponent<Animator>();
        blasted = false;
        GetMineStats();
        mine.CalculateDamage();
    }

    // Update is called once per frame
    void Update () {
        WithinReachCheck();
		if(blasted == true)
        {
            timer--;

            if (timer < 1)
            {
                Destroy(gameObject);
            }
        }
	}

    public void WithinReachCheck()
    {
        if(blasted == false)
        {
            float dist;

            for (int i = 0; i < gi.ec.Count; i++)
            {
                if(gi.ec[i] != null)
                {
                    dist = Vector2.Distance(gi.ec[i].transform.position, transform.position);
                    if (dist < 0.5f)
                    {
                        int dmg = mine.GetDamage();
                        gi.SetDmgTextColor(1);
                        print(dmg);
                        gi.ec[i].pinfo.DecreaseHP(dmg,null);
                        anim.SetInteger("anim", 1);
                        blasted = true;
                        break;
                    }
                }
            }
        }
    }

    void GetMineStats()
    {
        for (int i = 0; i < pcon.pinfo.abilities.Length; i++)
        {
            int id = pcon.pinfo.abilities[i].GetID();
            if (id == 3) //since 3 is mine
            {
                mine = (Mine)pcon.pinfo.abilities[i];
                mine.CalculateDamage();
            }
        }
    }
}
