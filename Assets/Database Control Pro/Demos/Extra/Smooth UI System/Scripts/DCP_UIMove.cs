using UnityEngine;
using System.Collections;

public class DCP_UIMove : MonoBehaviour {

    public DCP_UIPath path;
    public bool fullPath = false;
    public DCP_UIMoveEvents moveEvents;
    public RectTransform moveObj;

    private RectTransform rTrans;
    private moveDir dir = moveDir.Still;
    private float percentageAlongPath = 0.0f;
    private bool useMoveEvents = false;

    enum moveDir
    {
        Still,
        Forward,
        Back
    }

    void Awake ()
    {
        if (moveObj == null)
        {
            moveObj = gameObject.GetComponent<RectTransform>();
        }
        rTrans = moveObj;
        if (moveEvents != null)
        {
            useMoveEvents = true;
        }
    }

    void FixedUpdate ()
    {
        if (dir == moveDir.Forward)
        {
            percentageAlongPath = percentageAlongPath + (path.speedForward / 100);
            if (percentageAlongPath > 1)
            {
                percentageAlongPath = 1;
				dir = moveDir.Still;
				if (useMoveEvents == true)
                {
                    moveEvents.endForward.Invoke();
                }
            }
            if (percentageAlongPath < 0)
            {
                percentageAlongPath = 0;
            }
            float percentageXY = path.forwardCurve.Evaluate(percentageAlongPath);
            SetPositionByPercentage(percentageXY);
        }
        if (dir == moveDir.Back)
        {
            percentageAlongPath = percentageAlongPath - (path.speedBack / 100);
            if (percentageAlongPath > 1)
            {
                percentageAlongPath = 1;
            }
            if (percentageAlongPath < 0)
            {
                percentageAlongPath = 0;
                dir = moveDir.Still;
                if (useMoveEvents == true)
                {
                    moveEvents.endBack.Invoke();
                }
            }
            float percentageXY = path.backwardCurve.Evaluate(percentageAlongPath);
            SetPositionByPercentage(percentageXY);
        }
    }

    public void MoveForward ()
    {
        if (path.canMoveForward == true)
        {
            if (useMoveEvents == true)
            {
                moveEvents.startForward.Invoke();
            }
            dir = moveDir.Forward;
            if (fullPath == true)
            {
                percentageAlongPath = 0;
                SetPosition(path.startPoint.anchoredPosition);
            }
            else
            {
                CalcPercentage();
            }
        }
    }

    public void MoveBack ()
    {
        dir = moveDir.Back;
        if (path.canMoveBack == true)
        {
            if (useMoveEvents == true)
            {
                moveEvents.startBack.Invoke();
            }
            if (fullPath == true)
            {
                percentageAlongPath = 1;
                SetPosition(path.endPoint.anchoredPosition);
            }
            else
            {
                CalcPercentage();
            }
        }
    }

    public void StopMoving ()
    {
        if (dir != moveDir.Still)
        {
            if (useMoveEvents == true)
            {
                moveEvents.onStop.Invoke();
            }
        }
        dir = moveDir.Still;
    }

    public void StopMovingSnap ()
    {
        if (dir != moveDir.Still)
        {
            if (useMoveEvents == true)
            {
                moveEvents.onStop.Invoke();
            }
            dir = moveDir.Still;
            CalcPercentage();
            if (percentageAlongPath > 0.5)
            {
                SetPosition(path.endPoint.anchoredPosition);
            }
            else
            {
                SetPosition(path.startPoint.anchoredPosition);
            }
        }
    }

    void CalcPercentage ()
    {
        float distToStart = Vector3.Distance(GetPosition(), path.startPoint.anchoredPosition);
        float totalDistance = Vector3.Distance(path.startPoint.anchoredPosition, path.endPoint.anchoredPosition);
        percentageAlongPath = distToStart / totalDistance;
    }

    Vector3 GetPosition ()
    {
        return rTrans.anchoredPosition;
    }

    void SetPosition (Vector3 position)
    {
        
            rTrans.anchoredPosition = position;
        
    }
    void SetPositionByPercentage (float p)
    {
        float difInX = path.startPoint.anchoredPosition.x - path.endPoint.anchoredPosition.x;
        float xFromEnd = difInX * (1-p);
        float newX = path.endPoint.anchoredPosition.x + xFromEnd;
        float difInY = path.startPoint.anchoredPosition.y - path.endPoint.anchoredPosition.y;
        float yFromEnd = difInY * (1-p);
        float newY = path.endPoint.anchoredPosition.y + yFromEnd;
        SetPosition(new Vector3(newX, newY, 0));
    }

}
