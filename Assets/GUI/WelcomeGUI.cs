using UnityEngine;
using System.Collections;

public class WelcomeGUI : MonoBehaviour {
	
	// Private members for GUI settings and entered data
	private string[] handStrings = new string[] {"Left", "Right"};
	private int handSelectionInt = -1;
	private bool validSettings = false;
	private string subjectID = "ID";
	private int numericSubjectID;
	
	// Run every update
	void OnGUI() {	
		// Style for text labels
		GUIStyle labelStyle = new GUIStyle();
		labelStyle.alignment = TextAnchor.MiddleCenter;
		labelStyle.normal.textColor = Color.white;
		
		// Add welcome label
		GUI.Label(new Rect(Screen.width/2, Screen.height/2 - 50, 0, 0), "Welcome! Please enter a subject ID:",labelStyle);
		subjectID = GUI.TextField(new Rect(Screen.width/2 - 50,Screen.height/2 - 30,100,20), subjectID, 5);
		
		// Add hand selection label
		GUI.Label(new Rect(Screen.width/2, Screen.height/2 + 20, 0, 0), "Select your handedness:",labelStyle);
		handSelectionInt = GUI.SelectionGrid(new Rect(Screen.width/2 - 75, Screen.height/2 + 40, 150, 30), (int) handSelectionInt, handStrings, 2);
		
		// Check that settings are valid
		if (validSettings) {
			// If 'Begin' button was pressed
			if(GUI.Button(new Rect(Screen.width/2 - 40,Screen.height/2 + 100,80,20), "Begin")) {
				// Send entered data to ExperimentManager
				PopulateManagerWithSettings();
				// Load the first scene
				Application.LoadLevel("Leap_Project");
			}
		}		
	}
	
	private void Update() {
		ValidateSelections();
	}
	
	// Validate entered data
	private void ValidateSelections() {
		validSettings = true;
		
		// A hand must be selected
		if (handSelectionInt != 0 && handSelectionInt != 1)
			validSettings = false;
		
		// ID must be numeric
		bool idIsNumeric = int.TryParse(subjectID.Trim(), out numericSubjectID);
		if (!idIsNumeric)
			validSettings = false;
	}
	
	// Send entered data to ExperimentManager
	private void PopulateManagerWithSettings() {
		ExperimentManager.instance.subjectID = numericSubjectID;
		ExperimentManager.instance.handedness = (ExperimentManager.Handedness) handSelectionInt;
	}
}
