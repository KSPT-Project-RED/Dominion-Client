using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Logging;
using Sfs2X.Util;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;

public class GameController : MonoBehaviour
{
    private SmartFox sfs;

    void Awake()
    {
        Debug.Log("game awake1");
        Application.runInBackground = true;

        if (SmartFoxConnection.IsInitialized)
        {
            Debug.Log("game awake2");
            sfs = SmartFoxConnection.Connection;
        }
        else
        {
            SceneManager.LoadScene("Login");
            return;
        }

        sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        sfs.AddEventListener(SFSEvent.PUBLIC_MESSAGE, OnPublicMessage);
        sfs.AddEventListener(SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom);
        sfs.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);

        // setCurrentGameState(GameState.WAITING_FOR_PLAYERS);
        Debug.Log("game awake3");
    }

    void Update()
    {
        if (sfs != null)
            sfs.ProcessEvents();
    }


    private void OnConnectionLost(BaseEvent evt)
    {
        // Remove SFS2X listeners
        reset();

        //if (shuttingDown == true)
        //    return;

        // Return to login scene
        SceneManager.LoadScene("Login");
    }

    private void OnPublicMessage(BaseEvent evt)
    {
        User sender = (User)evt.Params["sender"];
        string message = (string)evt.Params["message"];

        Debug.Log(sender+" "+ message);
    }

    private void OnUserEnterRoom(BaseEvent evt)
    {
        User user = (User)evt.Params["user"];

        // Show system message
        Debug.Log("User " + user.Name + " entered the room");
    }

    private void OnUserExitRoom(BaseEvent evt)
    {
        User user = (User)evt.Params["user"];

        if (user != sfs.MySelf)
        {
            // Show system message
            Debug.Log("User " + user.Name + " left the room");
        }
    }
    private void reset()
    {
        // Remove SFS2X listeners
        sfs.RemoveAllEventListeners();
    }

    
}
