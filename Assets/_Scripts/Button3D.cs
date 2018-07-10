using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Button3D : MonoBehaviour {

	public bool isActive = true;

	public UnityEvent function;

	bool beingClicked;

	void Start(){
		ToggleShaded (!isActive);
	}

	public void OnClick(){

		if (isActive) {
			OnClicked (true);
		}

	}

	void OnClicked(bool beingClicked){

		this.beingClicked = beingClicked;
		ToggleShaded (beingClicked);

		if (beingClicked) {
			StartCoroutine (HoldDown ());
		} 

	}

	void ToggleShaded(bool shaded){

		Color color = GetComponent <SpriteRenderer> ().color;

		if (shaded) {
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

				OnClicked (false);
				
			} 

			yield return null;

		}

	}

}
