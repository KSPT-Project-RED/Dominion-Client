using System.Collections;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine;

public class DominionGame : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame(ISFSArray cards)
    {
        Debug.Log("Getting cards number: " + cards.Count);
        for(int i = 0; i < cards.Count; i++)
        {
            Debug.Log(cards.GetSFSObject(i).GetText("Name"));
            Debug.Log(cards.GetSFSObject(i).GetText("Description"));
        }
    }
}
