using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DCP_TPD_ControlTime : MonoBehaviour {

    //This script pauses time based on the alpha value of the fading in black image
    //This probably is not the best way to do this, but it works well for a demo

    public Image fadeInBlack; // The black image which fades in and out
    public float alphaCutout = 0.2f; // < If the alpha of the image is greater than this time is paused
    public bool gamePaused = false;

    //It doesn't use Time.timeScale as that would pause the ui so it just disables and enables components (except the player rigidbody which uses isKnematic)
    public Animator animator;
    public Rigidbody playerBody;
    public UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter playerScript;
    public UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl playerControl;
    public UnityStandardAssets.Cameras.AutoCam cameraScript;

    void Update()
    {
        if (fadeInBlack != null)
        {
            if ((gamePaused == true) && (fadeInBlack.color.a < alphaCutout))
            {
                //Game should not be paused
                gamePaused = false;

                //enable everything
                animator.enabled = true;
                playerBody.isKinematic = false;
                playerScript.enabled = true;
                playerControl.enabled = true;
                cameraScript.enabled = true;
            }
            if ((gamePaused == false) && (fadeInBlack.color.a > alphaCutout))
            {
                //game should be paused
                gamePaused = true;

                //disable everything
                animator.enabled = false;
                playerBody.isKinematic = true;
                playerScript.enabled = false;
                playerControl.enabled = false;
                cameraScript.enabled = false;
            }
        }
    }

}
