using UnityEngine;
using System.Collections;

public class ExperimentManager : MonoBehaviour {
	
	// Enumerations for task, device, and handedness
	public enum Task {TranslationRotation, TranslationIntensity};
	public enum Device {Mouse, Leap};
	public enum Handedness {Left, Right, Unspecified};
	
	// Current task, device, and handedness settings
	public Device currentDevice;
	public Task currentTask;
	public Handedness handedness;	
	
	public int subjectID;
	public int currentTrial = -1;
	public int handedSelection = 8;
	public int tasksPerTrial = 4;
	public int totalTrials = 3;
	public int[,] trials = {{0,1,2,3},{4,5,6,7},{8,9,10,11}};
	public int[] trialList;
	
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
	
	void PrintTrials(int[,] trials) {
		string display = "trials: {";
		
		for (int i = 0; i < totalTrials; i++) {
			display += "{";
			for (int j = 0; j < tasksPerTrial; j++) {
				display += trials[i,j];
				if (tasksPerTrial - j > 1)
					display += ",";
			}
			display += "}";
			if (totalTrials - i > 1)
				display += ",";
		}
		
		display += "}";
		
		Debug.Log (display);
	}
	
	void Start() {
		// Randomize trial order
		System.Random r = new System.Random();
		for (int i = totalTrials - 1; i > 0; i--) {
			int j = r.Next(i + 1);
			// Debug.Log("swapping trial " + i + " with trial " + j);
			for (int k = 0; k < tasksPerTrial; k++) {
				int temp = trials[i,k];
				trials[i,k] = trials[j,k];
				trials[j,k] = temp;
			}			
		}		
		
		// Randomize task order
		for (int i = 0; i < totalTrials; i++) {
			// Debug.Log ("randomizing trial " + i);
			for (int j = tasksPerTrial - 1; j > 0; j--) {				
				int k = r.Next (j + 1);
				// Debug.Log("swapping task " + j + " with task " + k);
				int temp = trials[i,j];
				trials[i,j] = trials[i,k];
				trials[i,k] = temp;
				// PrintTrials(trials);
			}
		}
		
		// Flatten ordering and save in trialList
		trialList = new int[totalTrials * tasksPerTrial];
		for (int i = 0; i < totalTrials; i++) {
			for (int j = 0; j < tasksPerTrial; j++) {
				int index = i * tasksPerTrial + j;
				trialList[index] = trials[i,j];
			}			
		}
	}
	
	public void AdvanceLevel() {
		currentTrial++;
		
		if (currentTrial < (totalTrials * tasksPerTrial)) {
			GetCurrentLightSettings();
			Application.LoadLevel("Leap_Project");
			// Debug.Log ("setting up trial: " + currentTrial);
			UpdateDeviceAndTask();
		} else if (currentTrial >= 12) {
			// Save data here
			Application.LoadLevel("End");
		}
	}
	
	// Update the current device and task
	private void UpdateDeviceAndTask() {
		UpdateDevice();
		UpdateTask();
	}
	
	// Task modes:
	// --------------------------|
	//              Mouse | Leap |
	// --------------------------|
	// Trans./Rot. |   0  |   2  |
	// Trans./Int. |   1  |   3  |
	// --------------------------|
	
	// Update the current device based on the current trial
	private void UpdateDevice() {
		if (trialList[currentTrial] % 2 == 0)
			currentTask = Task.TranslationRotation;
		else
			currentTask = Task.TranslationIntensity;
	}
	
	// Update the current task based on the current trial
	private void UpdateTask() {
		if (trialList[currentTrial] % tasksPerTrial < tasksPerTrial / 2)
			currentDevice = Device.Mouse;
		else
			currentDevice = Device.Leap;
	}

	private void SetupLevel(int newLevel) {		
		for (int i = 0; i < totalTrials; ++i) {
			if (i != newLevel) {
				string targetTag = "Trial" + i;
				GameObject[] toDeactivate = GameObject.FindGameObjectsWithTag(targetTag);
				foreach (GameObject obj in toDeactivate) {
					obj.SetActive(false);
				}
			}
		}
	}
	
	private void OnLevelWasLoaded (int newLevel) {
		if (Application.loadedLevelName == "Leap_Project" && currentTrial >= 0) {
			SetupLevel(trialList[currentTrial]/tasksPerTrial);
		}
	}
	
	private void GetCurrentLightSettings() {
		GameObject spotlight = GameObject.Find("SpotLight-1");
		GameObject light = GameObject.Find("SpotLight-1/light");
		Debug.Log ("position: " + spotlight.transform.localPosition);
		Debug.Log ("rotation: " + light.transform.localRotation);
	}
	
	private void Update() {
		CheckForKeyUps();
	}
	
	private void CheckForKeyUps() {
		bool changeLeap = false;
		if (Input.GetKeyUp(KeyCode.Alpha1)) {
			currentDevice = Device.Leap;
			currentTask = Task.TranslationRotation;
			changeLeap = true;
		} else if (Input.GetKeyUp(KeyCode.Alpha2)) {
			currentDevice = Device.Leap;
			currentTask = Task.TranslationIntensity;
			changeLeap = true;
		} else if (Input.GetKeyUp(KeyCode.Alpha3)) {
			currentDevice = Device.Mouse;
			currentTask = Task.TranslationRotation;
			changeLeap = true;
		} else if (Input.GetKeyUp(KeyCode.Alpha4)) {
			currentDevice = Device.Mouse;
			currentTask = Task.TranslationIntensity;
			changeLeap = true;
		}
		if (changeLeap) {
			int leapMode = currentTask == Task.TranslationRotation ? 0 : 1;
			LeapInput.ChangeMode(currentDevice == Device.Leap ? leapMode : -5);
			changeLeap = false;
		}
	}
}