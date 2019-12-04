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
    public DominionGame dominionGame;
    public List<GameObject> cards;
    private GameState gameState = GameState.WAITING;
    private string myTurn = "FIRST";//!!!!
    private int myId;

    private enum GameState
    {
        WAITING = 0,
        RUNNING,
        FIRST_ACTION,
        FIRST_BUY,
        SECOND_ACTION,
        SECOND_BUY,
        END,
        CONNECTION_LOST
    };

    public bool isMyStep()
    {
        return gameState.ToString().Contains(myTurn);
    }

    public bool isMyAction()
    {
        return gameState.ToString().Contains(myTurn) && gameState.ToString().Contains("ACTION");
    }

    public bool isMyBuy()
    {
        return gameState.ToString().Contains(myTurn) && gameState.ToString().Contains("BUY");
    }


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

        myId = sfs.MySelf.PlayerId;

        sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        sfs.AddEventListener(SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom);
        sfs.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);
        sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);

        //устанавливаем текущее состояние: ожидание других игроков
        gameState = GameState.WAITING;
        stateText.text = "Waiting";


        //подготавляваем новую игровую сессию
        //dominionGame = new DominionGame();
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
            case "cards":
                SetPlayerCards(dataObject);
                break;
            case "state":
                SetCurrentState(dataObject);
                break;
            case "buy":
                UpdateNumberCard(dataObject);
                break;
            case "step":
                UpdateStep(dataObject);
                break;

            case "second":
   
                break;
        }
    }

    public void UpdateStep(SFSObject dataObject)
    {
        Debug.Log("Update step: " + dataObject.GetUtfString("Step"));
        Enum.TryParse(dataObject.GetUtfString("Step"), out GameState state);
        gameState = state;
        stateText.text = gameState.ToString();
    }

    public void endTurn()
    {
        if (!isMyStep()) return;
   
        sfs.Send(new ExtensionRequest("endTurn", new SFSObject(), sfs.LastJoinedRoom));
        Debug.Log("END TURN");
    }

    public void checkBuy(string name)
    {
        if (!isMyStep()) return;

        ISFSObject msg = new SFSObject();
        msg.PutUtfString("Name", name);
        sfs.Send(new ExtensionRequest("buy", msg, sfs.LastJoinedRoom));
        Debug.Log("CHECK BUY");
    }

    public void doAction(String nameCard)
    {
        if (!isMyStep()) return;

        ISFSObject msg = new SFSObject();
        msg.PutUtfString("NameCard", nameCard);
        sfs.Send(new ExtensionRequest("action", msg, sfs.LastJoinedRoom));

        Debug.Log("DO ACTION");
    }

    public void UpdateNumberCard(SFSObject dataObject)
    {
        Debug.Log("Received checking cost card!");
        dominionGame.buyCard(dataObject);

    }

    public void SetPlayerCards(SFSObject dataObject)
    {
        Debug.Log("Received started cards!");
        dominionGame.InitPlayerCards(dataObject);
    }

    public void SetCurrentState(SFSObject dataObject)
    {
        Debug.Log("Received current state!");
        dominionGame.UpdateCurrentState(dataObject);
    }

    public void SetStartGame(SFSObject dataObject)
    {
        Debug.Log("Game started!");
        if(dataObject.GetInt("t") == myId)
        {
            myTurn = "FIRST";
        }
        else
        {
            myTurn = "SECOND";
        }
        Debug.Log(myTurn);

        gameState = GameState.FIRST_BUY;
        stateText.text = gameState.ToString();


        dominionGame.StartGame(
                 dataObject.GetSFSArray("cards"), cards);
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
