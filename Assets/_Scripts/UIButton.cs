using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButton : MonoBehaviour {

	Settings settings;

	void Start(){
		settings = Settings.GetInstance ();
	}

	public void SetSkin(int index){

		settings.chosenSkin = settings.PiecesSkins [index];
		PlayerPrefsManager.SetSkin (index);

		settings.piecesMat.mainTexture = settings.chosenSkin;

	}

	public void SetSkin(){
		settings.piecesMat.mainTexture = settings.chosenSkin;
	}

}
