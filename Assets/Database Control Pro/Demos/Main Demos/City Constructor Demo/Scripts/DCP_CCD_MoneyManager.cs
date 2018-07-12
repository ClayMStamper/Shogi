using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DCP_CCD_MoneyManager : MonoBehaviour {
	
	//This script keeps track of all of the player's money and level information
	
	//The ui texts to dislpay the player level, money and the number of people required for the next level up
    public Text levelNumText;
    public Text moneyText;
    public Text peopleRequired;
	
	//References to the other managers which are required
    public DCP_CCD_BuildManager buildManager;
    public DCP_CCD_PlayerBuildingsManager buildingsManager;
    public DCP_CCD_DCPManager dcpManager;
	
	//Variables to store the player money, level, etc
    int money = 400;
    int curNumOfPeople = 0;
    int numOfPeopleRequired = 1;
    int curLevel = 1;
	
	//Is the level up command sequence being run
    bool isLoading = false;
	
	//Is the collect money Command Sequence being run
    bool isLoadingCollect = false;
	DCP_CCD_BuildingInstance collectBuilding; // << The building which the money should be collected from

    void Update()
	{
		//Calculate the number of people required for a level up
        float numOfPeople = 0.5f * curLevel * (curLevel + 1.0f); // << This means the number of people required for each level follow the triangle numbers 1,3,6,10,15,21 ... etc
		numOfPeopleRequired = (int)Mathf.Round(numOfPeople);
		
		//Calculate the maximum amount of money which can be stored
        int maxMoney = 500;
        foreach (DCP_CCD_BuildingInstance b in buildingsManager.buildings)
        {
        	//Iterate through the player buildings and check for banks
            if (b.buildingTypeIndex == 2)
            {
                //the building is a bank so increase the maximum amount of money which can be stored
                maxMoney = maxMoney + ((b.levelNumber ^ 3) * 500);
            }
        }
		
		//Limit the player's money to the maximum money amount
        if (money > maxMoney)
        {
            money = maxMoney;
        }
		
		//Calculate the percentage of the maximum money which the player has and display it
        float p = ((float)money / (float)maxMoney) * 100f;
        int percentageFull = (int)Mathf.Round(p);
		moneyText.text = "$" + money + "  " + percentageFull + "%";
		
		//Display the current player level
		levelNumText.text = "Level " + curLevel;
		
		//Display the number of people required for the next level up
        if ((numOfPeopleRequired - curNumOfPeople) == 1)
        {
            peopleRequired.text = "1 person required";
        }
        else
        {
            peopleRequired.text = (numOfPeopleRequired - curNumOfPeople) + " people required";
        }
		
		//If there are enough people for a level up, then level up
        if ((numOfPeopleRequired - curNumOfPeople) < 1)
        {
            LevelUp();
        }
    }

    void LevelUp ()
    {
	    if (isLoading == false) // << If a level up Command Sequence is not already being run
        {
		    isLoading = true;
		    
		    //Tell the dcpManager to run the 'Level Up' Command Sequence
            dcpManager.Run(new DCP_CCD_DCPManager.RunSequence(DCP_CCD_DCPManager.RunSequence.sequenceType.LevelUp, 0, ""));
        }
    }

    public void FinishLevelUp ()
	{
		//This is called by the dcpManager when the 'Level Up' Command Sequence returns 'Success'
		
		//Increase the player level
		curLevel++;
		
		//Recalculate the number of people required for the next level up
        float numOfPeople = 0.5f * curLevel * (curLevel + 1.0f); // << This means the number of people required for each level follow the triangle numbers 1,3,6,10,15,21 ... etc
		numOfPeopleRequired = (int)Mathf.Round(numOfPeople);
		
		//Tell the buildingsManager a level up has taken place so the particles can be instantiated at each building's position
		buildingsManager.LevelUp();
		
        isLoading = false;
    }

    public void CancelLevelUp ()
	{
		//This is called by the dcpManager when the 'Level Up' Command Sequence doesn't return 'Success'
        isLoading = false;
    }

    public void Reset ()
	{
		//This resets the variables when the player logs in or registers a new account
	    money = 400;
        curNumOfPeople = 0;
        numOfPeopleRequired = 1;
        curLevel = 1;
    }

    public bool isEnoughMoney (int moneyRequired)
	{
		//This is called by the buildManager or UpgradeManager to check there is enough money for a build/upgrade
        if (money < moneyRequired)
        {
            return false;
        } else
        {
            return true;
        }
    }

    public void RemoveMoney (int amount)
	{
		//Removes the money after build/upgrade
        money = money - amount;
    }

    public bool isLevelEnough (int requiredLevel)
	{
		//This is called by the buildManager of UpgradeManager to check the player level is high enough
        if (curLevel < requiredLevel)
        {
            return false;
        } else
        {
            return true;
        }
    }

    public int GetLevel ()
	{
		//returns the current player level
        return curLevel;
    }

    public void SellBuilding (DCP_CCD_BuildingInstance building)
	{
		//Called by the SellManager to sell a building
		
		int sellPrice = 0;
		
		//Gets the initial cost of the builing
		int initialCost = buildManager.buildings[building.buildingTypeIndex].cost;
		
		//Works out the total cost by adding together all of the building's upgrade costs
        int totalCost = initialCost;
        for (var i = 1; i < building.levelNumber; i++)
        {
            totalCost = totalCost + (initialCost + ((i * i) * (initialCost / 2)));
        }
		
		//Works out the sell price by halving the total cost
		sellPrice = (int)Mathf.Floor(totalCost * 0.5f);
		
		//Increase the money
        money = money + sellPrice;
    }

    public void UpdateNumberOfPeople (int people)
	{
		//Called by the buildingsManager to update the number of people in the city
        curNumOfPeople = people;
        if ((numOfPeopleRequired - curNumOfPeople) < 1)
        {
            LevelUp();
        }
    }

    public void CollectMoney (DCP_CCD_BuildingInstance building)
	{
		//Called by the buildingsManager when a factories 'Collect' button has been pressed
		
		if (isLoadingCollect == false) {
	        if (building.doesGenerateMoney)
	        {
	            isLoadingCollect = true;
		        collectBuilding = building;
		        
		        //Tells the dcpManager to run the Command Sequence to collect the money from the building
	            dcpManager.Run(new DCP_CCD_DCPManager.RunSequence(DCP_CCD_DCPManager.RunSequence.sequenceType.Collect, buildingsManager.buildings.IndexOf(collectBuilding), ""));
	        }
		}
    }

    public void FinishCollectMoney ()
	{
		//Called by the dcpManager when the 'Collect' Command Sequence has finished running and was a 'Success'
        collectBuilding.currentMoney = 0;
        isLoadingCollect = false;
    }

    public void CancelCollectMoney ()
	{
		//Called by the dcpManager when the 'Collect' Command Sequence has finished running and wasn't a 'Success'
        isLoadingCollect = false;
    }

    public void SetMoney (int moneyValue)
	{
		//Sets the player's money when logging in
        money = moneyValue;
    }
    
    public void SetLevel (int lvl)
	{
		//Sets the player's level when logging in
        curLevel = lvl;
    }
}
