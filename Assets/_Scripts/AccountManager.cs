using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DatabaseControl; 

//set on register or by another manager
public class AccountManager : MonoBehaviour {

	#region Singleton
	private static AccountManager instance;

	void Awake() {
		if (instance == null) {
			instance = this;
		} else {
			Debug.Log("More than one " + transform.name + " in the scene");
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

    string loginError = "";
    string registerError = "";

	public static bool isSettingData, isGettingData = false;

	public delegate void OnDataRecievedCallback(string data);

	#region Public methods

	void Start(){

		StartCoroutine (DataReader.GetAndPrint ("1"));

	}

	void PrintData(string data){

		Debug.Log (data);

	}

	//get my data
	public void InvokeGetData(OnDataRecievedCallback onDataRecieved) {

		StartCoroutine(GetData(onDataRecieved));

	}

	//get any player's data
	public void InvokeGetData (string username, OnDataRecievedCallback onDataRecieved){

		StartCoroutine(GetData (username, onDataRecieved));

	}
		

	public void InvokeSetData(string data) {
		
		StartCoroutine(SetData(data));

	}

	public void RegisterOrLogin (){

		print (username);

		if (username == "")
			return;

		IEnumerator e = DCP.RunCS(databaseName, "Register", new string[2] { username, username });

		StartCoroutine (Register());

	}

	#endregion

    #region Run Command Sequences

	//Run the command sequence to register a user
	IEnumerator Register () {

		print ("Registering " + username);

		//Run the command sequence called 'Register' on the database name which has been retrieved in the start method. Sends the username and password (from the ui input fields) as parameters
		IEnumerator e = DCP.RunCS(databaseName, "Register", new string[2] { username, username });

		Debug.Log (e.ToString ());

		//wait for the command sequence to finish loading
		while (e.MoveNext())
		{
			yield return e.Current;
		}

		//get the command sequence result
		string returnText = e.Current as string;

		Debug.Log (returnText);

		if (returnText == "Success") {
			Debug.Log ("Registered on database: " + username);
			//Registration was successful
			string data = username + "|";
			StartCoroutine (SetData (data));
			StartCoroutine(Login());
			yield return new WaitForSeconds(0.5f);
		} else
		{
			if (returnText == "username in use")
			{
				//The username is already in use
				registerError = "The username is in use. Logging in.";
				//get data and then login
				StartCoroutine(Login());
			} else
			{
				//Some other error which is unlikely to happen but should be considered if it does
				registerError = "Problem with server. Try again later.";
			}
			yield return new WaitForSeconds(0.5f);
		}
	}

    //Run the command sequence to login a user
    IEnumerator Login () {

        //Run the command sequence called 'Login' on the database name which has been retrieved in the start method. Sends the username and password (from the ui input fields) as parameters
		IEnumerator e = DCP.RunCS(databaseName, "Login", new string[2] { username, username});

        //wait for the command sequence to finish loading
        while (e.MoveNext())
        {
            yield return e.Current;
        }

        //get the command sequence result
        string returnText = e.Current as string;

        if (returnText == "Success")
        {
			Debug.Log ("Logged in on database: " + username );
            //Login was successful
            loginError = ""; // << make ui text for login error disappear

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

    //Run the command sequence to set user data
	IEnumerator SetData (string data) {

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

    //Run the command sequence to get user data
	IEnumerator GetData(OnDataRecievedCallback onDataRecieved) {

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

	IEnumerator GetData(string username, OnDataRecievedCallback onDataRecieved) {

		Debug.Log ("Getting data for: " + username);

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

    #endregion
}
