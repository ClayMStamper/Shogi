using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DCP_TPD_InventoryUI : MonoBehaviour {

    //This script accesses the PlayerInventory script on the 'InventoryManager' object and enables/disables and renames the UI buttons in the Inventory list to show the up-to-date player inventory

    public DCP_TPD_PlayerInventory inventoryScript;
    public GameObject emptyInventoryObj; // << The UI Text object which says 'Empty'
    public GameObject inventoryButtonsObj; // << The parent of all of the UI inventory item buttons
    public GameObject[] buttonObjs; // << The 5 individual inventory button objects
    public Text[] buttonTexts; // << The UI Text components of the text displayed on each button. They are renamed to show the inventory in the correct order. E.g. 'Red Cube', 'Yellow Cube', 'Blue Cube', etc

    void Awake ()
    {
        //This disables all of the inventory buttons and shows the 'Empty' text at the very start of the game
        UpdateInventory();
    }

    //Called by the PlayerInventory script when the inventory changes to update the UI
	public void UpdateInventory () {
	    if ((inventoryScript != null) && (emptyInventoryObj != null) && (inventoryButtonsObj != null) && (buttonObjs != null) && (buttonTexts != null)) //Always a good idea to check things aren't null
        {
            //Disable all of the inventory button objects
            foreach (GameObject b in buttonObjs)
            {
                b.gameObject.SetActive(false);
            }

            //Check if inventory is empty or not, by checking the list on the PlayerInventory script
            bool hasInventory = false;
            if (inventoryScript.items != null)
            {
                if (inventoryScript.items.Count > 0)
                {
                    hasInventory = true;
                }
            }
            if (hasInventory == true)
            {
                //Inventory is not empty, enable the parent buttons obj, and disable the 'Empty' text
                inventoryButtonsObj.gameObject.SetActive(true);
                emptyInventoryObj.gameObject.SetActive(false);

                //Iterate through the cubes in the inventory, enable a button for each of them and set the button text to the cube type
                for (int i = 0; i < inventoryScript.items.Count; i++)
                {
                    if ((buttonObjs[i] != null) && (buttonTexts[i] != null))
                    {
                        buttonObjs[i].gameObject.SetActive(true);
                        if (inventoryScript.items[i] == DCP_TPD_PlayerInventory.Cube.Red)
                        {
                            buttonTexts[i].text = "Red Cube";
                        }
                        if (inventoryScript.items[i] == DCP_TPD_PlayerInventory.Cube.Blue)
                        {
                            buttonTexts[i].text = "Blue Cube";
                        }
                        if (inventoryScript.items[i] == DCP_TPD_PlayerInventory.Cube.Yellow)
                        {
                            buttonTexts[i].text = "Yellow Cube";
                        }
                        if (inventoryScript.items[i] == DCP_TPD_PlayerInventory.Cube.Pink)
                        {
                            buttonTexts[i].text = "Pink Cube";
                        }
                        if (inventoryScript.items[i] == DCP_TPD_PlayerInventory.Cube.Green)
                        {
                            buttonTexts[i].text = "Green Cube";
                        }
                    }
                }
            } else
            {
                //Inventory is empty, disable inventory buttons and show 'Empty' text
                inventoryButtonsObj.gameObject.SetActive(false);
                emptyInventoryObj.gameObject.SetActive(true);
            }
        }
	}
}
