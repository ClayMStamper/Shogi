﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum IndexKey {name, winRate}

public class DataWriter : MonoBehaviour {

	#region coroutines

	//adds on to the end of a given data string
	public static IEnumerator AppendData (string key, string addition){

		MenusManager.GetInstance ().ToggleLoading (true, "Sending Data");

		IEnumerator e = DataReader.Refresh (key);
		while (e.MoveNext ())
			yield return e.Current;

		Debug.Log ("Data before addition: " + DataReader.data);

		DataReader.data += addition;

		Debug.Log ("Data after addition: " + DataReader.data);

		AccountManager.GetInstance ().InvokeSetData (key, DataReader.data);

		MenusManager.GetInstance ().ToggleLoading (false);
		yield return null;

	}

	public static IEnumerator SubtractData (string key, string subtraction){

		MenusManager.GetInstance ().ToggleLoading (true, "Sending Data...");

		IEnumerator e = DataReader.Refresh (key);
		while (e.MoveNext ())
			yield return e.Current;

		Debug.Log ("Data before subtraction: " + DataReader.data);

		if (DataReader.data.Contains (subtraction)) {
			DataReader.data = DataReader.data.Replace (subtraction, "");
		} else {
			Debug.LogError ("Data to remove: " + subtraction + ", was not found in data: " + DataReader.data);
		}

		Debug.Log ("Data after subtraction: " + DataReader.data);

		AccountManager.GetInstance ().InvokeSetData (key, DataReader.data);

		MenusManager.GetInstance ().ToggleLoading (false);
		yield return null;

	}

	public static IEnumerator UserInit(){

		MenusManager.GetInstance ().ToggleLoading (true);
		MenusManager.GetInstance ().loadMsg.text = "Performing first time setup";

		//get data string
		IEnumerator e = DataReader.GetNewestUserID ();
		while (e.MoveNext())
			yield return e.Current;

		//parse data for my unique identifier
		string myID = ((int.Parse (DataReader.data)) + 1).ToString();

		//edit and send data string with my user ID (key)
		e = AppendData("users", (myID + "|"));
		while (e.MoveNext())
			yield return e.Current;

		//create user account date
		e = AccountManager.GetInstance().Register (myID);
		while (e.MoveNext ())
			yield return e.Current;

		PlayerPrefsManager.SetIsUserID (int.Parse (myID));

		Debug.Log ("User: " + myID + " should be init");

		MenusManager.GetInstance ().ToggleLoading (false);

	}
	#endregion

}
