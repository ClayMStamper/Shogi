using UnityEngine;
using System.Collections;

public class DCP_TPD_Cube : MonoBehaviour {

    //This is attached to every cube and stores information about it

    public Transform centre; // << The centre of the cube, used to find distance to the player to determine if it can be picked up
    public DCP_TPD_PlayerInventory.Cube cubeType; // << The color of the cube

}
