using UnityEngine;
using System.Collections;

public class DCP_UIMoveGroup : MonoBehaviour {

    public DCP_UIMove[] moves2d;
    public DCP_UIMove1DImage[] movesImages;
    public DCP_UIMove1DText[] movesTexts;
    public DCP_UIMoveGroup[] moveGroups;

    public void MoveForward ()
    {
        if (moves2d != null)
        {
            foreach (DCP_UIMove m in moves2d)
            {
                m.MoveForward();
            }
        }
        if (movesImages != null)
        {
            foreach (DCP_UIMove1DImage m in movesImages)
            {
                m.MoveForward();
            }
        }
        if (movesTexts != null)
        {
            foreach (DCP_UIMove1DText m in movesTexts)
            {
                m.MoveForward();
            }
        }
        if (moveGroups != null)
        {
            foreach (DCP_UIMoveGroup m in moveGroups)
            {
                m.MoveForward();
            }
        }
    }
    public void MoveBack()
    {
        if (moves2d != null)
        {
            foreach (DCP_UIMove m in moves2d)
            {
                m.MoveBack();
            }
        }
        if (movesImages != null)
        {
            foreach (DCP_UIMove1DImage m in movesImages)
            {
                m.MoveBack();
            }
        }
        if (movesTexts != null)
        {
            foreach (DCP_UIMove1DText m in movesTexts)
            {
                m.MoveBack();
            }
        }
        if (moveGroups != null)
        {
            foreach (DCP_UIMoveGroup m in moveGroups)
            {
                m.MoveBack();
            }
        }
    }
    public void StopMoving()
    {
        if (moves2d != null)
        {
            foreach (DCP_UIMove m in moves2d)
            {
                m.StopMoving();
            }
        }
        if (movesImages != null)
        {
            foreach (DCP_UIMove1DImage m in movesImages)
            {
                m.StopMoving();
            }
        }
        if (movesTexts != null)
        {
            foreach (DCP_UIMove1DText m in movesTexts)
            {
                m.StopMoving();
            }
        }
        if (moveGroups != null)
        {
            foreach (DCP_UIMoveGroup m in moveGroups)
            {
                m.StopMoving();
            }
        }
    }
    public void StopMovingSnap()
    {
        if (moves2d != null)
        {
            foreach (DCP_UIMove m in moves2d)
            {
                m.StopMovingSnap();
            }
        }
        if (movesImages != null)
        {
            foreach (DCP_UIMove1DImage m in movesImages)
            {
                m.StopMovingSnap();
            }
        }
        if (movesTexts != null)
        {
            foreach (DCP_UIMove1DText m in movesTexts)
            {
                m.StopMovingSnap();
            }
        }
        if (moveGroups != null)
        {
            foreach (DCP_UIMoveGroup m in moveGroups)
            {
                m.StopMovingSnap();
            }
        }
    }

}
