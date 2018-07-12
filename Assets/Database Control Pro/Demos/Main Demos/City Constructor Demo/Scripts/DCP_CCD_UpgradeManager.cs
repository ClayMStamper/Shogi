using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DCP_CCD_UpgradeManager : MonoBehaviour {
	
	//This script controls the upgrading of buildings when the in-game upgrade menu is selected
	
	//UI GameObjects to enable/disable in the menu
    public GameObject noBuildingSelectedText;
    public GameObject buildingSelectedUI;
    public GameObject cannotAffordText;
    public GameObject playerLevelTooLowText;
	public GameObject upgradeButton;
	
	//The UI texts for upgrade cost and building name which should be changed when a building is seletected
    public Text upgradeCostText;
	public Text selectedNameText;
	
	//The references to the required manager scripts
    public DCP_CCD_MoneyManager moneyManager;
    public DCP_CCD_BuildManager buildManager;
	public DCP_CCD_PlayerBuildingsManager buildingsManager;
	public DCP_CCD_DCPManager dcpManager;
	
	//The upgrade particles prefab to instatiate when an upgrade is successful
    public GameObject particles;
	
	//Whether a building is selected or not and a reference to the selected building
    bool buildingSelected = false;
    DCP_CCD_BuildingInstance selectedBulding;
	
	//Variable to remember the selectd building when the 'Upgrade' Command Sequence is being run
    DCP_CCD_BuildingInstance tempSelectedBuilding;
	
	//The calculated upgrade cost to display
    int upgradeCost;
	
	//Whether the 'Upgrade' Command Seuqence is running or not
    bool isLoading = false;

    void Update ()
    {
        //calculate cost of upgrade and required player level
        int requiredLevel = 1;
        if (buildingSelected == true)
        {
	        int intialCost = buildManager.buildings[selectedBulding.buildingTypeIndex].cost; // get the initial cost of the building from the buildingsManager script
            int buildingLevel = selectedBulding.levelNumber;
            upgradeCost = intialCost + ((buildingLevel * buildingLevel) * (intialCost / 2));
	        requiredLevel = (int)Mathf.Floor(selectedBulding.levelNumber / 2) + 1;
        }

        //set the ui text for the upgrade cost
	    upgradeCostText.text = "" + upgradeCost;
	    
	    if (buildingSelected == true) // If a building has been selected
	    {
		    //Display the building name and level as ui text
		    selectedNameText.text = buildManager.buildings[selectedBulding.buildingTypeIndex].name + " lvl." + selectedBulding.levelNumber;
		    
		    //Show the building selected ui
            noBuildingSelectedText.gameObject.SetActive(false);
		    buildingSelectedUI.gameObject.SetActive(true);
		    
		    //Show the different ui's for the different cases
            if (moneyManager.isEnoughMoney(upgradeCost))
            {
                cannotAffordText.gameObject.SetActive(false);
                if (moneyManager.isLevelEnough(requiredLevel))
                {
                	//The player can upgrade the building so enable the upgrade ui so the 'Upgrade' button can be pressed
                    upgradeButton.gameObject.SetActive(true);
                    playerLevelTooLowText.gameObject.SetActive(false);
                } else
                {
                	//The player's level is not high enough for the upgrade
                    upgradeButton.gameObject.SetActive(false);
                    playerLevelTooLowText.gameObject.SetActive(true);
                }
            } else
            {
            	//Player cannot afford the upgrade so show cannot afford text
                cannotAffordText.gameObject.SetActive(true);
                upgradeButton.gameObject.SetActive(false);
                playerLevelTooLowText.gameObject.SetActive(false);
            }
        } else
	    {
        	//show the ui for no building has been selected
            noBuildingSelectedText.gameObject.SetActive(true);
            buildingSelectedUI.gameObject.SetActive(false);
        }
    }

	public void Reset ()
	{
		//This is called to reset the script or cancel upgrading the selected building
		
		buildingSelected = false; // << No building is selected
        selectedBulding = null;
    }

    public void ClickedOnBuilding (DCP_CCD_BuildingInstance build)
	{
		//This is called by instantiated buildings individually when they are clicked on to select them
		
		if (isLoading == false) // If the 'Upgrade' sequence is not being run
		{
			//Select the building which was clicked on
            buildingSelected = true;
            selectedBulding = build;
        }
    }

    public void UpgradeButtonPressed ()
	{
		//This is called when the 'Upgrade' ui button is pressed
		
		//Calculates the required player level and upgrade cost
        int requiredLevel = 1;
        if (buildingSelected == true)
        {
            int intialCost = buildManager.buildings[selectedBulding.buildingTypeIndex].cost;
            int buildingLevel = selectedBulding.levelNumber;
            upgradeCost = intialCost + ((buildingLevel * buildingLevel) * (intialCost / 2));
            requiredLevel = (int)Mathf.Ceil(selectedBulding.levelNumber / 2);
        }

        if (buildingSelected == true)
        {
        	//double checks there is enough money and the player level is high enough
            if (moneyManager.isEnoughMoney(upgradeCost))
            {
                if (moneyManager.isLevelEnough(requiredLevel))
                {
                	//Upgrade the building
                	
                	//This tells the dcpManager to run the 'Upgrade' Command Sequence
                    dcpManager.Run(new DCP_CCD_DCPManager.RunSequence(DCP_CCD_DCPManager.RunSequence.sequenceType.Upgrade, buildingsManager.buildings.IndexOf(selectedBulding), ""));
                    isLoading = true;
                    tempSelectedBuilding = selectedBulding;
                }
            }
        }
    }

    public void FinishUpgrade ()
	{
		//This is called by the dcpManager when the 'Upgrade' Command Sequence finishes and returns 'Success'
		
		isLoading = false; // The Command Sequence is no longer running
		
		//If it is a factory it should start with empty money
		tempSelectedBuilding.EmptyMoney();
		
		//Show the upgrade particles at the position of the building
		Instantiate(particles, tempSelectedBuilding.transform.position, tempSelectedBuilding.transform.rotation);
		
		//Increase the level number of the selected building
		tempSelectedBuilding.levelNumber = tempSelectedBuilding.levelNumber + 1;
		
		//Tell the buildings manager to recount the number of each building and population of the city for a potential level up
        buildingsManager.UpdateBuildingsCount();
    }

    public void CancelUpgrade ()
	{
		// This is called by the dcpManager when the 'Upgrade Command Sequence finishes and wasn't successful
		
		// It cancels the current upgrade (the building remains selected)
        isLoading = false;
    }
}
