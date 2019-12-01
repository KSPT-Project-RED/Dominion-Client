//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro ;

//public class Game
//{
//    public List<CardStruct> EnemyDeck, PlayerDeck;


//    public Game()
//    {
//        EnemyDeck = GiveDeckCard();
//        PlayerDeck = GiveDeckCard();
//    }

//    List<CardStruct> GiveDeckCard()
//    {

//        List<CardStruct> list = new List<CardStruct>();
//        for(int i=0; i < 10; i++)
//        {
//            list.Add(CardStructManager.allCardStructs[Random.RandomRange(0, CardStructManager.allCardStructs.Count)]);
//        }

//        return list;

//    }
          
//}


//public class LogicManager : MonoBehaviour
//{

//    public Game currentGame;
//    public Transform EnemyHand, PlayerHand, EnemyField, PlayerField;
//    public GameObject CardPref;
//    int Turn, TurnTime = 30;
//    public TextMeshProUGUI TurnTimeTxt;
//    public Button EndTurnBtn;


//    public List<CardInfo> PlayerHandCards = new List<CardInfo>(),
//                          PlayerFieldCards = new List<CardInfo>(),
//                          EnemyHandCards = new List<CardInfo>(),
//                          EnemyFieldCards = new List<CardInfo>();

//    public bool IsPlayerTurn
//    {
//        get
//        {
//            return Turn % 2 == 0;
//        }
//    }

//    void Start()
//    {
//        Turn = 0;  
//        currentGame = new Game();
//        GiveHandCards(currentGame.EnemyDeck, EnemyHand);
//        GiveHandCards(currentGame.PlayerDeck, PlayerHand);

//        StartCoroutine(TurnFunc());
//    }

//    void GiveHandCards(List<CardStruct> deck, Transform hand)
//    {
//        int i = 0;
//        while (i++ < 4)
//            GiveCardToHand(deck, hand);
//    }

//    void GiveCardToHand(List<CardStruct> deck, Transform hand)
//    {
//        if (deck.Count == 0)
//            return;

//        CardStruct card = deck[0];

//        //!!!!
//        GameObject cardGo = Instantiate(CardPref, hand, false);

//        if (hand == EnemyHand)
//        {
//            cardGo.GetComponent<CardInfo>().HideInfoCard(card);
//            EnemyHandCards.Add(cardGo.GetComponent<CardInfo>());
//        }
//        else
//        {
//            cardGo.GetComponent<CardInfo>().ShowCardInfo(card, true);
//            PlayerHandCards.Add(cardGo.GetComponent<CardInfo>());
//            cardGo.GetComponent<CardAttack>().enabled = false;
//        }

//        deck.RemoveAt(0);
//    }

//    IEnumerator TurnFunc()
//    {
//        TurnTime = 30;
//        TurnTimeTxt.text = TurnTime.ToString();

//        foreach (var card in PlayerFieldCards)
//        {
//            card.DeHighlightCard();
//        }

//        if (IsPlayerTurn)
//        {
//            foreach (var card in PlayerFieldCards)
//            {
//                card.card.ChangeAttackState(true);
//                card.HighlightCard();
//            }

//            while(TurnTime-- > 0)
//            {
//                TurnTimeTxt.text = TurnTime.ToString();
//                yield return new WaitForSeconds(1);
//            }
//        }
//        else
//        {
//            foreach (var card in EnemyFieldCards)
//                card.card.ChangeAttackState(true);

//            while (TurnTime-- > 27)
//            {
//                TurnTimeTxt.text = TurnTime.ToString();
//                yield return new WaitForSeconds(1);
//            }

//            if (EnemyHandCards.Count > 0)
//                EnemyTurn(EnemyHandCards);
//        }

//        ChangeTurn();
//    }

//    void EnemyTurn(List<CardInfo> cards)
//    {
//        int count = cards.Count == 1 ? 1 : Random.RandomRange(0, cards.Count);

//        for( int i=0; i< count; i++)
//        {
//            if (EnemyFieldCards.Count > 5)
//                return;

//            cards[0].ShowCardInfo(cards[0].card, false);
//            cards[0].transform.SetParent(EnemyField);

//            EnemyFieldCards.Add(cards[0]);
//            EnemyHandCards.Remove(cards[0]);
//        }

//        foreach (var activeCard in EnemyFieldCards.FindAll(x => x.card.canAttack))
//        {
//            if (PlayerFieldCards.Count == 0)
//                return;

//            var enemy = PlayerFieldCards[Random.Range(0, PlayerFieldCards.Count)];

//            Debug.Log(activeCard.card.name + " (" + activeCard.card.attack + ";" + activeCard.card.defense + "---> " +
//                enemy.card.name + " (" + enemy.card.attack + ";" + enemy.card.defense+")");

//            activeCard.card.ChangeAttackState(false);
//            CardsFight(enemy, activeCard);
//        }
//    }


//    public void ChangeTurn()
//    {
//        StopAllCoroutines();
//        Turn++;
//        EndTurnBtn.interactable = IsPlayerTurn;

//        if(IsPlayerTurn)
//        {
//            GiveNewCard();
//        }

//        StartCoroutine(TurnFunc());
//    }

//    void GiveNewCard()
//    {
//        GiveCardToHand(currentGame.EnemyDeck, EnemyHand);
//        GiveCardToHand(currentGame.PlayerDeck, PlayerHand);
//    }

//    public void CardsFight(CardInfo playerCard, CardInfo enemyCard)
//    {
//        playerCard.card.GetDamage(enemyCard.card.attack);
//        enemyCard.card.GetDamage(playerCard.card.attack);

//        if (!playerCard.card.isAlive)
//            DestroyCard(playerCard);
//        else
//            playerCard.RefreshData();

//        if (!enemyCard.card.isAlive)
//            DestroyCard(enemyCard);
//        else
//            enemyCard.RefreshData();
//    }

//    void DestroyCard(CardInfo card)
//    {
//        card.GetComponent<CardMovement>().OnEndDrag(null);
//        //!!!!! =>
//        if (EnemyFieldCards.Exists(x => x == card))
//            EnemyFieldCards.Remove(card);

//        if (PlayerFieldCards.Exists(x => x == card))
//            PlayerFieldCards.Remove(card);

//        Destroy(card.gameObject);

//    }


//}
