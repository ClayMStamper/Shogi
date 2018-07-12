using UnityEngine;
using System.Collections;
using DatabaseControl; //        << Reference to namespace needed

public class DCP_Demos_SpaceBarRunCS : MonoBehaviour {

    //Variables which are set in the Inspector
    public string databaseName = "Database Name";
    public string csName = "Sequence Name";
    public string[] inputValues;

    void Update()
    {
        //Check if the Space Bar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Runs IEnumerator to run sequence
            StartCoroutine(RunSequence());

            Debug.Log("Running Sequence ...");
        }
    }

    IEnumerator RunSequence() //   << This must be an IEnumerator as it will cause a delay
    {
        //Run the Command Sequence providing the Database Name, Command Sequence Name and the string[] for the input variable's values
        IEnumerator e = DCP.RunCS(databaseName, csName, inputValues); // The inputValues must match up with the input variables of the Command Sequence which can be seen on the start node of the sequence in the Sequencer Window. They must be in the same order.

        //You could use:
        //IEnumerator e = DCP.RunCS(databaseName, csName);
        //if your sequence doesn't have any input variables

        //Wait for the sequence to finish
        while (e.MoveNext()) {
			yield return e.Current;
		}
		string returnText = e.Current as string; //retrieve the result

        //Show the result in the Console
        Debug.Log("Result: " + returnText);
    }
}
