﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenusManager : MonoBehaviour {

#region singleton

	private static MenusManager instance;

	void Awake(){

		if (instance == null) {
			instance = this;
		} else {
			Destroy (gameObject);
		}

	//	DontDestroyOnLoad (gameObject);
	
	}

	public static MenusManager GetInstance(){
		return instance;
	}

#endregion

	[SerializeField]
	Transform canvas;

	[SerializeField]
	GameObject settingsContent = null, profileContent = null;

	[HideInInspector]
	public Transform buttonPressed;
	[HideInInspector]
	public GameObject openMenu;

	bool settingsOpen, profileOpen;

	void Start(){
		if (canvas == null) {
			try {
			canvas = WorldCanvas.GetInstance ().transform;
			} catch {
				//this scene doesnt need a canvas refernce
			}
		}
	}

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

	public void toggleMenu(){
			
		if (openMenu != null) { // toggle off
			//StartCoroutine (FadeOut (openMenu, 0.01f, true));
			Destroy (openMenu);
		} else { //toggle on

			switch (buttonPressed.name) {
			
			case "Profile Button":
				openMenu = Instantiate (profileContent, canvas);
				break;
			case "Settings Button":
				openMenu = Instantiate (settingsContent, canvas);
				break;
			default:
				Debug.LogError ("Button name: \"" + buttonPressed.name + "\" was not recognized");
				break;

			}

		}
	}

	public IEnumerator FadeOut (GameObject obj, float fadeSpeed, bool destroy){

		SpriteRenderer img = obj.GetComponent <SpriteRenderer> ();

		while (img.color.a > 0 && img != null) {

			Color newColor = img.color;
			newColor.a -= fadeSpeed * Time.deltaTime;

			img.color = newColor;

			yield return null;

			if (destroy) {
				Destroy (obj);
			} else { //reset
				newColor.a = 1;
				img.color = newColor;
			}

		}

	}
		
}
