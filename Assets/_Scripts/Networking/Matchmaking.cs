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
		StartCoroutine(FindHost (hostOnNull: true));

		//if not null then split and pick take index 0
		//set ip and Button.Connect();
		//MultiplayerManager.GetInstance ().Connect ();

		//else become host

	}

	IEnumerator FindHost (bool hostOnNull){

		menus.ToggleLoading (true, "Searching for a host");

		IEnumerator e = DataReader.Refresh("hosting");
		while (e.MoveNext ())
			yield return e.Current;

		//split to array of IDs
		string[] dataSplits = DataReader.data.Split('|');

		if (dataSplits.Length >= 2) {
			Debug.Log ("Found a match"); 
			for (int i = 0; i < dataSplits.Length; i++) {
				Debug.Log ("Data split i: " + dataSplits [i]);
			} 
			menus.ToggleLoading (true, "Found a match: Connecting...");
			//check that ip is in proper format (todo)
			multiplayer.hostAddress = dataSplits[1];
			StartCoroutine(multiplayer.CloseRoomToGuests ());
			e = multiplayer.JoinMatchRoom ();
			while (e.MoveNext ())
				yield return e.Current;
		} else {
			Debug.Log ("No hosts found");
			menus.ToggleLoading (true, "Searching for a host");
			StartCoroutine (BecomeHost ());
		}

	}

	IEnumerator BecomeHost(){

		//start loading
		Debug.Log ("Becoming host");
		menus.ToggleLoading (true, "Becoming Host...");

		//cache IP Address
		string myIp = Network.player.ipAddress;
		multiplayer.hostAddress = myIp;

		//Open room for guests by adding IP to the database
		IEnumerator e = DataWriter.AppendData ("hosting", myIp + "|");
		while (e.MoveNext ())
			yield return e.Current;

		Debug.Log ("appended data");

		//Go to room
		StartCoroutine (multiplayer.HostMatchRoom ());

		yield return null;

	}

}
