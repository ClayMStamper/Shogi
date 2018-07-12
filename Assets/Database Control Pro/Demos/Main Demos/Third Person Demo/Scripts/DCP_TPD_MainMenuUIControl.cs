using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DatabaseControl;

public class DCP_TPD_MainMenuUIControl : MonoBehaviour {

    //This script controls the login/register system and runs the necessary Command Sequences

    #region public variables
    public GameObject loginStuff;
    public GameObject registerStuff;
    public GameObject loginErrorStuff;
    public GameObject registerErrorStuff;
    public GameObject loadingStuff;

    //Login Stuff
    public InputField login_usernameField;
    public InputField login_passwordField;

    //Register Stuff
    public InputField register_usernameField;
    public InputField register_passwordField;
    public InputField register_confirmField;

    //Error Stuff
    public Text loginErrorText;
    public Text registerErrorText;

    public DCP_TPD_TransitionControl transControl;
    #endregion

    #region other variables
    string databaseName = "";
    bool canRunSequences = false;
    string seqUsername = "";
    string seqPassword = "";
    #endregion

    void Start()
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

    void BlankFields()
    {
        //All of the username and password input fields are blanked
        login_passwordField.text = "";
        login_usernameField.text = "";
        register_usernameField.text = "";
        register_passwordField.text = "";
        register_confirmField.text = "";
    }

    public void Reset ()
    {
        //Resets all of the login UI when a player logs out, blanks all the field and shows the 'Login' UI elements, disabling the 'Register', error and 'Loading' UI elements
        BlankFields();
        loginStuff.gameObject.SetActive(true);
        registerStuff.gameObject.SetActive(false);
        registerErrorStuff.gameObject.SetActive(false);
        loginErrorStuff.gameObject.SetActive(false);
        loadingStuff.gameObject.SetActive(false);
    }

    #region Button Pressed Methods
    //The Register button was pressed on the Login UI elements
    public void Login_RegisterButtonPressed ()
    {
        //Disables the login ui elements and shows the register UI elements
        loginStuff.gameObject.SetActive(false);
        registerStuff.gameObject.SetActive(true);
        BlankFields();
    }

    //The Back button was pressed on the Register UI elements
    public void Register_BackButtonPressed ()
    {
        //Disables the register ui elements and shows the login UI elements
        loginStuff.gameObject.SetActive(true);
        registerStuff.gameObject.SetActive(false);
        BlankFields();
    }

    //The 'OK' button was pressed on the login UI elements
    public void LoginError_OkPressed ()
    {
        //Go back to the login UI elements and disable the error UI elements
        loginErrorStuff.gameObject.SetActive(false);
        loginStuff.gameObject.SetActive(true);
    }

    //The 'OK' button was pressed on the register UI elements
    public void RegisterError_OkPressed ()
    {
        //Go back to the register UI elements and disable the error UI elements
        registerErrorStuff.gameObject.SetActive(false);
        registerStuff.gameObject.SetActive(true);
    }

    //The proper 'Login' button was pressed to login a player
    public void Login_LoginButtonPressed ()
    {
        //Gets the username and passwords from the input fields
        string username = login_usernameField.text;
        string password = login_passwordField.text;
        if (username.Length < 4)
        {
            //Checks the username length and shows an login error if it is too short
            loginErrorText.text = "That is not your username. It is too short.";
            loginErrorStuff.gameObject.SetActive(true);
            loginStuff.gameObject.SetActive(false);
        } else
        {
            if (password.Length < 6)
            {
                //Checks the password length and shows an login error if it is too short
                loginErrorText.text = "That is not your password. It is too short.";
                loginErrorStuff.gameObject.SetActive(true);
                loginStuff.gameObject.SetActive(false);
            } else
            {
                if (canRunSequences == true) // << Has the demo been setup correctly in the Setup window
                {
                    //Disable all of the login UI elements and show the 'Loading...' text
                    loadingStuff.gameObject.SetActive(true);
                    loginStuff.gameObject.SetActive(false);
                    seqUsername = username;
                    seqPassword = password;
                    StartCoroutine(Login()); //Start the Coroutine to run the 'Login' Command Sequence
                }
            }
        }
    }

    //The proper 'Register' button was pressed to login a player
    public void Register_RegisterButtonPressed ()
    {
        //Gets the username and passwords from the input fields
        string username = register_usernameField.text;
        string password = register_passwordField.text;
        string confirm = register_confirmField.text;
        if (confirm != password)
        {
            //Shows a register error if the password and confirm password fields do not match
            registerErrorText.text = "Passwords do not match";
            registerStuff.gameObject.SetActive(false);
            registerErrorStuff.gameObject.SetActive(true);
        } else
        {
            if (username.Length < 4)
            {
                //Checks the username length and shows an register error if it is too short
                registerErrorText.text = "Username too short";
                registerStuff.gameObject.SetActive(false);
                registerErrorStuff.gameObject.SetActive(true);
            } else
            {
                if (password.Length < 6)
                {
                    //Checks the password length and shows an register error if it is too short
                    registerErrorText.text = "Password too short";
                    registerStuff.gameObject.SetActive(false);
                    registerErrorStuff.gameObject.SetActive(true);
                } else
                {
                    if (canRunSequences == true) // << Has the demo been setup correctly in the Setup window
                    {
                        //Disable all of the register UI elements and show the 'Loading...' text
                        loadingStuff.gameObject.SetActive(true);
                        registerStuff.gameObject.SetActive(false);
                        seqUsername = username;
                        seqPassword = password;
                        StartCoroutine(Register()); //Start the Coroutine to run the 'Register' Command Sequence
                    }
                }
            }
        }
    }
    #endregion

    #region Run Command Sequences
    //Run the command sequence to login a user
    IEnumerator Login()
    {
        //Run the command sequence called 'Login' on the database name which has been retrieved in the start method. Sends the username and password (from the ui input fields) as parameters
        IEnumerator e = DCP.RunCS(databaseName, "Login", new string[2] { seqUsername, seqPassword });

        //wait for the command sequence to finish loading
        while (e.MoveNext())
        {
            yield return e.Current;
        }

        //get the command sequence result
        string returnText = e.Current as string;

        //If the username and password were correct, then Success/PlayerData will be returned, where 'PlayerData' could be a variety of different strings
        //This means we have to split the string based on '/' to check it starts with 'Success'
        string[] splitData = returnText.Split(new string[1] { "/" }, System.StringSplitOptions.None);
        string mainData = splitData[0];

        if (mainData == "Success")
        {
            //Login was successful
            transControl.StorePlayerData(seqUsername, seqPassword, splitData[1]); //The data is only stored and not loaded here as the player can still see the main menu. The transition of the black stripes moveing in triggers the loading of the data when it finishes and the screen is black.
            transControl.StartTransition(); // Start the transition for the black stripes to cover the screen
        }
        else
        {
            if (mainData == "UserError")
            {
                //The username does not exist, show a login error
                loginErrorText.text = "The username does not exist";
                loginErrorStuff.gameObject.SetActive(true);
                loadingStuff.gameObject.SetActive(false);
            }
            else
            {
                if (mainData == "PassError")
                {
                    //The username was found but the password was incorrect, show a login error
                    loginErrorText.text = "The password was incorrect";
                    loginErrorStuff.gameObject.SetActive(true);
                    loadingStuff.gameObject.SetActive(false);
                }
                else
                {
                    //Some other error which is unlikely to happen but should be considered if it does
                    loginErrorText.text = "There was a problem with the server. Please try again later.";
                    loginErrorStuff.gameObject.SetActive(true);
                    loadingStuff.gameObject.SetActive(false);
                }
            }
        }
    }

    //Run the command sequence to register a user
    IEnumerator Register()
    {
        //Run the command sequence called 'Register' on the database name which has been retrieved in the start method. Sends the username and password (from the ui input fields) as parameters
        IEnumerator e = DCP.RunCS(databaseName, "Register", new string[2] { seqUsername, seqPassword });

        //wait for the command sequence to finish loading
        while (e.MoveNext())
        {
            yield return e.Current;
        }

        //get the command sequence result
        string returnText = e.Current as string;

        //This Command Sequence doesn't return the player data as the player will be registerd for the first time so we know the data will be blank

        if (returnText == "Success")
        {
            //Registration was successful
            transControl.StorePlayerData(seqUsername, seqPassword, ""); //The data (which we know is blank) is only stored and not loaded here as the player can still see the main menu. The transition of the black stripes moveing in triggers the loading of the data when it finishes and the screen is black.
            transControl.StartTransition(); // Start the transition for the black stripes to cover the screen
        }
        else
        {
            if (returnText == "username in use")
            {
                //The username is already in use, show register error
                registerErrorText.text = "The username is in use. Please choose another.";
                registerErrorStuff.gameObject.SetActive(true);
                registerStuff.gameObject.SetActive(false);
            }
            else
            {
                //Some other error which is unlikely to happen but should be considered if it does
                registerErrorText.text = "Problem with server. Try again later.";
                registerErrorStuff.gameObject.SetActive(true);
                registerStuff.gameObject.SetActive(false);
            }
        }
    }
    #endregion
}
