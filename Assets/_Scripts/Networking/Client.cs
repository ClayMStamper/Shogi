using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System;

public class Client : MonoBehaviour {

	public int port = 6321;
	public string clientName;
	public bool isHost;
	public bool socketReady;

	private TcpClient socket;
	private NetworkStream stream;
	private StreamWriter writer;
	private StreamReader reader;
	private BoardManager boardManager;
	private SideTableManager sideTable;

	private List <GameClient> connectedPlayers = new List<GameClient>();

	//entry point
	public bool ConnectToServer (string hostAddress, int port){

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
			socketReady = false;
			MultiplayerManager.GetInstance ().OnJoinFailed ();

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

		Debug.Log ("Client: " + data);

		if (boardManager == null) {
			boardManager = BoardManager.GetInstance ();
		}

		string[] aData = data.Split ('|');

		switch (aData [0]) {
		case "SWHO":
			for (int i = 1; i < aData.Length - 1; i++) {
				UserConnected (aData [i], false);
			}
			Send ("CWHO|" + clientName + "|" + (isHost ? "host":"!host"));
			break;
		case "SCNN":
			UserConnected (aData [1], false);
			break;
		case "SMOV":

			if (isHost != boardManager.isPlayerOnesTurn) {

				int xOld = int.Parse (aData [1]);
				int yOld = int.Parse (aData [2]);
				int xNew = int.Parse (aData [3]);
				int yNew = int.Parse (aData [4]);

				boardManager.selectedPiece = (boardManager.pieces [xOld, yOld]);
				Square move = new Square (new Vector2Int (xNew, yNew));
				boardManager.MovePiece (move);
			}

			break;
		case "SDROP":

			sideTable = isHost ? boardManager.table2 : boardManager.table1;

			if (isHost != boardManager.isPlayerOnesTurn) {

				int xOld = int.Parse (aData [1]);
				int yOld = int.Parse (aData [2]);
				int xNew = int.Parse (aData [3]);
				int yNew = int.Parse (aData [4]);

				sideTable.selectedX = xOld;
				sideTable.selectedY = yOld;

				sideTable.SelectPiece ();

				sideTable.DropPiece (xNew, yNew);
			}

			break;
		}

	}

	private void UserConnected (string name, bool isHost){
		
		GameClient c = new GameClient ();
		c.name = name;

		connectedPlayers.Add (c);

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
