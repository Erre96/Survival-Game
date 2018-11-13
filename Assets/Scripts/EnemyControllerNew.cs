using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControllerNew : MonoBehaviour
{
    int fingerId = -1;
    public HealthbarController hpcon;
    EnemyControllerNew ec;
    public PlayerInfo pinfo;
    TravelerController trav;
    TimerEC attTimer;
    public GameObject spear;
    public bool attacking;
    SpriteRenderer markBox;
    AssetsLibrary assetsLib;

    public SpriteRenderer GetMarkBox()
    {
        return markBox;
    }

    // Use this for initialization
    void Start()
    {
        assetsLib = GameObject.FindGameObjectWithTag("Assets").GetComponent<AssetsLibrary>();
        markBox = gameObject.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>();
        trav = GetComponent<TravelerController>();
        hpcon = gameObject.transform.GetChild(1).gameObject.GetComponent<HealthbarController>();
        pinfo = new PlayerInfo(2, hpcon);
        ec = gameObject.GetComponent<EnemyControllerNew>();
        attTimer = ec.pinfo.stats.attackCooldown;
        gi.ec.Add(ec);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MarkFoeManage();

        if (attacking == false)
        {
            attTimer.Countdown();
            float timeLeft = attTimer.GetCurTimerInSeconds();
            if (timeLeft < 0)
            {
                Vector3 posNoDecimals = new Vector3(trav.pos.x, trav.pos.y, -1);
                if (transform.position == posNoDecimals)
                {
                    MeasureDistanceToTarget();
                }
            }
        }

        if (attacking == true)
        {
            Attack();
        }
    }

    void MeasureDistanceToTarget()
    {
        Vector2 targetPos = new Vector2(gi.plrPos.x, gi.plrPos.y);
        float dist = Vector2.Distance(targetPos, transform.position);

        if (dist < pinfo.stats.range)
        {
            attacking = true;
        }

        if (dist > pinfo.stats.range)
        {
            attacking = false;
        }
    }

    void Attack()
    {
        gi.dmg = pinfo.stats.GetRandomDamage();
        gi.SetDmgTextColor(2);
        Instantiate(spear, transform.position, Quaternion.identity);
        attTimer.ResetTimer();
        attacking = false;
    }
    
    private void OnMouseDown()
    {
        Target.SetTargetUnit(pinfo, markBox);
        PlayerController pcon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        pcon.Attack(false);
    }

    void MarkFoeManage()
    {
        foreach (Touch touch in Input.touches)
        {

            Vector3 position = Camera.main.ScreenToWorldPoint(touch.position);
            position.z = 1;

            RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);

            if (hit)
            {
                if (hit.collider.gameObject == gameObject)
                {
                    Target.SetTargetUnit(pinfo, markBox);
                    PlayerController pcon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            pcon.Attack(false);
                            fingerId = touch.fingerId;
                            break;

                        case TouchPhase.Stationary:
                            pcon.Attack(false);
                            fingerId = touch.fingerId;
                            break;

                        case TouchPhase.Ended:
                            if (fingerId == touch.fingerId)
                            {
                                fingerId = -1;
                            }
                            break;
                    }
                }
            }
        }
    }
}
