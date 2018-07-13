using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour {

	//highlight moves are on
	public static void SetToggleIsOn(string key, bool toggleValue){

		if (toggleValue) {
			PlayerPrefs.SetInt (key, 1); // 1 for true
		} else {
			PlayerPrefs.SetInt (key, 0); // 0 for false
		}
	}

	public static bool GetToggleIsOn(string key){
		
		int toggleValue = PlayerPrefs.GetInt (key);

		if (toggleValue == 1) {
			return true;
		} else {
			return false;
		}

	}
		

}