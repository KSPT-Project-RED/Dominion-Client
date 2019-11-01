using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardInfo : MonoBehaviour
{

    public CardStruct card;
    public Image logo;
    public TextMeshProUGUI name, attack, defense;
    public TextMeshProUGUI description;
    public GameObject HideObj, HighlitedObject;
    public bool isPlayer;

    public void HideInfoCard(CardStruct card)
    {
        this.card = card;
        HideObj.SetActive(true);
        isPlayer = false;
  
    }

    public void ShowCardInfo(CardStruct card, bool isPlayer)
    {
        this.isPlayer = isPlayer;
        HideObj.SetActive(false);
        this.card = card;
        this.logo.sprite = card.logo;
        logo.preserveAspect = true;
        this.name.text = card.name;
        this.description.text = card.description;
        this.attack.text = card.attack.ToString();
        this.defense.text = card.defense.ToString();
    }

    public void RefreshData()
    {
        this.attack.text = card.attack.ToString();
        this.defense.text = card.defense.ToString();
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
