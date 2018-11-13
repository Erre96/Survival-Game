using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointController : MonoBehaviour {
    Spawner sp;

	// Use this for initialization
	void Start () {
        sp = GameObject.Find("Assets").GetComponent<Spawner>();
        sp.spawns.Add(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
