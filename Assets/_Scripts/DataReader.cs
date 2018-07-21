using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataReader : MonoBehaviour {

	AccountManager accountManager;

	public static string data = "fuqwpfqpfhq";

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

		MenusManager.GetInstance ().ToggleLoading (true);
		MenusManager.GetInstance ().loadMsg.text = "Fetching data";

		string startData = data;

		while (data == startData) {
			Debug.Log ("waiting");
			yield return new WaitForSeconds (0.1f);
		}

		MenusManager.GetInstance ().ToggleLoading (false);

	}

	public static IEnumerator Refresh(string key){

		IEnumerator e = AccountManager.GetInstance ().GetData (key, OnDataRecieved);
		while (e.MoveNext ())
			yield return e.Current;

		Debug.Log ("Data refereshed");

	}

	public static IEnumerator GetAndPrint(string key){

		IEnumerator e = Refresh (key);
		while (e.MoveNext()) 
			yield return e.Current;

		Debug.Log (data);
		yield return null;

	}

	public static IEnumerator GetNewestUserID(){

		MenusManager.GetInstance ().ToggleLoading (true);
		MenusManager.GetInstance ().loadMsg.text = "Registering for online play";

		IEnumerator e = Refresh ("users");
		while (e.MoveNext())
			yield return e.Current;

		//split to array of IDs
		string[] dataSplits = DataReader.data.Split('|');

		//set data to last ID
		DataReader.data = dataSplits [dataSplits.Length - 2];
		Debug.Log (DataReader.data);
		MenusManager.GetInstance ().ToggleLoading (false);
		yield return null;

	}

	#endregion

}
