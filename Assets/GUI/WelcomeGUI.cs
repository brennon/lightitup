using UnityEngine;
using System.Collections;

public class WelcomeGUI : MonoBehaviour {	
	void OnGUI() {
		ExperimentManager instance = ExperimentManager.instance;
		
		// Store typed text in ExperimentManager
		instance.subjectID = GUI.TextField(new Rect(Screen.width/2 - 75,Screen.height/2,100,20), instance.subjectID, 5);
		
		GUIStyle labelStyle = new GUIStyle();
		labelStyle.alignment = TextAnchor.MiddleCenter;
		labelStyle.normal.textColor = Color.white;
		GUI.Label(new Rect(Screen.width/2, Screen.height/2 - 20, 0, 0), "Welcome! Please enter a subject ID and click 'Begin':",labelStyle);
		// Print ID entry widgets
		if(GUI.Button(new Rect(Screen.width/2 + 30,Screen.height/2,50,20), "Begin")) {
			Application.LoadLevel("Training");
		}
	}
}
