using UnityEngine;
using System.Collections;

public class DCP_UIOnStartEvents : MonoBehaviour {

    public UnityEngine.UI.Button.ButtonClickedEvent executeEvents;

    void Start () {
        executeEvents.Invoke();
    }
}
