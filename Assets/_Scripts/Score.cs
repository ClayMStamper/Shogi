using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {

	#region singleton

	//ensure that there is only one of these in the scene

	private static Score instance;

	void Awake(){

		if (instance == null) {
			instance = this;
		} else {
			Destroy (gameObject);
		}
	}

	public static Score GetInstance(){
		return instance;
	}

	#endregion

	void Start(){

		Settings settings = Settings.GetInstance ();

		if (!settings.showScore) {
			Destroy (transform.parent.gameObject);
		}

	}

	public void UpdateScore(){

		BoardManager boardManager = BoardManager.GetInstance ();
		Board currentBoard = new Board (boardManager.pieces);

		Text text = GetComponent <Text> ();
		text.text = "Score: " + currentBoard.eval;

	}


}
