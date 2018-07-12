using UnityEngine;
using System.Collections;

public class DCP_CCD_DestroyParticles : MonoBehaviour {
	
	//This script isn't very important. It justs adds a lifetime to the gameObjects with the particle systems attached when they are instantiated

	public int lifetime = 5; // << The length of the gameObject's lifetime

	void Start () {
        StartCoroutine(DestroyObject());
	}
	
    IEnumerator DestroyObject ()
    {
	    yield return new WaitForSeconds(lifetime); // Wait for the end of the lifetime of the gameObject and destroy it
        Destroy(gameObject);
    }
}
