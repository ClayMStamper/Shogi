using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour {

	#region singleton

	public static Settings instance;

	void Awake(){

		if (instance == null) {
			instance = this;
		} else {
			Destroy (gameObject);
		}

		DontDestroyOnLoad (gameObject);

	}

	public static Settings GetInstance(){
		return instance;
	}

	#endregion

	[SerializeField]
	private Material piecesMat;
	public Texture[] PiecesSkins;
	public Texture chosenSkin;
	public float volume;
	public bool highlightMoves;
	public bool fingerMarker;
	public bool showScore;
	public bool soundsOn;
	public bool musicOn;
	public bool warnWhenChecked;



}
