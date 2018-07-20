using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum IndexKey {name, winRate}

public class DataWriter : MonoBehaviour {

	#region coroutines

	//adds on to the end of a given data string
	public static IEnumerator AppendData (string key, string addition){

		IEnumerator e = DataReader.Refresh (key);
		while (e.MoveNext ())
			yield return e.Current;

		Debug.Log ("Data before addition: " + DataReader.data);

		DataReader.data += addition;

		Debug.Log ("Data after addition: " + DataReader.data);

		AccountManager.GetInstance ().InvokeSetData (key, DataReader.data);

		yield return null;

	}

	public static IEnumerator UserInit(){

		//get data string
		IEnumerator e = DataReader.GetNewestUserID ();
		while (e.MoveNext())
			yield return e.Current;

		Debug.Log ("Old Data: " + DataReader.data);

		//parse data for my unique identifier
		string myID = ((int.Parse (DataReader.data)) + 1).ToString();

		Debug.Log ("My ID: " + myID);

		//edit and send data string with my user ID (key)
		e = AppendData("users", (myID + "|"));
		while (e.MoveNext())
			yield return e.Current;

		//create user account date
		e = AccountManager.GetInstance().Register (myID);
		while (e.MoveNext ())
			yield return e.Current;

	//	PlayerPrefsManager.SetIsUserInit (true);

		Debug.Log ("User: " + myID + " should be init");



	}
	#endregion

}
