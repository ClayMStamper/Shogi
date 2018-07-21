using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Net;
using System.IO;

public class Server : MonoBehaviour {

	public int port = 6321;

	public List <ServerClient> clients;
	private List <ServerClient> disconnectList;

	private TcpListener server;
	public bool isStarted;

	//call when server created
	public void Init(){

		//starting from menu scene

		DontDestroyOnLoad (gameObject);
		clients = new List<ServerClient> ();
		disconnectList = new List<ServerClient> ();

		try {

			server = new TcpListener(IPAddress.Any, port);
			server.Start();
			Debug.Log ("Server started");

			//listen for incoming connection
			StartListening();

			isStarted = true;

		} catch (System.Exception e){
			Debug.Log ("Socket error: " + e.Message);
			isStarted = false;
			MultiplayerManager.GetInstance ().OnHostFailed ();
		}

	}
		
	void Update(){

		if (!isStarted)
			return;

		foreach (ServerClient c in clients) {
			// is the client still connected?
			if (!IsConnected (c.tcp)) {
				c.tcp.Close ();
				disconnectList.Add (c);
				continue;
			} else {
				NetworkStream s = c.tcp.GetStream ();
				if (s.DataAvailable) {
					StreamReader reader = new StreamReader (s, true);
					string data = reader.ReadLine ();

					if (data != null) {
						OnIncomingData (c, data);
					}

				}
			}
		}

		for (int i = 0; i < disconnectList.Count - 1; i++) {

			// tell our player sombody has disconnected

			clients.Remove (disconnectList [i]);
			disconnectList.RemoveAt (i);

		}

	}

	private void StartListening(){
		server.BeginAcceptTcpClient (AcceptTcpClient, server);
	}

	private void AcceptTcpClient(System.IAsyncResult ar){
		//connection is live now: cache reference
		TcpListener listener = (TcpListener)ar.AsyncState;

		string allUsers = "";
		foreach (ServerClient i in clients){
			allUsers += i.clientName + '|';
		}

		ServerClient sc = new ServerClient (listener.EndAcceptTcpClient (ar));
		clients.Add (sc);

		StartListening ();

		Debug.Log ("Somebody has connected!"); //another communication needs to be done before we know who

		Broadcast ("SWHO|", clients[clients.Count - 1]);

	}

	private bool IsConnected(TcpClient c){
		
		try {

			//check if we're connected with these calls
			if (c != null && c.Client != null && c.Client.Connected){
				if (c.Client.Poll (0, SelectMode.SelectRead)){
					return !(c.Client.Receive (new byte[1], SocketFlags.Peek) == 0);
				} 
				return true;
			} else {
				return false;
			} 

		} catch {
			//we weren't able to get a response
			return false;
		}
	}

	//server send all
	private void Broadcast (string data, List <ServerClient> cl){
		foreach (ServerClient sc in cl) {
			try{
				StreamWriter writer = new StreamWriter (sc.tcp.GetStream());
				writer.WriteLine (data);
				writer.Flush();
			} catch (Exception e) {
				Debug.Log ("Write error: " + e.Message);
			}
		}
	}

	//server send
	private void Broadcast (string data, ServerClient c){
		try{
			StreamWriter writer = new StreamWriter (c.tcp.GetStream());
			writer.WriteLine (data);
			writer.Flush();
		} catch (Exception e) {
			Debug.Log ("Write error: " + e.Message);
		}
	}

	//server read
	private void OnIncomingData(ServerClient c, string data){

		Debug.Log ("Server: " + data);

		string[] aData = data.Split ('|');

		switch (aData [0]) {
		case "CWHO":
			c.clientName = aData [1];
			c.isHost = (aData [2] == "host") ? true : false;
			Broadcast ("SCNN|" + c.clientName, clients);
			break;
		case "CMOV":

			data = data.Replace ("CMOV", "SMOV");

			if (c.isHost) {
				Broadcast (data, clients[1]);
			} else {
				Broadcast (data, clients[0]);
			}

			break;
		case "CDROP":

			data = data.Replace ("CDROP", "SDROP");

			if (c.isHost) {
				Debug.Log ("Broadcasting " + data + " to host");
				Broadcast (data, clients [1]);
			} else {
				Debug.Log ("Broadcasting " + data + " to non-host client");
				Broadcast (data, clients [0]);
			}
			break;

		}

	}

	private void OnApplicationQuit(){
		stopServer ();
	}

	private void OnDisable(){
		stopServer ();
	}

	private void stopServer(){

		if (!isStarted)
			return;

		server.Stop ();
		isStarted = false;

	}

}

//a tcp client with other cached data
public class ServerClient{

	public string clientName;
	public TcpClient tcp;
	public bool isHost;

	public ServerClient (TcpClient tcp){
		this.tcp = tcp;
	}

}
