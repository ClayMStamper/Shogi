using UnityEngine;
using System.Collections;

public class DCP_UIPath1D : MonoBehaviour
{
    public AnimationCurve forwardCurve;
    public AnimationCurve backwardCurve;
    public float speedForward = 1.0f;
    public float speedBack = 1.0f;
    public float startValue = 0.0f;
    public float endValue = 0.0f;

    [HideInInspector]
    public bool canMoveForward = false;
    [HideInInspector]
    public bool canMoveBack = false;

    void Awake()
    {
        if (startValue != endValue)
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
}

