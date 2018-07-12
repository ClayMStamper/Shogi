using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DatabaseControl; // << Include the DatabaseControl namespace

public struct TokenType {
	public static string all = "AccessTokens";
	public static string freshmen = "Freshmen";
	public static string seniors = "Seniors";
}

public class AccountManager : MonoBehaviour {

	#region Singleton
	private static AccountManager instance;

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

	public static AccountManager GetInstance() {
		return instance;
	}

	#endregion

	public Text loginErrorText;
	public Text registerErrorText;

	string databaseName = "Accounts";

	public string username = "";
	public static string accountType = TokenType.all;

	string loginError = "";
	string registerError = "";

	public static bool isSettingData, isGettingData = false;

	public delegate void OnDataRecievedCallback(string data);

	//Called when login button is pressed
	public void FacebookLogin (string token)
	{
		this.username = token;

		StartCoroutine(Login()); // << Runs the IEnumerator which runs the command sequence

	}

	public void InvokeGetData(OnDataRecievedCallback onDataRecieved) {

		StartCoroutine(GetMyData(onDataRecieved));

	}

	public void InvokeGetData (string token, OnDataRecievedCallback onDataRecieved){

		StartCoroutine(GetData (token, onDataRecieved));

	}

	//sets local user data
	public void InvokeSetData(string data) {

		StartCoroutine(SetData(data));

	}
	//sets some other accounts data
	public void InvokeSetData(string token, string data) {

		StartCoroutine(SetData(token, data));

	}

	public void RegisterOrLogin (string tokenString){

		username = tokenString;

		StartCoroutine (Register());
	}
		
	void TokenRegisterCallback(string data){

		string newData = ""; // = data + "/" + FacebookManager.myToken.TokenString;

		//edit TokenType.all to sort types of accounts' access tokens
		switch (AccountManager.accountType) {



		case "AccessTokens":
			InvokeSetData (TokenType.all, newData);
			break;
		case "Freshmen":
			InvokeSetData (TokenType.all, newData);
			InvokeSetData (TokenType.freshmen, newData);
			break;
		case "Seniors":
			InvokeSetData (TokenType.all, newData);
			InvokeSetData (TokenType.seniors, newData);
			break;
		default:
			Debug.LogError ("Account type / Token type not recognized");
			break;


		}



	}


	#region Run Command Sequences
	//Run the command sequence to login a user
	IEnumerator Login ()
	{
		//Run the command sequence called 'Login' on the database name which has been retrieved in the start method. Sends the username and password (from the ui input fields) as parameters
		IEnumerator e = DCP.RunCS(databaseName, "Login", new string[2] { username, username });

		//wait for the command sequence to finish loading
		while (e.MoveNext())
		{
			yield return e.Current;
		}

		//get the command sequence result
		string returnText = e.Current as string;

		if (returnText == "Success")
		{
			Debug.Log ("Logged in on database: " + username);
			//Login was successful
			loginError = ""; // << make ui text for login error disappear
			yield return new WaitForSeconds(0.5f);
		} else
		{
			if (returnText == "UserError")
			{
				//The username does not exist
				loginError = "The username does not exist";
			} else
			{
				if (returnText == "PassError")
				{
					//The username was found but the password was incorrect
					loginError = "The password is incorrect";
				} else
				{
					//Some other error which is unlikely to happen but should be considered if it does
					loginError = "Problem with server. Try again later.";
				}
			}
			yield return new WaitForSeconds(0.5f);
		}
	}

	//Run the command sequence to register a user
	IEnumerator Register ()
	{
		//Run the command sequence called 'Register' on the database name which has been retrieved in the start method. Sends the username and password (from the ui input fields) as parameters
		IEnumerator e = DCP.RunCS(databaseName, "Register", new string[2] { username, username });

		//wait for the command sequence to finish loading
		while (e.MoveNext())
		{
			yield return e.Current;
		}

		//get the command sequence result
		string returnText = e.Current as string;

		if (returnText == "Success")
		{
			Debug.Log ("Registered on database: " + username);

			//this line stores my access token
			InvokeGetData ("AccessTokens", TokenRegisterCallback);

			//Registration was successful
			registerError = ""; // << make ui text for register error disappear
			StartCoroutine(Login());
			yield return new WaitForSeconds(0.5f);
		} else
		{
			if (returnText == "username in use")
			{
				//The username is already in use
				registerError = "The username is in use. Logging in.";
				StartCoroutine(Login());
			} else
			{
				//Some other error which is unlikely to happen but should be considered if it does
				registerError = "Problem with server. Try again later.";
			}
			yield return new WaitForSeconds(0.5f);
		}
	}

	//Run the command sequence to set user data
	IEnumerator SetData (string data)
	{

		isSettingData = true;

		//Run the command sequence called 'Set Data' on the database name which has been retrieved in the start method. Sends the username, password and data string (from the ui input fields) as parameters
		IEnumerator e = DCP.RunCS(databaseName, "Set Data", new string[3] { username, username, data });

		//wait for the command sequence to finish loading
		while (e.MoveNext())
		{
			yield return e.Current;
		}

		//get the command sequence result
		string returnText = e.Current as string;

		if (returnText == "Success")
		{
			//set data was successful
			yield return new WaitForSeconds(0.5f);
		}
		else
		{
			//Either error with server or error with user's username and password so logout
			yield return new WaitForSeconds(0.5f);
		}
	}

	IEnumerator SetData (string token, string data)
	{

		isSettingData = true;

		//Run the command sequence called 'Set Data' on the database name which has been retrieved in the start method. Sends the username, password and data string (from the ui input fields) as parameters
		IEnumerator e = DCP.RunCS(databaseName, "Set Data", new string[3] { token, token, data });

		//wait for the command sequence to finish loading
		while (e.MoveNext())
		{
			yield return e.Current;
		}

		//get the command sequence result
		string returnText = e.Current as string;

		if (returnText == "Success")
		{
			//set data was successful
			yield return new WaitForSeconds(0.5f);
		}
		else
		{
			//Either error with server or error with user's username and password so logout
			yield return new WaitForSeconds(0.5f);
		}
	}

	//Run the command sequence to get user data
	IEnumerator GetMyData(OnDataRecievedCallback onDataRecieved)
	{

		isGettingData = true;

		//Run the command sequence called 'Get Data' on the database name which has been retrieved in the start method. Sends the username and password (from the ui input fields) as parameters
		IEnumerator e = DCP.RunCS(databaseName, "Get Data", new string[2] { username, username });

		//wait for the command sequence to finish loading
		while (e.MoveNext())
		{
			yield return e.Current;
		}

		//get the command sequence result
		string data = e.Current as string;

		if (data != null) {
			onDataRecieved.Invoke(data);
		} else {
			Debug.Log("Data null");
		}
		yield return new WaitForSeconds(0.5f);

		isGettingData = false;

	}

	IEnumerator GetData(string creds, OnDataRecievedCallback onDataRecieved)
	{

		isGettingData = true;

		//Run the command sequence called 'Get Data' on the database name which has been retrieved in the start method. Sends the username and password (from the ui input fields) as parameters
		IEnumerator e = DCP.RunCS(databaseName, "Get Data", new string[2] { creds, creds });

		//wait for the command sequence to finish loading
		while (e.MoveNext())
		{
			yield return e.Current;
		}

		//get the command sequence result
		string data = e.Current as string;

		if (data != null) {
			onDataRecieved.Invoke(data);
		} else {
			Debug.Log("Data null");
		}
		yield return new WaitForSeconds(0.5f);

		isGettingData = false;

	}

	#endregion
}
