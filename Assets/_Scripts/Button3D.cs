using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Button3D : MonoBehaviour {

	public UnityEvent function;

	bool beingClicked;

	public void OnClick(){

		Debug.Log (name + " was clicked");

		ToggleBeingClicked (true);

	}

	void ToggleBeingClicked(bool beingClicked){

		this.beingClicked = beingClicked;

		Debug.Log ("Toggling being clickd to " + beingClicked);

		Color color = GetComponent <SpriteRenderer> ().color;

		if (beingClicked) {
			color.a = 0.7f;
			StartCoroutine (HoldDown ());
		} else {
			color.a = 1f;
		}

		GetComponent <SpriteRenderer> ().color = color;

	}

	void DoEvent(){

		Debug.Log ("doing event");

		function.Invoke ();

	}

	IEnumerator HoldDown(){

		while (beingClicked) {

			Debug.Log ("Holding down: " + name);

			if (Input.GetMouseButtonUp (0)) {

				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

				if (Physics.Raycast (ray, out hit, 100f)) {

					Debug.Log (hit.transform);

					if (hit.transform == this.transform) {

						Debug.Log ("Mouse is still on " + name + " on release");

						DoEvent ();

					}

				} 

				ToggleBeingClicked (false);
				
			} 

			yield return null;

		}

	}

}
