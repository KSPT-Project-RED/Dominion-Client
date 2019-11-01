using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardAttack : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (!GetComponent<CardMovement>().LogicManager.IsPlayerTurn)
            return;

        CardInfo card = eventData.pointerDrag.GetComponent<CardInfo>();

        if(card && card.card.canAttack && transform.parent == GetComponent<CardMovement>().LogicManager.EnemyField)
        {
            card.card.ChangeAttackState(false);

            if (card.isPlayer)
                card.DeHighlightCard();

            GetComponent<CardMovement>().LogicManager.CardsFight(card, GetComponent<CardInfo>());
        }
    }
}
