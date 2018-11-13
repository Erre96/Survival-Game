using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearControllerPlr : MonoBehaviour {
    PlayerInfo target;
    PlayerInfo plr;
    Vector3 targetPos;
    TimerEC timeToDie;
    PlayerController pcon;

    // Use this for initialization
    void Start()
    {
        target = Target.GetTargetUnit();
        if (target != null)
        {
            Vector2 temp = target.GetPos();
            targetPos = new Vector3(temp.x, temp.y, -1);

            pcon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            plr = pcon.pinfo;
            timeToDie = new TimerEC(5);
        }

        if (target == null)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timeToDie.Countdown();
        float timeLeft = timeToDie.GetCurTimerInSeconds();
        if (timeLeft < 0)
        {
            Destroy(gameObject);
        }

        TurnToPlayer();
        MoveTowardsTarget();
    }

    private void MoveTowardsTarget()
    {
        if (target == null)
        {
            Destroy(gameObject);
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.25f);
        if (transform.position == targetPos)
        {
            int dmg = pcon.pinfo.stats.GetRandomDamage();
            gi.SetDmgTextColor(1);
            target.DecreaseHP(dmg, null);
            Destroy(gameObject);
        }
    }

    void TurnToPlayer()
    {
        Vector3 pos = transform.position;

        string dir = "";
        if (targetPos.x > pos.x)
        {
            dir = "r";
        }

        if (targetPos.x < pos.x)
        {
            dir = "l";
        }

        if (targetPos.y < pos.y)
        {
            dir = dir + "d";
        }

        if (targetPos.y > pos.y)
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