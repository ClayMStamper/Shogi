using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataReader : MonoBehaviour {

	AccountManager accountManager;

	public static string data = "";

	#region private static methods

	static void FetchBegin(string key){
		AccountManager.GetInstance().InvokeGetData (key, OnDataRecieved);
	}

	static void OnDataRecieved(string _data){
		data = _data;
	}

	#endregion

	#region coroutines

	//waits until data is up to date with the server
	static IEnumerator Wait(){

		string startData = data;

		while (data == startData) {
			yield return new WaitForSeconds (0.1f);
		}
	}

	public static IEnumerator Refresh(string key){

		FetchBegin (key);

		IEnumerator e = Wait ();
		while (e.MoveNext ())
			yield return e.Current;

	}

	public static IEnumerator GetAndPrint(string key){
		
		IEnumerator e = Refresh (key);
		while (e.MoveNext()) yield return e.Current;

		Debug.Log (data);
	}

	public static IEnumerator GetNewestUserID(){

		IEnumerator e = Refresh ("users");
		while (e.MoveNext())
			yield return e.Current;

		//split to array of IDs
		string[] dataSplits = DataReader.data.Split('|');

		//set data to last ID
		DataReader.data = dataSplits [dataSplits.Length - 2];
		Debug.Log (DataReader.data);

	}

	#endregion

}
