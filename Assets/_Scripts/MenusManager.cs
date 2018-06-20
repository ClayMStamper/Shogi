using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenusManager : MonoBehaviour {

	[SerializeField]
	Sprite toglOn, toglOff;

	public void Toggle(Toggle togl){

		togl.image.sprite = (togl.isOn) ? toglOn : toglOff;

		Debug.Log (togl.image.sprite);

	}
}
