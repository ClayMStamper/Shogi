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
	MenusManager menus;

	void Start(){

		hostAddress = Network.player.ipAddress;

		levelManager = LevelManager.GetInstance ();
		levelManager.onLevelWasLoadedCallback += OnLoadedMenu;

		menus = MenusManager.GetInstance ();

	}

	public void Connect(){

		// Try to join - Else become server
		//Join (hostOnFail: true);

	}

	public IEnumerator JoinMatchRoom(){

		menus.ToggleLoading (true, "Connecting...");

		IEnumerator e = Join ();
		while (e.MoveNext ())
			yield return e.Current;

		//Stop looking for guests
		e = DataWriter.SubtractData ("hosting", hostAddress + "|");
		while (e.MoveNext ())
			yield return e.Current;

		OnJoin ();

	}

	public IEnumerator HostMatchRoom(){

		menus.ToggleLoading (true, "Connecting...");

		//set up match with guest
		IEnumerator e = Host ();
		while (e.MoveNext ())
			yield return e.Current;

		//Stop looking for guests
		e = DataWriter.SubtractData ("hosting", hostAddress + "|");
		while (e.MoveNext ())
			yield return e.Current;

		OnHost ();

	}

	IEnumerator Host(){

		menus.ToggleLoading (true, "Waiting for opponent...");

		server = Instantiate (serverPrefab).GetComponent <Server>();

		while (!server.isStarted) {
			server.Init();
			yield return null;
		}

		client = Instantiate (clientPrefab).GetComponent <Client> ();

		while (!client.socketReady){
			client.ConnectToServer (hostAddress, client.port);
			client.isHost = true;
			yield return null;
		}

		while (server.clients.Count < 2) {
			yield return null;
		}

	}

	IEnumerator Join(){

		client = Instantiate (clientPrefab).GetComponent <Client> ();

		while (!client.socketReady) {
			client.ConnectToServer (hostAddress, client.port);
			yield return new WaitForSeconds (1f);
		}
//		Server.clientInstances.Add (client);

	}

	public IEnumerator CloseRoomToGuests(){

		IEnumerator e = DataWriter.SubtractData ("hosting", hostAddress + "|");
		while (e.MoveNext ())
			yield return e.Current;

		Debug.Log ("Room closed to guests");

	}

	public void OnJoinFailed(){

		Debug.Log ("Failed to join");
		if (client != null) {
			Destroy (client.gameObject);
		}
		menus.ToggleLoading (false);
		menus.SetNetworkErrorActive ();
		StopAllCoroutines ();

		StartCoroutine(CloseRoomToGuests ());

	}

	public void OnHostFailed(){

		Debug.Log ("Failed to host");
		menus.SetNetworkErrorActive ();
		menus.ToggleLoading (false);
		StopAllCoroutines ();

		StartCoroutine(CloseRoomToGuests ());

	}

	public void OnJoin(){
		levelManager.LoadLevel ("02c_Online");
		StartCoroutine(CloseRoomToGuests ());
	}

	public void OnHost(){
		levelManager.LoadLevel ("02c_Online");
		StartCoroutine(CloseRoomToGuests ());
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
