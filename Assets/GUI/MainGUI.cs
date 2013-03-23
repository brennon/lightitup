using UnityEngine;
using System.Collections;

public class MainGUI : MonoBehaviour {
	
	public string idString = "Enter ID";

	void OnGUI () {
		// Make a background box
		// GUI.Box(new Rect(10,10,100,90), "Loader Menu");
		
		idString = GUI.TextArea(new Rect(100, 100, 200, 100), idString, 5);
		Debug.Log(idString);

		// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
		if(GUI.Button(new Rect(20,40,80,20), "Training")) {
			Application.LoadLevel("Training");
		}

		// Make the second button.
		if(GUI.Button(new Rect(20,70,80,20), "Begin Experiment")) {
			Application.LoadLevel("UnitySandbox");
		}
	}
}