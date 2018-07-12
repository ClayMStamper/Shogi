using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DCP_CCD_BuildingInstance : MonoBehaviour {
	
	//This script is attached to every instantiated building

    public DCP_CCD_UpgradeManager upgradeManager;
    public DCP_CCD_MoveManager moveManager;
	public DCP_CCD_SellManager sellManager;
	
	public BoxCollider col; // << The box collider attached to this object. Used to check where new buildings can be built
	public int levelNumber = 1; // << The level number of the building. All start on level 1
	public int buildingTypeIndex = 0; // << The index of the building in the buildManager's buildings array. 0 for house. 1 for factory and 2 for bank

	public bool doesGenerateMoney = false; // Does the building generate money. This is only true for the factory
	public float maxMoneyMultiplier = 25; // Used to calculate the maximum amount of generated money the building can store
	public float currentMoney = 0.0f; // The amount of money which has been generated and has not been collected
    //money is generated at $levelNumber per second e.g. factory level 3 generates at $3 per second

	public bool doesHaveUI = false; // Does the building have ui. This is only true for the factory
	public GameObject UIParent; // The parent which the UIPrefab should be instantiated as a child of
    public GameObject UIPrefab;
	public RectTransform rectTransCanvas; // The RectTransform of the Canvas
	public GameObject cameraPosObj; // The parent of the camera gameobject
	
	//These are used to determine the alpha value of the factory's ui
    public float minUIFadeDist = 3.0f;
	public float maxUIFadeDist = 5.0f;
	
	GameObject uiObj; // The instantiated ui object
	RectTransform uiRectTrans; // .. and its RectTransform
	DCP_CCD_BuildingUIList uiGroup; // The list of ui elements for the ui object so all can be faded in and out to the correct alpha value

	int generateNumber = 0; // << This is changed to cancel the currently running IEnumerator generating money if it needs to be stopped (factory only)
	bool mouseOver = false;

    void Update ()
    {
        if (mouseOver == true)
        {
	        if (Input.GetMouseButtonDown(0)) //If the building is clicked on
            {
		        if (EventSystem.current.IsPointerOverGameObject() == false) // If the UI is not clicked on at the same time
		        {
			        //Tell the necessary in-game managers that this building has been clicked on
                    upgradeManager.ClickedOnBuilding(this as DCP_CCD_BuildingInstance);
                    moveManager.ClickedOnBuilding(this as DCP_CCD_BuildingInstance);
                    sellManager.ClickedOnBuilding(this as DCP_CCD_BuildingInstance);
                }
            }
        }
	    if (doesHaveUI == true) // Is it a factory with instantiated ui
        {
            if (uiObj != null)
            {
                if (uiRectTrans == null)
                {
                    uiRectTrans = uiObj.gameObject.GetComponent<RectTransform>() as RectTransform;
                }
                if (uiGroup == null)
                {
                    uiGroup = uiObj.gameObject.GetComponent<DCP_CCD_BuildingUIList>() as DCP_CCD_BuildingUIList;
                }
	            //Position the ui over the building.    Based on code from: http://answers.unity3d.com/questions/799616/unity-46-beta-19-how-to-convert-from-world-space-t.html
                Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(gameObject.transform.position);
                Vector2 WorldObject_ScreenPosition = new Vector2(
                ((ViewportPosition.x * rectTransCanvas.sizeDelta.x) - (rectTransCanvas.sizeDelta.x * 0.5f)),
                ((ViewportPosition.y * rectTransCanvas.sizeDelta.y) - (rectTransCanvas.sizeDelta.y * 0.5f)));
                uiRectTrans.anchoredPosition = WorldObject_ScreenPosition;
	            float distanceToCameraFocus = Vector3.Distance(cameraPosObj.transform.position, new Vector3(transform.position.x, 0, transform.position.z)); //Determine the distance of the building from the camera's main focus point
	            
	            // Work out what the alpha value of the ui should be based on the distance
	            float alpha = 0;
                if (distanceToCameraFocus < minUIFadeDist)
                {
                    alpha = 1;
                }
                else
                {
                    if (distanceToCameraFocus > maxUIFadeDist)
                    {
                        alpha = 0;
                    }
                    else
                    {
                        float diff = maxUIFadeDist - minUIFadeDist;
                        float disDiff = distanceToCameraFocus - minUIFadeDist;
                        alpha = 1 - (disDiff / diff);
                    }
                }
	            
	            //Set the alpha value of all of the UI elements by getting the lists of them from the uiGroup script
                foreach (Image i in uiGroup.images)
                {
                	//The i.color.a cannot be set individually so we have to set i.color instead. This means we have to make a copy of the color, change the alpha of the copy and apply it to the image color
                    Color newCol = i.color;
                    newCol.a = alpha;
                    i.color = newCol;
                    i.gameObject.transform.localScale = new Vector3(alpha, alpha, alpha);
                }
                foreach (Text i in uiGroup.texts)
                {
                    Color newCol = i.color;
                    newCol.a = alpha;
                    i.color = newCol;
                    i.gameObject.transform.localScale = new Vector3(alpha, alpha, alpha);
                }
	            
	            
                if (doesGenerateMoney == true)
                {
                	//work out the maximum amount of money the factory can store
	                float maxMoney = (levelNumber*levelNumber) * maxMoneyMultiplier;
	                
	                //limit the currentMoney to this maximum amount
                    if (currentMoney > maxMoney)
                    {
                        currentMoney = maxMoney;
                    }
	                
	                //display the percentage on the ui
                    int percentage = (int)Mathf.Round((currentMoney / maxMoney) * 100);
                    uiGroup.texts[0].text = "Collect " + percentage + "%";
                }
            }
        }
    }

	void OnMouseEnter ()
	{
		//Keeps track of the mouse position so we know when the building is clicked on
        mouseOver = true;
    }

    void OnMouseExit ()
	{
		//Keeps track of the mouse position so we know when the building is clicked on
        mouseOver = false;
    }

    public void StartGeneratingMoney ()
    {
        if (doesGenerateMoney == true)
        {
	        generateNumber++;
	        
	        //Run the IEnumerator to start generating money. This IEnumerator forms a cycle continuously generating money
            StartCoroutine(GenerateMoney(generateNumber));
        }
    }

    public void StopGeneratingMoney ()
    {
        if (doesGenerateMoney == true)
        {
        	//This causes the currently running IEunumerator from continuing and generating money
            generateNumber++;
        }
    }

    IEnumerator GenerateMoney (int number)
	{
		//The IEnumerator used to generate money
		//It generates the factory level number / 5, $'s of money every 0.2 seconds. This is smoother than $levelNumber every 1 second and has the same result.
		
        if (doesGenerateMoney == true)
        {
	        if (generateNumber == number) // This checks it has not been cancelled
            {
                yield return new WaitForSeconds(0.2f);
                if (generateNumber == number) // This checks it has not been cancelled
                {
                    currentMoney = currentMoney + (levelNumber / 5f);
                    generateNumber++;
	                StartCoroutine(GenerateMoney(generateNumber)); // << Starts the IEnumerator again so it forms a cycle
                }
            }
        }
    }

    public void CollectMoney ()
	{
		
		//Called when the factory UI's collect button is pressed.
		//Passes on the message to the moneyManager script providing a reference to this building script
		
        if (doesGenerateMoney == true)
        {
            StopGeneratingMoney();
            upgradeManager.moneyManager.CollectMoney(this as DCP_CCD_BuildingInstance);
            StartGeneratingMoney();
        }
    }

    public void SetUIInteractable (bool isInteractable)
	{
		//On a factory, this enables/disables the ui's 'Collect' button from being pressed. It is used when all the ui buttons need to be disabled when a Command Sequence is being run
        if (uiGroup != null)
        {
            foreach (Button b in uiGroup.buttons)
            {
                b.interactable = isInteractable;
            }
        }
    }
	
	public void EmptyMoney () {
		//This sets the current money to 0
		if (doesGenerateMoney == true) {
			currentMoney = 0;
		}
	}

    public void OnCreated (GameObject uiParent, RectTransform canv, GameObject camPos)
	{
		//Called when the building is created. If it is a factory it instantiates the ui prefab and keeps references to it
        if (doesHaveUI == true)
        {
            uiObj = (GameObject)Instantiate(UIPrefab, transform.position, transform.rotation);
            UIParent = uiParent;
            rectTransCanvas = canv;
            cameraPosObj = camPos;
            uiObj.transform.SetParent(UIParent.transform);
            uiGroup = uiObj.gameObject.GetComponent<DCP_CCD_BuildingUIList>() as DCP_CCD_BuildingUIList;
            uiGroup.building = this as DCP_CCD_BuildingInstance;
        }
    }

    public void OnDestroyed ()
	{
		//Called by the transitionManager when logging out. It tells the building to destroy it's ui gameObject (if it has one) before it is destroyed itself
        if (doesHaveUI == true)
        {
            if (uiObj != null)
            {
                Destroy(uiObj.gameObject);
                uiObj = null;
            }
        }
    }
}
