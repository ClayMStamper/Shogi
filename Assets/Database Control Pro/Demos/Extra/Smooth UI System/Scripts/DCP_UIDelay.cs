using UnityEngine;
using System.Collections;

public class DCP_UIDelay : MonoBehaviour {

    public bool runOnStart = false;
    public bool onlyOnce = false;
    public float delay = 1.0f;
    public UnityEngine.UI.Button.ButtonClickedEvent executeEvents;

    private bool hasRun = false;

    void Start()
    {
        if (runOnStart == true)
        {
            StartDelay();
        }
    }

    public void StartDelay ()
    {
        if ((onlyOnce == false) || (hasRun == false))
        {
            StartCoroutine(Move());
            hasRun = true;
        }
    }

    IEnumerator Move()
    {
        yield return new WaitForSeconds(delay);
        executeEvents.Invoke();
    }


}
