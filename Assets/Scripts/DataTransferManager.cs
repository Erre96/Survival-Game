using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;

[Serializable]
public class Inventory
{
    public Item[] items = new Item[6];
}

[Serializable]
public class DataHolder
{
    public string name;
    public int weapon;

    public int skillPoints;
    public int[] passiveLv = new int[5];
    public int totalAbilities;

    public int[] abilId;
    public int[] abilLv;

    public DataHolder()
    {
        SetDefaultValues();
    }

    public void SetDefaultValues()
    {
        name = "";
        weapon = 0;
        skillPoints = 0;
        totalAbilities = 2;

        abilId = new int[totalAbilities];
        abilLv = new int[totalAbilities];

        ResetPassives();
        ResetAbilities();
    }

    void ResetPassives()
    {
        for (int i = 0; i < passiveLv.Length; i++)
        {
            passiveLv[i] = 1;
        }
    }

    void ResetAbilities()
    {
        for (int i = 0; i < totalAbilities; i++)
        {
            abilId[i] = 0;
            abilLv[i] = 0;
        }
    }
}



public class DataTransferManager : MonoBehaviour {
    public static bool canSave;
    public static bool gameLoaded;
    public static Inventory inv;
    public static DataHolder dataHolder = new DataHolder();
    public static Hiscore hiscore = new Hiscore();
    public string invPath; 
    public string specsPath;
    public static string hiscorePath;

    private void Start()
    {
        invPath = Application.persistentDataPath + "/Inventory.xml";
        specsPath = Application.persistentDataPath + "/Specs.xml";
        hiscorePath = Application.persistentDataPath + "/Hiscore.xml";
    }

    private void OnMouseDown() //used only in loading
    {
        DeserializeXML_Inventory();
        ReadXMLspecs();
        gameLoaded = true;
    }

    public void Work (bool write) {
        if(write)
        {
            if(canSave == true)
            {
                //SaveBaseStats();
                SaveInventory();
                SaveSpecs();
                ReadXMLspecs();

                Spawner sp = GameObject.FindGameObjectWithTag("Assets").GetComponent<Spawner>();
                sp.sm.SetMessage("Game Saved");
            }
        }

        if(!write)
        {
            gi.canContinue = true;
            DeserializeXML_Inventory();
            ReadXMLspecs();
            gameLoaded = true;
        }
    }

    // Update is called once per frame
    public void SaveBaseStats()
    {
        PlayerController pcon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        PlayerInfo pinfo = pcon.pinfo;
        BaseStats baseStats = pinfo.baseStats;

        System.Xml.Serialization.XmlSerializer writer =
            new System.Xml.Serialization.XmlSerializer(typeof(BaseStats));

        var path = "PlayerData/"+"BaseStats.xml";
        System.IO.FileStream file = System.IO.File.Create(path);

        writer.Serialize(file, baseStats);
        file.Close();
    }

    public void SaveInventory()
    {
        PlayerController pcon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        PlayerInfo pinfo = pcon.pinfo;
        inv = new Inventory();
        for(int i = 0; i < inv.items.Length; i++)
        {
            inv.items[i] = pinfo.items[i];
        }

        XmlSerializer writer =
    new XmlSerializer(typeof(Inventory));
        var path = "PlayerData/Inventory.xml";
        System.IO.FileStream file = System.IO.File.Create(invPath);

        writer.Serialize(file, inv);
        file.Close();
    }


    public void SaveSpecs()
    {
        PlayerController pcon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        PlayerInfo pinfo = pcon.pinfo;
        String name = pinfo.name;
        int weapon = pinfo.stats.GetWeapon();

        int[] abilID = new int[pinfo.abilities.Length];

        for (int i = 0; i < pinfo.abilities.Length; i++)
        {
            abilID[i] = pinfo.abilities[i].GetID();
        }

        XmlTextWriter xWriter = new XmlTextWriter(specsPath, System.Text.Encoding.UTF8);
        xWriter.Formatting = Formatting.Indented;
        xWriter.WriteStartElement("Specs");

        xWriter.WriteStartElement("Name");
        xWriter.WriteString(pinfo.name);
        xWriter.WriteEndElement();

        xWriter.WriteStartElement("Gold");
        xWriter.WriteString(gi.gold.ToString());
        xWriter.WriteEndElement();

        xWriter.WriteStartElement("Weapon");
        xWriter.WriteString(weapon.ToString());
        xWriter.WriteEndElement();

        xWriter.WriteStartElement("Skillpoints");
        xWriter.WriteString(pinfo.skillPoints.ToString());
        xWriter.WriteEndElement();

        xWriter.WriteStartElement("Level");
        xWriter.WriteString(gi.wave.ToString());
        xWriter.WriteEndElement();

        xWriter.WriteStartElement("Stage");
        xWriter.WriteString(gi.stage.ToString());
        xWriter.WriteEndElement();

        //Abilities
        xWriter.WriteStartElement("Abilities");
        for (int i = 0; i < pinfo.abilities.Length; i++)
        {
            int level = pinfo.abilities[i].GetLevel();
            int id = pinfo.abilities[i].GetID();
            xWriter.WriteStartElement("Ability");
            xWriter.WriteElementString("ID",id.ToString());
            xWriter.WriteElementString("Level", level.ToString());
            xWriter.WriteEndElement();
        }
        xWriter.WriteEndElement();


        //Passives
        xWriter.WriteStartElement("Passives");
        for (int i = 0; i < pinfo.passiveSkills.Length; i++)
        {
            int level = pinfo.passiveSkills[i].level;
            string header = pinfo.passiveSkills[i].name;

            xWriter.WriteStartElement(header);
            xWriter.WriteElementString("Level", level.ToString());
            xWriter.WriteEndElement();
        }
        xWriter.WriteEndElement();


        xWriter.WriteEndElement();
        xWriter.Close();
    }









    public void DeserializeXML_Inventory ()
    {
        if(File.Exists(invPath))
        {
            XmlSerializer reader = new XmlSerializer(typeof(Inventory));
            System.IO.StreamReader file = new System.IO.StreamReader(invPath);
            inv = (Inventory)reader.Deserialize(file);

            file.Close();
        }
    }

    public void ReadXMLspecs()
    {
        if(File.Exists(specsPath))
        {
            XmlDocument xd = new XmlDocument();
            xd.Load(specsPath);
            XmlNodeList nodelist = xd.SelectNodes("Specs");

            foreach (XmlNode node in nodelist) // for each <testcase> node
            {
                try
                {
                    dataHolder.name = node.SelectSingleNode("Name").InnerText;
                    gi.wave = int.Parse(node.SelectSingleNode("Level").InnerText);
                    gi.stage = int.Parse(node.SelectSingleNode("Stage").InnerText);
                    dataHolder.skillPoints = int.Parse(node.SelectSingleNode("Skillpoints").InnerText);
                    dataHolder.weapon = int.Parse(node.SelectSingleNode("Weapon").InnerText);
                    gi.gold = int.Parse(node.SelectSingleNode("Gold").InnerText);

                    //Abilities
                    XmlNode abilitiesNode = node.SelectSingleNode("Abilities");
                    XmlNodeList abilList = abilitiesNode.SelectNodes("Ability");

                    for (int i = 0; i < abilList.Count; i++)
                    {
                        XmlNode abil = abilList.Item(i);
                        dataHolder.abilId[i] = int.Parse(abil.SelectSingleNode("ID").InnerText);
                        dataHolder.abilLv[i] = int.Parse(abil.SelectSingleNode("Level").InnerText);
                    }


                    //Passives
                    XmlNodeList Passives = node.SelectNodes("Passives");

                    XmlNodeList attSpec = Passives.Item(0).SelectNodes("Attack");
                    XmlNodeList defSpec = Passives.Item(0).SelectNodes("Defense");
                    XmlNodeList magSpec = Passives.Item(0).SelectNodes("Magic");
                    XmlNodeList vitSpec = Passives.Item(0).SelectNodes("Vitality");
                    XmlNodeList engSpec = Passives.Item(0).SelectNodes("Energy");

                    dataHolder.passiveLv[0] = int.Parse(attSpec.Item(0).SelectSingleNode("Level").InnerText);
                    dataHolder.passiveLv[1] = int.Parse(defSpec.Item(0).SelectSingleNode("Level").InnerText);
                    dataHolder.passiveLv[2] = int.Parse(magSpec.Item(0).SelectSingleNode("Level").InnerText);
                    dataHolder.passiveLv[3] = int.Parse(vitSpec.Item(0).SelectSingleNode("Level").InnerText);
                    dataHolder.passiveLv[4] = int.Parse(engSpec.Item(0).SelectSingleNode("Level").InnerText);
                }
                catch (Exception ex)
                {

                }
            }
        } 
    }

    public static void SaveHiscore()
    {
        XmlTextWriter xWriter = new XmlTextWriter(hiscorePath, System.Text.Encoding.UTF8);
        xWriter.Formatting = Formatting.Indented;
        xWriter.WriteStartElement("Hiscores");

        for (int i = 0; i < 3; i++)
        {
            xWriter.WriteStartElement("Hiscore");
            xWriter.WriteElementString("Name", hiscore.name[i]);
            xWriter.WriteElementString("Value", hiscore.value[i].ToString());
            xWriter.WriteEndElement();
        }
        xWriter.WriteEndElement();
        xWriter.Close();
    }

    public static void ReadHiscore()
    {
        if(File.Exists(hiscorePath))
        {

            XmlDocument xd = new XmlDocument();
            xd.Load(hiscorePath);
            XmlNode collection = xd.SelectSingleNode("Hiscores");
            XmlNodeList nodelist = collection.SelectNodes("Hiscore");

            for (int i = 0; i < nodelist.Count; i++)
            {
                try
                {
                    //Abilities
                    hiscore.name[i] = nodelist.Item(i).SelectSingleNode("Name").InnerText;
                    string value = (nodelist.Item(i).SelectSingleNode("Value").InnerText);
                    hiscore.value[i] = int.Parse(value);


                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
