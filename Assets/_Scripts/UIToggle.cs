using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIToggle : MonoBehaviour {

	[SerializeField]
	private string key;
	public bool value;

	public Settings settings;
	public Image myImg;

	void Start(){

		settings = Settings.GetInstance();
		myImg = GetComponent <Image> ();

		Debug.Log (myImg);

		value = PlayerPrefsManager.GetToggleIsOn (key);
		myImg.sprite = value ? settings.toggleOn : settings.toggleOff;

	}

	public void Toggle(){

		value = settings.Toggle (key);
		myImg.sprite = value ? settings.toggleOn : settings.toggleOff;

	}

}
