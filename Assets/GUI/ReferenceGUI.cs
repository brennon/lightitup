using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class ReferenceGUI : MonoBehaviour {
	
	public int toolbarInt = 0;
	public int buttonCount;
	public string[] toolbarStrings;
	public int currentTrial;

	// Use this for initialization
	void Start () {
		buttonCount = ExperimentManager.instance.totalTrials;
		toolbarStrings = new string[buttonCount];
		for (int i = 0; i < buttonCount; i++) {
			toolbarStrings[i] = "" + i;
		}
		currentTrial = GetCurrentTrialNumber();
		toolbarInt = currentTrial;
	}
	
	// Update is called once per frame
	void Update () {
		if (toolbarInt != currentTrial) {
		//	toolbarInt = currentTrialNumber;
			Debug.Log ("loading level " + "Reference" + toolbarInt);
			Application.LoadLevel("Reference" + toolbarInt);
		}
	}
	
	private int GetCurrentTrialNumber() {
		string numRegex = "(\\d)";
		Match m = Regex.Match(Application.loadedLevelName, numRegex);		
		int capturedNumber;
		System.Int32.TryParse(m.Groups[0].Value, out capturedNumber);
		return capturedNumber;
	}
	
	void OnGUI () {
		// Add scene buttons
        toolbarInt = GUI.Toolbar(new Rect(10, Screen.height - 24 - 10, buttonCount * 30, 24), toolbarInt, toolbarStrings);
		
		// Add reset button
		if(GUI.Button(new Rect(Screen.width - 130, Screen.height - 34, 120, 24), "Reset")) {
			// Load the welcome scene
			Application.LoadLevel("Welcome");
		}
    }
}
