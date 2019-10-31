using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardInfo : MonoBehaviour
{

    public CardStruct card;
    public Image logo;
    public TextMeshProUGUI name;
    public TextMeshProUGUI description;

    public void HideInfoCard(CardStruct card)
    {
        //this.card = card;
        //this.logo.sprite = null;
        //this.name.text = "";
        //this.description.text = "";

        ShowCardInfo(card);
    }

    public void ShowCardInfo(CardStruct card)
    {
        this.card = card;
        this.logo.sprite = card.logo;
        logo.preserveAspect = true;
        this.name.text = card.name;
        this.description.text = card.description;
    }

    private void Start()
    {
        //ShowCardInfo(CardStructManager.allCardStructs[transform.GetSiblingIndex()] );
    }
}
