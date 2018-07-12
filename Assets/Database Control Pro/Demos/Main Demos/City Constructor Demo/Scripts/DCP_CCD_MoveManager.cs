using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class DCP_CCD_MoveManager : MonoBehaviour {
	
	//This script controls moving buildings through the in-game move menu
	
	//References to other managers
    public DCP_CCD_PlayerBuildingsManager buildingsManager;
    public DCP_CCD_BuildManager buildManager;
	public DCP_CCD_DCPManager dcpManager;
	
	//The green/red transparent materials for tempBuildings
    public Material greenMat;
    public Material redMat;
	
	//Has a building been selected and the DCP_CCD_BuildingInstance script of the building
    bool isBuildingSelected = false;
	DCP_CCD_BuildingInstance selectedBuilding;
	
	//The tempBuilding which should be moved around when deciding where to move the selected building to
    GameObject tempBuilding;
	float tempBuildingHeight; // << The y value to place the tempBuilding at
	Renderer[] tempBuildingRenderers; // << The renderers of the tempBuilding so the green/red material can be applied
	
	//Is the 'Move' Command Sequence being run
    bool isLoading = false;

    void Update ()
    {
        if (isLoading == false)
        {
        	//If the right mouse button is pressed, cancel move
            if (Input.GetMouseButtonDown(1))
            {
                Reset();
            }
	        
	        //If the left mouse button is pressed while over UI, cancel move
            if (EventSystem.current.IsPointerOverGameObject() == true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Reset();
                }
            }

            if (isBuildingSelected == true)
            {
            	//If a building is selected use raycast to find the position of the tempBuilding
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	            if (Physics.Raycast(ray, out hit)) // If it hits the plane for buildings
	            {
		            //enable the tempBuilding
		            tempBuilding.gameObject.SetActive(true);
		            
		            //Check the if the bounds of the tempBuilding overlap with any instantiated buildings in the buildingManager's list
                    bool canBuild = true;
                    Bounds tempBounds = tempBuilding.GetComponent<BoxCollider>().bounds;
                    foreach (DCP_CCD_BuildingInstance b in buildingsManager.buildings)
                    {
                        if (b != selectedBuilding)
                        {
                            Bounds bBounds = b.col.bounds;
                            if (tempBounds.Intersects(bBounds))
                            {
                                canBuild = false;
                            }
                        }
                    }
		            
		            //tempBuilding has red material if they overlap and green if they don't
                    if (canBuild == true)
                    {
                        foreach (Renderer r in tempBuildingRenderers)
                        {
                            r.material = greenMat;
                        }
                    }
                    else
                    {
                        foreach (Renderer r in tempBuildingRenderers)
                        {
                            r.material = redMat;
                        }
                    }
		            
		            //Set the position of the tempBuilding
                    Vector3 buildingPos = new Vector3(buildManager.gridSize * Mathf.Round(hit.point.x / buildManager.gridSize), buildManager.gridSize * Mathf.Round(hit.point.y / buildManager.gridSize), buildManager.gridSize * Mathf.Round(hit.point.z / buildManager.gridSize));
                    tempBuilding.transform.position = new Vector3(buildingPos.x, tempBuildingHeight, buildingPos.z);
		            
		            //If the mouse is pressed to move the building
		            if ((Input.GetMouseButtonDown(0)) && (canBuild == true))
		            {
			            //put the string together for the new move position which needs to be saved to the database
                        string newPos = tempBuilding.transform.position.x + "*" + tempBuilding.transform.position.y + "*" + tempBuilding.transform.position.z;
			            
			            //Tell the dcpManager to run the 'Move' Command Sequence
			            dcpManager.Run(new DCP_CCD_DCPManager.RunSequence(DCP_CCD_DCPManager.RunSequence.sequenceType.Move, buildingsManager.buildings.IndexOf(selectedBuilding), newPos));
                        isLoading = true;
                    }
                }
                else
	            {
                	//If the raycast does not hit the plane for buildings then disable the tempBuilding
                    tempBuilding.gameObject.SetActive(false);
                }
            }
        }
    }

    public void Reset()
	{
		//This is called to reset the script or cancel moving a building
		
        if (selectedBuilding != null)
        {
        	//enable the selected building
            selectedBuilding.StartGeneratingMoney();
	        selectedBuilding.gameObject.SetActive(true);
	        
	        //disable the tempBuilding object
            buildManager.buildings[selectedBuilding.buildingTypeIndex].tempBuilding.gameObject.SetActive(false);
        }
		isBuildingSelected = false;
		selectedBuilding = null;
		
		//Make sure future raycasts do hit buildings so one can be selected in order to move it
        buildingsManager.RaycastHitsBuildings();
    }

    public void ClickedOnBuilding(DCP_CCD_BuildingInstance build)
	{
		//This is called by an individual building if it is clicked on
		
		if (isLoading == false) // If the 'Move' Command Sequence is not being run
        {
			if (gameObject.activeSelf == true) // << Makes sure this manager object is active and enabled as this method might still be called when it isn't
            {
				if (isBuildingSelected == false) // If no building has already been selected
				{
					//select the building to move it by disabling the building and enabling its tempBuilding from the buildManager's buildings array
                    build.StopGeneratingMoney();
                    isBuildingSelected = true;
					selectedBuilding = build; //remember this is the selected building
					selectedBuilding.gameObject.SetActive(false); // disable this building
					tempBuilding = buildManager.buildings[selectedBuilding.buildingTypeIndex].tempBuilding; // get the tempBuilding
					tempBuilding.gameObject.SetActive(true);// enable the tempBuilding
					tempBuildingHeight = buildManager.buildings[selectedBuilding.buildingTypeIndex].heightToPlace; // get the y value to position the tempBuilding at
					
					//Reposition the tempBuiling where the selected building is
					tempBuilding.gameObject.transform.position = new Vector3(selectedBuilding.transform.position.x, tempBuildingHeight, selectedBuilding.transform.position.z);
					
					//Make sure the next raycast does not hit buildings and hits the plane the buildings are on instead
					buildingsManager.RaycastDoesntHitBuildings();
					
					//Get the renderers of the tempBuilding so the material can be changed to green/red when required
                    tempBuildingRenderers = buildManager.buildings[selectedBuilding.buildingTypeIndex].tempBuildingRenderers;
                }
            }
        }
    }
	
	public void FinishMove () {
		//This is called by the dcpManager when the 'Move' Command Sequence has finished and returned 'Success'
		
		//enable the selected building and position it where the tempBuilding is
        selectedBuilding.gameObject.SetActive(true);
		selectedBuilding.transform.position = tempBuilding.transform.position;
		
		//Disable the tempBuilding
		tempBuilding.gameObject.SetActive(false);
		
		//Reset the script
        Reset();
		isLoading = false; // << The Command Sequence has finished loading
    }

    public void CancelMove ()
	{
		//This is called by the dcpManager when the 'Move' Command Sequence has finished and has not been successful
		
		//enable the selected building without changing its position
		selectedBuilding.gameObject.SetActive(true);
		
		//Disable the tempBuilding
		tempBuilding.gameObject.SetActive(false);
		
		//Reset the script
        Reset();
        isLoading = false; // << The Command Sequence has finished loading
    }
}
