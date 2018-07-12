using UnityEngine;
using System.Collections;
using DatabaseControl;

public class DCP_TPD_SaveLoadManager : MonoBehaviour {

    //This script saves and loads the player data to and from a string.
    //It also runs the Command Sequence to save the player data
    //A command sequence is not needed to load the player data as the data can be remembered from the last time is was saved (or from when the player logged in)

    public GameObject[] cubes; // << The list of cubes. Used to get and set the positions and if they are active or not
    public DCP_TPD_PlayerCubes playerCubesScript;
    public Transform player; // << The player object. Gets and sets the postion and rotation
    public UnityEngine.UI.Text playerNameText; //The UI Text in the in-game menu showing the player name. It is set when the game is loaded
    public DCP_UIMove saveDialogueOut; //The 'saving...' text which moves down the screen when the 'Save Game' button is pressed
    public DCP_UIMove1DImage blackFade; //The black image which partly fades in when 'Save Game' 'Load Game' and 'Logout' buttons are pressed
    public DCP_TPD_TransitionControl transScript;
    public DCP_TPD_PlayerInventory invScript;

    string username;
    string password;
    string playerData;

    Vector3 playerPos;
    Vector3 playerRot;

    string databaseName = "";
    bool canRunSequences = false;

    //This method is called by the 'CallAwakeOnDisabledObjects' gameObject at the very start of the game
    //We could not use Awake as this object is disabled at the Start of the game so Awake would not be run
    public void AwakeMethod ()
    {
        //Remembers the orgional player position and rotation so it can be reset if required
        playerPos = player.transform.position;
        playerRot = player.transform.eulerAngles;

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

    //Called by other scripts to reset all the player object and the cubes based on the playerData string
    public void ResetPlayerStuff (string u, string p, string d)
    {
        username = u;
        password = p;
        playerData = d;
        playerNameText.text = username;
        if (playerData == "")
        {
            //The data is blank so the player and cubes should be at the default positions
            playerCubesScript.ResetCubes();
            ResetPlayer();
        } else
        {
            //Load the player and cube positions from the data string
            DataStringToPositions(playerData);
        }
    }

    //This method is called when the in-game menu's 'Save Game' button is pressed
    public void SaveButtonPressed ()
	{
		if (canRunSequences == true) {
    		playerData = PositionsToDataString(); //Writes the player data to a string
        	StartCoroutine(SaveData()); //Runs a coroutine to run the 'Save Data' Command Sequence
			transScript.StorePlayerData(username, password, playerData); //Stores the player data so it returns to this point if the game is reloaded
		}
    }

    //This method is called when the in-game menu's 'Load Game' button is pressed
    public void LoadButtonPressed()
    {
        //The load button also causes the black bars transition to cover the screen

        //This tells the transition script, the reason for the transition is because the player data is being reloaded and not a login/logout 
        transScript.isReloading = true;
    }

    //Run the command sequence to save player data
    IEnumerator SaveData()
    {
        //Run the command sequence called 'Save Data' on the database name which has been retrieved in the start method. Sends the username, password and data string (from the ui input fields) as parameters
        IEnumerator e = DCP.RunCS(databaseName, "Save Data", new string[3] { username, password, playerData });

        //wait for the command sequence to finish loading
        while (e.MoveNext())
        {
            yield return e.Current;
        }

        //With this sequence we can just ignore the response string and assume the data has been saved
        saveDialogueOut.MoveForward(); // << The 'Saving...' text moves off the screen
        blackFade.MoveBack(); // << The black backgound fades back out
    }

    /*
    The player Data string has the following setup:
    PlayerPositionX\\PlayerPositionY\\PlayerPositionZ\\PlayerRotationZ\\PlayerRotationY\\PlayerRotationZ\\RedCubeStuff\\BlueCubeStuff\\GreenCubeStuff\\YellowCubeStuff\\PinkCubeStuff

    Where each of the cubes has data setup like this:

    if the cube is in the inventory:
    1

    otherwise:
    CubePositionX\CubePositionY\CubePositionZ
    */

    //This method sets the positions of the player and the cubes based on the data string
    void DataStringToPositions (string dataString)
    {
        //Reset the player inventory to empty
        invScript.items = new System.Collections.Generic.List<DCP_TPD_PlayerInventory.Cube>();

        //Split the data string by '\\'
        string[] splitData = dataString.Split(new string[1] { @"\\" }, System.StringSplitOptions.None);
        if (splitData.Length == 11) // The length of this should always be 11, as 6 for player data, and 5 for cube data, 6+5=11
        {
            //Set the player position and rotation based on the first 6 parts of the array
            player.transform.position = new Vector3(float.Parse(splitData[0]), float.Parse(splitData[1]), float.Parse(splitData[2]));
            player.transform.eulerAngles = new Vector3(float.Parse(splitData[3]), float.Parse(splitData[4]), float.Parse(splitData[5]));

            for (int i = 0; i < cubes.Length; i++)
            {
                //Get the data string for each individual cube and split it by '\'
                string cubeData = splitData[6 + i];
                string[] splitCubeData = cubeData.Split(new string[1] { @"\" }, System.StringSplitOptions.None);
                if (splitCubeData.Length == 3) //If it splits into 3 for PosX, PosY, PosZ
                {
                    //The cube is not in the inventory so set it to active and set the position
                    cubes[i].gameObject.SetActive(true);
                    cubes[i].transform.position = new Vector3(float.Parse(splitCubeData[0]), float.Parse(splitCubeData[1]), float.Parse(splitCubeData[2]));
                } else
                {
                    //If it doesn't split into three, it must be in the player's inventory
                    cubes[i].gameObject.SetActive(false); // << Disable the cube
                    if (i == 0)
                    {
                        invScript.AddItem(DCP_TPD_PlayerInventory.Cube.Red); // << Add the cube to the inventory
                    }
                    if (i == 1)
                    {
                        invScript.AddItem(DCP_TPD_PlayerInventory.Cube.Blue);
                    }
                    if (i == 2)
                    {
                        invScript.AddItem(DCP_TPD_PlayerInventory.Cube.Green);
                    }
                    if (i == 3)
                    {
                        invScript.AddItem(DCP_TPD_PlayerInventory.Cube.Yellow);
                    }
                    if (i == 4)
                    {
                        invScript.AddItem(DCP_TPD_PlayerInventory.Cube.Pink);
                    }
                }
            }
        }
        else
        {
            //If the length of the split data array was not 11. The data was not correct so assume it is the default and reset the player and cube positions to the default positions
            playerCubesScript.ResetCubes();
            ResetPlayer();
        }
    }

    //This method creates the data string based on the positions of the player and the cubes
    string PositionsToDataString ()
    {
        string returnString = "";

        //Combine the player position and rotation data into the string
        returnString = player.gameObject.transform.position.x + @"\\" + player.gameObject.transform.position.y + @"\\" + player.gameObject.transform.position.z + @"\\" + player.gameObject.transform.eulerAngles.x + @"\\" + player.gameObject.transform.eulerAngles.y + @"\\" + player.gameObject.transform.eulerAngles.z;

        foreach (GameObject cube in cubes)
        {
            string cubeString;
            if (cube.activeSelf == false)
            {
                //If the cube is in the player inventory, the cubes data is '1'
                cubeString = "1";
            } else
            {
                //If the cube is not in the player inventory, the cube data is its position
                cubeString = cube.transform.position.x + @"\" + cube.transform.position.y + @"\" + cube.transform.position.z;
            }
            //add the data of each cube to the main string
            returnString = returnString + @"\\" + cubeString;
        }
        return returnString;
    }

    public void ResetPlayer ()
    {
        //This method resets the player position and rotation to the default which was saved in the 'AwakeMethod' method
        player.transform.position = playerPos;
        player.transform.eulerAngles = playerRot;
    }
}
