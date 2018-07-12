using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class DCP_UIDisableButtonBasedOnAlpha : MonoBehaviour {

    public float alphaDisable = 0.7f;

    Button button;
    Image image;
    CanvasGroup canv;

    void Start()
    {
        button = gameObject.GetComponent<Button>();
        image = gameObject.GetComponent<Image>();
        canv = gameObject.GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if ((button != null) && (image != null))
        {
            if ((image.color.a < alphaDisable) && (button.enabled == true))
            {
                button.enabled = false;
                canv.blocksRaycasts = false;
                canv.interactable = false;
            }
            if ((image.color.a >= alphaDisable) && (button.enabled == false))
            {
                button.enabled = true;
                canv.blocksRaycasts = true;
                canv.interactable = true;
            }
        }
    }
}
