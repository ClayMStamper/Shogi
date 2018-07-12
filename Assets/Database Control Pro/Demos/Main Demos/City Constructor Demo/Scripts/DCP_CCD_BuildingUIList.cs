using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DCP_CCD_BuildingUIList : MonoBehaviour {
	
	//This is used to store links to all of the ui elements for a building (only used by factorys)

    public Image[] images;
    public Text[] texts;
    public Button[] buttons;
	public DCP_CCD_BuildingInstance building; // Stores link to the building which instantiated it

    public void CollectButtonPressed ()
	{
		//This is directly called when the 'Collect' button is pressed. It passes the message onto the building's script which in turn passes it on to the moneyManager script
		
        building.CollectMoney();
    }

}
