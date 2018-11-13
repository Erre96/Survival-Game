using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    GameObject plrChar;
    GameObject sword;
    public static Vector2Int pos;
    AttackControllerPlr attcon;
    public Spawner spawner;
    public Vector2Int goingTo;
    public bool walking;
    public bool movementAttempt;
    HealthbarController hpCon;
    public ActionButtonController abc;
    public AttackControllerPlr acp;
    public AnimationManager anim;
    public PlayerInfo pinfo;


    // Use this for initialization

    void Start()
    {
    }

    public void InitializeData () {
        hpCon = gameObject.transform.GetChild(1).gameObject.GetComponent<HealthbarController>();
        plrChar = gameObject.transform.GetChild(0).gameObject;
        sword = plrChar.transform.GetChild(0).gameObject;
        attcon = sword.transform.GetChild(0).GetComponent<AttackControllerPlr>();
        spawner = GameObject.FindGameObjectWithTag("Assets").GetComponent<Spawner>();
        anim = gameObject.transform.GetChild(0).gameObject.GetComponent<AnimationManager>();
        pinfo.name = DataTransferManager.dataHolder.name;
        pinfo = new PlayerInfo(1, hpCon);
        pinfo.SetStartingStats();

        RefreshPosAsInt(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)));
        MapDataController.map[pos.x, pos.y].SetNpc(pinfo);
        SetABC_ToAbilities(); //reference to action button group, for easier communication between buttons and abilities
        pinfo.dir = "";

        abc.pinfo = pinfo;
        acp.SetTimer();
    }

    // Update is called once per frame

    void FixedUpdate()
    {
        InputManager();
        ManageMoveMent();

        if (!walking)
        {
            anim.AnimateSprite(false);
        }

    }

    public static void SetAnimState(bool move, Animator anim)
    {
        if(move)
        {
            anim.SetInteger("anim", 1);
        }

        if(!move)
        {
            anim.SetInteger("anim", 0);
        }
    }

    public static Vector3 GetPosAsVector3()
    {
        Vector3 holder = new Vector3(pos.x, pos.y, -1);
        return holder;
    }

    void InputManager()
    {
        if( walking == false && movementAttempt == false)
        {
            if (Input.GetKey(KeyCode.W))
            {
                pinfo.dir = "u";
                movementAttempt = true;
                return;
            }

            if (Input.GetKey(KeyCode.D))
            {
                pinfo.dir = "r";
                movementAttempt = true;
                return;
            }

            if (Input.GetKey(KeyCode.S))
            {
                pinfo.dir = "d";
                movementAttempt = true;
                return;
            }

            if (Input.GetKey(KeyCode.A))
            {
                pinfo.dir = "l";
                movementAttempt = true;
                return;
            }
        }

        if (Input.GetKey(KeyCode.H))
        {
            Attack(true);
        }
    }

    public bool CheckIfTraversable(int x, int y)
    {
        MapCoordinate[,] map = MapDataController.map;
        int type = map[x, y].GetType();
        PlayerInfo trav = map[x, y].GetNpc();
        if(type == 1)
        {
            return false;
        }

        if(trav == pinfo)
        {
            return true;
        }

        if(trav != null)
        {
            return false;
        }
        return true;
    }

    public void RefreshPosAsInt(Vector2Int newPos)
    {
        pos.x = newPos.x;
        pos.y = newPos.y;

        pinfo.SetPos(pos);
    }

    public Vector2Int GetPos()
    {
        return pos;
    }

    void ManageMoveMent()
    {
        if (walking)
        {
            anim.AnimateSprite(true);
            TakeStep();
        }

        if(walking == false)
        {
            if (movementAttempt)
            {
                CheckForDirection();
            }
        }
    }

    void TakeStep()
    {
        Vector2 cur = new Vector2(transform.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, goingTo, pinfo.baseStats.stepSpeed);
        if (cur == goingTo)
        {
            walking = false;
            RefreshPosAsInt(goingTo);
        }
    }
    void CheckForDirection()
    {
        if (pinfo.dir == "lu")
        {
            anim.SetSpriteLoopValues(8, 11,"walking_up");
            MoveIfTraversable(pos.x -1, pos.y + 1);
        }

        if (pinfo.dir == "u")
        {
            anim.SetSpriteLoopValues(8, 11, "walking_up");
            MoveIfTraversable(pos.x, pos.y + 1);
        }

        if (pinfo.dir == "ru")
        {
            anim.SetSpriteLoopValues(8, 11, "walking_up");
            MoveIfTraversable(pos.x + 1, pos.y + 1);
        }

        if (pinfo.dir == "r")
        {
            anim.SetSpriteLoopValues(4, 7, "walking_right");
            MoveIfTraversable(pos.x + 1, pos.y);
        }

        if (pinfo.dir == "l")
        {
            anim.SetSpriteLoopValues(4, 7, "walking_left");
            MoveIfTraversable(pos.x - 1, pos.y);
        }

        if (pinfo.dir == "ld")
        {
            anim.SetSpriteLoopValues(0, 3, "walking_down");
            MoveIfTraversable(pos.x -1, pos.y - 1);
        }

        if (pinfo.dir == "d")
        {
            anim.SetSpriteLoopValues(0, 3, "walking_down");
            MoveIfTraversable(pos.x, pos.y - 1);
        }

        if (pinfo.dir == "rd")
        {
            anim.SetSpriteLoopValues(0, 3, "walking_down");
            MoveIfTraversable(pos.x + 1, pos.y - 1);
        }

        anim.ChangeDir();
        Rotate(pinfo.dir);
        gi.plrX = transform.position.x;
        gi.plrY = transform.position.y;
        gi.plrPos = new Vector3(gi.plrX, gi.plrY, -1);
    }

    void MoveIfTraversable(int x, int y)
    {
        bool traversable = CheckIfTraversable(x,y);
        if (traversable)
        {
            movementAttempt = false;
            UpdateMapPos(x, y);
        }

        if(!traversable)
        {
            movementAttempt = false;
        }
    }

    public void UpdateMapPos(int x, int y)
    {
        Rotate(pinfo.dir);
        Vector2 g = new Vector2(x,y);
        MapDataController.map[pos.x, pos.y].SetNpc(null);
        goingTo = new Vector2Int(x, y);
        pos = goingTo;
        MapDataController.map[x, y].SetNpc(pinfo);

        walking = true;
        movementAttempt = false;
    }

    public void Rotate(string dir)
    {
        /*
        Quaternion rot = plrChar.transform.rotation;
        float rotZ = rot.eulerAngles.z;

        switch (dir)
        {
            case "lu":
                rotZ = 225;
                rot = Quaternion.Euler(0, 0, rotZ);
                plrChar.transform.rotation = rot;
                break;

            case "u":
                rotZ = 180;
                rot = Quaternion.Euler(0, 0, rotZ);
                plrChar.transform.rotation = rot;
                break;

            case "ru":
                rotZ = 135;
                rot = Quaternion.Euler(0, 0, rotZ);
                plrChar.transform.rotation = rot;
                break;

            case "l":
                rotZ = 270;
                rot = Quaternion.Euler(0, 0, rotZ);
                plrChar.transform.rotation = rot;
                break;

            case "ld":
                rotZ = 315;
                rot = Quaternion.Euler(0, 0, rotZ);
                plrChar.transform.rotation = rot;
                break;

            case "d":
                rotZ = 0;
                rot = Quaternion.Euler(0, 0, rotZ);
                plrChar.transform.rotation = rot;
                break;

            case "rd":
                rotZ = 45;
                rot = Quaternion.Euler(0, 0, rotZ);
                plrChar.transform.rotation = rot;
                break;

            case "r":
                rotZ = 90;
                rot = Quaternion.Euler(0, 0, rotZ);
                plrChar.transform.rotation = rot;
                break;
        }*/
    }

    public void Attack(bool fromButton)
    {
        attcon.attacking = true;

        if (fromButton)
        {
            attcon.fromButton = true;
        }

        if (!fromButton)
        {
            attcon.fromButton = false;
        }
    }

    public void StopAttack()
    {
        attcon.attacking = false;
    }

    void SetABC_ToAbilities()
    {

    }
}
