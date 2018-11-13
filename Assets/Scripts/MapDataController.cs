using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCoordinate
{
    int type; // 0 = traversable, 1 = obstacle
    PlayerInfo npc;

    public void SetType(int type)
    {
        this.type = type;
    }

    public int GetType()
    {
        return this.type;
    }

    public void SetNpc(PlayerInfo npc)
    {
        this.npc = npc;
    }

    public PlayerInfo GetNpc()
    {
        return npc;
    }

    public void RemoveNpc()
    {
        npc = null;
    }
}
public class MapDataController : MonoBehaviour
{
    public static bool plrSurrounded;
    public string adress;
    public static MapCoordinate[,] map;
    public static SpriteRenderer[,] terrainSprites;
    public static int width;
    public static int height;
    public static int area;

    public GameObject spawnPoint;

    public GameObject tree;
    public GameObject grass;
    public GameObject water;
    public GameObject trail;
    public GameObject dessert;
    public GameObject jetty;

    int timer = 30;
    PlayerController pcon;
    public static int mult = 2;

    // Use this for initialization

    // Update is called once per frame
    void Start()
    {
        CreateMap();

        pcon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        if (gi.firstStage)
        {
            pcon.InitializeData();
            ActionButtonController abc = GameObject.FindGameObjectWithTag("ActionButtonController").GetComponent<ActionButtonController>();
            abc.SetIcons();
            gi.firstStage = false;
        }

        pcon.abc.ResetTimers();
        pcon.abc.RefreshPotsAmountText();
        Spawner spawner = GameObject.FindGameObjectWithTag("Assets").GetComponent<Spawner>();
        spawner.enabled = true;

        pcon.gameObject.transform.position = gi.startPos;
        int x = Mathf.RoundToInt(gi.startPos.x);
        int y = Mathf.RoundToInt(gi.startPos.y);
        Vector2Int pos = new Vector2Int(x, y);
        pcon.RefreshPosAsInt(pos);
        pcon.UpdateMapPos(x, y);
    }

    void Update()
    {
        if(map.Length > 0)
        {
            CheckIfPlayerSurrounded();
            //RevealPositions();
        }
    }

    void RevealPositions()
    {
        timer--;
        if (timer < 0)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    PlayerInfo npc = map[x, y].GetNpc();
                    if (npc != null)
                    {
                        MarkTargetNodeWithColor(Color.blue, new Vector2Int(x, y));
                    }
                    if (npc == null)
                    {
                        MarkTargetNodeWithColor(Color.white, new Vector2Int(x, y));
                    }
                }
            }
            timer = 1;
        }
    }

    void SetWorldSize(int x, int y)
    {
        height = y;
        width = x;
        area = width * height;
        print(area);
        map = new MapCoordinate[width, height];
        terrainSprites = new SpriteRenderer[width, height];

    }
    public void CreateMap()
    {
        Texture2D mapTexture = Resources.Load<Texture2D>("Maps/"+adress);
        Texture2D spawnPointsTexture = Resources.Load<Texture2D>("Maps/"+adress+"_sp");

        int x = mapTexture.width;
        int y = mapTexture.height;
        SetWorldSize(x,y);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                map[i, j] = new MapCoordinate();
                Color32 color = mapTexture.GetPixel(i, j);
                CreateObjectAtCoordinate(color, i, j);

                Color32 sp_color = spawnPointsTexture.GetPixel(i, j);
                CreateSpawnPointAtCoordinate(sp_color, i, j);
            }
        }
    }

    void MarkTargetNodeWithColor(Color color, Vector2Int p)
    {
        Vector2Int holder = p;
        MapDataController.terrainSprites[holder.x, holder.y].color = color;
    }


    public void CreateSpawnPointAtCoordinate(Color32 color, int x, int y)
    {
        int r = color.r;
        int g = color.g;
        int b = color.b;

        if (r == 0 && g == 0 && b == 0) //enemy spawn
        {
            Instantiate(spawnPoint, new Vector3(x, y, 0), Quaternion.identity);
        }

        if (r == 255 && g == 0 && b == 0) //player position
        {
            gi.startPos = new Vector3(x, y,-1);
            //print("STARTPOS" + gi.startPos);
        }
    }
    public void CreateObjectAtCoordinate(Color32 color, int x, int y) //takes the color values and creates the corresponding object
    {
        int r = color.r;
        int g = color.g;
        int b = color.b;
        //print("red : "+r+"  green : "+g+"   blue : "+b + "      Grid : "+x+","+y);

        if (r == 64 && g == 192 && b == 0) //grass
        {
            Instantiate(grass, new Vector3(x, y, 0), Quaternion.identity);
            map[x, y].SetType(0);
        }

        if (r == 32 && g == 96 && b == 0) //trees
        {
            Instantiate(tree, new Vector3(x, y, 0), Quaternion.identity);
            Instantiate(grass, new Vector3(x, y, 0), Quaternion.identity);
            map[x, y].SetType(1);
        }

        if (r == 128 && g == 64 && b == 64) //forest trail
        {
            Instantiate(trail, new Vector3(x, y, 0), Quaternion.identity);
            map[x, y].SetType(0);
        }

        if (r == 255 && g == 255 && b == 0) //Dessert
        {
            Instantiate(dessert, new Vector3(x, y, 0), Quaternion.identity);
            map[x, y].SetType(0);
        }

        if (r == 192 && g == 128 && b == 64) //Jetty
        {
            Instantiate(jetty, new Vector3(x, y, 0), Quaternion.identity);
            map[x, y].SetType(0);
        }

        if (r == 0 && g == 160 && b == 192) //water
        {
            Instantiate(water, new Vector3(x, y, 0), Quaternion.identity);
            map[x, y].SetType(0);
        }
        /*
        if (r == 224 && g == 128 && b == 64) //nodes
        {
            Instantiate(node, new Vector3(x, y, 0), Quaternion.identity);
            nodes[x, y] = 2;
        }*/
    }

    void CheckIfPlayerSurrounded()
    {
        bool doSearch = true;
        Vector2 pos = pcon.pinfo.GetPos();
        int posX = Mathf.RoundToInt(pos.x);
        int posY = Mathf.RoundToInt(pos.y);

        for (int x = posX -1; x <= posX+1; x++)
        {
            for(int y = posY -1; y <= posY+1; y++)
            {
                PlayerInfo onPos = map[x, y].GetNpc();

                int type = map[x, y].GetType();

                if (type == 0) //if any surrounding node is empty, we are obviously not surrounded
                {
                    if (onPos == null)
                    {
                        //print("NOT SURROUNDED");
                        doSearch = false;
                        plrSurrounded = false;
                        break;
                    }
                }

                if(doSearch == true)
                {
                    //when loop is at the last search
                    if (x == posX + 1 && y == posY + 1)
                    {
                        if (type == 1)
                        {
                            plrSurrounded = true;
                            break;
                        }

                        if (type == 0)
                        {
                            if (onPos != null)
                            {
                                plrSurrounded = true;
                                //print("SURROUNDED");
                            }

                            if (onPos == null)
                            {
                                //print(x + "   " + y);
                                plrSurrounded = false;
                            }
                        }
                    }
                }
            }
        }
    }
}