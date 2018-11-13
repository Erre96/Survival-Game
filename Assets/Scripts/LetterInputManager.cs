using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterInputManager : MonoBehaviour
{
    Text text;
    bool capitalized;
    GameObject buttons;
    public Text[] letters;

    // Use this for initialization
    void Start()
    {
        text = GetComponent<Text>();
        capitalized = true;
        buttons = GameObject.FindGameObjectWithTag("Buttons").gameObject;
        letters = buttons.GetComponentsInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ManageInput(string textValue)
    {

        if (textValue == "back")
        {
            if (text.text.Length > 0)
            {
                text.text = text.text.Remove(text.text.Length - 1);
            }
            return;
        }

        if (textValue == "cap")
        {
            capitalized = !capitalized;

            if (capitalized == false)
            {
                CapKeyboardToLower();
            }
            else CapKeyboardToUpper();
            return;
        }

        if (textValue == "space")
        {
            text.text = text.text + " ";
            return;
        }


        if (text.text.Length < 15) //because max length for player name is 15 characters
        {
            if(capitalized)
            {
                text.text += textValue.ToUpper();
            }

            if(!capitalized)
            {
                text.text += textValue.ToLower();
            }
        }
        DataTransferManager.dataHolder.name = text.text;
    }

    void CapKeyboardToUpper()
    {
        for (int i = 10; i < letters.Length; i++)
        {
            if(letters[i].text.Length == 1)
            {
                //print(letters[i].text+ "    : TO UPPER");
                letters[i].text = letters[i].text.ToUpper();
            }
        }
    }

    void CapKeyboardToLower()
    {
        for (int i = 10; i < letters.Length; i++)
        {
            if (letters[i].text.Length == 1)
            {
                //print(letters[i].text +"    : TO LOWER");
                letters[i].text = letters[i].text.ToLower();
            }
        }
    }
}