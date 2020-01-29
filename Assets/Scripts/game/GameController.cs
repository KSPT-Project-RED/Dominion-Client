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
    private int myTurn = 0;//!!!!
    private int myId;
    private int currentTurn = 0;
    public int numOfDrop = 0;
    public bool isDropping = false;
    public GameObject dropping;
    public GameObject endDropBtn;
    public String dropType = "";

    private enum GameState
    {
        WAITING = 0,
        RUNNING,
        ACTION,
        BUY,
        END,
        CONNECTION_LOST
    };

    public bool isMyStep()
    {
        return currentTurn == myTurn;
    }

    public bool isMyAction()
    {
        return currentTurn == myTurn && gameState.ToString().Contains("ACTION") && !isDropping;
    }

    public bool isMyBuy()
    {
        return currentTurn == myTurn && gameState.ToString().Contains("BUY") && !isDropping;
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

    public void DropSomeCards(int num)
    {
        DropSomeCards(num, "");
    }

    public void DropSomeCards(int num, String type)
    {
        isDropping = true;
        numOfDrop = num;
        dropType = type;
        dropping.gameObject.active = true;

        stateText.text = "Dropping " + numOfDrop + " cards \n &" + gameState.ToString();
    }

    public void DropCard(String nameDropCard)
    {
        ISFSObject msg = new SFSObject();
        msg.PutUtfString("DropCard", nameDropCard);
        sfs.Send(new ExtensionRequest("DropCard", msg, sfs.LastJoinedRoom));
        numOfDrop--;

        if(numOfDrop == 0)
        {
            isDropping = false;
            stateText.text = gameState.ToString();
            dropping.gameObject.active = false;
            dropType = "";
        }
        else
        {
            stateText.text = "Dropping " + numOfDrop + " cards \n &" + gameState.ToString();
        }
        Debug.Log("Drop Card: " + nameDropCard);
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
            case "command":
                DoCommand(dataObject);
                break;

            case "second":
   
                break;
        }
    }

    public void DoCommand(SFSObject dataObject)
    {
        Debug.Log("GETTING COMMAND ");
        Debug.Log("GETTING COMMAND " + dataObject.GetUtfString("command"));
        if (dataObject.GetUtfString("command").Equals("DropToMin3"))
        {
            List<GameObject> cards = dominionGame.tmpInst;
            for (int i = 0; i < cards.Count; i++)
            {
                Debug.Log("RRRRRRR " + cards[i].GetComponent<CardInfo>().getName());
                if (cards[i].GetComponent<CardInfo>().getName().Equals("ров"))
                {
                    Debug.Log("RRRRRRR");
                    return;
                }
            }
            DropSomeCards(2);
   
        }else if (dataObject.GetUtfString("command").Equals("MainPlayerDropAnyCards"))
        {
            
            endDropBtn.gameObject.active = true;
            DropSomeCards(5);

        }
        else if (dataObject.GetUtfString("command").Equals("DropGoldGetGoldPlus3"))
        {

            endDropBtn.gameObject.active = true;
            DropSomeCards(1, "деньги");

        }
    }

    public void EndDrop()
    {

        endDropBtn.gameObject.active = false;

        isDropping = false;
        dropType = "";
        stateText.text = gameState.ToString();
        dropping.gameObject.active = false;

        SFSObject num = new SFSObject();
        num.PutInt("num", 5 - numOfDrop);
        sfs.Send(new ExtensionRequest("endDrop", num, sfs.LastJoinedRoom));

        Debug.Log("End Drop"+(5 - numOfDrop));
        numOfDrop = 0;
    }

    public void UpdateStep(SFSObject dataObject)
    {
        Debug.Log("Update step: " + dataObject.GetUtfString("Step"));
        Enum.TryParse(dataObject.GetUtfString("Step"), out GameState state);
        gameState = state;
        currentTurn = dataObject.GetInt("currentTurn");
        Debug.Log("currentTurn " + currentTurn);
        Debug.Log("MyTurn " + myTurn);

        if (isDropping)
        {
            stateText.text = "Dropping "+ numOfDrop + "cards \n &" + gameState.ToString();
        }
        else {
            stateText.text = gameState.ToString();
        }
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
        for(int i = 0; i < dataObject.GetInt("num"); i++)
        {
            if (dataObject.GetInt("t"+i) == myId)
            {
                myTurn = i;
            }
        }

        Debug.Log(myTurn);

        gameState = GameState.BUY;
        currentTurn = 0;

        if (isDropping)
        {
            stateText.text = "Dropping " + numOfDrop + "cards \n &" + gameState.ToString();
        }
        else
        {
            stateText.text = gameState.ToString();
        }


        dominionGame.StartGame(dataObject.GetSFSArray("cards"), cards);
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
