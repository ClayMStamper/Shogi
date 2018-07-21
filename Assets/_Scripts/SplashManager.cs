using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class SplashManager : MonoBehaviour {

	[SerializeField]
	private VideoPlayer player;

	void Update(){
		if (player.time >= 4.5f) {
			LevelManager.GetInstance ().LoadLevel ("01a_Menu");
		}
	}

}
