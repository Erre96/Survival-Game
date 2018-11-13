using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldTextController : MonoBehaviour {
    Text text;
    int timer;

	// Use this for initialization
	void Start () {
        text = gameObject.GetComponent<Text>();
        //text.text = gi.dmg.ToString();
        timer = 880;
    }
	
	// Update is called once per frame
	void Update () {
        transform.Translate(0, 0.05f, 0);
        timer--;
        if(timer <1)
        {
            Destroy(transform.parent.gameObject);
        }

    }
}
