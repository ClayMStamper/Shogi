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
	//public string hostAddress = "127.0.0.1";
	public string hostIP = "127.0.0.1";

	Client client;
	Server server;

	LevelManager levelManager;
	MenusManager menus;

	void Start(){

	//	hostAddress = Network.player.ipAddress;

		levelManager = LevelManager.GetInstance ();
		levelManager.onLevelWasLoadedCallback += OnLoadedMenu;

		menus = MenusManager.GetInstance ();

	}

	public void Connect(){

		// Try to join - Else become server
		//Join (hostOnFail: true);

	}
		

	void Host(){

		//menus.ToggleLoading (true, "Waiting for opponent...");

		server = Instantiate (serverPrefab).GetComponent <Server>();
		//server.Init ();

		client = Instantiate (clientPrefab).GetComponent <Client> ();
		client.ConnectToServer (hostIP, client.port);
		client.isHost = true;

	}

	public void Join(){
		
		client = Instantiate (clientPrefab).GetComponent <Client> ();
		client.ConnectToServer (hostIP, client.port);

	}

	public void OnJoinFailed(){

		Debug.Log ("Failed to join");
		if (client != null) {
			Destroy (client.gameObject);
		}
		menus.ToggleLoading (false);
		menus.SetNetworkErrorActive ();
		StopAllCoroutines ();

//		StartCoroutine(CloseRoomToGuests ());

	}

	public void OnHostFailed(){

		Debug.Log ("Failed to host");
		menus.SetNetworkErrorActive ();
		menus.ToggleLoading (false);
		StopAllCoroutines ();

//		StartCoroutine(CloseRoomToGuests ());

	}

	public void OnJoin(){
		menus.ToggleLoading (false);
		levelManager.LoadLevel ("02c_Online");
		//StartCoroutine(CloseRoomToGuests ());
	}

	public void OnHost(){
		menus.ToggleLoading (false);
		levelManager.LoadLevel ("02c_Online");
	//	StartCoroutine(CloseRoomToGuests ());
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
