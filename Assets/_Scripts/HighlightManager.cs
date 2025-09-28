using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightManager : MonoBehaviour {

	#region singleton

	//ensure that there is only one of these in the scene

	private static HighlightManager instance;

	void Awake(){

		if (instance == null) {
			instance = this;
		} else {
			Destroy (gameObject);
		}
	}

	public static HighlightManager GetInstance(){
		return instance;
	}

	#endregion

	public GameObject highlightPrefab;
	private List <GameObject> highlights;

	void Start(){
		highlights = new List<GameObject> ();
	}

	//creates an object pool out of "highlights" List
	GameObject GetHighlightObject(){

		//predicate looks for an inactive highlight in the obj pool
		// g will be the first object that matches the condition
		GameObject go = highlights.Find (g => !g.activeSelf);

		//if no inactive objs in pool then make one
		if (go == null) {
			go = Instantiate (highlightPrefab);
			go.transform.SetParent (transform.Find ("Highlights"));
			highlights.Add (go);
		}

		return go;

	}

	public void ShowLegalMoves(bool[,] legalMoves){

		for (int i = 0; i < 9; i++) {
			for (int j = 0; j < 9; j++) {
				if (legalMoves[i,j]){
					GameObject go = GetHighlightObject ();
					go.SetActive (true);
					go.transform.position = new Vector3 (i + 0.5f, 0, j + 0.5f);
				} 
			}
		}
	}

	public void HideMoves(){
		foreach (GameObject go in highlights) {
			if (go.activeSelf) {
				go.SetActive (false);
			}
		}
	}

}
