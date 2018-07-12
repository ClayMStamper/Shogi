using UnityEngine;
using System.Collections;

public class DCP_CCD_CameraControl : MonoBehaviour {
	
	//This script controls the position of the camera's parent which can be moved around with the arrow keys
	//No other scripts rely on this and it isn't very important so can be ignored
	
	//Controls the camera speed and direction
    public float moveSpeed = 5.0f;
    public float slowDownSpeed = 2;
	public AnimationCurve moveCurve;
	float horizontalPos = 0.5f;
	float verticalPos = 0.5f;
	
	//The limits of the camera's position
    public float maxX;
    public float minX;
    public float maxZ;
    public float minZ;

	void Update () {
		
		//Limit the camera position on the x axis
        if (transform.position.x > maxX)
        {
            if (Input.GetAxisRaw("Horizontal") <= 0)
            {
                horizontalPos = horizontalPos + (Input.GetAxisRaw("Horizontal") / 100);
            }
        } else
        {
            if (transform.position.x < minX)
            {
                if (Input.GetAxisRaw("Horizontal") >= 0)
                {
                    horizontalPos = horizontalPos + (Input.GetAxisRaw("Horizontal") / 100);
                }
            }
            else
            {
                horizontalPos = horizontalPos + (Input.GetAxisRaw("Horizontal") / 100);
            }
        }
		
		//Work out the position on the animation curve that the camera speed should be at on the horizontal axis
        if (horizontalPos > 1)
        {
            horizontalPos = 1;
        }
        if (horizontalPos < -1)
        {
            horizontalPos = -1;
        }
        float distance = 0;
        if (horizontalPos > 0.5f)
        {
            distance = horizontalPos - 0.5f;
        } else
        {
            distance = 0.5f - horizontalPos;
        }
        if (horizontalPos > 0.5)
        {
            horizontalPos = horizontalPos - ((slowDownSpeed / 1000)*(distance * 5));
        }
        if (horizontalPos < 0.5)
        {
            horizontalPos = horizontalPos + ((slowDownSpeed / 1000) * (distance * 5));
        }
		transform.Translate(Vector3.right * moveCurve.Evaluate(horizontalPos) * Time.deltaTime * moveSpeed); // Move the camera based on the calculated speed along the x axis

		//Limit the camera position on the z axis
        if (transform.position.z > maxZ)
        {
            if (Input.GetAxisRaw("Vertical") <= 0)
            {
                verticalPos = verticalPos + (Input.GetAxisRaw("Vertical") / 100);
            }
        }
        else
        {
            if (transform.position.z < minZ)
            {
                if (Input.GetAxisRaw("Vertical") >= 0)
                {
                    verticalPos = verticalPos + (Input.GetAxisRaw("Vertical") / 100);
                }
            } else
            {
                verticalPos = verticalPos + (Input.GetAxisRaw("Vertical") / 100);
            }
        }
		
		//Work out the position on the animation curve that the camera speed should be at on the vertical axis
        if (verticalPos > 1)
        {
            verticalPos = 1;
        }
        if (verticalPos < -1)
        {
            verticalPos = -1;
        }
        distance = 0;
        if (verticalPos > 0.5f)
        {
            distance = verticalPos - 0.5f;
        }
        else
        {
            distance = 0.5f - verticalPos;
        }
        if (verticalPos > 0.5)
        {
            verticalPos = verticalPos - ((slowDownSpeed / 1000) * (distance * 5));
        }
        if (verticalPos < 0.5)
        {
            verticalPos = verticalPos + ((slowDownSpeed / 1000) * (distance * 5));
        }
		transform.Translate(Vector3.forward * moveCurve.Evaluate(verticalPos) * Time.deltaTime * moveSpeed); // Move the camera based on the calculated speed along the z axis
    }
}
