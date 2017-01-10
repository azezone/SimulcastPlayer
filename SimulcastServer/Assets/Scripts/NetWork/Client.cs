using UnityEngine;
using System.Collections;

public class Client : MonoBehaviour
{

	//config ip and port
	string IP = "192.168.1.42";
	int port = 8081;
	
	string Message = "disconnected";
	bool isConnecting = false;
	NetworkView networkView;

	// Use this for initialization
	void Start ()
	{
		this.gameObject.AddComponent<NetworkView> ();
		networkView = this.GetComponent<NetworkView> ();
		StartConnect ();
	}
	
	// Update is called once per frame
	
	void OnGUI ()
	{
		GUILayout.Label (Message);
		switch (Network.peerType) {
		case NetworkPeerType.Disconnected:
			if (isConnecting == false) {
				StartConnect ();
			}
			break;
		case NetworkPeerType.Client:
			Online ();
			break;
		case NetworkPeerType.Connecting:
			break;
		}
	}
    void StartConnect()
    {
        isConnecting = true;
        NetworkConnectionError error = Network.Connect(IP, port);
        Debug.Log("Status:" + error);
    }

	public void Online ()
	{ 
		if (GUILayout.Button ("断开服务器")) {
			Network.Disconnect ();
		}
	}


	

	void OnFailedToConnect (NetworkConnectionError error)
	{
		isConnecting = false;
		Debug.Log (error.ToString());
		StartConnect ();
	}

	void OnDisconnectedFromServer (NetworkDisconnection info)
	{
		isConnecting = false;
		Message = "disconnected";
		Debug.Log ("OnDisconnectedFromServer");
	}

	void OnConnectedToServer ()
	{
		isConnecting = true;
		Message = "connected";
		Debug.Log ("OnConnectedToServer");
	}


	[RPC]
	void RequestMessage (string command, string value, NetworkMessageInfo info)
	{
		//play  playing playing-pause pause replay
		Message = command + ":" + value;
		Debug.Log ("command:" + command + "   value:" + value);
	}
}
