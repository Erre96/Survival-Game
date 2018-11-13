using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CoordinateRouteInfo
{
    public Vector2Int pos;
    public int gCost; //distance from starting node
    public int hCost; //distannce from end node
    public int fCost; //gCost + hCost
    public CoordinateRouteInfo prevNode;

    public Vector2Int GetPos()
    {
        return pos;
    }

    public CoordinateRouteInfo(Vector2Int start, Vector2Int pos, Vector2Int goal, CoordinateRouteInfo prevNode)
    {

        this.pos = pos;
        SetHCost(goal);

        if (prevNode != null)
        {
            this.prevNode = prevNode;
            SetGCost(start);
        }

        if(prevNode == null)
        {
            gCost = 0;
        }

        SetFCost();
        //Debug.Log("POS " + pos);
        //Debug.Log("Start " + start);
        //Debug.Log("Goal " + goal);

    }

    void SetGCost(Vector2Int start)
    {
        float dist = Vector2.Distance(prevNode.pos, pos);
        dist *= 10;
        dist += prevNode.gCost;
        gCost = Mathf.RoundToInt(dist);
    }

    void SetHCost(Vector2Int goal)
    {
        float dist = Vector2.Distance(goal, pos);
        hCost = Mathf.RoundToInt(dist * 10);
    }

    void SetFCost()
    {
        fCost = gCost + hCost;
    }

    public void DebugAllCostValues()
    {
        Debug.Log("F Cost : " + fCost);
        Debug.Log("G Cost : " + gCost);
        Debug.Log("H Cost : " + hCost);
    }

    public int GetFCost()
    {
        return fCost;
    }
}

public class TravelerController : MonoBehaviour {
    public Vector2Int goal;
    public Vector2Int pos;
    public bool waiting;

    public List<Vector2Int> route;
    public bool routeSet;
    public int onRouteNode;

    public List<CoordinateRouteInfo> openNodes;
    public List<CoordinateRouteInfo> closedNodes;
    public CoordinateRouteInfo current;
    public List<CoordinateRouteInfo> routeBackwards;

    public int totalSearch;
    public bool atGoal;
    public bool showSearch;
    public bool couldNotReachGoal;
    public float distToGoal;

    TimerEC waitTimer;
    public int timer;

    GameObject plrChar;
    EnemyControllerNew ec;
    public bool isReallyHere;
    public bool walking;
    int stepsTaken; //since last search
    AnimationManager anim;




    // Use this for initialization
    void Start () {
        anim = gameObject.transform.GetChild(0).GetComponent<AnimationManager>();
        ec = GetComponent<EnemyControllerNew>();
        plrChar = gameObject.transform.GetChild(0).gameObject;
        waitTimer = new TimerEC(0.75f);
        UpdatePosAsInt();

        totalSearch = 0;
        MapDataController.map[pos.x, pos.y].SetNpc(ec.pinfo);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //print(MapDataController.map[pos.x, pos.y].GetNpc());
        if (walking)
        {

            if(onRouteNode < route.Count)
            {
                WalkToNode();
            }

            if(onRouteNode > route.Count)
            {
                ClearRoute();
                walking = false;
                atGoal = false;
                SetGoal();
            }
        }

        if(walking == false)
        {
            anim.AnimateSprite(false);
            if (MapDataController.plrSurrounded == false)
            {
                if (ec.attacking == false)
                {
                    if (couldNotReachGoal == true)
                    {
                        waiting = true;
                    }

                    if (waiting == true)
                    {
                        //print("WAITING");
                        Wait();
                    }

                    if (waiting == false)
                    {
                        if (!atGoal)
                        {
                            if (stepsTaken >= 20)
                            {
                                stepsTaken = 0;
                                ClearRoute();
                                SetGoal();
                                FindPath();
                            }
                        }

                        if (routeSet == false)
                        {
                            SetGoal();
                            FindPath();

                            if(route.Count == 0)
                            {
                                couldNotReachGoal = true;
                            }
                        }

                        if (routeSet == true)
                        {
                            CheckEachNodeInRoute();
                        }

                        if(atGoal)
                        {
                            CheckIfWithinReach();
                        }
                    }
                }
            }
        }
    }
        
    void CheckIfWithinReach()
    {
        float dist = Vector2.Distance(goal, pos);
        if (dist > 1.5f)
        {
            SetGoal();
            atGoal = false;
            walking = false;
            ClearRoute();
        }
    }

void FindPath()
    {
        MapCoordinate[,] map = MapDataController.map;
        AddOpenNode(pos.x, pos.y);
        current = openNodes[0];
        int searches = 0;

        while (openNodes.Count > 0)
        {
            if(MapDataController.plrSurrounded == true)
            {
                break;
            }

            int IndexWithLowestFCost = SearchForLowestOpenFCost();
            current = openNodes[IndexWithLowestFCost];

            RemoveOpenNode(IndexWithLowestFCost);
            AddClosedNode(current);

            if (closedNodes.Count >= (MapDataController.area))
            {
                waiting = true;
                return;
            }

            if (current.pos == goal)
            {
                SetRoute();
                return;
            }

            if (searches > (MapDataController.area / 4))
            {
                SetRoute();
                break;
            }

            for (int i = 1; i <= 9; i++)
            {
                if (i == 5)
                {
                    i++;
                }

                Vector2Int neightbourPos = GetNeightbour(current.pos, i);
                //print(current.pos + " Current");
                //print(neightbourPos + " Neightbour " + i);
                CoordinateRouteInfo neightbour = new CoordinateRouteInfo(pos, neightbourPos, goal, current);

                bool SkipToNextNeightbour = false;
                if (SkipToNextNeightbour == false)
                {
                    //if neightbour is not traversable
                    int neightbourPosType = map[neightbourPos.x, neightbourPos.y].GetType();
                    PlayerInfo neightbourPosNpc = map[neightbourPos.x, neightbourPos.y].GetNpc();
                    int neightbourTeam = 2;

                    if (neightbourPosNpc != null)
                    {
                        neightbourTeam = neightbourPosNpc.GetTeam();
                    }

                    if (neightbourPosType == 1 || (neightbourPosNpc != null && neightbourTeam == 2))
                    {
                        SkipToNextNeightbour = true;
                    }

                    if (SkipToNextNeightbour == false)
                    {
                        //or neightbour is in closed
                        bool neightbourIsInClosed = CheckIfNeightboorIsInClosed(neightbourPos);
                        if (neightbourIsInClosed)
                        {
                            SkipToNextNeightbour = true;
                        }
                    }
                }



                bool stepTwoControl = false;
                if (stepTwoControl == false && SkipToNextNeightbour == false)
                {
                    //if new path to neightbour is shorter
                    if (neightbour.fCost < current.fCost)
                    {
                        openNodes.Add(neightbour);
                        stepTwoControl = true;
                    }
                    if (stepTwoControl == false)
                    {
                        //or neightbour is not in open
                        bool neightbourIsInOpen = CheckIfNeightboorIsInOpen(neightbourPos);
                        if (!neightbourIsInOpen)
                        {
                            openNodes.Add(neightbour);
                            stepTwoControl = true;
                        }
                    }
                }

                if (showSearch)
                {
                    MarkopenNodessWithColor();
                    MarkClosedNodessWithColor();
                    MarkCurrentNodeWithColor();
                }

                totalSearch++;
                searches ++;
            }
        }
    }

    int SearchForLowestOpenFCost()
    {
        //gets all the fCost values from the open list
        int fCost;
        int lowestFCost = 0;
        int lowestFCostIndex = 0;

        for(int i = 0; i < openNodes.Count; i++)
        {
            fCost = openNodes[i].GetFCost();

            if(i == 0)
            {
                lowestFCost = fCost;
            }

            if(i > 0)
            {
                if(fCost < lowestFCost)
                {
                    lowestFCostIndex = i;
                    lowestFCost = openNodes[lowestFCostIndex].fCost;
                }
            }
        }
        return lowestFCostIndex;
    }

    void AddClosedNode(CoordinateRouteInfo current)
    {
        closedNodes.Add(current);
    }

    void RemoveOpenNode(int i)
    {
        openNodes.Remove(openNodes[i]);
    }

    void AddOpenNode(int x, int y)
    {
        Vector2Int searchPos = new Vector2Int(x, y);
        int posType = MapDataController.map[pos.x, pos.y].GetType();

        if (posType == 0)
        {
            CoordinateRouteInfo cri = new CoordinateRouteInfo(pos, searchPos, goal, null);
            openNodes.Add(cri);
        }
    }
    void UpdatePosAsInt()
    {
        pos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        ec.pinfo.SetPos(pos);
    }
    void SetGoal()
    {
        int x = PlayerController.pos.x;
        int y = PlayerController.pos.y;
        goal = new Vector2Int(x, y);
    }

    void CheckEachNodeInRoute()
    {
        if(route.Count > 0)
        {
            TurnToPlayer();
            MapCoordinate[,] map = MapDataController.map;
            Vector2Int nextStep = route[onRouteNode];
            PlayerInfo npc;
            npc = map[nextStep.x, nextStep.y].GetNpc();
            if (npc == null)
            {
                map[pos.x, pos.y].RemoveNpc();
                RefreshPosAsInt(route[onRouteNode]);
                map[pos.x, pos.y].SetNpc(ec.pinfo);

                walking = true;
            }

            if (npc != null && npc != ec.pinfo)
            {
                walking = false;
                ClearRoute();
            }
        }

        if(route.Count == 0)
        {
            walking = false;
            ClearRoute();
        }
    }

    void WalkToNode()
    {
        anim.AnimateSprite(true);
        Vector3 tempGoal = new Vector3(route[onRouteNode].x, route[onRouteNode].y, -1);
        transform.position = Vector3.MoveTowards(transform.position, tempGoal, ec.pinfo.baseStats.stepSpeed);
        if(transform.position == tempGoal)
        {
            walking = false;
            if(onRouteNode < route.Count-1)
            {
                stepsTaken++;
                onRouteNode++;
                return;
            }

            if(onRouteNode == route.Count-1)
            {
                ClearRoute();
                Vector2Int pos = new Vector2Int(0,0);
                pos.x = Mathf.RoundToInt(tempGoal.x);
                pos.y = Mathf.RoundToInt(tempGoal.y);
                SetGoal();


                if (pos != goal)
                {
                    float dist = Vector2.Distance(goal, pos);
                    if(dist > ec.pinfo.stats.range)
                    {
                        couldNotReachGoal = true;
                    }
                }

                if(route.Count == 0)
                {
                    couldNotReachGoal = true;
                }
            }
        }
    }

    void RefreshPosAsInt(Vector2Int newPos)
    {
        pos.x = newPos.x;
        pos.y = newPos.y;

        ec.pinfo.SetPos(pos);
    }

    void MarkRouteBackwardsWithColor()
    {
        for (int i = 0; i < routeBackwards.Count; i++)
        {
            Vector2Int holder = routeBackwards[i].GetPos();

            MapDataController.terrainSprites[holder.x, holder.y].color = Color.black;
        }
    }

    void MarkRouteWithColor()
    {
        for (int i = 0; i < route.Count; i++)
        {
            Vector2Int holder = route[i];

            MapDataController.terrainSprites[holder.x, holder.y].color = Color.white;
        }
    }

    void MarkTargetNodeWithColor(Color color, Vector2Int p)
    {
        Vector2Int holder = p;
        MapDataController.terrainSprites[holder.x, holder.y].color = color;
    }

    void MarkCurrentNodeWithColor()
    {
        Vector2Int holder = current.GetPos();
        MapDataController.terrainSprites[holder.x, holder.y].color = Color.cyan;
    }

    void MarkopenNodessWithColor()
    {
        for(int i = 0; i < openNodes.Count; i++)
        {
            Vector2Int holder = openNodes[i].GetPos();

            MapDataController.terrainSprites[holder.x, holder.y].color = Color.blue;
        }
    }

    void MarkClosedNodessWithColor()
    {
        for (int i = 0; i < closedNodes.Count; i++)
        {
            Vector2Int holder = closedNodes[i].GetPos();

            MapDataController.terrainSprites[holder.x, holder.y].color = Color.red;
        }
    }

    Vector2Int GetNeightbour(Vector2Int curPos,int i)
    {
        int difX = 0;
        int difY = 0;

        switch(i)
        {
            case 1:
                difX = -1;
                difY = -1;
                break;

            case 2:
                difX = 0;
                difY = -1;
                break;

            case 3:
                difX = +1;
                difY = -1;
                break;

            case 4:
                difX = -1;
                difY = 0;
                break;

            case 6:
                difX = 1;
                difY = 0;
                break;

            case 7:
                difX = -1;
                difY = 1;
                break;

            case 8:
                difX = 0;
                difY = 1;
                break;

            case 9:
                difX = 1;
                difY = 1;
                break;
        }

        Vector2Int neightBoor = new Vector2Int(curPos.x + difX, curPos.y + difY);
        return neightBoor;
    }

    bool CheckIfNeightboorIsInClosed(Vector2Int neightboor)
    {

        for(int i = 0; i < closedNodes.Count; i++)
        {
            if(closedNodes[i].pos == neightboor)
            {
                return true;
            }
        }
        return false;
    }

    bool CheckIfNeightboorIsInOpen(Vector2Int neightboor)
    {

        for (int i = 0; i < openNodes.Count; i++)
        {
            if (openNodes[i].pos == neightboor)
            {
                return true;
            }
        }
        return false;
    }

    void BuildRouteBackwards()
    {
        int i = 0;

        if(current.prevNode != null)
        {
            goal = current.prevNode.pos;
            routeBackwards.Add(current.prevNode);
        }

        if(current.prevNode == null)
        {
            routeBackwards.Add(current);
        }


        while (routeBackwards[i].prevNode != null)
        {
            CoordinateRouteInfo prev = routeBackwards[i].prevNode;
            routeBackwards.Add(prev);
            i++;
        }
    }

    void BuildRoute()
    {
        for (int i = routeBackwards.Count-2; i >= 0; i--)
        {
            Vector2Int curPos = routeBackwards[i].GetPos();
            route.Add(curPos);
        }
    }

    void Wait()
    {
        waitTimer.Countdown();
        float timeLeft = waitTimer.GetCurTimerInSeconds();
        if(timeLeft<0)
        {
            couldNotReachGoal = false;
            waiting = false;
            waitTimer.ResetTimer();
            ClearRoute();
        }
    }

    void SetRoute()
    {
        BuildRouteBackwards();
        BuildRoute();
        if(showSearch)
        {
            MarkRouteWithColor();
        }
        routeSet = true;
        if(current.pos == pos)
        {
            //print("COult not reach goal");
            couldNotReachGoal = true;
        }
    }

    public void ClearRoute()
    {
        route.Clear();
        routeBackwards.Clear();
        openNodes.Clear();
        closedNodes.Clear();
        routeSet = false;
        onRouteNode = 0;
    }

    void TurnToPlayer()
    {
        string dir = "";
        if (goal.x > pos.x)
        {
            dir = "r";
        }

        if (goal.x < pos.x)
        {
            dir = "l";
        }

        if (goal.y < pos.y)
        {
            dir = dir+ "d";
        }

        if (goal.y > pos.y)
        {
            dir = dir + "u";
        }

        Rotate(dir);
    }

    void Rotate(string dir)
    {
        //print(dir);

        Quaternion rot = plrChar.transform.rotation;
        float rotZ = rot.eulerAngles.z;

        switch (dir)
        {
            case "u":
                //rotZ = 180;
                anim.SetSpriteLoopValues(8, 11,"walking_up");
                break;

            case "l":
                //rotZ = 270;
                anim.SetSpriteLoopValues(4, 7, "walking_left");
                break;

            case "d":
                //rotZ = 0;
                anim.SetSpriteLoopValues(0, 3, "walking_down");
                break;

            case "r":
                //rotZ = 90;
                anim.SetSpriteLoopValues(4, 7, "walking_right");
                break;

            case "lu":
                //rotZ = 225;
                //anim.SetSpriteLoopValues(8, 11, "walking_up");
                break;

            case "ld":
                //rotZ = 325;
                //anim.SetSpriteLoopValues(0, 3, "walking_down");
                break;

            case "ru":
                //rotZ = 135;
                //anim.SetSpriteLoopValues(8, 11, "walking_up");
                break;

            case "rd":
                //rotZ = 45;
                //anim.SetSpriteLoopValues(0, 3, "walking_down");
                break;
        }

        //rot = Quaternion.Euler(0, 0, rotZ);
        plrChar.transform.rotation = rot;
        
    }
}
