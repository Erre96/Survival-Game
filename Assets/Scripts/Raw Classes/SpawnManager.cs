using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[Serializable]
public class SpawnManager
{
    public int spawnSum;
    public int killsNeed;

    public bool spawnNow;

    public SpawnManager()
    {

        spawnNow = true;
        SetKillsNeed();
    }

    public void AddSpawnSum()
    {
        spawnSum++;
        if (spawnSum >= killsNeed)
        {
            spawnNow = false;
        }
    }

    public void SetKillsNeed()
    {
        float f = (gi.wave * 1.2f) * 2.5f;

        if(f > 60)
        {
            f = 60;
        }

        killsNeed = Mathf.RoundToInt(f);
    }

    public void ResetSpawnSum()
    {
        spawnSum = 0;
    }
    public void RaiseWave()
    {
        gi.wave++;
        SetMessage("You are now on wave "+gi.wave);
        spawnSum = 0;
        SetKillsNeed();
        StartNewWave();

        gi.stage++;

        if (gi.stage > gi.totalStages)
        {
            gi.stage = 1;
        }

        string scene = gi.stage.ToString();
        Spawner sp = GameObject.FindGameObjectWithTag("Assets").GetComponent<Spawner>();
        PlayerController pcon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        Vector2Int movePlrHere = new Vector2Int(1, 1);
        pcon.RefreshPosAsInt(movePlrHere);
        sp.spawns.Clear();
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    public void StartNewWave()
    {
        spawnNow = true;
    }
    public void RemoveEC(EnemyControllerNew i)
    {
        gi.ec.Remove(i);
        CheckEC();
        //Debug.Log(gi.ec.Count);
    }

    public void CheckEC()
    {
        if (gi.ec.Count < 1 && spawnSum >= killsNeed)
        {
            gi.canContinue = true;
            PlayerInfo pinfo = GetPlayerInfo();
            pinfo.skillPoints += 3;
            pinfo.CalculateAll();
            pinfo.stats.RestoreHealth();
            pinfo.hpCon.RefreshHpBar();
            SetMessage("You have completed wave "+gi.wave);
        }
    }

    public static PlayerInfo GetPlayerInfo()
    {
        PlayerController pcon = GameObject.Find("Player").GetComponent<PlayerController>();
        PlayerInfo pinfo = pcon.pinfo;
        return pinfo;
    }
    public void SetMessage(string message)
    {
        Image image = GameObject.FindGameObjectWithTag("Canvas").gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        image.gameObject.SetActive(true);

        Text text = image.transform.GetChild(0).gameObject.GetComponent<Text>();
        text.text = message;
    }
}