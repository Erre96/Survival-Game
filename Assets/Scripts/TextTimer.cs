using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTimer : MonoBehaviour {
    int timer;
    public int timerStart;
	// Use this for initialization
	void Start () {
        timer = timerStart;
	}
	
	// Update is called once per frame
	void Update () {
        timer--;
        if(timer<1)
        {
            timer = timerStart;
            gameObject.transform.parent.gameObject.SetActive(false);
        }
	}
}
