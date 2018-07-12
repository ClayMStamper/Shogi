using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DatabaseControl;

public class DCP_CCD_DCPManager : MonoBehaviour {
	
	//This script controls the running of all in-game Command Sequences

    public class RunSequence
	{
		//This class stores information about a Command Sequence which should be run
		
        public enum sequenceType
        {
            Build,
            Upgrade,
            Move,
            Sell,
            LevelUp,
            ClaimReward,
            Collect
        }

        public RunSequence (sequenceType t, int index, string pos)
        {
            type = t;
            buildingIndex = index;
            buildingPosition = pos;
        }

        public sequenceType type;
        public int buildingIndex;
        public string buildingPosition;
    }

	public Button[] buttons; // << All of the in-game ui buttons in the scene which should be disabled when a Command Sequence is being run
	
	//The list of the Command Sequences which need to be run
    public List<RunSequence> sequencesToRun = new List<RunSequence>();
	
	//References to all of the needed in-game managers
    public DCP_CCD_PlayerBuildingsManager buildingsManager;
    public DCP_CCD_MoneyManager moneyManager;
    public DCP_CCD_BuildManager buildManager;
    public DCP_CCD_UpgradeManager upgradeManager;
    public DCP_CCD_MoveManager moveManager;
    public DCP_CCD_SellManager sellManager;
    public DCP_CCD_UIManager uiManager;
	
	//The gameObject for the UI which shows 'Loaing...' at the top of the screen
    public GameObject loadingStuff;
	
	//Username and password remembered from logging in or registering
    string username;
    string password;

    //These are used to make sure the demo scene has been setup correctly and to store the databse name which is used to run Command Sequences
    string databaseName = "";
    bool canRunSequences = false;

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

    public void SetUsernameAndPass (string u, string p)
	{
		//This is called when logging in or registering a users so the username and password are remembered as they are checked by Command Sequences
        username = u;
        password = p;
        Reset();
    }
	
    public void Reset ()
	{
		//This is called to reset the script when a logout/login takes place
		
		//Creates a new blank array of Command Sequences to be run
		sequencesToRun = new List<RunSequence>();
		
		//Iterates through all of the in-game buttons and enables them
        foreach (Button b in buttons)
        {
            b.interactable = true;
        }
		//Tells all of the instantiated buildings that their ui buttons should work
        foreach (DCP_CCD_BuildingInstance b in buildingsManager.buildings)
        {
            b.SetUIInteractable(true);
        }
		
		//Disable the 'Loading...' UI Text as no Command Sequence is running
        loadingStuff.gameObject.SetActive(false);
    }
	
    public void Run (RunSequence sequenceToRun)
	{
		//This is called by other scripts to add a Command Sequence to the list of Command Sequences to be run and to run it
        sequencesToRun.Add(sequenceToRun);
        RunNextSequence();
    }

    void RunNextSequence()
    {
	    if (sequencesToRun.Count == 0) // << Makes sure there are Command Sequences to be run
        {
            Reset();
        }
        else
	    {
        	//Disable all of the in-game ui buttons
            foreach (Button b in buttons)
            {
                b.interactable = false;
            }
            foreach (DCP_CCD_BuildingInstance b in buildingsManager.buildings)
            {
                b.SetUIInteractable(false);
            }
	        
	        //Show 'Loading...' ui text
	        loadingStuff.gameObject.SetActive(true);
	        
	        //Get the next sequence which needs to be run and run the correct IEnumerator based on the sequence type, providing the required information in order to run the sequence
            RunSequence seq = sequencesToRun[0];
            if (seq.type == RunSequence.sequenceType.Build)
            {
                StartCoroutine(RunBuildSequence(seq.buildingIndex, seq.buildingPosition));
            }
            if (seq.type == RunSequence.sequenceType.Upgrade)
            {
                StartCoroutine(RunUpgradeSequence(seq.buildingIndex));
            }
            if (seq.type == RunSequence.sequenceType.Move)
            {
                StartCoroutine(RunMoveSequence(seq.buildingIndex, seq.buildingPosition));
            }
            if (seq.type == RunSequence.sequenceType.Sell)
            {
                StartCoroutine(RunSellSequence(seq.buildingIndex));
            }
            if (seq.type == RunSequence.sequenceType.LevelUp)
            {
                StartCoroutine(RunLevelUpSequence());
            }
            if (seq.type == RunSequence.sequenceType.ClaimReward)
            {
                StartCoroutine(RunClaimDailyRewardSequence());
            }
            if (seq.type == RunSequence.sequenceType.Collect)
            {
                StartCoroutine(RunCollectSequence(seq.buildingIndex));
            }
            sequencesToRun.RemoveAt(0);
        }
    }

    #region IEnumerators
    IEnumerator RunBuildSequence (int buildingType, string buildingPos)
    {
        if (canRunSequences == true)
        {
            //Run the command sequence called 'Build' on the database name which has been retrieved in the start method
	        IEnumerator e = DCP.RunCS(databaseName, "Build", new string[4] { username, password, "" + buildingType, buildingPos });

            //wait for the command sequence to finish loading
            while (e.MoveNext())
            {
                yield return e.Current;
            }

            //get the command sequence result
            string returnText = e.Current as string;
	        
	        //Split up the returned text as it will be in the form 'Success/PlayerMoney' where PlayerMoney is the amount of money the player now has
            string[] splitString = returnText.Split(new string[1] { "/" }, System.StringSplitOptions.None);
            bool isSuccess = false;
            if (splitString != null)
            {
                if (splitString.Length == 2)
                {
                    if (splitString[0] == "Success")
                    {
	                    buildManager.FinishBuild(); // << Tell the build manager that the building can be built
                        isSuccess = true;
                        float money = float.Parse(splitString[1]);
	                    moneyManager.SetMoney((int)Mathf.Floor(money)); // << Set the player's money so it is in-sync with the server
                    }
                }
            }
            if (isSuccess == false)
            {
            	//Tell the buildManager that the building should not be built
                buildManager.CancelBuild();
            }
        }
	    RunNextSequence(); // << Run the next sequence (if there is a next sequence which needs to be run)
    }
    IEnumerator RunUpgradeSequence (int buildingIndex)
    {
        if (canRunSequences == true)
        {
            //Run the command sequence called 'Upgrade' on the database name which has been retrieved in the start method
            IEnumerator e = DCP.RunCS(databaseName, "Upgrade", new string[3] { username, password, "" + buildingIndex});

            //wait for the command sequence to finish loading
            while (e.MoveNext())
            {
                yield return e.Current;
            }

            //get the command sequence result
            string returnText = e.Current as string;

	        //Split up the returned text as it will be in the form 'Success/PlayerMoney' where PlayerMoney is the amount of money the player now has
            string[] splitString = returnText.Split(new string[1] { "/" }, System.StringSplitOptions.None);
            bool isSuccess = false;
            if (splitString != null)
            {
                if (splitString.Length == 2)
                {
                    if (splitString[0] == "Success")
                    {
	                    upgradeManager.FinishUpgrade();// << Tell the upgrade manager that the upgrade can take place
                        isSuccess = true;
                        float money = float.Parse(splitString[1]);
                        moneyManager.SetMoney((int)Mathf.Floor(money)); // << Set the player's money so it is in-sync with the server
                    }
                }
            }
            if (isSuccess == false)
            {
	            upgradeManager.CancelUpgrade(); // Tell the upgrade manager that the upgrade cannot take place
            }
        }
        RunNextSequence(); // << Run the next sequence (if there is a next sequence which needs to be run)
    }
    IEnumerator RunMoveSequence (int buildingIndex, string buildingPos)
    {
        if (canRunSequences == true)
        {
            //Run the command sequence called 'Move' on the database name which has been retrieved in the start method.
            IEnumerator e = DCP.RunCS(databaseName, "Move", new string[4] { username, password, "" + buildingIndex, buildingPos });

            //wait for the command sequence to finish loading
            while (e.MoveNext())
            {
                yield return e.Current;
            }

            //get the command sequence result
            string returnText = e.Current as string;

            if (returnText == "Success")
            {
	            moveManager.FinishMove(); // Tell the moveManager that the building can be moved
            } else
            {
	            moveManager.CancelMove(); // Tell the moveManage that the building cannot be moved
            }
        }
        RunNextSequence(); // << Run the next sequence (if there is a next sequence which needs to be run)
    }
    IEnumerator RunSellSequence (int buildingIndex)
    {
        if (canRunSequences == true)
        {
            //Run the command sequence called 'Sell' on the database name which has been retrieved in the start method.
            IEnumerator e = DCP.RunCS(databaseName, "Sell", new string[3] { username, password, "" + buildingIndex });

            //wait for the command sequence to finish loading
            while (e.MoveNext())
            {
                yield return e.Current;
            }

            //get the command sequence result
            string returnText = e.Current as string;

            string[] splitString = returnText.Split(new string[1] { "/" }, System.StringSplitOptions.None);
            bool isSuccess = false;
            if (splitString != null)
            {
                if (splitString.Length == 2)
                {
                    if (splitString[0] == "Success")
                    {
                        isSuccess = true;
	                    sellManager.FinishSell(); // Tell the sell manager that the selling should take place and the building should be destroyed
                        float money = float.Parse(splitString[1]);
                        moneyManager.SetMoney((int)Mathf.Floor(money)); // << Set the player's money so it is in-sync with the server
                    }
                }
            }
            if (isSuccess == false)
            {
	            sellManager.CancelSell(); // Tell the sell manager that the selling shouldn't take place and the building should not be destroyed
            }
        }
        RunNextSequence(); // << Run the next sequence (if there is a next sequence which needs to be run)
    }
    IEnumerator RunLevelUpSequence ()
    {
        if (canRunSequences == true)
        {
            //Run the command sequence called 'Level Up' on the database name which has been retrieved in the start method
            IEnumerator e = DCP.RunCS(databaseName, "Level Up", new string[2] { username, password });

            //wait for the command sequence to finish loading
            while (e.MoveNext())
            {
                yield return e.Current;
            }

            //get the command sequence result
            string returnText = e.Current as string;

            if (returnText == "Success")
            {
	            moneyManager.FinishLevelUp(); // Tell the moneyManager that the level up was successful and can take place
            } else
            {
	            moneyManager.CancelLevelUp(); // Tell the moneyManager that the level up wasn't successful and cannot take place
            }
        }
        RunNextSequence(); // << Run the next sequence (if there is a next sequence which needs to be run)
    }
    IEnumerator RunClaimDailyRewardSequence ()
    {
        if (canRunSequences == true)
        {
            //Run the command sequence called 'Claim Daily Reward' on the database name which has been retrieved in the start method
            IEnumerator e = DCP.RunCS(databaseName, "Claim Daily Reward", new string[2] { username, password });

            //wait for the command sequence to finish loading
            while (e.MoveNext())
            {
                yield return e.Current;
            }

            //get the command sequence result
            string returnText = e.Current as string;

            string[] splitString = returnText.Split(new string[1] { "/" }, System.StringSplitOptions.None);
            if (splitString != null)
            {
                if (splitString.Length == 2)
                {
                    if (splitString[0] == "Success")
                    {
	                    uiManager.FinishClaimReward(); // Tell the uiManager that the claim reward was successful so the claim reward ui should be changed
                        float money = float.Parse(splitString[1]);
                        moneyManager.SetMoney((int)Mathf.Floor(money)); // << Set the player's money so it is in-sync with the server
                    }
                }
            }
        }
        RunNextSequence(); // << Run the next sequence (if there is a next sequence which needs to be run)
    }
    IEnumerator RunCollectSequence (int buildingIndex)
    {
        if (canRunSequences == true)
        {
            //Run the command sequence called 'Collect' on the database name which has been retrieved in the start method
            IEnumerator e = DCP.RunCS(databaseName, "Collect", new string[3] { username, password, "" + buildingIndex });

            //wait for the command sequence to finish loading
            while (e.MoveNext())
            {
                yield return e.Current;
            }

            //get the command sequence result
            string returnText = e.Current as string;

            Debug.Log("collect: " + returnText);


            string[] splitString = returnText.Split(new string[1] { "/" }, System.StringSplitOptions.None);
            bool isSuccess = false;
            if (splitString != null)
            {
                if (splitString.Length == 2)
                {
                    if (splitString[0] == "Success")
                    {
                        isSuccess = true;
	                    moneyManager.FinishCollectMoney(); // Tell the moneyManager that the collection was successful so the buildings money should be reset
                        float money = float.Parse(splitString[1]);
                        moneyManager.SetMoney((int)Mathf.Floor(money)); // << Set the player's money so it is in-sync with the server
                    }
                }
            }
            if (isSuccess == false)
            {
	            moneyManager.CancelCollectMoney(); // Tell the money manager that the collection was not successful
            }
        }
        RunNextSequence(); // << Run the next sequence (if there is a next sequence which needs to be run)
    }
    #endregion
}
