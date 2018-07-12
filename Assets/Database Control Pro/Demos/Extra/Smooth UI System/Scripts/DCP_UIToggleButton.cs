using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

[Serializable]
public class ChangeImage
{
    public UnityEngine.UI.Image image;
    public Sprite trueImage;
    public Sprite falseImage;
}

[Serializable]
public class ChangeText
{
    public UnityEngine.UI.Text text;
    public string trueText;
    public string falseText;
}

public class DCP_UIToggleButton : MonoBehaviour {

	public bool toggleState = true;
    [SerializeField]
    public ChangeImage[] imagesToChange;
    [SerializeField]
    public ChangeText[] textToChange;
    public UnityEngine.UI.Button.ButtonClickedEvent buttonTrue;
    public UnityEngine.UI.Button.ButtonClickedEvent buttonFalse;
    public bool keyboardShortcut;
    public KeyCode keyCodeShortcut;
    public bool useSwitchMouseOutsideUI = false;

    void Start ()
    {
        if (toggleState == true)
        {
            foreach (ChangeImage im in imagesToChange)
            {
                im.image.sprite = im.trueImage;
            }
            foreach (ChangeText te in textToChange)
            {
                te.text.text = te.trueText;
            }
        }
        else
        {
            foreach (ChangeImage im in imagesToChange)
            {
                im.image.sprite = im.falseImage;
            }
            foreach (ChangeText te in textToChange)
            {
                te.text.text = te.falseText;
            }
        }
    }

    void Update ()
    {
        if (keyboardShortcut == true)
        {
            if (Input.GetKeyDown(keyCodeShortcut))
            {
                ButtonPressed();
            }
        }
        if (useSwitchMouseOutsideUI == true)
        {
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (toggleState == false)
                    {
                        ButtonPressed();
                    }
                }
            }
        }
    }

    void ToggleSwitch ()
    {
        toggleState = !toggleState;
        if (toggleState == true)
        {
            foreach (ChangeImage im in imagesToChange)
            {
                im.image.sprite = im.trueImage;
            }
            foreach (ChangeText te in textToChange)
            {
                te.text.text = te.trueText;
            }
        } else
        {
            foreach (ChangeImage im in imagesToChange)
            {
                im.image.sprite = im.falseImage;
            }
            foreach (ChangeText te in textToChange)
            {
                te.text.text = te.falseText;
            }
        }
    }

    public void ButtonPressed ()
    {
        if (toggleState)
        {
            buttonTrue.Invoke();
        } else
        {
            buttonFalse.Invoke();
        }
        ToggleSwitch();
    }

    public void ForceTrue ()
    {
        if (toggleState == false)
        {
            buttonFalse.Invoke();
            ToggleSwitch();
        }
    }

    public void ForceFalse()
    {
        if (toggleState)
        {
            buttonTrue.Invoke();
            ToggleSwitch();
        }
    }

}
