using UnityEngine;
using System.Collections;

public class TrainingGUI : MonoBehaviour {	
	void OnGUI() {
		ExperimentManager instance = ExperimentManager.instance;
		
		// Render 'Begin Trial' button
		if(GUI.Button(new Rect(Screen.width - 110,Screen.height - 30,100,20), "Begin Trials")) {
			instance.AdvanceLevel();
		}
	}
}
