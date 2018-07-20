using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum IndexKey {name, winRate}

public class DataWriter : MonoBehaviour {

	#region coroutines

	public static IEnumerator AppendData (string key, string addition){

		IEnumerator e = DataReader.Refresh (key);
		while (e.MoveNext ())
			yield return e.Current;

		DataReader.data += addition;

	//	AccountManager.GetInstance().InvokeSetData (

	}

	public static IEnumerator UserInit(){

		//get data string
		IEnumerator e = DataReader.Refresh ("users");
		while (e.MoveNext())
			yield return e.Current;

		//parse data for my unique identifier
		string myID = ((int.Parse (DataReader.data)) + 1).ToString();

		//edit data string with my user name

		//send data string

	}
	#endregion

}
