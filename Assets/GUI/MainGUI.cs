using UnityEngine;
using System.Collections;

public class MainGUI : MonoBehaviour {
	
	public string subjectID = "Enter ID";
	public Rect subjectIDRect = new Rect();

	void OnGUI () {
		// Make a background box
		// GUI.Box(new Rect(10,10,100,90), "Loader Menu");
		
		subjectID = GUI.TextArea(subjectIDRect, subjectID, 5);

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