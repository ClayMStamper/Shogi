using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class DCP_CCD_BuildManager : MonoBehaviour {
	
	//This script controls creating new buildings through the in-game build menu
	//It also stores information about the different buildings available which is used by other scripts

    [Serializable]
	public class Building
	{
		//This is a class which represents a building
		
        public string name = "Building Name";
        public int cost = 100;
		public GameObject tempBuilding; // << The object with the green/red transparent material which is moved around when deciding the position of the building
		public Renderer[] tempBuildingRenderers; // << The renderers attached to the tempBuilding so the material can be changed between green and red
		public float heightToPlace = 0f; // The height the building should be instantiated at
		public DCP_CCD_BuildingInstance prefab; // The building prefab
		public bool areNumberLimited = false; // Should the number of these buildings available be limited based on the player's level
		public float limitedNumberMultiplier = 0.2f; // The number of these buildings available for each player level
    }

    [SerializeField]
	public Building[] buildings; // The array of the buildings which can be built
	
	//If a building has been selected and the index of the selected building in the buildings array
    public bool buildSelected = true;
	public int buildingIndex = 0;
	
	//The main camera object and its parent
	public Camera mainCamera;
	public GameObject camPosObj;
	
	//The green and red materials for the tempBuildingRenderers to be set to
    public Material RedMaterial;
	public Material GreenMaterial;
	
	public float gridSize = 0.5f; // The grid size for snapping the tempBuilding when deciding the position of the building
	
	//References to other manager scripts in the scene
    public DCP_CCD_UpgradeManager upgradeManager;
    public DCP_CCD_MoveManager moveManager;
    public DCP_CCD_SellManager sellManager;
	public DCP_CCD_PlayerBuildingsManager buildingsManager;
	public DCP_CCD_MoneyManager moneyManager;
	public DCP_CCD_DCPManager dcpManager;
	
	//Arrays for the ui elements of the buildings in the in-game build menu
    public GameObject[] buildButtons;
    public GameObject[] cannotAffordTexts;
	public GameObject[] numberOfBuildingsLimitedUI;
	
	//The parent of instantiated buildings
	public GameObject buildingsParent;
	
	//These references are passed on the buidling instance script of a building when it is created so the building's ui can be created and positioned correctly
	public GameObject buildingsUIParent;
    public RectTransform canvasRectTrans;
	
	//These are used to store information about the building to be built when the game is paused as the 'Build' Command Sequence is being run
    int tempBuildingIndex;
    Vector3 tempBuildingPos;
    bool isLoading = false;

    void Update()
    {
	    if (isLoading == false) // If the Command Sequence is not being run
	    {
		    //This enables/disables the UI elements in the in-game build menu based on whether another building can be built and whether the player has enough money
            int index = 0;
            foreach (Building b in buildings)
            {
                if (moneyManager.isEnoughMoney(b.cost))
                {
                    if (b.areNumberLimited == true)
                    {
                        int limitNum = (int)Mathf.Floor(moneyManager.GetLevel() * b.limitedNumberMultiplier) + 1;
                        if (buildingsManager.numOfBuildings[index] < limitNum)
                        {
                            buildButtons[index].gameObject.SetActive(true);
                            cannotAffordTexts[index].gameObject.SetActive(false);
                            numberOfBuildingsLimitedUI[index].gameObject.SetActive(false);
                        }
                        else
                        {
                            buildButtons[index].gameObject.SetActive(false);
                            cannotAffordTexts[index].gameObject.SetActive(false);
                            numberOfBuildingsLimitedUI[index].gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        buildButtons[index].gameObject.SetActive(true);
                        cannotAffordTexts[index].gameObject.SetActive(false);
                        numberOfBuildingsLimitedUI[index].gameObject.SetActive(false);
                    }
                }
                else
                {
                    buildButtons[index].gameObject.SetActive(false);
                    cannotAffordTexts[index].gameObject.SetActive(true);
                    numberOfBuildingsLimitedUI[index].gameObject.SetActive(false);
                }
                index++;
            }
		    
		    //When the mouse is pressed over a ui element, any progress with creating a building is canceled.
            if (EventSystem.current.IsPointerOverGameObject() == true)
            {
                if ((Input.GetMouseButtonDown(0)) || (Input.GetMouseButtonDown(1)))
                {
                    if (buildSelected == true)
                    {
                        buildSelected = false;
                        DisableBuild();
                    }
                }
            }
		    
		    //If a building has been selected in the menu this positions the tempBuilding to choose the position of the building
            if (buildSelected == true)
            {
                if (buildings != null)
                {
                    if ((buildingIndex > -1) && (buildingIndex < buildings.Length))
                    {
                        RaycastHit hit;
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	                    if (Physics.Raycast(ray, out hit)) // If the mouse is over the plane which buildings can be built on
                        {
                        	//check the bounds of the colliders of all existing buildings. If they intersect with the tempBuilding then it cannot be built at its current position
                            bool canBuild = true;
                            Bounds tempBounds = buildings[buildingIndex].tempBuilding.GetComponent<BoxCollider>().bounds;
                            foreach (DCP_CCD_BuildingInstance b in buildingsManager.buildings)
                            {
                                Bounds bBounds = b.col.bounds;
                                if (tempBounds.Intersects(bBounds))
                                {
                                    canBuild = false;
                                }
                            }
	                        //Shows the green/red material if the building can or cannot be built at the current position of the tempBuilding
                            if (canBuild == true)
                            {
                                foreach (Renderer r in buildings[buildingIndex].tempBuildingRenderers)
                                {
                                    r.material = GreenMaterial;
                                }
                            }
                            else
                            {
                                foreach (Renderer r in buildings[buildingIndex].tempBuildingRenderers)
                                {
                                    r.material = RedMaterial;
                                }
                            }
	                        
	                        //enable the tempBuilding and set its position to the point where this mouse is over the plane
                            Vector3 buildingPos = new Vector3(gridSize * Mathf.Round(hit.point.x / gridSize), gridSize * Mathf.Round(hit.point.y / gridSize), gridSize * Mathf.Round(hit.point.z / gridSize));
                            buildings[buildingIndex].tempBuilding.SetActive(true);
                            buildings[buildingIndex].tempBuilding.transform.position = new Vector3(buildingPos.x, buildings[buildingIndex].heightToPlace, buildingPos.z);
		                    
		                    //If the left mouse button is pressed where a building can be built
		                    if ((Input.GetMouseButtonDown(0)) && (canBuild == true))
		                    {
			                    //remember information about the building to be built so it can be continued after the Command Sequence has been run
                                tempBuildingIndex = buildingIndex;
			                    tempBuildingPos = new Vector3(buildingPos.x, buildings[buildingIndex].heightToPlace, buildingPos.z);
			                    
			                    //Tell the dcpManager to run the 'Build' Command Sequence providing the necessary information
                                dcpManager.Run(new DCP_CCD_DCPManager.RunSequence(DCP_CCD_DCPManager.RunSequence.sequenceType.Build, buildingIndex, tempBuildingPos.x + "*" + tempBuildingPos.y + "*" + tempBuildingPos.z));
			                    
			                    isLoading = true; //This prevents anything from happening while the Command Sequence is running
                            }
                        }
                        else
	                    {
                        	//The mouse is not over an area where buildings can be built so the temp building is disabled
                            buildings[buildingIndex].tempBuilding.SetActive(false);
	                    }
	                    
	                    //If the right mouse button is pressed, cancel the build
                        if (Input.GetMouseButtonDown(1))
                        {
                            DisableBuild();
                            buildSelected = false;
                        }
                    }
                }
            }
        }
    }

    void DisableBuild ()
	{
		//This is called to cancel the build
        foreach (Building b in buildings)
        {
        	//disable all of the tempBuilding objects
            b.tempBuilding.gameObject.SetActive(false);
        }
    }

    public void SelectBuilding (int index)
	{
		//This is called when a building's button is pressed in the in-game build ui menu
		
        if (buildSelected == false)
        {
        	//If a building has not already been selected, select the building which has had it's ui button pressed
            if (buildings != null)
            {
                if ((index > -1) && (index < buildings.Length))
                {
                    buildingIndex = index;
                    buildSelected = true;
                }
            }
        } else
        {
        	// If a building has already been selected, disable build
            buildSelected = false;
            DisableBuild();
        }
     }

    public void Reset ()
	{
		//Resets this script by disabling the build
	    DisableBuild();
	    buildSelected = false;
    }

    public void FinishBuild()
	{
		//Called by the dcpManager when the 'Build' Command Sequence has finished being run and has returned 'Success'
		
		isLoading = false; //Enable everything as it has finished loading
		
		//Instantiate the building and set its parent
        DCP_CCD_BuildingInstance newBuilding = (DCP_CCD_BuildingInstance)Instantiate(buildings[tempBuildingIndex].prefab, tempBuildingPos, buildings[tempBuildingIndex].prefab.transform.rotation);
		newBuilding.gameObject.transform.SetParent(buildingsParent.transform);
		
		//Add references to the building which it will need
        newBuilding.upgradeManager = upgradeManager;
        newBuilding.moveManager = moveManager;
		newBuilding.sellManager = sellManager;
		
		//Add the building to the array of instantiated buildings on the buildingsManager script
		buildingsManager.buildings.Add(newBuilding);
		
		//If it is a factory it should start generating money
		newBuilding.StartGeneratingMoney();
		
		//Disable this build as it has finished
        DisableBuild();
		buildSelected = false;
		
		//Tell the buildingsManager to recount the buildings and number of people in them for a potential level up
		buildingsManager.UpdateBuildingsCount();
		
		//If the building has ui, this tells the new building to create it providing the necessary references
        newBuilding.OnCreated(buildingsUIParent, canvasRectTrans, camPosObj);
    }

	public void CancelBuild()
	{
		//Called by the dcpManager when the 'Build' Command Sequence has finished but it was not a success
		
		//Cancels the current build
		DisableBuild();
		buildSelected = false;
        isLoading = false; //Enable everything as it has finished loading
    }
}
