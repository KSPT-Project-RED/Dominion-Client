using System.Collections;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using TMPro;
using UnityEngine;

public class DominionGame : MonoBehaviour
{

    public List<GameObject> prefabCards;
    public GameObject CardPref;
    public Transform PlayerHand;
    public List<CardInfo> PlayerHandCards = new List<CardInfo>();
    public TextMeshProUGUI coins, money, actions, buy;
    public TextMeshProUGUI dropNumber, deckNumber;

    private List<GameObject> tmpInst = new List<GameObject>();


    public void UpdateCurrentState(SFSObject dataObject)
    {
        money.text = "money: " + dataObject.GetInt("Money");
        actions.text = "actions: " + dataObject.GetInt("Action");
        buy.text = "buy: " + dataObject.GetInt("Buy");
    }

    public void InitPlayerCards(SFSObject dataObject)
    {
        ISFSArray hand = dataObject.GetSFSArray("hand");
        ISFSArray hideCards = dataObject.GetSFSArray("hide");
        ISFSArray dropCards = dataObject.GetSFSArray("drop");

        dropNumber.text = dropCards.Size().ToString();
        deckNumber.text = hideCards.Size().ToString();

        for (int i = 0; i < tmpInst.Count; i++)
        {
            Destroy(tmpInst[i]);
        }
        tmpInst.Clear();

        for (int i = 0; i < hand.Size(); i++)
        {
            GiveCardToHand(hand.GetSFSObject(i));
        }
    }

    public void buyCard(SFSObject dataObject)
    {
        Debug.Log(dataObject.GetUtfString("Name"));
        for (int i = 0; i < prefabCards.Count; i++)
        {
            Debug.Log(prefabCards[i].GetComponent<CardInfo>().getName());
            if (prefabCards[i].GetComponent<CardInfo>().getName().Equals(dataObject.GetUtfString("Name"))){
                prefabCards[i].GetComponent<CardInfo>().MinusNumber();
            }
        }
    }

    public void StartGame(ISFSArray cards, List<GameObject> prefabCards)
    {
        Debug.Log("Getting cards number: " + cards.Count);

        for(int i = 0; i < cards.Count && i< 10; i++)
        {
            CardStruct card = new CardStruct(cards.GetSFSObject(i).GetText("Name"),
                cards.GetSFSObject(i).GetText("Description"),
                "Sprites/Cards/"+cards.GetSFSObject(i).GetText("ImageId"),
                cards.GetSFSObject(i).GetInt("Cost"),
                10, cards.GetSFSObject(i).GetText("Type"),
                cards.GetSFSObject(i).GetInt("Money"),
                cards.GetSFSObject(i).GetInt("Action"),
                cards.GetSFSObject(i).GetInt("Buy"));

            this.prefabCards.Add(prefabCards[i]);
            prefabCards[i].GetComponent<CardInfo>().ShowCardInfo(card);
        }
    }

    private void GiveCardToHand(ISFSObject card)
    {
       
        GameObject cardGo = Instantiate(CardPref, PlayerHand, false);
        tmpInst.Add(cardGo);

        CardStruct tmpCard = new CardStruct(card.GetText("Name"),
                card.GetText("Description"),
                "Sprites/Cards/" + card.GetText("ImageId"),
                card.GetInt("Cost"),
                10, card.GetText("Type"),
                card.GetInt("Money"), card.GetInt("Action"), card.GetInt("Buy"));

        cardGo.GetComponent<CardInfo>().ShowCardInfo(tmpCard);

        //if (hand == EnemyHand)
        //{
        //    cardGo.GetComponent<CardInfo>().HideInfoCard(card);
        //    EnemyHandCards.Add(cardGo.GetComponent<CardInfo>());
        //}
        //else
        //{
        //    //cardGo.GetComponent<CardInfo>().ShowCardInfo(card, true);
        //    PlayerHandCards.Add(cardGo.GetComponent<CardInfo>());
        //    cardGo.GetComponent<CardAttack>().enabled = false;
        //}

        //deck.RemoveAt(0);
    }
}
