using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIToggle : MonoBehaviour {

	[SerializeField]
	private string key;
	public bool value;

	Settings settings;

	void Awake(){

		settings = Settings.GetInstance();

		value = PlayerPrefsManager.GetToggleIsOn (key);
		GetComponent <Image> ().sprite = value ? settings.toggleOn : settings.toggleOff;

	}

	public void Toggle(){

		settings.Toggle (key, GetComponent <Image> ());

	}

}
