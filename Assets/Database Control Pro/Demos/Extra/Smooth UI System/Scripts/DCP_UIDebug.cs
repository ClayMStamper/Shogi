using UnityEngine;
using System.Collections;

public class DCP_UIDebug : MonoBehaviour {

	public string debugString = "DCP_UIDebug script method called";

	public void RunScript () {
		Debug.Log (debugString);
	}
}
