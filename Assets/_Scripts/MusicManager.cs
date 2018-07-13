using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour {

	#region Singleton
	private static MusicManager instance;

	void Awake(){
		if (instance == null) {
			instance = this;
		} else {
			//    Debug.LogError("More than one " + transform.name + " in the scene");
			Destroy (gameObject);
		}
		DontDestroyOnLoad (gameObject);
	}

	public static MusicManager GetInstance(){
		return instance;
	}

	#endregion

	LevelManager levelManager;

	AudioSource audioSource;
	[SerializeField]
	[Tooltip("Audio clips for each scene (in build index order)")]
	AudioClip[] clips;

	private int sceneIndex = 0;

	[SerializeField]
	float fadeSpeed;

	void Start(){

		levelManager = LevelManager.GetInstance ();
		levelManager.onLevelWasLoadedCallback += OnLevelLoaded;
		audioSource = GetComponent <AudioSource> ();

		fadeSpeed = 0.1f;

		Play ();

	}

	void OnLevelLoaded(){

		sceneIndex = levelManager.GetCurrentLevelIndex();

		if (audioSource.clip != clips [sceneIndex]) {
			
			StartCoroutine(fadeOutClip(true));

		}

	}

	IEnumerator fadeOutClip(bool fadeBackIn){

		//Debug.Log ("Clip fading out");

		while (audioSource.volume > 0f){

			//Debug.Log ("Volume: " + audioSource.volume);

			if (Settings.GetInstance ().musicIsOn){
				fadeBackIn = true;
				break;
			}

			audioSource.volume -= Time.deltaTime * fadeSpeed;

			yield return null;

		}

		if (fadeBackIn) {
			StartCoroutine (FadeInClip ());
		} else {
			audioSource.Pause ();
		}

		yield return null;

	}

	IEnumerator FadeInClip(){

		if (audioSource.volume > 0)
			audioSource.volume = 0;

	//	Debug.Log ("Clip fading in with volume at: " + audioSource.volume);

		int sceneIndex = levelManager.GetCurrentLevelIndex();
		audioSource.clip = clips [sceneIndex];
		if (audioSource.isPlaying) {
			audioSource.UnPause ();
		} else {
			audioSource.Play ();
		}

		while (audioSource.volume < Settings.GetInstance().volume){ 

	//		Debug.Log ("Volume: " + audioSource.volume);
			if (!Settings.GetInstance ().musicIsOn)
				break;
			
			audioSource.volume += Time.deltaTime * fadeSpeed;

			yield return null;

		}

		//after first fade
		fadeSpeed = .5f;

		yield return null;

	}

	public void UpdateVolume(){
		audioSource.volume = Settings.GetInstance().volume;
	}

	public void Play(){

		Debug.Log ("Should be playing");

		audioSource.volume = Settings.GetInstance ().volume;


		if (Settings.GetInstance ().musicIsOn) {
			audioSource.volume = 0;
		//	StopCoroutine (fadeOutClip (false));
			StartCoroutine (FadeInClip ());
		} else {
			StopCoroutine (FadeInClip());
			StartCoroutine (fadeOutClip (false));
		}

	}

}
