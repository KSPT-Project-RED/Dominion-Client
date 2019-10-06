using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Camera mainCamera;
    private Vector3 offset;
    public Transform defaultParent, defaultTempCardParent;
    GameObject tempCardGO; 

    void Awake()
    {
        mainCamera = Camera.allCameras[0];
        tempCardGO = GameObject.Find("TempCardGO");
    }
      
    public void OnBeginDrag(PointerEventData eventData)
    {
        offset = transform.position - mainCamera.ScreenToWorldPoint(eventData.position);
        defaultParent = defaultTempCardParent =  transform.parent;
        tempCardGO.transform.SetParent(defaultParent);
        tempCardGO.transform.SetSiblingIndex(transform.GetSiblingIndex());
        transform.SetParent(defaultParent.parent);
        GetComponent<CanvasGroup>().blocksRaycasts = false; 
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 newPos = mainCamera.ScreenToWorldPoint(eventData.position);
        newPos.z = 0;
        transform.position = newPos + offset ;

        if(tempCardGO.transform.parent != defaultTempCardParent)
        {
            tempCardGO.transform.SetParent(defaultTempCardParent);
        }

        CheckPosition();
    }
      
    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(defaultParent);
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        transform.SetSiblingIndex(tempCardGO.transform.GetSiblingIndex());
        tempCardGO.transform.SetParent(GameObject.Find("Canvas").transform);
        tempCardGO.transform.localPosition = new Vector3(2500, 0);
    }

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
