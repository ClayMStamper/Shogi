using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;

public class MultiplayerManager : MonoBehaviour {

	#region singleton

	//ensure that there is only one of these in the scene

	private static MultiplayerManager instance;

	void Awake(){

		if (instance == null) {
			instance = this;
		} else {
			Destroy (gameObject);
		}

		DontDestroyOnLoad (gameObject);

	}

	public static MultiplayerManager GetInstance(){
		return instance;
	}

	#endregion

	public GameObject serverPrefab, clientPrefab;
	public string hostAddress = "127.0.0.1";

	Client client;
	Server server;

	LevelManager levelManager;

	void Start(){

		hostAddress = Network.player.ipAddress;

		levelManager = LevelManager.GetInstance ();
		levelManager.onLevelWasLoadedCallback += OnLoadedMenu;

	}

	public void Connect(){

		// Try to join - Else become server
		Join (hostOnFail: true);

		levelManager.LoadLevel ("02c_Online");

	}

	void Host(){

		try {

			server = Instantiate (serverPrefab).GetComponent <Server>();
			server.Init();
//			Server.hostInstance = server;

			client = Instantiate (clientPrefab).GetComponent <Client> ();
			client.isHost = true;
			client.ConnectToServer (hostAddress, client.port, false);
//			Server.clientInstances.Add (client);

		} catch (Exception e){
			Debug.Log (e.Message);
		}

	}

	void Join(bool hostOnFail){

		client = Instantiate (clientPrefab).GetComponent <Client> ();
		client.ConnectToServer (hostAddress, client.port, hostOnFail);
//		Server.clientInstances.Add (client);

	}

	public void OnJoinFailed(bool shouldHost){

		Debug.Log ("Failed to join");
		Destroy (client.gameObject);

		if (shouldHost) {
			Debug.Log ("Becoming new host on failed join");
			Host ();
		}

	}

	public void OnHostFailed(){

		Debug.Log ("Failed to host");

	}

	void OnLoadedMenu(){

		if (levelManager.GetCurrentLevelName () == "01a_Menu") {
			try {
				Destroy (FindObjectOfType <Server> ().gameObject);
			} catch { }
			try {
				Destroy (FindObjectOfType <Client> ().gameObject);
			} catch { }
		}
	}
}
