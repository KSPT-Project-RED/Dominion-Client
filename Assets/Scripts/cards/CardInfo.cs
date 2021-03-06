﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardInfo : MonoBehaviour
{

    public CardStruct card;
    public Image logo;
    public TextMeshProUGUI name, cost, number;
    public TextMeshProUGUI description;
    public GameObject HideObj, HighlitedObject;
    public bool isPlayer;

    public string getName()
    {
        return name.text;
    }

    public string getType()
    {
        return card.type;
    }

    public void HideInfoCard(CardStruct card)
    {
        this.card = card;
        HideObj.SetActive(true);
        isPlayer = false;
  
    }

    public void MinusNumber()
    {
        Debug.Log("MINUS");
        if (Int32.Parse(number.text) - 1 == 0)
        {
            this.gameObject.SetActive(false);

        }
        else
        {
            this.number.text = "" + (Int32.Parse(number.text) - 1);
        }
    }

    public void ShowCardInfo(CardStruct card)
    {
       // this.isPlayer = isPlayer;
        HideObj.SetActive(false);
        this.card = card;
        this.logo.sprite = card.logo;
        logo.preserveAspect = true;
        this.name.text = card.name;
        this.description.text = card.description;
        this.cost.text = card.cost.ToString();
        this.number.text = card.number.ToString();
    }

    public void RefreshData()
    {
        this.cost.text = card.cost.ToString();
        this.number.text = card.number.ToString();
    }

    public void HighlightCard()
    {
        HighlitedObject.SetActive(true);
    }

    public void DeHighlightCard()
    {
        HighlitedObject.SetActive(false);
    }
}
