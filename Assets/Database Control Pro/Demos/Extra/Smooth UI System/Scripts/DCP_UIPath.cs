using UnityEngine;
using System.Collections;

public class DCP_UIPath : MonoBehaviour {

    public bool drawPath = true;
    public AnimationCurve forwardCurve;
    public AnimationCurve backwardCurve;
    public float speedForward = 1.0f;
    public float speedBack = 1.0f;
    public RectTransform startPoint;
    public RectTransform endPoint;

    [HideInInspector]
    public bool canMoveForward = false;
    [HideInInspector]
    public bool canMoveBack = false;

    void Awake()
    {
        if ((startPoint != null) && (endPoint != null))
        {
            if (forwardCurve != null)
            {
                canMoveForward = true;
            }
            if (backwardCurve != null)
            {
                canMoveBack = true;
            }
        }
    }

    void OnDrawGizmos ()
    {
        if (transform != null)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }
        if (startPoint != null)
        {
            startPoint.anchoredPosition = new Vector3(startPoint.anchoredPosition.x, startPoint.anchoredPosition.y, 0);
        }
        if (endPoint != null)
        {
            endPoint.anchoredPosition = new Vector3(endPoint.anchoredPosition.x, endPoint.anchoredPosition.y, 0);
        }
        RectTransform rTrans = gameObject.GetComponent<RectTransform>();
        if (rTrans!= null)
        {
            rTrans.anchoredPosition = new Vector3(rTrans.anchoredPosition.x, rTrans.anchoredPosition.y, 0);
        }
        if (drawPath == true)
        {
            if ((startPoint != null) && (endPoint != null))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(startPoint.position, endPoint.position);
            }
        }
    }

}
