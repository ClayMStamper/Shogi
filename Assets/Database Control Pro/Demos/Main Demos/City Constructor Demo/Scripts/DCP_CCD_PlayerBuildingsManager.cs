using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DCP_CCD_PlayerBuildingsManager : MonoBehaviour {
	
	//This script keeps a list of all of the instantiated buildings and links to them
	
	//The list of all the instantiated buildings
	public List<DCP_CCD_BuildingInstance> buildings = new List<DCP_CCD_BuildingInstance>();
	
	//These are used to get the ignore raycast and default layers so raycasts on the build, upgrade and move managers work correctly
    public GameObject anyTempObj;
	public GameObject groundPlane;
	
	//References to the necessary managers
    public DCP_CCD_BuildManager buildManager;
    public DCP_CCD_MoneyManager moneyManager;
    public DCP_CCD_UpgradeManager upgradeManager;
    public DCP_CCD_MoveManager moveManager;
	public DCP_CCD_SellManager sellManager;
	
	//The numbers of each type of building
	public int[] numOfBuildings;
	
	//The particle emitter prefab for level up
	public GameObject levelUpParticles;
	
	//The gameObject which is the parent of all instantiated buildings
    public GameObject buildingsParent;

    void Awake ()
    {
        UpdateBuildingsCount();
    }

    public void Reset ()
	{
		//This is called to reset the script and all of the player's buildings
		
		//Iterate through all of the instantiated buildings
        foreach (DCP_CCD_BuildingInstance b in buildings)
        {
        	b.OnDestroyed(); // << Call a method on the building to tell it to destroy its ui objects
	        Destroy(b.gameObject); // Destroy the building
        }
		
		//Reset the array of player buildings
        buildings = new List<DCP_CCD_BuildingInstance>();
	}
	
	
	// The following two methods are used so we didn't have to include our own layers in the 'Database Control Pro' package to prevent problems ocurring with other packages

    public void RaycastHitsBuildings ()
	{
		// This method is called to change the layers of the buildings to default so that raycasts hit the buildings
		// This is required when upgrade, move or sell are selected, but no building has been selected so clicking on a building will select it
		
        foreach (DCP_CCD_BuildingInstance b in buildings)
        {
	        b.gameObject.layer = groundPlane.layer; // Set the layer of each building to the layer of the ground which should be default
        }
    }
    public void RaycastDoesntHitBuildings ()
	{
		// This method is called to change the layers of the buildings to default so that raycasts hit the buildings
		// This is required when e.g. build is selected and a building type is selected so you can see the tempBuilding. In order to position the tempBuilding correctly the raycasts must not be blocked by any instantiated buildings in the way
		
        foreach (DCP_CCD_BuildingInstance b in buildings)
        {
	        b.gameObject.layer = anyTempObj.layer; // Set the layer of each building to the layer of a tempBuilding which should be ignore raycast
        }
    }

    public void UpdateBuildingsCount ()
	{
		// This method is used to recount the number of each type of building (to limit building more) and the number of people in the city (to level up)
		
		//Reset the array for the numbers of each type of building
		numOfBuildings = new int[buildManager.buildings.Length];
		
		//Iterate through the array and set each element to 0
        for (var i = 0; i < numOfBuildings.Length; i++)
        {
            numOfBuildings[i] = 0;
        }
		
		//Iterate through the buildings and add 1 to the number of buildings of its type
        foreach (DCP_CCD_BuildingInstance b in buildings)
        {
            numOfBuildings[b.buildingTypeIndex] = numOfBuildings[b.buildingTypeIndex] + 1;
        }

        //calculate the total number of people and update the money manager with the new information
        int people = 0;
		foreach (DCP_CCD_BuildingInstance b in buildings) // Iterate through all of the buildings
        {
            if (b.buildingTypeIndex == 0)
            {
                //The building is a house and contains people
	            int numOfPeople = (int)Mathf.Round(b.levelNumber * 1.2f); // work out the number of people the house contains
	            people = people + numOfPeople; // add it together to give the total number of people
            }
        }
		moneyManager.UpdateNumberOfPeople(people); // Updates the moneyManager so it can check for level ups
    }

    public void LevelUp ()
	{
		//This is called when the 'Level Up' Command Sequence returns 'Success' to instantiate level up particles at the position of every building in the list
        foreach (DCP_CCD_BuildingInstance b in buildings)
        {
            Instantiate(levelUpParticles, b.gameObject.transform.position, b.gameObject.transform.rotation);
        }
    }

    public void CreateBuilding (int buildingType, int buildingLevel, string buildingPos, float hoursSinceLastCollect)
	{
		// This is called when logging in to create a building with the building information which has been extracted from the player data string which was returned by the 'Login' Command Sequence
		
		//Split up the position string
        string[] bPos = buildingPos.Split(new string[1] { "*" }, System.StringSplitOptions.None);
        Vector3 bPosition = Vector3.zero;
		if (bPos.Length == 3) // If it is in the correct format
		{
			//Create a Vector3 with the buildings position
            bPosition = new Vector3(float.Parse(bPos[0]), float.Parse(bPos[1]), float.Parse(bPos[2]));
		}
		
		//Instantiate the building getting the prefab from the buildManager using the buildingType as the index
        DCP_CCD_BuildingInstance newBuilding = (DCP_CCD_BuildingInstance)Instantiate(buildManager.buildings[buildingType].prefab, new Vector3(bPosition.x, buildManager.buildings[buildingType].heightToPlace, bPosition.z), buildManager.buildings[buildingType].prefab.transform.rotation);
		
		//Set the parent of the created building
		newBuilding.gameObject.transform.SetParent(buildingsParent.transform);
		
		//Set the required references of the building
        newBuilding.upgradeManager = upgradeManager;
        newBuilding.moveManager = moveManager;
		newBuilding.sellManager = sellManager;
		
		//Set the buildings level
		newBuilding.levelNumber = buildingLevel;
		
		//If it is a factory, work out and set the amount of money it should contain
        if (hoursSinceLastCollect > 0)
        {
            float secondsSinceLastCollect = hoursSinceLastCollect * 60 * 60;
            float moneyAmount = secondsSinceLastCollect * buildingLevel;
            newBuilding.currentMoney = moneyAmount;
        }
		
		//Add the created building to the array of buildings
		buildings.Add(newBuilding);
		
		newBuilding.StartGeneratingMoney(); //If it is a factory it should start generating money
		newBuilding.OnCreated(buildManager.buildingsUIParent, buildManager.canvasRectTrans, buildManager.camPosObj); // If it is a factory it should instantiate it's ui prefab
    }
}
