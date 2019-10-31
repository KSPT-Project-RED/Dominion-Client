using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum FieldType
{
    SELF_HAND,
    SELF_FIELD,
    ENEMY_HAND,
    ENEMY_FIELD
}


public class DropPlace : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public FieldType Type;

    public void OnDrop(PointerEventData eventData)
    {
        if (Type != FieldType.SELF_FIELD)
            return;

        //Получаем объект (карту), которую мы переносим над областью игрового полz
        CardMovement card = eventData.pointerDrag.GetComponent<CardMovement>();

        if (card)
        {
            //устанавливаем в качестве дефолтного родителя себя
            card.defaultParent = transform;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //проверяется, перетягивается ли какой-нибудь объект
        if (eventData.pointerDrag == null || Type == FieldType.ENEMY_FIELD || Type == FieldType.ENEMY_HAND) return;

        CardMovement card = eventData.pointerDrag.GetComponent<CardMovement>();

        if (card)
            card.defaultTempCardParent = transform;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //проверяется, перетягивается ли какой-нибудь объект
        if (eventData.pointerDrag == null) return;

        CardMovement card = eventData.pointerDrag.GetComponent<CardMovement>();

        if (card && card.defaultTempCardParent == transform)
            card.defaultTempCardParent = card.defaultParent;
    }
}
