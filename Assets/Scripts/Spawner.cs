using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    int timerStart;
    int timer;
    //GameObject[] spawns = new GameObject[4];
    public List<GameObject> spawns = new List<GameObject>();
    public GameObject enemy;

    public SpawnManager sm = new SpawnManager();
    PlayerInfo pinfo;

    // Use this for initialization
    void Start () {
        timerStart = 55;
        PlayerController pcon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        pinfo = pcon.pinfo;

        if(DataTransferManager.gameLoaded)
        {
            Spawner sp = GetComponent<Spawner>();
            sm.spawnNow = false;
            sp.enabled = false;
        }
	}
	
	// Update is called once per frame
    void FixedUpdate()
    {
        if(sm.spawnNow == true && spawns.Count > 0)
        {
            Spawn();
        }
    }

	void Spawn () {
        timer--;
        if(timer < 0)
        {
            int i = Random.Range(0, spawns.Count-1);

            Vector3 spawnPos = spawns[i].transform.position;
            int x = Mathf.RoundToInt(spawnPos.x);
            int y = Mathf.RoundToInt(spawnPos.y);
            PlayerInfo npc = (MapDataController.map[x, y].GetNpc());
            if(npc == null)
            {
                GameObject.Instantiate(enemy, spawns[i].transform.position, Quaternion.identity);
                timer = timerStart;
                sm.AddSpawnSum();
            }
        }
	}

    public void RaiseWave()
    {
        Spawner sp = GetComponent<Spawner>();
        if(sp.enabled == false)
        {
            sp.enabled = true;
        }

        if(gi.canContinue == true)
        {
            sm.RaiseWave();
            pinfo.CalculateAll();
            pinfo.hpCon.RefreshHpBar();
            AssetsLibrary al = gameObject.GetComponent<AssetsLibrary>();
            al.SetContinueButtonStatus(false);
            al.SetSaveButtonStatus(false);
            gi.canContinue = false;
        }
    }
}
