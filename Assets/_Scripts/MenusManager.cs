using System.Collections;
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

    List<GameObject> menus = new List<GameObject>();
    Animator anim;

    [SerializeField]
    int currentMenu = 0;

    [Range(0, 1f)]
    float scrollSpeed;

    private void Start()
    {

        anim = transform.parent.gameObject.GetComponent<Animator>();

        GetMenus();
        SetMenu();

    }

    private void GetMenus(){
        
        menus = new List<GameObject>();

        foreach (Transform child in transform){
            menus.Add(child.gameObject);
        }

    }

    private void SetMenu(){

        for (int i = 0; i < menus.Count; i++){

            if (i == currentMenu){
                menus[i].SetActive(true);
            } else {
                menus[i].SetActive(false);
            }
        }

    }

    private void SetMenu(int i){
        currentMenu = i;
        SetMenu();
    }

    //incr menu index or reset to 0 if busts
    public void NextMenu(){
        currentMenu = ++currentMenu >= menus.Count ? 0 : currentMenu++;
        StartCoroutine(SwitchMenus());
    }

    public void BackMenu(){
        currentMenu = --currentMenu < 0 ? (menus.Count - 1) : currentMenu--;
        StartCoroutine(SwitchMenus());
    }

    IEnumerator SwitchMenus(){
        
        Debug.Log("Switching menus");

        anim.SetBool("isOpen", false);

        yield return new WaitForSeconds(scrollSpeed);

        anim.SetBool("isOpen", true);
        SetMenu();

    }

}
