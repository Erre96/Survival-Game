using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainData : MonoBehaviour {
    int x;
    int y;
    public int id; //0 = walkable, 2 = node, 3 = obstacle. The higher id number will override the lesser ones, as shown below in void Start

	// Use this for initialization
	void Start () {
        x = Mathf.RoundToInt(transform.position.x);
        y = Mathf.RoundToInt(transform.position.y);


        int mapCoordinateType = MapDataController.map[x, y].GetType();
        if (mapCoordinateType < id)
        {
            MapDataController.map[x, y].SetType(id);
        }

        if(gameObject.name != "Goal")
        {
            gameObject.name = "Terrain : " + x + " , " + y;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
