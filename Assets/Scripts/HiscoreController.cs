using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hiscore
{
    public string[] name = new string[3];
    public int[] value = new int[3];
}

public class HiscoreController : MonoBehaviour {
    public static bool newHiscoreEntry;
    public static bool scoreBeaten;
    public static int scoreBeatenIndex;
    public static Image[] hiscoreBar = new Image[3];
    Text[] hiscoreName = new Text[3];
    Text[] hiscoreValue = new Text[3];
    Text currentWaveText;
    GameObject scoreHeader;

	// Use this for initialization
	void Start () {
        SetRefs();
        SetHiscore();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void SetRefs()
    {
        for(int i = 0; i < hiscoreBar.Length; i++)
        {
            hiscoreBar[i] = gameObject.transform.GetChild(i).GetComponent<Image>();
            hiscoreName[i] = hiscoreBar[i].gameObject.transform.GetChild(0).GetComponent<Text>();
            hiscoreValue[i] = hiscoreBar[i].gameObject.transform.GetChild(1).GetComponent<Text>();
        }

        scoreHeader = gameObject.transform.GetChild(3).gameObject;
        currentWaveText = scoreHeader.transform.GetChild(0).GetComponent<Text>();
    }

    void SetHiscore()
    {
        if(newHiscoreEntry)
        {
            scoreHeader.SetActive(true);
            currentWaveText.text = gi.wave-1 + " Waves";
            DataTransferManager.dataHolder.SetDefaultValues();
            if(scoreBeaten)
            {
                UnmarkBars();
                MarkNewScore(scoreBeatenIndex);
            }
        }

        if (!newHiscoreEntry)
        {
            scoreHeader.SetActive(false);
        }

        DataTransferManager.ReadHiscore();
        for(int i = 0; i < hiscoreName.Length; i++)
        {
            hiscoreName[i].text = DataTransferManager.hiscore.name[i];
            hiscoreValue[i].text = DataTransferManager.hiscore.value[i].ToString();
        }

        newHiscoreEntry = false;
    }

    public static void UpdateHiscore()
    {
        newHiscoreEntry = true;
        DataTransferManager.ReadHiscore();
        int cs = gi.wave-1;
        string name = DataTransferManager.dataHolder.name;

        bool stopLoop = false;

        for(int i = 0; i < 3; i++)
        {
            if(cs > DataTransferManager.hiscore.value[i])
            {
                if(stopLoop == true)
                {
                    break;
                }

                scoreBeaten = true;
                scoreBeatenIndex = i;

                int formerValue = DataTransferManager.hiscore.value[i];
                string formerName = DataTransferManager.hiscore.name[i];

                DataTransferManager.hiscore.value[i] = cs;
                DataTransferManager.hiscore.name[i] = name;

                for (int j = i + 1; j < 3; j++)
                {
                    int tempValue = DataTransferManager.hiscore.value[j];
                    string tempName = DataTransferManager.hiscore.name[j];
                    DataTransferManager.hiscore.value[j] = formerValue;
                    DataTransferManager.hiscore.name[j] = formerName;
                    formerValue = tempValue;
                    formerName = tempName;
                    stopLoop = true;
                }
            }
        }
    }

    public static void MarkNewScore(int i)
    {
        AssetsLibrary al = GameObject.FindGameObjectWithTag("Assets").GetComponent<AssetsLibrary>();
        hiscoreBar[i].sprite = al.hiscoreBar[1];
    }

    public static void UnmarkBars()
    {
        for(int i = 0; i < hiscoreBar.Length; i++)
        {
            AssetsLibrary al = GameObject.FindGameObjectWithTag("Assets").GetComponent<AssetsLibrary>();
            hiscoreBar[i].sprite = al.hiscoreBar[0];
        }
    }
}
