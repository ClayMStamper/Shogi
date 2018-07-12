using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DCP_CCD_UIManager : MonoBehaviour {
	
	//This script controls the moving of the in-game menus (build, upgrade, move and sell) as well as the claim daily reward ui and logout button
	
	//An enumerator for the selected in-game menu
    enum menuType
    {
        none,
        build,
        move,
        update,
        sell
    }

    #region variables
	//Variables for moving the menu ui in and out
    public DCP_UIMove buildMenuMove;
    public DCP_UIMove updateMenuMove;
    public DCP_UIMove moveMenuMove;
    public DCP_UIMove sellMenuMove;
    public DCP_UIDelay buildMoveOut;
    public DCP_UIDelay upgradeMoveOut;
    public DCP_UIDelay moveMoveOut;
	public DCP_UIDelay sellMoveOut;
	
	//The Image components on the 4 ui buttons 'B' 'U' 'M' and 'S'. The selected one has to be changed to show mouse down image
    public Image buildButtonImage;
    public Image upgradeButtomImage;
    public Image moveButtonImage;
	public Image sellButtonImage;
	
	//Sprites for the above ui images. Normal and Selected sprites
    public Sprite normImg;
	public Sprite overImg;
	
	//References to the required managers
    public DCP_CCD_BuildManager buildManager;
    public DCP_CCD_UpgradeManager upgradeManager;
    public DCP_CCD_MoveManager moveManager;
    public DCP_CCD_SellManager sellManager;
    public DCP_CCD_PlayerBuildingsManager buildingsManager;
	public DCP_CCD_TransitionManager transManager;
	public DCP_CCD_DCPManager dcpManager;
	
	//The UI GameObjects for claiming the daily reward or having already claimed the daily reward
    public GameObject claimRewardStuff;
    public GameObject claimedRewardStuff;
    #endregion

    #region other variables
	// The selected in-game menu
    menuType mType = menuType.none;
    #endregion

    #region button presses
    public void BuildMenuButtonPressed ()
	{
		// This is called when the 'B' button is pressed for the in-game build menu
		
        if (mType != menuType.build)
        {
        	//If build is not already selected
            MoveMenuBack();
            if (mType != menuType.none)
            {
            	//Move the build menu in after a short delay waiting for the current menu to move out
                mType = menuType.build;
                MoveMenuForward();
            } else
            {
            	//Move the build menu in straight away
                mType = menuType.build;
                buildMenuMove.MoveForward();
            }
        } else
        {
        	//If build is already selected, move the menu back and changed the selected menu to none
            MoveMenuBack();
            mType = menuType.none;
        }
        MenuTypeChanged();
	}
	//The following 3 methods do the same as the one above but for the different in-game menus
    public void UpdateMenuButtonPressed ()
    {
        if (mType != menuType.update)
        {
            MoveMenuBack();
            if (mType != menuType.none)
            {
                mType = menuType.update;
                MoveMenuForward();
            }
            else
            {
                mType = menuType.update;
                updateMenuMove.MoveForward();
            }
        }
        else
        {
            MoveMenuBack();
            mType = menuType.none;
        }
        MenuTypeChanged();
    }
    public void MoveMenuButtonPressed ()
    {
        if (mType != menuType.move)
        {
            MoveMenuBack();
            if (mType != menuType.none)
            {
                mType = menuType.move;
                MoveMenuForward();
            }
            else
            {
                mType = menuType.move;
                moveMenuMove.MoveForward();
            }
        }
        else
        {
            MoveMenuBack();
            mType = menuType.none;
        }
        MenuTypeChanged();
    }
    public void SellMenuButtonPressed ()
    {
        if (mType != menuType.sell)
        {
            MoveMenuBack();
            if (mType != menuType.none)
            {
                mType = menuType.sell;
                MoveMenuForward();
            }
            else
            {
                mType = menuType.sell;
                sellMenuMove.MoveForward();
            }
        }
        else
        {
            MoveMenuBack();
            mType = menuType.none;
        }
        MenuTypeChanged();
    }
	
	
    public void LogoutButtonPressed ()
	{
		//This is called when the logout button is pressed
		
		//Tell the transition manager to logout the player
        transManager.Logout();
	}
	
	public void ClaimDailyRewardPressed() {
		//This is called when the 'Claim' button is pressed to claim the $100 daily reward
		
		//Tell the dcpManager to run the 'Claim Daily Reward' Command Sequence
        dcpManager.Run(new DCP_CCD_DCPManager.RunSequence(DCP_CCD_DCPManager.RunSequence.sequenceType.ClaimReward, 0, ""));
	}
	
    public void Reset ()
	{
		//This method is called to reset the script so no in-game menu is selected
		
		//Move any selected menu out
        MoveMenuBack();
		mType = menuType.none;
		
		//none of the 'B', 'U', 'M' or 'S' buttons should show mouse down sprite
        buildButtonImage.sprite = normImg;
        upgradeButtomImage.sprite = normImg;
        moveButtonImage.sprite = normImg;
		sellButtonImage.sprite = normImg;
		
		//The next raycast should hit the buildings
		buildingsManager.RaycastHitsBuildings();
		
		//All of the in-game menu managers are disabled as none of the menus are selected
        buildManager.gameObject.SetActive(false);
        upgradeManager.gameObject.SetActive(false);
        moveManager.gameObject.SetActive(false);
        sellManager.gameObject.SetActive(false);
    }
    #endregion

    #region other methods
    void MoveMenuForward()
	{
		//This method is called to start DCP_UIDelay's to move the selected menu in
		
		if (mType == menuType.build) // << If the build menu has been selected
		{
			//Start delay to move it in
            buildMoveOut.StartDelay();
        }
        if (mType == menuType.update)
        {
            upgradeMoveOut.StartDelay();
        }
        if (mType == menuType.move)
        {
            moveMoveOut.StartDelay();
        }
        if (mType == menuType.sell)
        {
            sellMoveOut.StartDelay();
        }
    }
    void MoveMenuBack()
	{
		//This method is called to make the selected menu move off the screen so no menu is being shown
		
		
		if (mType == menuType.build) // << If the build menu is selected
		{
			//Make the build menu move off the screen
            buildMenuMove.MoveBack();
        }
        if (mType == menuType.update)
        {
            updateMenuMove.MoveBack();
        }
        if (mType == menuType.move)
        {
            moveMenuMove.MoveBack();
        }
        if (mType == menuType.sell)
        {
            sellMenuMove.MoveBack();
        }
	}

    void MenuTypeChanged()
	{
		//This is called when the selected menu has changed in order to make the 'B', 'U', 'M' or 'S' button's sprites change to normal or mouse down
		
		if (mType == menuType.none) // << No menu type is selected
		{
			//All button sprites should be normal
            buildButtonImage.sprite = normImg;
            upgradeButtomImage.sprite = normImg;
            moveButtonImage.sprite = normImg;
			sellButtonImage.sprite = normImg;
			
			buildingsManager.RaycastHitsBuildings();
			
			//All of the managers should be disabled as none are selected
            buildManager.gameObject.SetActive(false);
            upgradeManager.gameObject.SetActive(false);
            moveManager.gameObject.SetActive(false);
            sellManager.gameObject.SetActive(false);
        }
		if (mType == menuType.build) // << Build menu type is selected
        {
			buildButtonImage.sprite = overImg; // 'B' button should have mouse down sprite, the others should have normal sprite
            upgradeButtomImage.sprite = normImg;
            moveButtonImage.sprite = normImg;
            sellButtonImage.sprite = normImg;
            buildingsManager.RaycastDoesntHitBuildings();
            if (buildManager.gameObject.activeSelf == false)
            {
            	//If the buildManager is disabled (it is about to be enabled), it should be reset
                buildManager.Reset();
            }
			buildManager.gameObject.SetActive(true); // The buildManager should be enabled as it's menu has been selected while all others are not selected so should be disabled
			upgradeManager.gameObject.SetActive(false);
            moveManager.gameObject.SetActive(false);
            sellManager.gameObject.SetActive(false);
        }
        if (mType == menuType.update)
        {
            buildingsManager.RaycastHitsBuildings();
            buildButtonImage.sprite = normImg;
            upgradeButtomImage.sprite = overImg;
            moveButtonImage.sprite = normImg;
            sellButtonImage.sprite = normImg;
            if (upgradeManager.gameObject.activeSelf == false)
            {
                upgradeManager.Reset();
            }
            buildManager.gameObject.SetActive(false);
            upgradeManager.gameObject.SetActive(true);
            moveManager.gameObject.SetActive(false);
            sellManager.gameObject.SetActive(false);
        }
        if (mType == menuType.move)
        {
            buildingsManager.RaycastHitsBuildings();
            buildButtonImage.sprite = normImg;
            upgradeButtomImage.sprite = normImg;
            moveButtonImage.sprite = overImg;
            sellButtonImage.sprite = normImg;
            if (moveManager.gameObject.activeSelf == false)
            {
                moveManager.Reset();
            }
            buildManager.gameObject.SetActive(false);
            upgradeManager.gameObject.SetActive(false);
            moveManager.gameObject.SetActive(true);
            sellManager.gameObject.SetActive(false);
        }
        if (mType == menuType.sell)
        {
            buildingsManager.RaycastHitsBuildings();
            buildButtonImage.sprite = normImg;
            upgradeButtomImage.sprite = normImg;
            moveButtonImage.sprite = normImg;
            sellButtonImage.sprite = overImg;
            if (sellManager.gameObject.activeSelf == false)
            {
                sellManager.Reset();
            }
            buildManager.gameObject.SetActive(false);
            upgradeManager.gameObject.SetActive(false);
            moveManager.gameObject.SetActive(false);
            sellManager.gameObject.SetActive(true);
        }
	}
	
    public void SetDailyRewardUI (bool shouldShowUI)
	{
		//This is called when a player has just logged in and the player data is being loaded
		
		//Enables the correct daily reward ui based on whether it has already been claimed or not
        claimRewardStuff.gameObject.SetActive(shouldShowUI);
        claimedRewardStuff.gameObject.SetActive(!shouldShowUI);
    }
    public void FinishClaimReward ()
	{
		//This is called by the dcpManager when the 'Claim Daily Reward' sequence returns 'Success'
		
		//This changes the ui (using the method above) so the player cannot claim the daily reward again
        SetDailyRewardUI(false);
    }
    #endregion
}
