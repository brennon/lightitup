using UnityEngine;
using System.Collections;

public class MainGUI : MonoBehaviour {
	
	public string subjectID = "ID";
	public int[] conditionList = new int[6];
	public int currentTrial = -1;
	
	void Start() {
		// Randomize trials
		int[,] conditions = {{0,1},{2,3},{4,5}};
		System.Random r = new System.Random();
		for (int i = 2; i > 0; i--) {
			int swapIndex = r.Next(i + 1);
			int currentLeft = conditions[swapIndex,0];
			int currentRight = conditions[swapIndex,1];
			conditions[swapIndex,0] = conditions[i,0];
			conditions[swapIndex,1] = conditions[i,1];
			conditions[i,0] = currentLeft;
			conditions[i,1] = currentRight;
		}
		
		// Randomize mouse/Leap order
		for (int i = 2; i >= 0; i--) {
			int swapIndex = r.Next(2);
			if (swapIndex == 0) {
				int current = conditions[i,1];
				conditions[i,1] = conditions[i,0];
				conditions[i,0] = current;
			}
		}
		
		// Save ordering to conditionList
		for (int i = 0; i < 3; i++) {
			conditionList[i*2] = conditions[i,0];
			conditionList[i*2+1] = conditions[i,1];
		}
		
		DontDestroyOnLoad(this);
	}

	void OnGUI() {
		// Store typed text in subjectID
		subjectID = GUI.TextArea(new Rect(Screen.width/2 - 75,Screen.height/2,100,20), subjectID, 5);
		
		// If this is the Welcome scene
		if (UnityEditor.EditorApplication.currentScene == "Assets/Scenes/Welcome.unity") {			
			GUIStyle labelStyle = new GUIStyle();
			labelStyle.alignment = TextAnchor.MiddleCenter;
			labelStyle.normal.textColor = Color.white;
			GUI.Label(new Rect(Screen.width/2, Screen.height/2 - 20, 0, 0), "Welcome! Please enter a subject ID and click 'Begin':",labelStyle);
			// Print ID entry widgets
			if(GUI.Button(new Rect(Screen.width/2 + 30,Screen.height/2,50,20), "Begin")) {
				Application.LoadLevel("Training");
			}
		}
		
		// If this is the Training scene
		if (UnityEditor.EditorApplication.currentScene == "Assets/Scenes/Welcome") {
		}
	}
	
	void AdvanceLevel() {
		if (currentTrial++ < 6) {
			if (conditionList[currentTrial] % 2 == 0) {
				Application.LoadLevel("Trial - Mouse");
			} else {
				Application.LoadLevel("Trial - Leap");
			}
		}
	}
}