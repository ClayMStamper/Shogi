using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DatabaseControl; // << Include the DatabaseControl namespace

public class LoginDemo_Accounts : MonoBehaviour {

    //All of the following variables are for the UI and are assigned in the Inspector

    public InputField Login_UsernameField;
    public InputField Login_PasswordField;
    public Text Login_Error;

    public InputField Register_UsernameField;
    public InputField Register_PasswordField;
    public InputField Register_ConfirmPasswordField;
    public Text Register_Error;

    public InputField Data_SaveDataField;
    public Text Data_LoadedData;
    public Text Data_LoggedInAs;

    public DCP_UIMoveGroup loginStuff;
    public DCP_UIMoveGroup registerStuff;
    public DCP_UIMoveGroup loadingStuff;
    public DCP_UIMoveGroup dataStuff;

    public DCP_UIDelay fadeLoadingIn;

    string databaseName = "";
    bool canRunSequences = false;

    string username = "";
    string password = "";

    string loginError = "";
    string registerError = "";

    void Start ()
    {
        //Gets the databaseName as it was setup through the editor
        GameObject linkObj = GameObject.Find("Link");
        if (linkObj == null)
        {
            Debug.LogError("DCP Error: Cannot find the link object in the scene so scripts running Command Sequences don't know the database name");
        } else
        {
            DCP_Demos_LinkDatabaseName linkScript = linkObj.gameObject.GetComponent<DCP_Demos_LinkDatabaseName>() as DCP_Demos_LinkDatabaseName;
            if (linkScript == null)
            {
                Debug.LogError("DCP Error: Cannot find the link script on link object so scripts running Command Sequences don't know the database name");
            } else
            {
                if (linkScript.databaseName == "")
                {
                    Debug.LogError("DCP Error: This demo scene has not been setup. Please setup the demo scene in the Setup window before use. Widnow>Database Control Pro>Setup Window");
                } else
                {
                    databaseName = linkScript.databaseName;
                    canRunSequences = true;
                }
            }
        }
    }
    void Update ()
    {
        //Writes the login and register errors to the ui texts
        Login_Error.text = loginError;
        Register_Error.text = registerError;
        Data_LoggedInAs.text = "Logged in as: " + username;
    }

    #region button pressed methods
    //Called when login button is pressed
    public void LoginStuff_LoginButtonPressed ()
    {
        if (Login_UsernameField.text.Length < 6)
        {
            //show login error as the username is too short
            loginError = "Username too short";
            return;
        }
        if (Login_PasswordField.text.Length < 4)
        {
            //show login error as the password is too short
            loginError = "Password too short";
            return;
        }
        if (canRunSequences == true) // << Makes sure the demo scene has been setup correctly so this script knows the database name
        {
            loginStuff.MoveBack();
            fadeLoadingIn.StartDelay();
            //Remember the username and password and clear the ui fields
            username = Login_UsernameField.text;
            password = Login_PasswordField.text;
            Login_UsernameField.text = "";
            Login_PasswordField.text = "";
            StartCoroutine(Login()); // << Runs the IEnumerator which runs the command sequence
        }
    }

    //Called when the register button is pressed
    public void RegisterStuff_RegisterButtonPressed ()
    {
        if (Register_UsernameField.text.Length < 6)
        {
            //show register error as the username is too short
            registerError = "Username too short";
            return;
        }
        if (Register_PasswordField.text.Length < 4)
        {
            //show register error as the password is too short
            registerError = "Password too short";
        }
        if (Register_PasswordField.text != Register_ConfirmPasswordField.text)
        {
            //show register error as the passwords dont match
            registerError = "Passwords don't match";
        }
        if (canRunSequences == true) // << Makes sure the demo scene has been setup correctly so this script knows the database name
        {
            registerStuff.MoveBack();
            fadeLoadingIn.StartDelay();
            //Remember the username and password and clear the ui fields
            username = Register_UsernameField.text;
            password = Register_PasswordField.text;
            Register_UsernameField.text = "";
            Register_PasswordField.text = "";
            Register_ConfirmPasswordField.text = "";
            StartCoroutine(Register()); // << Runs the IEnumerator which runs the command sequence
        }
    }

    //Called when the set data button is pressed
    public void Data_SetDataButtonPressed ()
    {
        dataStuff.MoveBack();
        fadeLoadingIn.StartDelay();
        StartCoroutine(SetData());
    }

    //Called when the get data button is pressed
    public void Data_GetDataButtonPressed()
    {
        dataStuff.MoveBack();
        fadeLoadingIn.StartDelay();
        StartCoroutine(GetData());
    }
    #endregion

    #region Run Command Sequences
    //Run the command sequence to login a user
    IEnumerator Login ()
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

        if (returnText == "Success")
        {
            //Login was successful
            loginError = ""; // << make ui text for login error disappear
            loadingStuff.MoveBack(); // Fade out loading ui and fade in the data ui
            Data_SaveDataField.text = "";
            Data_LoadedData.text = "Data ...";
            yield return new WaitForSeconds(0.5f);
            dataStuff.MoveForward();
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
            loadingStuff.MoveBack(); // Fade out loading ui and fade in the login ui
            yield return new WaitForSeconds(0.5f);
            loginStuff.MoveForward();
        }
    }

    //Run the command sequence to register a user
    IEnumerator Register ()
    {
        //Run the command sequence called 'Register' on the database name which has been retrieved in the start method. Sends the username and password (from the ui input fields) as parameters
        IEnumerator e = DCP.RunCS(databaseName, "Register", new string[2] { username, password });

        //wait for the command sequence to finish loading
        while (e.MoveNext())
        {
            yield return e.Current;
        }

        //get the command sequence result
        string returnText = e.Current as string;

        if (returnText == "Success")
        {
            //Registration was successful
            registerError = ""; // << make ui text for register error disappear
            loadingStuff.MoveBack(); // Fade out loading ui and fade in the data ui
            Data_SaveDataField.text = "";
            yield return new WaitForSeconds(0.5f);
            dataStuff.MoveForward();
        } else
        {
            if (returnText == "username in use")
            {
                //The username is already in use
                registerError = "The username is in use. Please choose another.";
            } else
            {
                //Some other error which is unlikely to happen but should be considered if it does
                registerError = "Problem with server. Try again later.";
            }
            loadingStuff.MoveBack(); // Fade out loading ui and fade in the login ui
            yield return new WaitForSeconds(0.5f);
            registerStuff.MoveForward();
        }
    }

    //Run the command sequence to set user data
    IEnumerator SetData ()
    {
        //Run the command sequence called 'Set Data' on the database name which has been retrieved in the start method. Sends the username, password and data string (from the ui input fields) as parameters
        IEnumerator e = DCP.RunCS(databaseName, "Set Data", new string[3] { username, password, Data_SaveDataField.text });
        Data_SaveDataField.text = "";

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
            loadingStuff.MoveBack(); // Fade out loading ui and fade in the data ui
            yield return new WaitForSeconds(0.5f);
            dataStuff.MoveForward();
        }
        else
        {
            //Either error with server or error with user's username and password so logout
            loadingStuff.MoveBack(); // Fade out loading ui and fade in the login ui
            yield return new WaitForSeconds(0.5f);
            loginStuff.MoveForward();
        }
    }

    //Run the command sequence to get user data
    IEnumerator GetData()
    {
        //Run the command sequence called 'Get Data' on the database name which has been retrieved in the start method. Sends the username and password (from the ui input fields) as parameters
        IEnumerator e = DCP.RunCS(databaseName, "Get Data", new string[2] { username, password });

        //wait for the command sequence to finish loading
        while (e.MoveNext())
        {
            yield return e.Current;
        }

        //get the command sequence result
        string returnText = e.Current as string;

        //With this sequence we will assume there are no errors as the data string could be anything (even the word 'Error')
        Data_LoadedData.text = returnText; // << shows the retrieved data on ui text

        loadingStuff.MoveBack(); // Fade out loading ui and fade in the data ui
        yield return new WaitForSeconds(0.5f);
        dataStuff.MoveForward();
    }
    #endregion
}
