using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataReader : MonoBehaviour {

	AccountManager accountManager;

	public static string data = "";
	public static bool isCurrent = false;

	static void FetchBegin(string key){

		AccountManager.GetInstance().InvokeGetData (key, OnDataRecieved);

	}

	static void OnDataRecieved(string _data){

		data = _data;

	}

	//waits until data is up to date with the server
	static IEnumerator Wait(){

		string startData = data;

		while (data == startData) {
			yield return new WaitForSeconds (0.1f);
		}
	}

	public static IEnumerator Refresh(string key){

		isCurrent = false;

		FetchBegin (key);

		IEnumerator e = Wait ();
		while (e.MoveNext ())
			yield return e.Current;
		
		isCurrent = true;

	}

	public static IEnumerator GetAndPrint(string key){
		
		IEnumerator e = Refresh (key);
		while (e.MoveNext()) yield return e.Current;

		Debug.Log (data);
	}

}
