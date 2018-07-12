using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DatabaseControl;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public class DCP_CCD_MenuManager : MonoBehaviour {
	
	//This script controls all of the login/register menu functionality
	
	//The enumerator for the part the menu is on
    enum MenuPart
    {
        Login,
        Register,
        LoggedIn
    }

    #region variables
	//Variables for moving the ui in and out
    public DCP_UIDelay loginMoveIn;
    public DCP_UIDelay registerMoveIn;
    public DCP_UIDelay errorMoveIn;
    public DCP_UIDelay timeMoveIn;
    public DCP_UIMove loginMoveOut;
    public DCP_UIMove registerMoveOut;
    public DCP_UIMove errorMoveOut;
	public DCP_UIMove timeMoveOut;
	
	//The UI texts to set for the error UI
    public Text errorTitleText;
	public Text errorContentsText;
	
	//The UI InputFields for usernames, passwords, etc
    public InputField login_UsernameField;
    public InputField login_PasswordField;
    public InputField register_UsernameField;
    public InputField register_PasswordField;
    public InputField register_ConfirmPasswordField;
	public InputField register_EmailField;
	
	//The UI text to display the time since the player last logged in
    public Text timeSinceLastLoginText;
	
	//References to the managers
    public DCP_CCD_BuildManager buildManager;
    public DCP_CCD_UpgradeManager upgradeManager;
    public DCP_CCD_MoveManager moveManager;
    public DCP_CCD_SellManager sellManager;
    public DCP_CCD_PlayerBuildingsManager buildingsManager;
    public DCP_CCD_MoneyManager moneyManager;
    public DCP_CCD_UIManager uiManager;
    public DCP_CCD_TransitionManager transManager;
    public DCP_CCD_DCPManager dcpManager;

	MenuPart part = MenuPart.Login; // << The part the menu is currently on
	string errorText = "Error"; // The text which the error ui's text should display

    string dataString = "";

    //These are used to make sure the demo scene has been setup correctly and to store the databse name which is used to run Command Sequences
    string databaseName = "";
    bool canRunSequences = false;
    #endregion

    void Awake()
    {
        //Gets the databaseName as it was setup through the editor
        GameObject linkObj = GameObject.Find("Link");
        if (linkObj == null)
        {
            Debug.LogError("DCP Error: Cannot find the link object in the scene so scripts running Command Sequences don't know the database name");
        }
        else
        {
            DCP_Demos_LinkDatabaseName linkScript = linkObj.gameObject.GetComponent<DCP_Demos_LinkDatabaseName>() as DCP_Demos_LinkDatabaseName;
            if (linkScript == null)
            {
                Debug.LogError("DCP Error: Cannot find the link script on link object so scripts running Command Sequences don't know the database name");
            }
            else
            {
                if (linkScript.databaseName == "")
                {
                    Debug.LogError("DCP Error: This demo scene has not been setup. Please setup the demo scene in the Setup window before use. Widnow>Database Control Pro>Setup Window");
                }
                else
                {
                    databaseName = linkScript.databaseName;
                    canRunSequences = true;
                }
            }
        }
    }

	void Update () {
		//Sets the error ui text based on the menu part
	    if (part == MenuPart.Login)
        {
            errorTitleText.text = "Login Error";
        } else
        {
            errorTitleText.text = "Register Error";
        }
        errorContentsText.text = errorText;
    }

    #region button pressed methods
    public void Login_LoginButtonPressed ()
	{
		//Called when the 'Login' button is pressed on the login ui
		
        if (canRunSequences == true)
        {
        	//Checks the length of the username and password
            if (login_UsernameField.text.Length < 6)
            {
            	//If it is too short move the login ui out and the error ui in
                errorText = "Username incorrect";
                loginMoveOut.MoveBack();
                errorMoveIn.StartDelay();
            } else
            {
                if (login_PasswordField.text.Length < 6)
                {
                    errorText = "Password incorrect";
                    loginMoveOut.MoveBack();
                    errorMoveIn.StartDelay();
                }
                else
                {
                	//Makes the dcpManager remember the submitted username and password to use when running in-game Command Sequences
	                dcpManager.SetUsernameAndPass(login_UsernameField.text, login_PasswordField.text);
	                
	                //Starts the IEnumerator to run the 'Login' Command Sequence
	                StartCoroutine(Login(login_UsernameField.text, login_PasswordField.text));
	                
	                //Make the login ui move out of view
                    loginMoveOut.MoveBack();
                }
            }
        }
    }
    public void Register_RegisterButtonPressed ()
	{
		//Called when the 'Register' button is pressed on the register ui
		
        if (canRunSequences == true)
        {
        	//Checks the lengths of the usernames and passwords and if the passwords match
            if (register_UsernameField.text.Length < 6)
            {
            	//If it is too short move the register ui out and the error ui in
                errorText = "Username too short";
                registerMoveOut.MoveBack();
                errorMoveIn.StartDelay();
            } else
            {
                if (register_PasswordField.text != register_ConfirmPasswordField.text)
                {
                    errorText = "Passwords do not match";
                    registerMoveOut.MoveBack();
                    errorMoveIn.StartDelay();
                } else
                {
                    if (register_PasswordField.text.Length < 6)
                    {
                        errorText = "Password too short";
                        registerMoveOut.MoveBack();
                        errorMoveIn.StartDelay();
                    } else
                    {
	                    if (TestEmail.IsEmail(register_EmailField.text) == false) // << Calls another method to check the submitted email address is in a valid format
	                    {
		                    //If it isn't valid move the error ui in and register ui out
                            errorText = "Email not valid";
                            registerMoveOut.MoveBack();
                            errorMoveIn.StartDelay();
                        } else
	                    {
                        	//All of the provided information is valid
                        	
                        	//Move the register ui out
	                        registerMoveOut.MoveBack();
	                        
	                        //Makes the dcpManager remember the submitted username and password to use when running in-game Command Sequence
	                        dcpManager.SetUsernameAndPass(register_UsernameField.text, register_PasswordField.text);
	                        
	                        //Runs the 'Register' IEnumerator to run the 'Register' Command Sequence
                            StartCoroutine(Register(register_UsernameField.text, register_PasswordField.text, register_EmailField.text));
                        }
                    }
                }
            }
        }
    }
    public void Login_RegisterButtonPressed ()
	{
		// Called when the 'or sign up now' button is pressed on the login ui
		// all of the ui moving is handled by the Button scripts
		
		//Set the menu part to register
        part = MenuPart.Register;
    }
    public void Register_BackButtonPressed ()
	{
		// Called when the 'back' button is pressed on the register ui
		// all of the ui moving is handled by the Button scripts
		
		//Set the menu part to login
        part = MenuPart.Login;
    }
    public void ErrorOkButtonPressed ()
	{
		// Called when the 'OK' button is pressed on the error ui
		// The ui moving is handled here as it depends on the whether the menu part is for login or register
		errorMoveOut.MoveBack(); // << move the error ui out
        if (part == MenuPart.Login)
        {
	        loginMoveIn.StartDelay(); // << move the login ui in
        } else
        {
	        registerMoveIn.StartDelay(); // << move the register ui in
        }
    }
    public void TimeStuffOkPressed ()
	{
		// Called when the 'OK' button is pressed on the time since last login ui (after logging in)
		
		// The time ui is moved off the screen
		timeMoveOut.MoveBack();
		
		//The menu part is changed to loggedIn
		part = MenuPart.LoggedIn;
		
		//Load the player data and log the player in
        LoadPlayerData(dataString);
        Login();
    }
    #endregion

    #region Request IEnumerators
    //Run the command sequence to login a user
    IEnumerator Login(string username, string password)
    {
        //Run the command sequence called 'Login' on the database name which has been retrieved in the start method. Sends the username and password (from the ui input fields) as parameters
        IEnumerator e = DCP.RunCS(databaseName, "Login", new string[2] { username, password });

        //wait for the command sequence to finish loading
        while (e.MoveNext())
        {
            yield return e.Current;
        }

        //get the command sequence result
        string returnText = e.Current as string;

        //Split up the result and check the first part. If it is 'Success', 'UserError' or 'PassError'
        string[] data = returnText.Split(new string[1] { "/" }, System.StringSplitOptions.None);
        if (data != null)
        {
            if (data.Length > 0)
            {
                if (data[0] == "Success")
                {
                	//It is a success so remember the returned player data string and move in the time ui
                    dataString = returnText;
                    LoadTimeSinceLastLogin(dataString);
                    timeMoveIn.StartDelay();
                } else
                {
                	//There was an error so set the error text and display the error ui
                    if (data[0] == "UserError")
                    {
                        errorText = "Username incorrect";
                    } else
                    {
                        if (data[0] == "PassError")
                        {
                            errorText = "Password incorrect";
                        }
                        else
                        {
                            errorText = "Server Error. Try again later.";
                        }
                    }
                    errorMoveIn.StartDelay();
                }
            } else
            {
                //some other error
                errorText = "Server Error. Try again later.";
                errorMoveIn.StartDelay();
            }
        } else
        {
            //some other error
            errorText = "Server Error. Try again later.";
            errorMoveIn.StartDelay();
        }
    }

    //Run the command sequence to register a user
    IEnumerator Register(string username, string password, string email)
    {
        //Run the command sequence called 'Register' on the database name which has been retrieved in the start method. Sends the username and password and email (from the ui input fields) as parameters
        IEnumerator e = DCP.RunCS(databaseName, "Register", new string[3] { username, password, email });

        //wait for the command sequence to finish loading
        while (e.MoveNext())
        {
            yield return e.Current;
        }

        //get the command sequence result
        string returnText = e.Current as string;

        if (returnText == "Success")
        {
        	//If it returns 'Success', log the player in with their new account
        	moneyManager.Reset();
	        transManager.Login();
        }
        else
        {
        	// Otherwise display the correct error based on what is returned
            if (returnText == "userError")
            {
                errorText = "Username is already in use. Please choose another.";
                errorMoveIn.StartDelay();
            } else
            {
                if (returnText == "hasAccount")
                {
                    errorText = "You already have an account!";
                    errorMoveIn.StartDelay();
                }
                else
                {
                    errorText = "Server Error. Try again later.";
                    errorMoveIn.StartDelay();
                }
            }
        }
    }
    #endregion

    #region other methods
    public void Reset()
	{
		//Called to reset this script when a user logs out
		
		//remove anything from the ui input fields
        login_PasswordField.text = "";
        login_UsernameField.text = "";
        register_ConfirmPasswordField.text = "";
        register_EmailField.text = "";
        register_PasswordField.text = "";
		register_UsernameField.text = "";
		
		//Change the menu part to show the login ui
		part = MenuPart.LoggedIn;
		
		//Make sure all ui is off the screen apart from the login ui which should be moving in
        errorMoveOut.MoveBack();
        registerMoveOut.MoveBack();
        timeMoveOut.MoveBack();
        loginMoveOut.MoveForward();
    }
    void LoadTimeSinceLastLogin(string data)
	{
		//Called after 'Login' Command Sequence returned success. It gets the time since last login from the returned player data
		
		//split up the player data
        string[] splitString = data.Split(new string[1] { "/" }, System.StringSplitOptions.None);
		if (splitString.Length > 4) // << Make sure it is in a valid form
        {
			string hoursSinceLast = splitString[3]; // << Get the string for the hours since last login
			float hours = float.Parse(hoursSinceLast); // Get the number of hours from the string
			
			//This determines the format in which the time since last login should be displayed e.g. days (and hours) or hours (and minutes) or minutes (and seconds)
			//Then it displays the ui in this format
            if (hours > 24)
            {
                //show in days
                int numOfDays = (int)Mathf.Floor(hours / 24);
                int numOfHours = (int)Mathf.Round(((hours / 24) - numOfDays) * 24);
                string timeText = "";
                if (numOfDays == 1)
                {
                    timeText = numOfDays + " day";
                }
                else
                {
                    timeText = numOfDays + " days";
                }
                if (numOfHours != 0)
                {
                    if (numOfHours == 1)
                    {
                        timeText = timeText + "\nand " + numOfHours + " hour";
                    }
                    else
                    {
                        timeText = timeText + "\nand " + numOfHours + " hours";
                    }
                }
                timeSinceLastLoginText.text = timeText;
            }
            else
            {
                int numOfHours = (int)Mathf.Floor(hours);
                int numOfMinutes = (int)Mathf.Round((hours - numOfHours) * 60);
                if (numOfHours < 1)
                {
                    //show in minutes
                    int numberOfMinutes = (int)Mathf.Floor(hours * 60);
                    int numOfSeconds = (int)Mathf.Round(((hours * 60) - numberOfMinutes) * 60);
                    string timeText = "";
                    if (numberOfMinutes == 1)
                    {
                        timeText = numberOfMinutes + " minute";
                    }
                    else
                    {
                        timeText = numberOfMinutes + " minutes";
                    }
                    if (numOfSeconds != 0)
                    {
                        if (numOfSeconds == 1)
                        {
                            timeText = timeText + "\nand " + numOfSeconds + " second";
                        }
                        else
                        {
                            timeText = timeText + "\nand " + numOfSeconds + " seconds";
                        }
                    }
                    timeSinceLastLoginText.text = timeText;
                }
                else
                {
                    //show in hours
                    string timeText = "";
                    if (numOfHours == 1)
                    {
                        timeText = numOfHours + " hour";
                    }
                    else
                    {
                        timeText = numOfHours + " hours";
                    }
                    if (numOfMinutes != 0)
                    {
                        if (numOfMinutes == 1)
                        {
                            timeText = timeText + "\nand " + numOfMinutes + " minute";
                        }
                        else
                        {
                            timeText = timeText + "\nand " + numOfMinutes + " minutes";
                        }
                    }
                    timeSinceLastLoginText.text = timeText;
                }
            }
        }
        else
		{
        	// If the player data is not in the correct format it displays 'Unavailable'
            timeSinceLastLoginText.text = "Unavailable";
        }
    }
    void LoadPlayerData (string playerData)
	{
		//This is called to load the buildings and player data from the returned string
		
        /*
        The Player data string is in the following format (as returned by the 'Login' Command Sequence)
        Success/Money/level/hoursSinceLastRequest/canClaimDailyReward/building1Data/building2Data/.../...

        where canClaimDailyReward is 'yes' or 'no'

        And building data string is in format:
        buildingType\buildingLevel\buildingPosition(\hoursSinceLastCollect if factory)

        where buildingPosition is in the format:
        posX*posY*posZ
        */
		
		//Resets the necessary scripts
        buildingsManager.Reset();
		moneyManager.Reset();
		
		//Split up the data string
        string[] splitString = playerData.Split(new string[1] { "/" }, System.StringSplitOptions.None);
		if (splitString.Length > 4) // << Make sure it is in the correct format
		{
			//Load and set the player level and money
            float money = float.Parse(splitString[1]);
            int playerMoney = (int)Mathf.Floor(money);
            int playerLevel = int.Parse(splitString[2]);
            moneyManager.SetMoney(playerMoney);
			moneyManager.SetLevel(playerLevel);
			
			//Load and set whether the player can claim the daily reward or not
            bool canClaimReward;
            if (splitString[4] == "yes")
            {
                canClaimReward = true;
            } else
            {
                canClaimReward = false;
            }
			uiManager.SetDailyRewardUI(canClaimReward);
			
            //create the buildings from the player's data string
            for (int i = 5; i < splitString.Length; i++)
            {
                string buildingData = splitString[i];

                string[] buildingDataSplit = buildingData.Split(new string[1] { @"\" }, System.StringSplitOptions.None);
                if ((buildingDataSplit.Length == 3) || (buildingDataSplit.Length == 4))
                {
                	//Get all building type and level information
                    int buildingType = int.Parse(buildingDataSplit[0]);
                    int buildinglevel = int.Parse(buildingDataSplit[1]);
                    string buildingPosition = buildingDataSplit[2];
                    float hoursSinceLastCollect = -1;
                    if (buildingType == 1)
                    {
                    	//If it is a factory get the time since it was last empty so we can work out how full it should be
                        hoursSinceLastCollect = float.Parse(buildingDataSplit[3]);
                    }
	                
	                //Tell the buildingsManager to create the building providing all the necessary information
                    buildingsManager.CreateBuilding(buildingType, buildinglevel, buildingPosition, hoursSinceLastCollect);
                }
            }
			//Tell the buildingsManager to recount the buildings to update the ui and check for level up
            buildingsManager.UpdateBuildingsCount();
        } else
		{
        	//If the player data was not in the correct format go back to the login ui, showing an error message first
            part = MenuPart.Login;
            errorText = "Server Error. Try again later.";
            errorMoveIn.StartDelay();
        }

    }
    void Login ()
	{
		//Resets all of the necessary scripts to log the player in
        buildManager.Reset();
        upgradeManager.Reset();
        sellManager.Reset();
        moveManager.Reset();
        transManager.Login();
    }
    #endregion


    //This method is used as a client-side way to check the email address (format) is valid. Note: The validity of email address itself cannot be checked without trying to connect to the emailing service, it would be a good idea to add this kind of system in a real game (although we will probably add email functionality to DCP in the future)
    //We got the code from here: http://www.codeproject.com/Articles/22777/Email-Address-Validation-Using-Regular-Expression
    public static class TestEmail
    {
        public const string MatchEmailPattern =
            @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
            + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
              + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
            + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

        public static bool IsEmail(string email)
        {
            if (email != null) return Regex.IsMatch(email, MatchEmailPattern);
            else return false;
        }
    }
}
