﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardMovement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private Camera mainCamera;
    private Vector3 offset;//значение отступа от центра карты
    public Transform defaultParent, defaultTempCardParent;
    GameObject tempCardGO; //временная карта, которая будет представлять призрак передвигающейся карты
    //public LogicManager LogicManager;
    public bool IsDraggable;

    void Awake()
    {
        mainCamera = Camera.allCameras[0];//на сцене всего одна камера
        tempCardGO = GameObject.Find("TempCardGO");//карта-призрак находится вне canvas
        //LogicManager = FindObjectOfType<LogicManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Mouse Clicked on hand card");
        if (mainCamera.GetComponent<GameController>().isDropping)
        {
            if (this.GetComponent<CardInfo>().card.type.Equals(mainCamera.GetComponent<GameController>().dropType)){
                mainCamera.GetComponent<GameController>().DropCard(this.GetComponent<CardInfo>().getName());
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (mainCamera.GetComponent<GameController>().isDropping)
        {
            if (this.GetComponent<CardInfo>().card.type.Equals(mainCamera.GetComponent<GameController>().dropType)){
                mainCamera.GetComponent<GameController>().DropCard(this.GetComponent<CardInfo>().getName());
            }
        }

        /*
         * Для получение отступа вычитаем из координат карты координаты мыши
         * в игровой области.
         */
        offset = transform.position - mainCamera.ScreenToWorldPoint(eventData.position);

        defaultParent = defaultTempCardParent = transform.parent;

        Debug.Log(this.GetComponent<CardInfo>().getType());

        IsDraggable = ((defaultParent.GetComponent<DropPlace>().Type == FieldType.SELF_HAND ||
            defaultParent.GetComponent<DropPlace>().Type == FieldType.SELF_FIELD)
            && (this.GetComponent<CardInfo>().getType().Equals("действие")
            && mainCamera.GetComponent<GameController>().isMyAction() ||
            (this.GetComponent<CardInfo>().getType().Equals("деньги")
            && (mainCamera.GetComponent<GameController>().isMyBuy()||
            mainCamera.GetComponent<GameController>().isMyAction()))));// && LogicManager.isActiveAndEnabled;

        IsDraggable = IsDraggable && defaultParent.GetComponent<DropPlace>().Type != FieldType.SELF_FIELD;
        //IsDraggable = IsDraggable && !defaultParent.GetComponent<DominionGame>().actions.text.Equals("0");


            if (!IsDraggable)
            return;

        tempCardGO.transform.SetParent(defaultParent);

        /*
         * Индекс дочернего объекта. Т.к. родитель - область, где находятся карты,
         * то нумерация (индекс карты - можно увидеть на сцене) идет в нашем случае
         * от 0 до N слева направо для карт, находящихся в родительской области
         */
        tempCardGO.transform.SetSiblingIndex(transform.GetSiblingIndex());

        /*
         * Вначале перетягивания поднимаем вверх по иерархии игровую карту, присвоив
         * родителя ее родителя
         */
        transform.SetParent(defaultParent.parent);

        //!!!
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {

        if (!IsDraggable)
            return;

        /*
         * Текущие координаты мыши, получаемые при перемещении курсором мыши карты.
         * Координаты мыши молучаем в плоскости экрана, но нам нужно
         * получить координаты в игровом мире.
         */
        Vector3 newPos = mainCamera.ScreenToWorldPoint(eventData.position);

        transform.position = newPos + offset;

        if (tempCardGO.transform.parent != defaultTempCardParent)
        {
            tempCardGO.transform.SetParent(defaultTempCardParent);
        }

        if (defaultParent.GetComponent<DropPlace>().Type != FieldType.SELF_FIELD)
            CheckPosition();
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        if (!IsDraggable)
            return;

        // возврат карты по иерархии, вернув ей своего настоящего родителя
        transform.SetParent(defaultParent);

        GetComponent<CanvasGroup>().blocksRaycasts = true;

        transform.SetSiblingIndex(tempCardGO.transform.GetSiblingIndex());


        //чтобы спрятать карту-призрак нужно ее спрятать после оканчания передвижения
        tempCardGO.transform.SetParent(GameObject.Find("Canvas").transform);
        tempCardGO.transform.localPosition = new Vector3(2500, 0);

        //переместили карту действия и отправили всем инфу об этом
        if(defaultParent.GetComponent<DropPlace>().Type == FieldType.SELF_FIELD){
            mainCamera.GetComponent<GameController>().doAction(this.GetComponent<CardInfo>().getName());
        }
    }

    /*
     * Проходим по каждому дочернему объекту сетки карт слева направо и 
     * сравниваем позицию по x.
     */
    void CheckPosition()
    {
        int newIndex = defaultTempCardParent.childCount;

        for (int i = 0; i < defaultTempCardParent.childCount; i++)
        {
            if (transform.position.x < defaultTempCardParent.GetChild(i).transform.position.x)
            {
                newIndex = i;

                if (tempCardGO.transform.GetSiblingIndex() < newIndex)
                {
                    newIndex--;
                }

                break;
            }
        }

        tempCardGO.transform.SetSiblingIndex(newIndex);
    }
}
