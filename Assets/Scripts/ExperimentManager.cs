using UnityEngine;
using System.Collections;

public class ExperimentManager : MonoBehaviour {
	
	public string subjectID = "ID";
	public int[] conditionList = new int[12];
	public int currentTrial = -1;
	public int handedSelection = 0;
	
	// s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
    private static ExperimentManager s_Instance = null;
	
	// This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static ExperimentManager instance {
        get {
            if (s_Instance == null) {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first AManager object in the scene.
                s_Instance =  FindObjectOfType(typeof (ExperimentManager)) as ExperimentManager;
            }
 
            // If it is still null, create a new instance
            if (s_Instance == null) {
                GameObject obj = new GameObject("ExperimentManager");
                s_Instance = obj.AddComponent(typeof (ExperimentManager)) as ExperimentManager;
                Debug.Log ("Could not locate an ExperimentManager object. \n ExperimentManager was Generated Automatically.");
				DontDestroyOnLoad(s_Instance);
            }
            return s_Instance;
        }
    }
 
    // Ensure that the instance is destroyed when the game is stopped in the editor.
    void OnApplicationQuit() {
        s_Instance = null;
    }
	
	void Start() {
		// Randomize trials
		int[,] conditions = {{0,1},{2,3},{4,5},{6,7},{8,9},{10,11}};
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
	}
	
	public void AdvanceLevel() {
		currentTrial++;
		
		if (currentTrial < 12) {
			Application.LoadLevel("Leap_Project");
			SetupLevel(currentTrial/2);
		} else if (currentTrial >= 12) {
			// Save data here
			Application.LoadLevel("End");
		}
	}

	private void SetupLevel(int newLevel) {
		for (int i = 0; i < 6; ++i) {
			if (i != newLevel) {
				string targetTag = "Trial" + i;
				GameObject[] toDeactivate = GameObject.FindGameObjectsWithTag(targetTag);
				foreach (GameObject obj in toDeactivate) {
					Debug.Log ("deactivating " + obj);
					obj.SetActive(false);
					obj.activeInHierarchy = false;
				}
			}
		}
//		Debug.Log("setting up: " + newLevel);
//		string targetTag = "Trial" + newLevel;
//		Debug.Log ("activating objects with tag: " + targetTag);
//		GameObject[] toActivate = GameObject.FindGameObjectsWithTag(targetTag);
//		if (toActivate.Length == 0)
//			Debug.Log("no objects found");
//		foreach (GameObject obj in toActivate) {
//			Debug.Log("found object: " + obj);
//			Debug.Log("activeSelf: " + obj.activeSelf);
//			Debug.Log("activeInHierarchy: " + obj.activeInHierarchy);
//			obj.SetActive(true);
//			Debug.Log("activated " + obj);
//			Debug.Log("activeSelf: " + obj.activeSelf);
//			Debug.Log("activeInHierarchy: " + obj.activeInHierarchy);
//		}
//		
//		GameObject piano = GameObject.Find("Barrel");
//		Debug.Log ("barrel: " + piano);
	}
}