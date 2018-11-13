using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearController : MonoBehaviour {
    Vector3 target;
    Vector3 pos;
    int dmg;

	// Use this for initialization
	void Start () {
        dmg = gi.dmg;
        target = GameObject.Find("Player").transform.position;
        pos = transform.position;
        TurnToPlayer();
    }
	
	// Update is called once per frame
	void Update () {
        MoveTowardsTarget();
	}

    private void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, 0.25f);
        if(transform.position == target)
        {
            Vector2Int targPos = PlayerController.pos;
            PlayerInfo targInfo = MapDataController.map[targPos.x, targPos.y].GetNpc();

            if(targInfo != null)
            {
                targInfo.DecreaseHP(dmg, null);
                Destroy(gameObject);
            }
        }
    }

    void TurnToPlayer()
    {
        string dir = "";
        if (target.x > pos.x)
        {
            dir = "r";
        }

        if (target.x < pos.x)
        {
            dir = "l";
        }

        if (target.y < pos.y)
        {
            dir = dir + "d";
        }

        if (target.y > pos.y)
        {
            dir = dir + "u";
        }

        Rotate(dir);
    }

    void Rotate(string dir)
    {
        //print(dir);

        Quaternion rot = gameObject.transform.rotation;
        float rotZ = rot.eulerAngles.z;

        switch (dir)
        {
            case "u":
                rotZ = 90;
                break;

            case "l":
                rotZ = 180;
                break;

            case "d":
                rotZ = 270;
                break;

            case "r":
                rotZ = 0;
                break;

            case "lu":
                rotZ = 135;
                break;

            case "ld":
                rotZ = 225;
                break;

            case "ru":
                rotZ = 45;
                break;

            case "rd":
                rotZ = 315;
                break;
        }

        rot = Quaternion.Euler(0, 0, rotZ);
        gameObject.transform.rotation = rot;
    }
}
