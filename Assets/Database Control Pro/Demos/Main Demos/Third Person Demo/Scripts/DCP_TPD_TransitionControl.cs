using UnityEngine;
using System.Collections;

public class DCP_TPD_TransitionControl : MonoBehaviour {

    //This script controls the transition between main menu and in-game

    string username;
    string password;
    string playerData;
    bool hasLoggedIn = false;

    public DCP_UIMoveGroup transition; // << This is the transition of the black bars moving in from the side to make the screen go black
    public DCP_UIDelay transitionBack; // This is a delay to make the bars move back out after a short delay
    public DCP_TPD_MainMenuUIControl menuUI;
    public DCP_TPD_SaveLoadManager saveManager;

    //The objects which need to be enabled and disabled when switching between the main menu and in-game
    public GameObject inGameStuff;
    public GameObject menuStuff;
    public GameObject inGameUIStuff;
    public GameObject menuUIStuff;

    public UnityEngine.UI.Image blackFadeIn; // << when a player loggs out, this image fades in so the alpha needs to be set back to 0 if the player logs in again

    public DCP_TPD_InventoryUI invScript;

    [HideInInspector]
    public bool isReloading = false; // << This is used to prevent a login/logout happening when the transition finishes after reloading the game


    //This is called directly after the login success responce is recieved
    public void StorePlayerData (string playerName, string pass, string PlayerData)
    {
        username = playerName;
        playerData = PlayerData;
        password = pass;
    }

    public void StartTransition ()
    {
        //Start the transition of the black bars moving in
        transition.MoveForward();
    }

    //This method is called when the transition UI has turned the screen black
    public void TransitionEnd ()
    {
        if (isReloading == true)
        {
            //If the game is reloading, log the player back in to reset everything
            isReloading = false;
            LoginPlayer();
            hasLoggedIn = true;
        }
        else
        {
            //If it is not reloading then it is either logging in or logging out
            if (hasLoggedIn == false)
            {
                //It is logging in, so run the method to log the player in
                LoginPlayer();
                hasLoggedIn = true; // It will be logging out next time
            }
            else
            {
                //It is logging out, so run the method to log the player out
                BackToMainMenu();
                hasLoggedIn = false; // It will be logging in next time
            }
        }
        transitionBack.StartDelay(); // Makes the bars move back off the screen after a short delay
    }


    //Method called to log the player in
    void LoginPlayer ()
    {
        saveManager.ResetPlayerStuff(username, password, playerData); //Load the player data
        //Enable all of the in-game objects and UI, and disable the main menu stuff
        inGameStuff.gameObject.SetActive(true);
        menuStuff.gameObject.SetActive(false);
        inGameUIStuff.gameObject.SetActive(true);
        menuUIStuff.gameObject.SetActive(false);
        Color col = blackFadeIn.color;
        col.a = 0;
        blackFadeIn.color = col; // Set the alpha of the black image which fades in to zero. This is needed if the player has previously logged out leaving the black image faded in
        invScript.UpdateInventory(); //Update the player Inventory UI
    }

    //Method called to go back to the main menu
    void BackToMainMenu ()
    {
        menuUI.Reset(); //Reset all of the input fields on the main menu and show the 'Login' UI elements
        saveManager.playerCubesScript.ResetCubes(); //Reset the position of the cubes so they can be seen in the menu

        //Enable all of the main menu objects and UI, and disable the in-game stuff
        inGameStuff.gameObject.SetActive(false);
        menuStuff.gameObject.SetActive(true);
        inGameUIStuff.gameObject.SetActive(false);
        menuUIStuff.gameObject.SetActive(true);
    }
}
