using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCanvas : MonoBehaviour {

	void Update(){

		if (Input.GetMouseButtonDown (0)) {

			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

			if (Physics.Raycast (ray, out hit, 100f)){

				Debug.Log (hit.transform);

				if (hit.transform.GetComponent<Button3D> ()) {
					
					Button3D button = hit.transform.GetComponent<Button3D> ();
					button.OnClick ();

				}

			}

		}

	}
}
