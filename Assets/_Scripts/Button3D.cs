using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Button3D : MonoBehaviour {

	public bool isActive = true;
	[SerializeField]
	private float clickToDragTolerance = 20.0f;

	public UnityEvent function;

	bool beingClicked;

	void Start(){
		ToggleShaded (!isActive);
	}

	MenusManager menusManager;

	public void OnClick(){

		if (menusManager == null) {
			menusManager = MenusManager.GetInstance ();
		}

		if (isActive) {
			ToggleIsClicked (true);
		}

	}

	void ToggleIsClicked(bool beingClicked){

		this.beingClicked = beingClicked;

		if (!name.Contains ("Profile") && !(name.Contains ("Settings"))) {
			ToggleShaded (beingClicked);
		} else {
			//do something about scrolls closing
		}

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

	//	Debug.Log ("doing event");

		if (name.Contains ("Profile") || (name.Contains ("Settings"))) {
			//clicked a scroll

			GetComponent <Animator> ().SetTrigger ("toggleOpen");
			menusManager.buttonPressed = transform;
			menusManager.toggleMenu ();

		} 

		function.Invoke ();

	}

	IEnumerator HoldDown(){

		Vector3 clickPos = Input.mousePosition;

		while (beingClicked) {

//			Debug.Log ("Holding down: " + name);

			if (Input.GetMouseButtonUp (0)) {

				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

				if (Physics.Raycast (ray, out hit, 100f)) {

					Debug.Log (hit.transform);

					if (hit.transform == this.transform) {

	//					Debug.Log ("Mouse is still on " + name + " on release");

						if ((Input.mousePosition - clickPos).magnitude > clickToDragTolerance){
							ToggleIsClicked (false);
							break; //dragged mouse away
						}

						DoEvent ();

					}

				} 

				ToggleIsClicked (false);
				
			} 

			yield return null;

		}

	}

	public void  LoadLevel(string levelName){

		LevelManager levelManager = LevelManager.GetInstance ();

		levelManager.LoadLevel (levelName);

	}

	public void Connect(){
		MultiplayerManager.GetInstance ().Connect ();
	}

	public void Queue(){
		Matchmaking.GetInstance ().Queue ();
	}

}
