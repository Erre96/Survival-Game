using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SellingItemController : MonoBehaviour {
    int index;
    AssetsLibrary assetsLib;
    public Image spr;

	// Use this for initialization
	void Start () {
        assetsLib = GameObject.Find("Assets").GetComponent<AssetsLibrary>();
        spr = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
        spr.sprite = assetsLib.itemSpr[index];
    }
}
