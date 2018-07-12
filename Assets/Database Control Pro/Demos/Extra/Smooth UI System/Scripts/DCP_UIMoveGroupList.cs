using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DCP_UIMoveGroupList : MonoBehaviour {

    public List<DCP_UIMove> moves2d;
    public List<DCP_UIMove1DImage> movesImages;
    public List<DCP_UIMove1DText> movesTexts;
    public List<DCP_UIMoveGroupList> moveGroupLists;

    public void MoveForward()
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
        if (moveGroupLists != null)
        {
            foreach (DCP_UIMoveGroupList m in moveGroupLists)
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
        if (moveGroupLists != null)
        {
            foreach (DCP_UIMoveGroupList m in moveGroupLists)
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
        if (moveGroupLists != null)
        {
            foreach (DCP_UIMoveGroupList m in moveGroupLists)
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
        if (moveGroupLists != null)
        {
            foreach (DCP_UIMoveGroupList m in moveGroupLists)
            {
                m.StopMovingSnap();
            }
        }
    }
}
