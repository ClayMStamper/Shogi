using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class DCP_UIDisableInputBasedOnAlpha : MonoBehaviour {

    public float alphaDisable = 0.7f;

    InputField input;
    Image image;
    CanvasGroup canv;

    void Start ()
    {
        input = gameObject.GetComponent<InputField>();
        image = gameObject.GetComponent<Image>();
        canv = gameObject.GetComponent<CanvasGroup>();
    }

    void Update () {
	    if ((input != null) && (image != null))
        {
            if ((image.color.a < alphaDisable) && (input.enabled == true))
            {
                input.enabled = false;
                canv.blocksRaycasts = false;
                canv.interactable = false;
            }
            if ((image.color.a >= alphaDisable) && (input.enabled == false))
            {
                input.enabled = true;
                canv.blocksRaycasts = true;
                canv.interactable = true;
            }
        }
	}
}
