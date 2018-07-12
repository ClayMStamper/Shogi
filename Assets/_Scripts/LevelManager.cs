using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    #region Singleton
    private static LevelManager instance;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            //Debug.LogError("More than one " + transform.name + " in the scene");
            Destroy(gameObject);
        }
   //     DontDestroyOnLoad(gameObject);
    }

    public static LevelManager GetInstance() {
        return instance;
    }

    #endregion

    public delegate void OnLevelLevelWasLoaded();
    public OnLevelLevelWasLoaded onLevelWasLoadedCallback;

	public bool autoloadNextLevel = false;
	public float autoLoadTime;

    void OnEnable() {
        SceneManager.sceneLoaded += LevelFinishedLoading;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= LevelFinishedLoading;
    }

	void Start(){

		if (autoloadNextLevel) {
			StartCoroutine (AutoLoad ());
		}

	}

    void LevelFinishedLoading(Scene scene, LoadSceneMode mode) {
        if (onLevelWasLoadedCallback != null) {
            onLevelWasLoadedCallback.Invoke();
        }
        //	Debug.Break ();
    }

    public void LoadLevel(string levelName) {
        SceneManager.LoadScene(levelName);
    }
	public void LoadLevel(int index) {
		SceneManager.LoadScene(index);
	}

    public string GetCurrentLevelName() {
        return SceneManager.GetActiveScene().name;
    }

	public int GetCurrentLevelIndex() {
		return SceneManager.GetActiveScene().buildIndex;
	}

	public void LoadNextLevel(){

		int myBuildIndex = SceneManager.GetActiveScene ().buildIndex;

		LoadLevel (myBuildIndex + 1);

	}

	IEnumerator AutoLoad(){

		yield return new WaitForSeconds (autoLoadTime);

		LoadNextLevel ();

	}

}