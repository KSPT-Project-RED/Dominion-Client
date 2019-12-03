using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActionCard : MonoBehaviour, IPointerClickHandler
{
    public GameController gameController;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Mouse Clicked");
        gameController.checkBuy(this.GetComponent<CardInfo>().getName());
    }

}
