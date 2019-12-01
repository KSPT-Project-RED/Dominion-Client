using System.Collections;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine;

public class DominionGame : MonoBehaviour
{

    public List<GameObject> prefabCards;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame(ISFSArray cards, List<GameObject> prefabCards)
    {
        Debug.Log("Getting cards number: " + cards.Count);

        for(int i = 0; i < cards.Count && i< 10; i++)
        {
            CardStruct card = new CardStruct(cards.GetSFSObject(i).GetText("Name"),
                cards.GetSFSObject(i).GetText("Description"),
                "Sprites/Cards/"+cards.GetSFSObject(i).GetText("ImageId"),
                cards.GetSFSObject(i).GetInt("Cost"),
                10);
           
            prefabCards[i].GetComponent<CardInfo>().ShowCardInfo(card);

            Debug.Log(cards.GetSFSObject(i).GetText("Name"));
            Debug.Log(cards.GetSFSObject(i).GetText("Description"));
        }
    }
}
