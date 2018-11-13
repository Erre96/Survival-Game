using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;


public static class Target
{
    private static PlayerInfo target;
    private static SpriteRenderer markBox;

    public static PlayerInfo GetTargetUnit()
    {
        return target;
    }

    public static void SetTargetUnit(PlayerInfo pinfo, SpriteRenderer markBox)
    {
        AssetsLibrary assetsLib = GameObject.FindGameObjectWithTag("Assets").GetComponent<AssetsLibrary>();

        Target.target = pinfo;

        if(Target.markBox != null)
        {
            Target.markBox.sprite = assetsLib.markBox[0];
        }

        Target.markBox = markBox;

        if (Target.markBox != null)
        {
            markBox.sprite = assetsLib.markBox[1];
        }
    }

    public static void RemoveTarget()
    {
        target = null;
        markBox = null;
        PlayerController pcon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        pcon.StopAttack();
    }
}

public static class gi 
{
    public static bool canContinue;
    public static int gold = 0;
    public static int wave = 1;
    public static int stage = 1;
    public static int totalStages = 4;

    public static bool firstStage;

    public static Color dmgTextColor = Color.white;
    public static float plrX;
    public static float plrY;
    public static Vector2 plrPos;
    public static string inputLetter;

    public static List<EnemyControllerNew> ec = new List<EnemyControllerNew>();

    public static int dmg; //for passing value to damage text

    public static Vector3 startPos;



    public static void SetDmgTextColor(int team)
    {
        if(team == 1)
        {
            dmgTextColor = Color.cyan;
        }

        if (team == 2)
        {
            dmgTextColor = Color.white;
        }
    }

    public static void ResetEC()
    {
        for(int i = 0; i < ec.Count; i++)
        {
            ec.RemoveAt(i);
        }
    }

    public static void SetWeaponChoise(int weapon)
    {
        DataTransferManager.dataHolder.weapon = weapon;
    }

    public static int GetWeaponChoise()
    {
        return DataTransferManager.dataHolder.weapon;
    }

}






//ab.effectValue = (attackTimer + ab.level) * 0.12f; frenzy

//ab.effectValue = (abPower * ab.level) * 0.25f; rage