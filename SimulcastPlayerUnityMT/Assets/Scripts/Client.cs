using System.Collections;
using UnityEngine;

public class Client : MonoBehaviour
{
    public static string IP = "192.168.1.2";
    public static int Port = 8081;

    string Message = "disconnected";
    NetworkView networkView = null;
    bool isConnecting = false;
    private NetState _netstate = NetState.error;
    private NetState netState
    {
        get
        {
            return _netstate;
        }
        set
        {
            _netstate = value;
            if (Application.platform == RuntimePlatform.Android)
            {
                Player.instance.OprateNetStateChanged(_netstate);
            }
        }
    }

    void Start()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            IP = "172.31.83.1";
        }
        if (networkView == null)
        {
            networkView = this.gameObject.AddComponent<NetworkView>();
        }
        StartCoroutine(StartConnectServer());
    }

    void Update()
    {
        switch (Network.peerType)
        {
            case NetworkPeerType.Disconnected:
                if (isConnecting == false)
                {
                    StartConnect();
                }
                break;
            case NetworkPeerType.Connecting:
                break;
        }
    }

    private IEnumerator StartConnectServer()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            while (StatePanel.instance == null || !StatePanel.instance.isEverythingReady())
            {
                yield return new WaitForSeconds(1f);
            }
            GetConfigData();
        }
        StartConnect();
        yield return 0;
    }

    private void GetConfigData()
    {
        string ip = null;
        string port = null;
        PicoUnityActivity.CallObjectMethod<string>(ref ip, "getKeyValue", Constant.SERVERIP);
        PicoUnityActivity.CallObjectMethod<string>(ref port, "getKeyValue", Constant.PORT);
        Debug.LogError("setdata ip:" + ip + " port:" + port);
        IP = ip;
        int.TryParse(port, out Port);
    }

    void StartConnect()
    {
        isConnecting = true;
        NetworkConnectionError error = Network.Connect(IP, Port);
        Debug.Log("Status:" + error);
    }

    void OnFailedToConnect(NetworkConnectionError error)
    {
        isConnecting = false;
        Debug.Log("OnFailedToConnect");
        //StartConnect();
        netState = NetState.error;
    }

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        isConnecting = false;
        Message = "disconnected";
        Debug.Log("OnDisconnectedFromServer");
        netState = NetState.error;
    }

    void OnConnectedToServer()
    {
        isConnecting = true;
        Message = "connected";
        Debug.Log("OnConnectedToServer");
        netState = NetState.success;
    }

    [RPC]
    void RequestMessage(string command, string value, NetworkMessageInfo info)
    {
        Message = command + ":" + value;
        Debug.Log("command:" + command + "   value:" + value);
        if (Player.instance != null)
        {
            Player.instance.OprateNetCommand(command, value);
        }
    }
}

public enum NetState
{
    error = 1,
    success = 2
}
