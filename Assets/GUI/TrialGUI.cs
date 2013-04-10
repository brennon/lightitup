using UnityEngine;
using System.Collections;

public class TrialGUI : MonoBehaviour {
	void OnGUI() {
		ExperimentManager instance = ExperimentManager.instance;
		
		// Render 'Next Trial' button
		if(GUI.Button(new Rect(Screen.width - 110,Screen.height - 30,100,20), "Next")) {
			instance.AdvanceLevel();
		}
		
		string deviceLabel = "Device: ";
		string taskLabel = "Task: ";
		
		if (instance.currentTask == ExperimentManager.Task.TranslationRotation)
			taskLabel += "Translation + Rotation";
		else
			taskLabel += "Translation + Intensity";
		
		if (instance.currentDevice == ExperimentManager.Device.Mouse)
			deviceLabel += "Mouse";
		else
			deviceLabel += "Leap";
		
		GUIStyle labelStyle = new GUIStyle();
		labelStyle.normal.textColor = Color.white;		
		labelStyle.alignment = TextAnchor.MiddleLeft;
		GUI.Label(new Rect(10, Screen.height - 40, 0, 0), deviceLabel,labelStyle);
		GUI.Label(new Rect(10, Screen.height - 20, 0, 0), taskLabel,labelStyle);
	}
}
