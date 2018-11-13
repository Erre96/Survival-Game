using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthbarController : MonoBehaviour {
    GameObject plr;
    AssetsLibrary assets;
    PlayerController pcon;
    SpriteRenderer spRe;
    EnemyControllerNew ec;
    PlayerInfo pinfo;
    Spawner spawner;


    // Use this for initialization
    void Start () {
        spawner = GameObject.Find("Assets").GetComponent<Spawner>();
        plr = gameObject.transform.parent.gameObject;
        assets = GameObject.Find("Assets").GetComponent<AssetsLibrary>();
        if (plr.name == "Player")
        {
            pcon = gameObject.transform.parent.GetComponent<PlayerController>();
        }
        else if (plr.name != "Player") { ec = gameObject.transform.parent.GetComponent<EnemyControllerNew>(); }

        spRe = gameObject.GetComponent<SpriteRenderer>();
        

    }
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(plr.transform.position.x, plr.transform.position.y+1, 0);
    }

    public void CheckIfDead()
    {
        RefreshHpBar();
        if (pinfo.stats.hpCur < 1)
        {
            int team = pinfo.GetTeam();

            if(team == 1)
            {
                GameObject canvas = GameObject.FindGameObjectWithTag("Canvas").gameObject;
                canvas.SetActive(false);
                spawner.enabled = false;
                plr.transform.position = new Vector3(1, 1);
                PlayerController pcon = plr.GetComponent<PlayerController>();
                pcon.RefreshPosAsInt(new Vector2Int(1, 1));
                Spawner sp = GameObject.FindGameObjectWithTag("Assets").GetComponent<Spawner>();

                sp.spawns.Clear();
                gi.canContinue = true;
                DataTransferManager.canSave = true;
                plr.SetActive(false);
                gi.ec.Clear();
                sp.sm.spawnNow = false;
                for(int i = 0; i < pcon.pinfo.items.Length; i++)
                {
                    pcon.pinfo.items[i].ResetItem();
                }
                sp.sm.spawnNow = true;

                HiscoreController.UpdateHiscore();
                DataTransferManager.SaveHiscore();
                SceneManager.LoadScene("Hiscores", LoadSceneMode.Single);
            }

            if(team == 2)
            {
                DiceForPot();
                gi.gold += gi.wave * 5;
                gi.ec.Remove(ec);
                Vector2Int pos = pinfo.GetPos();
                MapDataController.map[pos.x, pos.y].RemoveNpc();
                spawner.sm.RemoveEC(ec);
                Target.RemoveTarget();
                Destroy(transform.root.gameObject);
            }
        }
    }

    void DiceForPot()
    {
        int random = Random.Range(0, 4);
        print(random+"POT DICE");
        if(random == 3)
        {
            PlayerController pcon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            pcon.pinfo.AddHealingPots(1);
            pcon.abc.RefreshPotsAmountText();
        }

        if (random == 2)
        {
            PlayerController pcon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            pcon.pinfo.AddManaPots(1);
            pcon.abc.RefreshPotsAmountText();
        }
    }
    public void SetPlayerInfoRef(PlayerInfo pinfo)
    {
        this.pinfo = pinfo;
    }

    public void RefreshHpBar()
    {
        float i = (26f / pinfo.stats.hp) * pinfo.stats.hpCur;

        int j = Mathf.RoundToInt(i);
        if (j < 0) { j = 0; }
        else if (j > 25) { j = 25; }

        spRe.sprite = assets.hpBar[j];
    }
}
