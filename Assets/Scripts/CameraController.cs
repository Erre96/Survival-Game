using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    GameObject plr;

	// Use this for initialization
	void Start () {
        plr = GameObject.FindGameObjectWithTag("Player");
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(plr.transform.position.x, plr.transform.position.y, -10);
	}
}
