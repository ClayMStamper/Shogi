using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour {

    [SerializeField, HideInInspector]
    Vector3 tallPos;
    [SerializeField, HideInInspector]
    Vector3 widePos;

    const float MAX = 0.60f;
    const float MIN = 0.46f;


    private void Start() {

        float screenW= Screen.width;
        float screenH = Screen.height;

        float range = MAX - MIN;
        float ratio = screenW / screenH;

        //screem size on a scale of 0 to 1
        float normalized = (ratio - MIN) / range;

               Debug.Log("Ratio: " + ratio);
               Debug.Log("Percentage: " + normalized);

        MoveCam(normalized);
    }

    private void MoveCam(float screenSize) {
        transform.position = Vector3.Lerp(tallPos, widePos, screenSize);
    }

    [ContextMenu ("Save camera for wide screens")]
    public void SaveCamForWideScreen() {
        widePos = transform.position;
    }

    [ContextMenu("Save camera for tall screens")]
    public void SaveCamForTallScreen() {
        tallPos = transform.position;
    }


}

