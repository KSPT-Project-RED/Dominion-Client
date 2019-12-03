using System.Collections;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine;

public class DominionGame : MonoBehaviour
{

    public List<GameObject> prefabCards;
    public GameObject CardPref;
    public Transform PlayerHand;
    public List<CardInfo> PlayerHandCards = new List<CardInfo>();


    public void InitPlayerCards(SFSObject dataObject)
    {
        ISFSArray hand = dataObject.GetSFSArray("hand");
        ISFSArray hideCards = dataObject.GetSFSArray("hide");
        ISFSArray dropCards = dataObject.GetSFSArray("drop");
     
        for (int i = 0; i < hand.Size(); i++)
        {
            Debug.Log(CardPref == null);
            //Debug.Log(hand[i]);
            GiveCardToHand();
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
                10);
           
            prefabCards[i].GetComponent<CardInfo>().ShowCardInfo(card);

            //Debug.Log(cards.GetSFSObject(i).GetText("Name"));
            //Debug.Log(cards.GetSFSObject(i).GetText("Description"));
        }
    }

    void GiveCardToHand()
    {
        //if (deck.Count == 0)
        //    return;

        //CardStruct card = deck[0];

        //!!!!
        Debug.Log(CardPref);
        GameObject cardGo = Instantiate(CardPref, PlayerHand, false);
        //cardGo.GetComponent<CardInfo>().ShowCardInfo(card, true);

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
