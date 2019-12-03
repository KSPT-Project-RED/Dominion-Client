using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActionCard : MonoBehaviour
{
    public DominionGame dominionGame;
    public GameController gameController;
    //public TextMeshProUGUI numberLabel;
    //public int number = 10;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse Clicked");
            gameController.checkBuy(this.GetComponent<CardInfo>().getName());


            //number--;
            //if (number == 0)
            //{
            //    this.gameObject.SetActive(false);
            //}
            //numberLabel.text = ""+number;
            
        }
    }



}
