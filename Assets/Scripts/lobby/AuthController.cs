using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sfs2X;
using Sfs2X.Util;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;
using Sfs2X.Entities;
using UnityEngine.SceneManagement;

public class AuthController : MonoBehaviour
{

    [Tooltip("IP address or domain name of the SmartFoxServer 2X instance")]
	public string Host = "127.0.0.1";

    [Tooltip("TCP port listened by the SmartFoxServer 2X instance; used for regular socket connection in all builds except WebGL")]
    public int TcpPort = 9933;

    [Tooltip("WebSocket port listened by the SmartFoxServer 2X instance; used for in WebGL build only")]
    public int WSPort = 8080;

    [Tooltip("Name of the SmartFoxServer 2X Zone to join")]
    public string Zone = "Dominion";

    public TMP_InputField loginInput;
    public TMP_InputField passwordInput;
    public TMP_InputField emailInput;
    public Button signinButton;
    public Button signupButton;
    public Button submitButton;
    public TMP_Text authLoginLabel;
    public GameObject AuthLogin;

    public Transform gameListContent;
    public GameObject gameListItem;

    private SmartFox sfs;
    private bool firstJoin = true;
    private string CMD_SIGNUP = "$SignUp.Submit";

    private const string EXTENSION_ID = "DominionExtension";
    private const string EXTENSION_CLASS = "com.trpo.dominion.DominionExtension";

    //private void Awake()
    //{
    //    if (SmartFoxConnection.IsInitialized)
    //    {
    //        sfs = SmartFoxConnection.Connection;
    //    }
    //}

    void Start()
    {
        loginInput.text = "";
        passwordInput.text = "";
        emailInput.text = "";
        AuthLogin.SetActive(true);
    }

    void Update()
    {
        if (sfs != null)
            sfs.ProcessEvents();
    }

    //For admin test
    public void auth1()
    {
        OnSigninButtonClick();
        OnSubmitButtonClick();

        //должны быть в бд
        loginInput.text = "test1111";
        passwordInput.text = "test1111test1111";
    }

    //For admin test
    public void auth2()
    {
        OnSigninButtonClick();
        OnSubmitButtonClick();

        //должны быть в бд
        loginInput.text = "test444";
        passwordInput.text = "test444test444";
    }

    public void OnSigninButtonClick()
    {
        emailInput.gameObject.SetActive(false);
        authLoginLabel.text = "SIGN IN";
        firstJoin = false;
    }

    public void OnSignupButtonClick()
    {
        emailInput.gameObject.SetActive(true);
        authLoginLabel.text = "SIGN UP";
        firstJoin = true;
    }

    public void OnSubmitButtonClick()
    {
        enableLoginUI(false);

        ConfigData cfg = new ConfigData();
        cfg.Host = Host;

#if !UNITY_WEBGL
        cfg.Port = TcpPort;
#else
		cfg.Port = WSPort;
#endif

        // Инициализация SFS2X клиента и добавление перехватчиков
#if !UNITY_WEBGL
        sfs = new SmartFox();
#else
		sfs = new SmartFox(UseWebSocket.WS_BIN);
#endif

        sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
        sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
        sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
        sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
        sfs.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomJoinError);
        sfs.AddEventListener(SFSEvent.PUBLIC_MESSAGE, OnPublicMessage);
        sfs.AddEventListener(SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom);
        sfs.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);
        sfs.AddEventListener(SFSEvent.ROOM_ADD, OnRoomAdd);
        sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
        sfs.AddEventListener(SFSEvent.LOGOUT, LogOut);

        sfs.Connect(cfg.Host, cfg.Port);
    }

    private void LogOut(BaseEvent evt)
    {
        Debug.Log("Logout");
        reset();

        if (firstJoin)
        {
            firstJoin = false;
            OnSubmitButtonClick();
        }

    }

    private void OnConnection(BaseEvent evt)
    {
        Debug.Log("On Connection");
        if ((bool)evt.Params["success"])
        {
            Debug.Log("On connection success");
            SmartFoxConnection.Connection = sfs;

            if (firstJoin) {//сначала авторизация в качестве гостя
                Debug.Log("On connection success1");
               
                //нужно сделать гостевой вход!!!!!
                sfs.Send(new LoginRequest("test2", "test2test2", Zone));
                Debug.Log("On connection success2");
            }
            else
            {
                Debug.Log("On connection success2 "+ loginInput.text +" ; "+ passwordInput.text);
                sfs.Send(new LoginRequest(loginInput.text, passwordInput.text, Zone));
            }
        }
        else
        {
            reset();

            Debug.Log("Connection failed; is the server running at all?");
        }
    }

    private void OnConnectionLost(BaseEvent evt)
    {
        reset();

        string reason = (string)evt.Params["reason"];

        if (reason != ClientDisconnectionReason.MANUAL)
        {
            Debug.Log("Connection was lost; reason is: " + reason);
        }
    }

    void OnExtensionResponse(BaseEvent evt)
    {
        Debug.Log("On Extension");

        string cmd = (string)evt.Params["cmd"];
        ISFSObject objIn = (SFSObject)evt.Params["params"];

        if (cmd == CMD_SIGNUP)
        {
            if (objIn.ContainsKey("errorMessage"))
            {
                Debug.Log("Signup Error: ");
            }
            else if (objIn.ContainsKey("success"))
            {
                Debug.Log("Signup Successful"+ loginInput.text + " ; " + passwordInput.text);
                sfs.Send(new LogoutRequest());
            }
        }

        //// Populate Room list
        //populateRoomList(sfs.RoomList);

        //// Join first Room in Zone
        //if (sfs.RoomList.Count > 0) {
        //	sfs.Send(new Sfs2X.Requests.JoinRoomRequest(sfs.RoomList[0].Name));
        //}
    }

    private void OnLogin(BaseEvent evt)
    {
        Debug.Log("On Login");

        if (firstJoin)//авторизовались в качестве гостя, теперь регистрируемся
        {
            ISFSObject objOut = new SFSObject();
            objOut.PutUtfString("username", loginInput.text);
            objOut.PutUtfString("password", passwordInput.text);//должен быть больше 6 симаолов
            objOut.PutUtfString("email", emailInput.text);

            sfs.Send(new ExtensionRequest(CMD_SIGNUP, objOut));
        }
        else
        {
            User user = (User) evt.Params["user"];
            Debug.Log("Hello " + user.Name);
            AuthLogin.SetActive(false);

            populateGamesList();

            //sfs.Send(new JoinRoomRequest("Test1"));
        }

    }

    private void OnLoginError(BaseEvent evt)
    {
        Debug.Log("On Login error");

        sfs.Disconnect();

        reset();

        Debug.Log("Login failed: " + (string)evt.Params["errorMessage"]);
    }



    private void OnRoomJoin(BaseEvent evt)
    {
        Debug.Log("On Join room");

        Room room = (Room)evt.Params["room"];

        // If we joined a Game Room, then we either created it (and auto joined) or manually selected a game to join
        if (room.IsGame)
        {
            Debug.Log("You joined a Room: " + room.Name);
            // Remove SFS2X listeners
            reset();

            // Load game scene
            SceneManager.LoadScene("game");
        }
        else
        {
            Debug.Log("\nYou joined a Room: " + room.Name);

        }

    }

    private void OnRoomJoinError(BaseEvent evt)
    {
        Debug.Log("Room join failed: " + (string)evt.Params["errorMessage"]);
    }

    private void OnPublicMessage(BaseEvent evt)
    {
        Debug.Log("OnPublicMessage");

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

    private void OnRoomAdd(BaseEvent evt)
    {
        Debug.Log("OnRoomAdd");
        Room room = (Room)evt.Params["room"];
        Debug.Log(room.Name+" ; "+sfs.RoomList.Count);
    }


    private void enableLoginUI(bool enable)
    {
        loginInput.interactable = enable;
        passwordInput.interactable = enable;
        emailInput.interactable = enable;
        submitButton.interactable = enable;
        signinButton.interactable = enable;
        signupButton.interactable = enable;
    }

    private void reset()
    {
        sfs.RemoveEventListener(SFSEvent.CONNECTION, OnConnection);
        sfs.RemoveEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        sfs.RemoveEventListener(SFSEvent.LOGIN, OnLogin);
        sfs.RemoveEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
        sfs.RemoveEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
        sfs.RemoveEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomJoinError);
        sfs.RemoveEventListener(SFSEvent.PUBLIC_MESSAGE, OnPublicMessage);
        sfs.RemoveEventListener(SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom);
        sfs.RemoveEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);
        sfs.RemoveEventListener(SFSEvent.ROOM_ADD, OnRoomAdd);
        sfs.RemoveEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);

        sfs = null;

        enableLoginUI(true);
    }


    public void FindGameClick()
    {

        // Configure Game Room
        //RoomSettings settings = new RoomSettings(sfs.MySelf.Name + "'s game");
        RoomSettings settings = new RoomSettings("test1");
        settings.GroupId = "games";
        settings.IsGame = true;
        settings.MaxUsers = 2;
        settings.MaxSpectators = 0;
        settings.Extension = new RoomExtension(EXTENSION_ID, EXTENSION_CLASS);


        // Request Game Room creation to server
        sfs.Send(new CreateRoomRequest(settings, true, sfs.LastJoinedRoom));

        //!!!!!
        //reset();
        //SceneManager.LoadScene("game");
    }

    public void OnGameItemClick(int roomId)
    {
        Debug.Log("Room id: " + roomId);
        // Join the Room
        sfs.Send(new Sfs2X.Requests.JoinRoomRequest(roomId));
    }

    private void populateGamesList()
    {
        // For the gamelist we use a scrollable area containing a separate prefab button for each Game Room
        // Buttons are clickable to join the games
        List<Room> rooms = sfs.RoomManager.GetRoomList();

        foreach (Room room in rooms)
        {
            // Show only game rooms
            // Also password protected Rooms are skipped, to make this example simpler
            // (protection would require an interface element to input the password)
            if (!room.IsGame || room.IsHidden || room.IsPasswordProtected)
            {
                continue;
            }

            int roomId = room.Id;

            GameObject newListItem = Instantiate(gameListItem) as GameObject;
            GameListItem roomItem = newListItem.GetComponent<GameListItem>();
            roomItem.nameLabel.text = room.Name;

            roomItem.roomId = roomId;

            roomItem.button.onClick.AddListener(() => OnGameItemClick(roomId));

            newListItem.transform.SetParent(gameListContent, false);
        }
    }

    private void clearGamesList()
    {
        foreach (Transform child in gameListContent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

}
