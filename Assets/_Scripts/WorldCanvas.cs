using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCanvas : MonoBehaviour {

	#region singleton

	private static WorldCanvas instance;

	void Awake(){

		if (instance == null) {
			instance = this;
		} else {
			Destroy (gameObject);
		}

	}

	public static WorldCanvas GetInstance(){
		return instance;
	}

	#endregion



}
