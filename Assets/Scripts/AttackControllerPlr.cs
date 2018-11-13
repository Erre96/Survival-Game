using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackControllerPlr : MonoBehaviour {
    PlayerController pcon;
    AssetsLibrary AssetsLib;
    public TimerEC attTimer;
    public bool attacking = false;
    public bool fromButton;

    // Use this for initialization
    public void SetTimer () {
        pcon = gameObject.transform.parent.parent.parent.GetComponent<PlayerController>();
        AssetsLib = GameObject.FindGameObjectWithTag("Assets").GetComponent <AssetsLibrary>();
        float cooldownInSeconds = pcon.pinfo.GetAttackTimer();
        attTimer = pcon.pinfo.stats.attackCooldown;
	}
	
	// Update is called once per frame
	void Update () {
        if(attacking)
        {
            attTimer.Countdown();
            float timeLeft = attTimer.GetCurTimerInSeconds();

            if (timeLeft < 0)
            {
                attTimer.ResetTimer();

                PlayerInfo target = Target.GetTargetUnit();
                if(!fromButton)
                {
                    GetTarget();
                }

                if (fromButton)
                {
                    fromButton = false;
                    GetTargetFromAttackButton();
                }
            }
        }
	}
   
    public void GetTarget()
    {
        MapCoordinate[,] map = MapDataController.map;
        float range = pcon.pinfo.stats.range;

        PlayerInfo targetNpc = Target.GetTargetUnit();

        Vector2 enemyPos = targetNpc.GetPos();
        Vector2 plrPos = pcon.GetPos();

        float dist = Vector3.Distance(plrPos, enemyPos);

        if(dist <= range)
        {
            if (targetNpc != null)
            {
                int targetTeam = targetNpc.GetTeam();
                int team = pcon.pinfo.GetTeam();

                if (targetTeam != team)
                {
                    gi.SetDmgTextColor(1);
                    gi.dmg = pcon.pinfo.stats.GetRandomDamage();
                    AttackTarget(targetNpc);
                }
            }
        }
    }

    public void GetTargetFromAttackButton()
    {
        {
            MapCoordinate[,] map = MapDataController.map;
            Vector2Int orgPos = pcon.GetPos();
            float range = pcon.pinfo.stats.range;

            for (int i = 1; i <= range; i++)
            {
                Vector2Int targetPos = TargetPosition(orgPos.x, orgPos.y, i);
                PlayerInfo targetNpc = map[targetPos.x, targetPos.y].GetNpc();
                if (targetNpc != null)
                {
                    int targetTeam = targetNpc.GetTeam();
                    int team = pcon.pinfo.GetTeam();

                    if (targetTeam != team)
                    {
                        GameObject enemy = targetNpc.hpCon.gameObject.transform.parent.gameObject;
                        EnemyControllerNew ec = enemy.GetComponent<EnemyControllerNew>();
                        SpriteRenderer markBox = ec.GetMarkBox();
                        Target.SetTargetUnit(targetNpc,markBox);

                        gi.SetDmgTextColor(1);
                        gi.dmg = pcon.pinfo.stats.GetRandomDamage();
                        AttackTarget(targetNpc);
                    }
                }
            }
        }
    }


    Vector2Int TargetPosition(int x, int y, int range)
    {
        //the position that your character is faced towards
        Vector2Int targetPos = new Vector2Int(x, y);
        switch (pcon.pinfo.dir)
        {
            case "lu":
                targetPos = new Vector2Int(x - range, y + range);
                break;

            case "u":
                targetPos = new Vector2Int(x, y + range);
                break;

            case "ru":
                targetPos = new Vector2Int(x + range, y + range);
                break;

            case "ld":
                targetPos = new Vector2Int(x - range, y - range);
                break;

            case "d":
                targetPos = new Vector2Int(x, y - range);
                break;

            case "rd":
                targetPos = new Vector2Int(x + range, y - range);
                break;

            case "l":
                targetPos = new Vector2Int(x - range, y);
                break;

            case "r":
                targetPos = new Vector2Int(x + range, y);
                break;
        }

        return targetPos;

    }

    public void AttackCooldown()
    {
        attTimer.Countdown();
        float timeLeft = attTimer.GetCurTimerInSeconds();
        if (timeLeft < 0)
        {
            attTimer.ResetTimer();
        }
    }

    public void AttackTarget(PlayerInfo target)
    {
        Vector2Int posValues = target.GetPos();
        Vector3 placeHere = new Vector3(posValues.x, posValues.y, -1);

        if (pcon.pinfo.baseStats.specType != "Range")
        {
            GameObject.Instantiate(AssetsLib.dmgText, placeHere, Quaternion.identity);
            target.DecreaseHP(gi.dmg, pcon.pinfo);
        }

        if (pcon.pinfo.baseStats.specType == "Range")
        {
            GameObject.Instantiate(AssetsLib.plrSpear, pcon.gameObject.transform.position, Quaternion.identity);
        }
    }
}
