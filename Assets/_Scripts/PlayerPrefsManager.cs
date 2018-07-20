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

		return toggleValue == 0 ? false : true;

	}

	public static void SetSkin(int index){
		PlayerPrefs.SetInt ("piecesSkin", index);
	}

	public static int GetSkin(){
		return PlayerPrefs.GetInt ("piecesSkin");
	}

	public static void SetIsUserInit(bool toggleValue){

		if (toggleValue) {
			PlayerPrefs.SetInt ("isInit", 1); // 1 for true
		} else {
			PlayerPrefs.SetInt ("isInit", 0); // 0 for false
		}

	}

	public static bool GetIsUserInit(){

		int toggleValue = PlayerPrefs.GetInt ("isInit");

		return toggleValue == 0 ? false : true;

	}
		

}