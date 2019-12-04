using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoneyCard : MonoBehaviour, IPointerClickHandler
{
    public GameController gameController;
    public string name;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Mouse Clicked");
        gameController.checkBuy(name);
    }

}