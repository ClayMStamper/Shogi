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

	//serialize later
	public static void SetIsUserID(int userID){

		PlayerPrefs.SetInt ("id", userID); // 1 for true

	}

	public static int GetUserID(){
		return PlayerPrefs.GetInt ("id");
	}

	public static bool GetIsUserInit(){

		int toggleValue = PlayerPrefs.GetInt ("id");

		return toggleValue == 0 ? false : true;

	}


		

}