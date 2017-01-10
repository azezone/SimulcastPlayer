using UnityEngine;
using System.Collections;

public class Server : MonoBehaviour
{
    PlayStatus currentStatus = PlayStatus.Ready;
    float alreadyPlayedTime = 0.0f;
    int port = 8081;
    NetworkView networkView;
    int length = 0;

    void Start()
    {
        this.gameObject.AddComponent<NetworkView>();
        networkView = this.GetComponent<NetworkView>();
    }

    void OnGUI()
    {
        switch (Network.peerType)
        {
            case NetworkPeerType.Disconnected:
                StartServer();
                break;
            case NetworkPeerType.Server:
                OnServer();
                break;
            case NetworkPeerType.Client:
                break;
            case NetworkPeerType.Connecting:
                break;
        }
    }
    void StartServer()
    {
        //if (GUILayout.Button ("启动服务器")) 
        {
            bool useNat = !Network.HavePublicAddress();
            Network.InitializeServer(1000, port, false);
            //NetworkConnectionError error = Network.InitializeServer(1000, port, useNat);
            //Debug.Log("Status:" + error);
        }
    }
    void FixedUpdate()
    {
        if (currentStatus == PlayStatus.Playing)
        {
            alreadyPlayedTime++;
        }
    }




    void OnServer()
    {
        length = Network.connections.Length;
        //GUILayout.Label("在线人数：" + length);
        //GUILayout.Label("当前状态：" + currentStatus.ToString() + "     播放时间：" + alreadyPlayedTime);

        //		for (int i=0; i<length; i++) {
        //			NetworkPlayer player=Network.connections[i];
        //			GUILayout.Label("IP:"+player.ipAddress+"    Port:"+player.port);
        //		}

        //if (GUILayout.Button ("断开服务器")) {
        //    Network.Disconnect ();
        //    alreadyPlayedTime = 0;
        //    currentStatus=PlayStatus.Ready;
        //}

        //if (GUILayout.Button ("播放")) {
        //    currentStatus = PlayStatus.Playing;
        //    networkView.RPC ("RequestMessage", RPCMode.All, "play", "");
        //}

        //if (GUILayout.Button ("暂停")) {
        //    currentStatus = PlayStatus.Pause;
        //    networkView.RPC ("RequestMessage", RPCMode.All, "pause", "");
        //}
        //if (GUILayout.Button ("重置")) {
        //    currentStatus = PlayStatus.Ready;
        //    alreadyPlayedTime = 0;
        //    networkView.RPC ("RequestMessage", RPCMode.All, "reset", "");
        //}
    }

    /// <summary>当一个玩家连接时</summary>
	void OnPlayerConnected(NetworkPlayer player)
    {
        string command = "ready";
        switch (currentStatus)
        {
            case PlayStatus.Playing:
                command = "playing";
                break;
            case PlayStatus.Pause:
                command = "playing-pause";
                break;
            default:
                command = "ready";
                break;
        }
        //networkView.RPC ("RequestMessage",
        //                 player,
        //                 command,
        //                 alreadyPlayedTime.ToString ());
    }

    /// <summary>当一个玩家断开连接时</summary>
    void OnPlayerDisconnected(NetworkPlayer netWorkPlayer)
    {
        Debug.Log("OnPlayerDisconnected");
    }

    [RPC]
    void RequestMessage(string command, string value, NetworkMessageInfo info)
    {
        Debug.Log("command:" + command + "   value:" + value);
    }

    /// <summary>给客户端发送数据</summary>
    public void SendData(string command, string value)
    {
        networkView.RPC("RequestMessage", RPCMode.All, command, value);
        //Debug.LogError("commone:" + command + " value:" + value);
    }

    /// <summary>获取客户端在线数量</summary>
    public int GetOnlineCount
    {
        get
        {
            length = Network.connections.Length;
            return length;
        }
    }
}

public enum PlayStatus
{
    Ready,
    Playing,
    Pause
}
