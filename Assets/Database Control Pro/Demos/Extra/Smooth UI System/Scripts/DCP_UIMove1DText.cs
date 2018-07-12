using UnityEngine;
using System.Collections;
using System;

public class DCP_UIMove1DText : MonoBehaviour
{
    [Serializable]
    public class MoveText
    {
        public enum textStuff
        {
            xPos,
            yPos,
            scaleX,
            scaleY,
            alpha,
            width,
            height
        }
        public UnityEngine.UI.Text text;
        public textStuff imageModifier;
    }

    public DCP_UIPath1D path;
    public bool fullPath = false;
    public DCP_UIMoveEvents moveEvents;
    [SerializeField]
    public MoveText applyToImage;

    private moveDir dir = moveDir.Still;
    private float percentageAlongPath = 0.0f;
    private bool useMoveEvents = false;

    enum moveDir
    {
        Still,
        Forward,
        Back
    }

    void Awake()
    {
        if (moveEvents != null)
        {
            useMoveEvents = true;
        }
    }

    void FixedUpdate()
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

    public void MoveForward()
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
                SetPositionToStart();
            }
            else
            {
                CalcPercentage();
            }
        }
    }

    void SetPositionToStart()
    {
        SetPosition(GetStartFloat());
    }

    void SetPositionToEnd()
    {
        SetPosition(GetEndFloat());
    }

    float GetStartFloat()
    {
        return path.startValue;
    }

    float GetEndFloat()
    {
        return path.endValue;
    }

    public void MoveBack()
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
                SetPositionToEnd();
            }
            else
            {
                CalcPercentage();
            }
        }
    }

    public void StopMoving()
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

    public void StopMovingSnap()
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
                SetPositionToEnd();
            }
            else
            {
                SetPositionToStart();
            }
        }
    }

    void CalcPercentage()
    {
        float distToStart = GetPosition() - GetStartFloat();
        float totalDistance = GetStartFloat() - GetEndFloat();
        if (totalDistance < 0)
        {
            totalDistance = -totalDistance;
        }
        percentageAlongPath = distToStart / totalDistance;
        if (GetStartFloat() > GetEndFloat())
        {
            percentageAlongPath = -percentageAlongPath;
        }
    }

    float GetPosition()
    {
        float curVal = 0.0f;
        if (applyToImage.imageModifier == MoveText.textStuff.alpha)
        {
            curVal = applyToImage.text.color.a;
        }
        if (applyToImage.imageModifier == MoveText.textStuff.xPos)
        {
            curVal = applyToImage.text.rectTransform.anchoredPosition.x;
        }
        if (applyToImage.imageModifier == MoveText.textStuff.yPos)
        {
            curVal = applyToImage.text.rectTransform.anchoredPosition.y;
        }
        if (applyToImage.imageModifier == MoveText.textStuff.scaleX)
        {
            curVal = applyToImage.text.rectTransform.localScale.x;
        }
        if (applyToImage.imageModifier == MoveText.textStuff.scaleY)
        {
            curVal = applyToImage.text.rectTransform.localScale.y;
        }
        if (applyToImage.imageModifier == MoveText.textStuff.width)
        {
            curVal = applyToImage.text.rectTransform.sizeDelta.x;
        }
        if (applyToImage.imageModifier == MoveText.textStuff.height)
        {
            curVal = applyToImage.text.rectTransform.sizeDelta.y;
        }
        return curVal;
    }

    void SetPosition(float thingToSet)
    {
        if (applyToImage.imageModifier == MoveText.textStuff.alpha)
        {
            Color newcol = applyToImage.text.color;
            newcol.a = thingToSet;
            applyToImage.text.color = newcol;
        }
        if (applyToImage.imageModifier == MoveText.textStuff.xPos)
        {
            applyToImage.text.rectTransform.anchoredPosition = new Vector2(thingToSet, applyToImage.text.rectTransform.anchoredPosition.y);
        }
        if (applyToImage.imageModifier == MoveText.textStuff.yPos)
        {
            applyToImage.text.rectTransform.anchoredPosition = new Vector2(applyToImage.text.rectTransform.anchoredPosition.x, thingToSet);
        }
        if (applyToImage.imageModifier == MoveText.textStuff.scaleX)
        {
            applyToImage.text.rectTransform.localScale = new Vector2(thingToSet, applyToImage.text.rectTransform.localScale.y);
        }
        if (applyToImage.imageModifier == MoveText.textStuff.scaleY)
        {
            applyToImage.text.rectTransform.localScale = new Vector2(applyToImage.text.rectTransform.localScale.x, thingToSet);
        }
        if (applyToImage.imageModifier == MoveText.textStuff.width)
        {
            applyToImage.text.rectTransform.sizeDelta = new Vector2(thingToSet, applyToImage.text.rectTransform.sizeDelta.y);
        }
        if (applyToImage.imageModifier == MoveText.textStuff.height)
        {
            applyToImage.text.rectTransform.sizeDelta = new Vector2(applyToImage.text.rectTransform.sizeDelta.x, thingToSet);
        }
    }

    void SetPositionByPercentage(float p)
    {
        float difInValues = GetStartFloat() - GetEndFloat();
        float valFromEnd = difInValues * (1 - p);
        SetPosition(GetEndFloat() + valFromEnd);
    }

}
