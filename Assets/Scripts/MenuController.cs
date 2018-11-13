using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {
    public string portal;
    public bool stage;
    public bool firstStage;
    public bool DestroyGameManager;
    public bool resetBeforeLoad;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void OnMouseDown () {
        LoadLevel();
    }

    public void LoadLevel()
    {
        bool canProceed = true;
        if (portal == "")
        {
            portal = gi.stage.ToString();
        }

        if(resetBeforeLoad)
        {
            gi.wave = 1;
            gi.stage = 1;
            gi.gold = 0;
            DataTransferManager.gameLoaded = false;
            Spawner sp = GameObject.FindGameObjectWithTag("Assets").GetComponent<Spawner>();
            sp.sm.ResetSpawnSum();
            gi.ec.Clear();
        }

        GameManager gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        if (stage)
        {

            canProceed = CheckIfSetForStage();
            if (canProceed)
            {
                if (firstStage)
                {
                    Spawner sp = GameObject.FindGameObjectWithTag("Assets").GetComponent<Spawner>();
                    if(DataTransferManager.gameLoaded)
                    {
                        sp.sm.spawnNow = false;
                    }
                    gi.firstStage = true;
                }

                gm.ActivateAll();
            }
        }

        if (canProceed)
        {
            SceneManager.LoadScene(portal, LoadSceneMode.Single);
        }
    }

    bool CheckIfSetForStage()
    {
        bool canProceed = true;

        if (DataTransferManager.dataHolder.weapon != 0)
        {
            for (int i = 0; i < DataTransferManager.dataHolder.abilId.Length; i++)
            {
                if (DataTransferManager.dataHolder.abilId[i] == 0)
                {
                    canProceed = false;
                    break;
                }
            }
        }
        return canProceed;
    }
}
