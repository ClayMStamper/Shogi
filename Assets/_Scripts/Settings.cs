using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

	public const string HIGHLIGHT_KEY = "highlight", CHECK_WARNING_KEY = "checkWarning", FINGER_MARKER_KEY = "fingerMarker",
	SHOW_SCORE_KEY = "showScore", SOUND_KEY = "sound", MUSIC_KEY = "music";

	[SerializeField]
	public Sprite toggleOn, toggleOff;

	[SerializeField][Header ("Reference Textures")]
	private Material piecesMat;
	public Texture[] PiecesSkins;

	[Header ("Settings Values")]
	public Texture chosenSkin;
	public float volume = 1.0f;
	public bool highlightMoves = true;
	public bool fingerMarker = true;
	public bool showScore = true;
	public bool soundIsOn = true;
	public bool musicIsOn = true;
	public bool warnWhenChecked = true;

	void OnEnable(){

		highlightMoves = PlayerPrefsManager.GetToggleIsOn (HIGHLIGHT_KEY);
		fingerMarker = PlayerPrefsManager.GetToggleIsOn (FINGER_MARKER_KEY);
		showScore = PlayerPrefsManager.GetToggleIsOn (SHOW_SCORE_KEY);
		soundIsOn = PlayerPrefsManager.GetToggleIsOn (SOUND_KEY);
		musicIsOn = PlayerPrefsManager.GetToggleIsOn (MUSIC_KEY);
		warnWhenChecked = PlayerPrefsManager.GetToggleIsOn (CHECK_WARNING_KEY);

	}

	public bool Toggle(string key){

		switch (key) {

		case HIGHLIGHT_KEY:
			highlightMoves = !highlightMoves;
			PlayerPrefsManager.SetToggleIsOn (HIGHLIGHT_KEY, highlightMoves);
			return highlightMoves;
		case CHECK_WARNING_KEY:
			warnWhenChecked = !warnWhenChecked;
			PlayerPrefsManager.SetToggleIsOn (CHECK_WARNING_KEY, warnWhenChecked);
			return warnWhenChecked;
		case FINGER_MARKER_KEY:
			fingerMarker = !fingerMarker;
			PlayerPrefsManager.SetToggleIsOn (FINGER_MARKER_KEY, fingerMarker);
			return fingerMarker;
		case SHOW_SCORE_KEY:
			showScore = !showScore;
			PlayerPrefsManager.SetToggleIsOn (SHOW_SCORE_KEY, showScore);
			return showScore;
		case SOUND_KEY:
			soundIsOn = !soundIsOn;
			PlayerPrefsManager.SetToggleIsOn (SOUND_KEY, soundIsOn);
			return soundIsOn;
		case MUSIC_KEY:
			musicIsOn = !musicIsOn;
			PlayerPrefsManager.SetToggleIsOn (MUSIC_KEY, musicIsOn);

			MusicManager.GetInstance ().Play ();

			return musicIsOn;
		default:
			Debug.LogError (key + ": not recognized as a key. Toggle value couldn't be recorded");
			return false;

		}

	}

}
