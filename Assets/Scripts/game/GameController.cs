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
using TMPro;

public class GameController : MonoBehaviour
{
    private SmartFox sfs;
    public TMP_Text stateText;
    private DominionGame dominionGame;
    public List<GameObject> cards;

    private enum GameState
    {
        WAITING_FOR_PLAYERS = 0,
        RUNNING,
        YOU_WIN,
        SECOND_WIN,
        THIRD_WIN,
        FOURTH_WIN,
        CONNECTION_LOST
    };

    //инициализируем sfs соединение с использованием статического класса
    void Awake()
    {
        Application.runInBackground = true;

        if (SmartFoxConnection.IsInitialized)
        {
            sfs = SmartFoxConnection.Connection;
        }
        else
        {
            SceneManager.LoadScene("Login");
            return;
        }

        sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        sfs.AddEventListener(SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom);
        sfs.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);
        sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);

        //устанавливаем текущее состояние: ожидание других игроков
        setCurrentGameState(GameState.WAITING_FOR_PLAYERS);


        //подготавляваем новую игровую сессию
        dominionGame = new DominionGame();
        sfs.Send(new ExtensionRequest("ready", new SFSObject(), sfs.LastJoinedRoom));
    }

    void Update()
    {
        if (sfs != null)
            sfs.ProcessEvents();
    }

    public void OnLeaveGameButtonClick()
    {
        // Remove SFS2X listeners
        reset();

        // Destroy current game
        //DominionGame.DestroyGame();

        // Leave current room
        sfs.Send(new Sfs2X.Requests.LeaveRoomRequest());

        // Return to lobby scene
        SceneManager.LoadScene("Lobby");
    }

    


    private void setCurrentGameState(GameState state)
    {
        if (state == GameState.WAITING_FOR_PLAYERS)
        {
            stateText.text = "Waiting for your opponent";
        }
        else if (state == GameState.RUNNING)
        {
            stateText.text = "Running";
            // Nothing to do; the state text is updated by the DominionGame instance
        }
        else if (state == GameState.CONNECTION_LOST)
        {
            stateText.text = "Opponent disconnected; waiting for new player";
        }
        else
        {
            stateText.text = "GAME OVER";

            if (state == GameState.YOU_WIN)
            {
                stateText.text += "\nYou've lost!";
            }
            else if (state == GameState.SECOND_WIN)
            {
                stateText.text += "\nSecond win!";
            }
           
        }
    }


    /**
	 * Handle responses from server side Extension.
	 */
    public void OnExtensionResponse(BaseEvent evt)
    {
        string cmd = (string)evt.Params["cmd"];
        SFSObject dataObject = (SFSObject)evt.Params["params"];

        switch (cmd)
        {
            case "start":
                SetStartGame(dataObject);
              
                break;

            case "second":
   
                break;
        }
    }

    public void SetStartGame(SFSObject dataObject)
    {
        Debug.Log("Game started!");
        setCurrentGameState(GameState.RUNNING);

        dominionGame.StartGame(
                 dataObject.GetSFSArray("cards"), cards
                  //dataObject.GetInt("t"),
                  //dataObject.GetInt("p1i"),
                  //dataObject.GetInt("p2i"),
                  //dataObject.GetUtfString("p1n"),
                  //dataObject.GetUtfString("p2n")
                  );
    }

    public void SetGameOver(string result)
    {
        string message = "Game over!";
    }

    private void OnConnectionLost(BaseEvent evt)
    {
        // Remove SFS2X listeners
        reset();

        //if (shuttingDown == true)
        //    return;

        SceneManager.LoadScene("Login");
    }

    private void OnUserEnterRoom(BaseEvent evt)
    {
        User user = (User)evt.Params["user"];

        Debug.Log("User " + user.Name + " entered the room");
    }

    private void OnUserExitRoom(BaseEvent evt)
    {
        User user = (User)evt.Params["user"];

        if (user != sfs.MySelf)
        {
            Debug.Log("User " + user.Name + " left the room");
        }
    }
    private void reset()
    {
        sfs.RemoveAllEventListeners();
    }

    
}
