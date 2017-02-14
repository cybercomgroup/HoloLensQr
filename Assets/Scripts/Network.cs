using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Network : MonoBehaviour {

    private NetworkClient myClient;

	// Use this for initialization
	void Start () {
        
	}

    private void SetupClient(string ip, int port)
    {
        myClient = new NetworkClient();
        myClient.RegisterHandler(MsgType.Connect, OnConnected);
        myClient.Connect(ip,port);
        //isAtStartup = false;
    }

    public void SetupLocalClient()
    {
        myClient = ClientScene.ConnectLocalServer();
        myClient.RegisterHandler(MsgType.Connect, OnConnected);
        //isAtStartup = false;
    }

    // client function
    public void OnConnected(NetworkMessage netMsg)
    {
        Debug.Log("Connected to server");
    }

    public void Connect (string address)
    {
        SetupClient(address, 4444);
    }

    public void Disconnect ()
    {
        if (myClient != null)
        {
            myClient.Disconnect();
        }
    } 
}
