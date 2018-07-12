using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DCP_CCD_SellManager : MonoBehaviour {
	
	//This script controls selling a building when the in-game sell menu is selected
	
	//The UI gameObjects to show when a building is or isn't selected
    public GameObject noBuildingSelected;
    public GameObject buildingSelectedUI;
	
	//The UI texts to show the selected building's name and the money which would be recieved on selling
    public Text selectedBuildingNameText;
    public Text sellPriceText;
	
	//References to the other managers
    public DCP_CCD_BuildManager buildManager;
    public DCP_CCD_MoneyManager moneyManager;
    public DCP_CCD_PlayerBuildingsManager buildingsManager;
    public DCP_CCD_DCPManager dcpManager;
	
	//The particle emitter prefab for the red particles which should be created when a building has been sold
    public GameObject sellParticles;
	
	//If a building has been selected and a reference to the building
    bool isSelected = false;
    DCP_CCD_BuildingInstance selectedBuliding;
	
	//If the 'Sell' Command Sequence is being run
    bool isLoading = false;

    void Update ()
    {
	    if (isSelected == true) // If there is a building selected
        {
            //calculate sell price of the selected building
            int sellPrice = 0;
            int initialCost = buildManager.buildings[selectedBuliding.buildingTypeIndex].cost;
            int totalCost = initialCost;
            for (var i = 1; i < selectedBuliding.levelNumber; i++)
            {
            	//Add together all of the money spent of upgrades to get the total amount ever spent on the building
                totalCost = totalCost + (initialCost + ((i * i) * (initialCost / 2)));
            }
		    //Half the total cost to get the sell price
            sellPrice = (int)Mathf.Floor(totalCost * 0.5f);

            //Show the sell price as ui text
		    sellPriceText.text = "" + sellPrice;
		    
		    //Show the ui for a when a building is selected
            noBuildingSelected.gameObject.SetActive(false);
		    buildingSelectedUI.gameObject.SetActive(true);
		    
		    //Show the name of the building
            selectedBuildingNameText.text = buildManager.buildings[selectedBuliding.buildingTypeIndex].name;
        } else {
        	
        	//Show the ui for when a building isn't selected
        	noBuildingSelected.gameObject.SetActive(true);
	        buildingSelectedUI.gameObject.SetActive(false);
        }
    }

	public void Reset ()
	{
		//This is called to reset this script and cancel the selling of a building
		
		isSelected = false; // << No building should be selected
        selectedBuliding = null;
    }

    public void ClickedOnBuilding(DCP_CCD_BuildingInstance build)
	{
		//This is called by and individual building when it is clicked on
		
		if (isLoading == false) // << If the 'Sell' Command Sequence is not runnnning
		{
			//Select the building and store a reference to it
            isSelected = true;
            selectedBuliding = build;
        }
    }

    public void SellButtonPressed ()
	{
		//This is called when the 'Sell' button is pressed to sell the selected building
		
        if (isSelected == true)
        {
	        isLoading = true;
	        
	        //This tells the dcpManager to run the 'Sell' Command Sequence to sell the building
            dcpManager.Run(new DCP_CCD_DCPManager.RunSequence(DCP_CCD_DCPManager.RunSequence.sequenceType.Sell, buildingsManager.buildings.IndexOf(selectedBuliding), ""));
        }
    }
	
	public void FinishSell ()
	{
		//This is called by the dcpManager if the 'Sell' Command Seuquence returns 'Success'
		
		//If it is a factory it should stop its generating money IEnumerator
		selectedBuliding.StopGeneratingMoney();
		
		//Instantiate the red sell building particle prefab
		Instantiate(sellParticles, selectedBuliding.transform.position, selectedBuliding.transform.rotation);
		
		//Tell the money manager to sell the building
		moneyManager.SellBuilding(selectedBuliding);
		
		//Remove the building from the buildingsManager
		buildingsManager.buildings.Remove(selectedBuliding);
		
		//Destroy the building
		Destroy(selectedBuliding.gameObject);
		
		//no building is selected
		isSelected = false;
		selectedBuliding = null;
		
		//Tell the buildings manager to update the numbers of each building so 1 more can be built if it is a factory or bank
        buildingsManager.UpdateBuildingsCount();
        isLoading = false;
	}

    public void CancelSell ()
	{
		//This is called by the dcpManager if the 'Sell' Command Sequence finishes but is not a success
		
		//It ends trying to sell the building without selling it.
        isLoading = false;
    }
}
