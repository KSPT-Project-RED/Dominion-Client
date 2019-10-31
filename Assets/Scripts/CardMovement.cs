using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardMovement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Camera mainCamera;
    private Vector3 offset;//значение отступа от центра карты
    public Transform defaultParent, defaultTempCardParent;
    GameObject tempCardGO; //временная карта, которая будет представлять призрак передвигающейся карты
    LogicManager  LogicManager;
    public bool IsDraggable;

    void Awake()
    {
        mainCamera = Camera.allCameras[0];//на сцене всего одна камера
        tempCardGO = GameObject.Find("TempCardGO");//карта-призрак находится вне canvas
        LogicManager = FindObjectOfType<LogicManager>();
    }
      
    public void OnBeginDrag(PointerEventData eventData)
    {
        /*
         * Для получение отступа вычитаем из координат карты координаты мыши
         * в игровой области.
         */
        offset = transform.position - mainCamera.ScreenToWorldPoint(eventData.position);

        defaultParent = defaultTempCardParent =  transform.parent;

        IsDraggable = defaultParent.GetComponent<DropPlace>().Type == FieldType.SELF_HAND && LogicManager.isActiveAndEnabled;

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

        transform.position = newPos + offset ;

        if(tempCardGO.transform.parent != defaultTempCardParent)
        {
            tempCardGO.transform.SetParent(defaultTempCardParent);
        }

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
        tempCardGO.transform.localPosition =   new Vector3(2500, 0);
    }

    /*
     * Проходим по каждому дочернему объекту сетки карт слева направо и 
     * сравниваем позицию по x.
     */
    void CheckPosition()
    {
        int newIndex = defaultTempCardParent.childCount;

        for(int i=0; i< defaultTempCardParent.childCount; i++)
        {
            if(transform.position.x < defaultTempCardParent.GetChild(i).transform.position.x)
            {
                newIndex = i;

                if(tempCardGO.transform.GetSiblingIndex() < newIndex)
                {
                    newIndex--;
                }

                break;
            }
        }

        tempCardGO.transform.SetSiblingIndex(newIndex); 
    }
}
