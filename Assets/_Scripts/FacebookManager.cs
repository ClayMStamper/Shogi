using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class FacebookManager : MonoBehaviour {

	#region Singleton
	private static FacebookManager instance;

	void Awake() {
		if (instance == null) {
			instance = this;
		} else {
			Debug.LogError("More than one " + transform.name + " in the scene");
			Destroy(gameObject);
		}
		DontDestroyOnLoad(instance);
		instance = this;
	}

	public static FacebookManager GetInstance() {
		return instance;
	}

	#endregion

	public static AccessToken myToken;

	public void Login () {

		Debug.Log ("Logging in");

		FB.Init (InitCallback, OnHideUnity);

	}

	private void InitCallback () {

		Debug.Log ("Init");

		var perms = new List<string>(){"public_profile", "email"};
		FB.LogInWithReadPermissions(perms, AuthCallback);

	}

	private void OnHideUnity (bool isGameShown) {

		Debug.Log ("Unity is paused");

	}

	//account is authorized
	private void AuthCallback (ILoginResult result) {

		Debug.Log ("Auth");

		if (FB.IsLoggedIn) {
			Debug.Log ("Logged in");
			AccessToken token = Facebook.Unity.AccessToken.CurrentAccessToken;
			myToken = token;

			/*
			string tokenString = myToken.TokenString;
			string id = myToken.UserId;
			AccessToken testToken = new AccessToken (tokenString, id, System.DateTime.MaxValue, new List <string> () { "public_profile", "email" });
			Debug.Log (testToken.ToJson());
			foreach (string perm in token.Permissions) {
				Debug.Log(perm);
			}
			*/

			//display user data
			AccountManager.GetInstance().RegisterOrLogin(myToken.UserId);

		} else {

			Debug.LogError ("DASFASFWAFWWQFQA");

			Debug.Log (result.ResultDictionary);
		}

	}

}
