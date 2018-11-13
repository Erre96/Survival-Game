using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetsLibrary : MonoBehaviour {
    public Sprite[] hpBar;
    public Sprite[] itemSpr;
    public Sprite[] itemSprMrk;
    public static Sprite[] itemBar = new Sprite[2];
    public Sprite[] markBox;
    public Sprite[] hiscoreBar;

    public Image saveGameButtonImage;
    public Image ContinueButtonImage;
    public Text saveGameButtonText;
    public Text ContinueButtonText;

    public Sprite[] abilityIcons;

    public GameObject dmgText;
    public GameObject textOutput;
    public GameObject plr;

    //Ability objects
    public GameObject mine;
    public GameObject fireball;
    public GameObject plrSpear;

    public Canvas canvas;
    private GameObject[] UI = new GameObject[5];
    Spawner spawner;

    private static int placeOrder;  //Is used to make the text appear next to another text so it won't override the position
    private static float textDifX;
    private static float textDifY;

    public SellingController sc;

    // Use this for initialization
    void Start () {
        SetHpSprites();
        spawner = GetComponent<Spawner>();
        UI[0] = canvas.gameObject.transform.GetChild(1).gameObject;
        UI[1] = canvas.gameObject.transform.GetChild(2).gameObject;
        UI[2] = canvas.gameObject.transform.GetChild(3).gameObject;
        UI[3] = canvas.gameObject.transform.GetChild(4).gameObject;
        UI[4] = canvas.gameObject.transform.GetChild(5).gameObject;
        itemBar[0] = null;
        itemBar[1] = Resources.Load<Sprite>("GUI/ItemBarMrk");
    }

	// Update is called once per frame
	void Update () {
		
	}

    public void SwapUI(int target)
    {
        for (int i = 0; i < UI.Length; i++)
        {
            if(target != 4)
            {
                Time.timeScale = 0.0f;
            }

            if(target == 4)
            {
                Time.timeScale = 1;
            }

            if (target == 3)
            {
                PlayerController pcon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
                pcon.pinfo.CalculateAll();
            }

            if (target == 2)
            {
                if(sc.itemDataInfo != null)
                {
                    sc.RefreshItemImage();
                    sc.MarkItem(sc.markedItem);
                }
            }

            if (target == 0)
            {
                SetButtonStatuses();
            }

            if(i == target)
            {
                UI[i].SetActive(true);
            }

            if(i != target)
            {
                UI[i].SetActive(false);
            }
        }
    }
    public void CreateTextOutput()
    {
        Debug.Log("ooooooo");
        GameObject.Instantiate(textOutput,plr.transform.position, Quaternion.identity);
    }

    public void SetButtonStatuses()
    {
        if(gi.ec.Count > 0)
        {
            SetSaveButtonStatus(false);
            SetContinueButtonStatus(false);
        }

        if (gi.ec.Count == 0)
        {
            SetSaveButtonStatus(true);
            SetContinueButtonStatus(true);
        }
    }
    public void SetSaveButtonStatus(bool canSave)
    {
        if(canSave == false)
        {
            DataTransferManager.canSave = false;
            saveGameButtonImage.color = new Color32(0, 0, 255, 50);
            saveGameButtonText.color = new Color32(0, 0, 255, 50);
        }

        if(canSave == true)
        {
            DataTransferManager.canSave = true;
            saveGameButtonImage.color = new Color32(0, 9, 96, 255);
            saveGameButtonText.color = new Color32(255, 255, 255, 255);
        }
    }

    public void SetContinueButtonStatus(bool canContinue)
    {
        if(canContinue == false)
        {
            ContinueButtonImage.color = new Color32(0, 0, 255, 50);
            ContinueButtonText.color = new Color32(0, 0, 255, 50);
        }

        if (canContinue == true)
        {
            ContinueButtonImage.color = new Color32(0, 9, 96, 255);
            ContinueButtonText.color = new Color32(255, 255, 255, 255);
        }
    }

    public void SetHpSprites()
    {
        for(int i = 0; i < hpBar.Length; i++)
        {
            hpBar[i] = Resources.Load<Sprite>("Healthbar/25x/"+i);
        }
    }

    public void CreateDamageText(Vector3 pos, int value)
    {
        SetDifX();
        pos.x += textDifX;
        pos.y += textDifY;
        gi.dmg = value;
        GameObject.Instantiate(dmgText, pos, Quaternion.identity);
    }

    public static void SetDifX()
    {

        switch (placeOrder)
        {
            case 0:
                textDifX = 0;
                textDifY = 0;
                placeOrder = 1;
                break;
            case 1:
                textDifY = 1f;
                placeOrder = 2;
                break;
            case 2:
                textDifY = 2f;
                placeOrder = 0;
                break;

            case 3:
                textDifX = 1;
                textDifY = 0;
                placeOrder = 1;
                break;
            case 4:
                textDifY = 1f;
                placeOrder = 2;
                break;
            case 5:
                textDifY = 2f;
                placeOrder = 0;
                break;
        }
    }

    public void CreateMine(Vector2 placeHere)
    {
        Instantiate(mine, placeHere, Quaternion.identity);
    }

    public void CreateFireball(Vector2 placeHere)
    {
        Instantiate(fireball, placeHere, Quaternion.identity);
    }
}
