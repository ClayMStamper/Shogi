using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class SplashManager : MonoBehaviour {

	[SerializeField]
	private VideoPlayer player;

	void Start(){

	//	StartCoroutine (SpeedUp ());

	}

	void Update(){

		if (player.time >= 4.5f) {
			LevelManager.GetInstance ().LoadLevel ("01a_Menu");
		}

	}

	IEnumerator SpeedUp() {

		yield return new WaitForSeconds (1.5f);

		player.playbackSpeed *= 2;

	}

}
