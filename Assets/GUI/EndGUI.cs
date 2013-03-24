using UnityEngine;
using System.Collections;

public class EndGUI : MonoBehaviour {	
	void OnGUI() {		
		GUIStyle labelStyle = new GUIStyle();
		labelStyle.alignment = TextAnchor.MiddleCenter;
		labelStyle.normal.textColor = Color.white;
		GUI.Label(new Rect(Screen.width/2, Screen.height/2 - 20, 0, 0), "Thank you for your participation!",labelStyle);
	}
}
