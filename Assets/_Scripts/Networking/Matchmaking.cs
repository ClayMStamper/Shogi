using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

public class Matchmaking : MonoBehaviour {

	#region singleton

	private static Matchmaking instance;

	void Awake(){

		if (instance == null) {
			instance = this;
		} else {
			Destroy (gameObject);
		}

	}

	public static Matchmaking GetInstance(){
		return instance;
	}

	#endregion

	MenusManager menus;
	MultiplayerManager multiplayer;

	void Start(){
		menus = MenusManager.GetInstance ();
		multiplayer = MultiplayerManager.GetInstance ();
	}
		
	public void Queue(){

		Debug.Log ("Entering match queue");

		//fetch list of hosts
		//StartCoroutine(FindHost (hostOnNull: true));

		//if not null then split and pick take index 0
		//set ip and Button.Connect();
		//MultiplayerManager.GetInstance ().Connect ();

		//else become host

	}
		

}
