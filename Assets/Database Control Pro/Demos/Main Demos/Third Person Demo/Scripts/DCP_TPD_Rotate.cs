using UnityEngine;
using System.Collections;

public class DCP_TPD_Rotate : MonoBehaviour {

    //This script is attached to each cube and a parent of the camera in the main menu
    //It makes the object rotate

    public float rotSpeed = 50.0f; // the speed to rotate

	void Update () {
        //Rotate the object a bit more every frame, the amount influenced by the rotation speed and the time since the last frame
        transform.Rotate(Vector3.up * rotSpeed * Time.deltaTime);
    }
}
