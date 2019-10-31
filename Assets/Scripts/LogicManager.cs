using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro ;

public class Game
{
    public List<CardStruct> EnemyDeck, PlayerDeck, EnemyHand, PlayerHand, EnemyField, PlayerField;


    public Game()
    {
        EnemyDeck = GiveDeckCard();
        PlayerDeck = GiveDeckCard();

        EnemyHand = new List<CardStruct>();
        EnemyField = new List<CardStruct>();

        PlayerHand = new List<CardStruct>();
        PlayerField = new List<CardStruct>();
    }

    List<CardStruct> GiveDeckCard()
    {

        List<CardStruct> list = new List<CardStruct>();
        for(int i=0; i < 10; i++)
        {
            list.Add(CardStructManager.allCardStructs[Random.RandomRange(0, CardStructManager.allCardStructs.Count)]);
        }

        return list;

    }
          
}


public class LogicManager : MonoBehaviour
{

    public Game currentGame;
    public Transform EnemyHand, PlayerHand;
    public GameObject CardPref;
    int Turn, TurnTime = 30;
    public TextMeshProUGUI TurnTimeTxt;
    public Button EndTurnBtn;

    public bool IsPlayerTurn
    {
        get
        {
            return Turn % 2 == 0;
        }
    }

    void Start()
    {
        Turn = 0;  
        currentGame = new Game();
        GiveHandCards(currentGame.EnemyDeck, EnemyHand);
        GiveHandCards(currentGame.PlayerDeck, PlayerHand);

        StartCoroutine(TurnFunc());
    }

    void GiveHandCards(List<CardStruct> deck, Transform hand)
    {
        int i = 0;
        while (i++ < 4)
            GiveCardToHand(deck, hand);
    }

    void GiveCardToHand(List<CardStruct> deck, Transform hand)
    {
        if (deck.Count == 0)
            return;

        CardStruct card = deck[0];

        //!!!!
        GameObject cardGo = Instantiate(CardPref, hand, false);

        if (hand == EnemyHand)
            cardGo.GetComponent<CardInfo>().HideInfoCard(card);
        else
            cardGo.GetComponent<CardInfo>().ShowCardInfo(card);

        deck.RemoveAt(0);
    }

    IEnumerator TurnFunc()
    {
        TurnTime = 30;
        TurnTimeTxt.text = TurnTime.ToString();

        if (IsPlayerTurn)
        {
            while(TurnTime-- > 0)
            {
                TurnTimeTxt.text = TurnTime.ToString();
                yield return new WaitForSeconds(1);
            }
        }
        else
        {
            while (TurnTime-- > 26)
            {
                TurnTimeTxt.text = TurnTime.ToString();
                yield return new WaitForSeconds(1);
            }
        }

        ChangeTurn();
    }


    public void ChangeTurn()
    {
        StopAllCoroutines();
        Turn++;
        EndTurnBtn.interactable = IsPlayerTurn;

        if(IsPlayerTurn)
        {
            GiveNewCard();
        }

        StartCoroutine(TurnFunc());
    }

    void GiveNewCard()
    {
        GiveCardToHand(currentGame.EnemyDeck, EnemyHand);
        GiveCardToHand(currentGame.PlayerDeck, PlayerHand);
    }


}
