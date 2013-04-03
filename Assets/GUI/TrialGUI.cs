using UnityEngine;
using System.Collections;

public class TrialGUI : MonoBehaviour {
	void OnGUI() {
		ExperimentManager instance = ExperimentManager.instance;
		
		// Render 'Next Trial' button
		if(GUI.Button(new Rect(Screen.width - 110,Screen.height - 30,100,20), "Next")) {
			instance.AdvanceLevel();
		}
	}
}
