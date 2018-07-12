using UnityEngine;
using System.Collections;

public class DCP_CCD_TransitionManager : MonoBehaviour {
	
	//This script handles the transition between the game and menu
	
	//Arrays of all of the inGame gameObjects and the menu gameObjects which can be enabled/disabled when logging in or logging out
	public GameObject[] inGameObjs;
	public GameObject[] menuObjs;
	
	//References to the other managers which need to be reset
	public DCP_CCD_PlayerBuildingsManager buildingsManager;
    public DCP_CCD_MenuManager menuManager;
    public DCP_CCD_DCPManager dcpManager;
    public DCP_CCD_UIManager uiManager;
    public DCP_CCD_MoneyManager moneyManager;
    public DCP_CCD_BuildManager buildManager;

    #region public methods
	public void Login () {
		//This is called when the player logs in or registers
		
		//Disable all of the menu gameObjects and enable all of the in-game objects
		foreach (GameObject obj in inGameObjs) {
			obj.SetActive(true);
		}
		foreach (GameObject obj in menuObjs) {
			obj.SetActive(false);
		}
		
		//Reset some managers
        dcpManager.Reset();
        buildManager.Reset();
        uiManager.Reset();
    }
	public void Logout () {
		//This is called when the player logs out
		
		//Reset the buildingsManager first to destroy all instantiated buildings
		buildingsManager.Reset();
		
		//Enable all of the menu gameObjects and disable all of the in-game objects
		foreach (GameObject obj in inGameObjs) {
			obj.SetActive(false);
		}
		foreach (GameObject obj in menuObjs) {
			obj.SetActive(true);
		}
		
		//Reset some managers
        menuManager.Reset();
        dcpManager.Reset();
        uiManager.Reset();
		moneyManager.Reset();
		
		//Make the login ui move back onto the screen so the player can login again
		menuManager.loginMoveOut.MoveForward();
    }
    #endregion
}
