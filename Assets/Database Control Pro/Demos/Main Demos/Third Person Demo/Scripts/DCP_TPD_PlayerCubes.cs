using UnityEngine;
using System.Collections;

public class DCP_TPD_PlayerCubes : MonoBehaviour {

    //This script determines when a cube can be picked up by the player and it resets the position of the cubes

    public DCP_TPD_Cube[] cubes; // << The array of the 5 cubes
    public Transform player;
    public float pickUpRange = 1.5f; // << If a distance between the player and a cube is less than this, the cube can be picked up
    public GameObject pickUpUI; // << The UI Text saying 'Press E to pick up the item'
    public DCP_TPD_PlayerInventory invScript; // << The player's inventory script to add/remove cubes from

    //An array remembering the origional positions of the cubes so they can be reset
    Vector3[] startCubePositions;

    //This method is called by the 'CallAwakeOnDisabledObjects' gameObject at the very start of the game
    //We could not use Awake as this object is disabled at the Start of the game so Awake would not be run
    public void AwakeMethod ()
    {
        //Writes the positions of the cubes to the startCubePositions array
        startCubePositions = new Vector3[cubes.Length];
        for (int i = 0; i < cubes.Length; i++)
        {
            startCubePositions[i] = cubes[i].gameObject.transform.position;
        }
    }

    void Update ()
    {
        bool canPickUp = false;
        foreach (DCP_TPD_Cube cube in cubes)
        {
            //Finds the distance between the player and the centre of each cube
            float dist = Vector3.Distance(player.transform.position, cube.centre.transform.position);
            if ((dist < pickUpRange) && (cube.gameObject.activeSelf == true))
            {
                //If the distance is low enough the cube can be picked up
                canPickUp = true;
            }
        }
        pickUpUI.gameObject.SetActive(canPickUp); // << Shows the UI Text if a cube can be picked up
        if (canPickUp == true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                //If the E key is pressed when a cube can be picked up
                foreach (DCP_TPD_Cube cube in cubes)
                {
                    float dist = Vector3.Distance(player.transform.position, cube.centre.transform.position);
                    if ((dist < pickUpRange) && (cube.gameObject.activeSelf == true))
                    {
                        //Pick up all of the cubes which can be picked up.
                        invScript.AddItem(cube.cubeType); // << Add the cube to the inventory script
                        cube.gameObject.SetActive(false); // << Disable the cube object so it cannot be seen
                    }
                }
            }
        }
    }

    //This method is called when the UI Inventory buttons are pressed to drop a cube
    public void DropCube (int cubeNum)
    {
        if (invScript.items.Count > cubeNum) // << Checks the parameter value is valid
        {
            DCP_TPD_PlayerInventory.Cube cubeToDrop = invScript.items[cubeNum]; // << Gets the cube color to drop
            invScript.DropItem(cubeToDrop); // << Tells the inventory script to remove the cube color from the inventory
            if (cubeToDrop == DCP_TPD_PlayerInventory.Cube.Red)
            {
                //If the cube was red move it to the player's position and enable it
                cubes[0].transform.position = player.transform.position;
                cubes[0].gameObject.SetActive(true);
            }
            //Do the same with the other cube colors
            if (cubeToDrop == DCP_TPD_PlayerInventory.Cube.Blue)
            {
                cubes[1].transform.position = player.transform.position;
                cubes[1].gameObject.SetActive(true);
            }
            if (cubeToDrop == DCP_TPD_PlayerInventory.Cube.Green)
            {
                cubes[2].transform.position = player.transform.position;
                cubes[2].gameObject.SetActive(true);
            }
            if (cubeToDrop == DCP_TPD_PlayerInventory.Cube.Yellow)
            {
                cubes[3].transform.position = player.transform.position;
                cubes[3].gameObject.SetActive(true);
            }
            if (cubeToDrop == DCP_TPD_PlayerInventory.Cube.Pink)
            {
                cubes[4].transform.position = player.transform.position;
                cubes[4].gameObject.SetActive(true);
            }
        }
    }

    public void ResetCubes ()
    {
        //This method moves all of the cubes back to their origional positions and enables them

        for (int i = 0; i < cubes.Length; i++)
        {
            cubes[i].gameObject.transform.position = startCubePositions[i];
            cubes[i].gameObject.SetActive(true);
        }
        invScript.DropAll(); // << and remove all of the cubes from the player inventory as they are active in the scene
    }
}
