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

    private SmartFox sfs;
    private bool firstJoin = true;
    private string CMD_SIGNUP = "$SignUp.Submit";

    void Start()
    {
        loginInput.text = "";
        passwordInput.text = "";
        emailInput.text = "";
    }

    void Update()
    {
        if (sfs != null)
            sfs.ProcessEvents();
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
        Debug.Log("OnUserEnterRoom");
    }

    private void OnUserExitRoom(BaseEvent evt)
    {
        Debug.Log("OnUserExitRoom");
    }

    private void OnRoomAdd(BaseEvent evt)
    {
        Debug.Log("OnRoomAdd");
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

  
}
