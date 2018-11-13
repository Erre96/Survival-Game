using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddRendererToGlobalData : MonoBehaviour {
    SpriteRenderer spRe;
    int posX;
    int posY;

	// Use this for initialization
	void Start () {
        posX = Mathf.RoundToInt(transform.position.x);
        posY = Mathf.RoundToInt(transform.position.y);

        spRe = GetComponent<SpriteRenderer>();
        MapDataController.terrainSprites[posX, posY] = spRe;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
