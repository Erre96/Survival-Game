﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public GameObject plr;
	public GameObject assets;
	public GameObject GameCanvas;

	PlayerController pcon;
	MapDataController mc;
	Spawner spawner;

    public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.

    //Awake is always called before any Start functions
    void SingletonControl()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

        // Use this for initialization
        void Awake () {
        SingletonControl();
		pcon = plr.GetComponent<PlayerController>();
		mc = assets.GetComponent<MapDataController>();
		spawner = assets.GetComponent<Spawner>();
		spawner.enabled = false;
        DataTransferManager.dataHolder.SetDefaultValues();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ActivateAll()
	{
		assets.SetActive(true);
		plr.SetActive(true);
		GameCanvas.SetActive(true);
        //In the MapDataClass on the target stage, more important steps are made before game start
	}
}
