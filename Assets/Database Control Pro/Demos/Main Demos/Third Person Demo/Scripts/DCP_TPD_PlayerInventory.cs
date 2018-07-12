using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DCP_TPD_PlayerInventory : MonoBehaviour {

    //enum of the different cubes the player could have
    public enum Cube
    {
        Red,
        Blue,
        Green,
        Yellow,
        Pink
    }

    //The list of cubes the player has
    public List<Cube> items = new List<Cube>();

    //The inventory UI script, used to update the inventory UI when the inventory changes
    public DCP_TPD_InventoryUI uiScript;

    //This method makes sure the player doesn't have more than one of each cube in the inventory
    void CheckItems() {
        bool[] hasGotCube = new bool[5] { false, false, false, false, false };
        bool overwriteList = false;
        List<Cube> newList = new List<Cube>();
        foreach (Cube c in items)
        {
            int indexOfCube = 0;
            if (c == Cube.Blue)
            {
                indexOfCube = 1;
            }
            if (c == Cube.Green)
            {
                indexOfCube = 2;
            }
            if (c == Cube.Yellow)
            {
                indexOfCube = 3;
            }
            if (c == Cube.Pink)
            {
                indexOfCube = 4;
            }
            if (hasGotCube[indexOfCube])
            {
                //player has already got the cube
                overwriteList = true; // << The list must be overwriten so it doesn't contain 2 of the same cube
            } else
            {
                //player has not already got the cube
                newList.Add(c);
                hasGotCube[indexOfCube] = true;
            }
        }
        if (overwriteList == true)
        {
            items = newList;
            if (uiScript != null)
            {
                uiScript.UpdateInventory();
            }
        }
    }

    //Called by other scripts to add a cube to the inventory
    public void AddItem(Cube c)
    {
        items.Add(c);
        CheckItems();
        if (uiScript != null)
        {
            uiScript.UpdateInventory();
        }
    }

    //Called by other scripts to remove a cube from the inventory
    public void DropItem(Cube c)
    {
        CheckItems();
        items.Remove(c);
        if (uiScript != null)
        {
            uiScript.UpdateInventory();
        }
    }

    //Called by other scripts to remove all cubes from the inventory
    public void DropAll ()
    {
        items = new List<Cube>();
        if (uiScript != null)
        {
            uiScript.UpdateInventory();
        }
    }
}
