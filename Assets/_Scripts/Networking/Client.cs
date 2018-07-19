using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System;

public class Client : MonoBehaviour {

	public int port = 6321;

	private bool socketReady;
	private TcpClient socket;
	private NetworkStream stream;
	private StreamWriter writer;
	private StreamReader reader;

	//entry point
	public bool ConnectToServer (string hostAddress, int port, bool hostOnFail){

		if (socketReady)
			return false;

		try {
			
			socket = new TcpClient (hostAddress, port);
			stream = socket.GetStream();
			writer = new StreamWriter (stream);
			reader = new StreamReader (stream);

			socketReady = true;
			DontDestroyOnLoad (gameObject);

		} catch (Exception e) {

			Debug.Log ("Socket error: " + e.Message);
			MultiplayerManager.GetInstance ().OnJoinFailed (hostOnFail);

		}

		return socketReady;

	}

	private void Update(){

		if (socketReady) {
			if (stream.DataAvailable) {
				
				string data = reader.ReadLine ();

				if (data != null)
					OnIncomingData (data);

			}
		}

	}

	//Send To the server
	public void Send (string data){
		if (!socketReady)
			return;

		writer.WriteLine (data);
		writer.Flush ();

	}

	// Read messages from the server
	private void OnIncomingData (string data){
		Debug.Log (data);
	}

	//built in function
	private void OnApplicationQuit(){
		CloseSocket ();
	}

	private void OnDisable(){
		CloseSocket ();
	}

	private void CloseSocket(){
		
		if (!socketReady)
			return;

		writer.Close ();
		reader.Close ();
		socket.Close ();
		socketReady = false;

	}

}

public class GameClient {

	public string name;
	public bool isHost;

}
