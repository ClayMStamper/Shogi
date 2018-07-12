using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class DCP_UIBlockRaycastsBasedOnAlpha : MonoBehaviour {

    public float alphaDisable = 0.7f;

    Image image;
    CanvasGroup canv;

    void Start()
    {
        image = gameObject.GetComponent<Image>();
        canv = gameObject.GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if ((canv != null) && (image != null))
        {
            if ((image.color.a < alphaDisable) && (canv.blocksRaycasts == true))
            {
                canv.blocksRaycasts = false;
            }
            if ((image.color.a >= alphaDisable) && (canv.blocksRaycasts == false))
            {
                canv.blocksRaycasts = true;
            }
        }
    }
}
