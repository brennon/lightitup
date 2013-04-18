using UnityEngine;
using System.Collections;
using System;
using System.IO;


public class NMouseController : MonoBehaviour {
	
	public int taskMode; // This is the mode being set in each trial by the ExperimentManager
	public static GameObject clickedGmObj = null;
	//public static bool MouseMode = true;
	//public bool RightHandUser = true;
	//public  List<float> integers = new List<float>();
	

	public string LightName = "NewSpotLight";
	
	public static int InteractionMode;
	private bool lightSelect = false;
	private float DELAY_CONST = 0.2f;
	private GameObject preObj;
	private bool leftClicked = false;
	private bool doubleClicked = false;
	private bool rightClicked = false;
	 
	
	// Use this for initialization
	void Start () {

	
	}
	
	// Update is called once per frame
	void Update () {
		
		if(ExperimentManager.instance.currentDevice == ExperimentManager.Device.Mouse)
		{
			//Handedness
			bool RightHandUser = false;
			if(ExperimentManager.instance.handedness == ExperimentManager.Handedness.Right)
				RightHandUser = true;
			else
				RightHandUser = false;
			
			if(ExperimentManager.instance.currentTask == ExperimentManager.Task.TranslationRotation)
				SpotLight.InteractionMode = 1;
			else if(ExperimentManager.instance.currentTask == ExperimentManager.Task.TranslationIntensity)
			{
				SpotLight.InteractionMode = 2;
			}
			
			Mouse_Interaction(RightHandUser);
		}
	}
	
	GameObject GetClickedGameObject()
    {
        // Builds a ray from camera point of view to the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
 
        // Casts the ray and get the first game object hit
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
			return hit.transform.gameObject;
        else
			return null;
    }
	
	public static void SetGameObj (GameObject light) {
		print ("OBJ: "+light);
		clickedGmObj = light;
	}
	
	void Mouse_Interaction(bool RightHandMode)
	{
		int rightHand = 0;
		if(RightHandMode == false)
			rightHand = 1;
		
		//left button down
		if (Input.GetMouseButtonDown(rightHand))
		{
			if(Input.GetMouseButtonDown(1-rightHand))
			{
				clickedGmObj = GetClickedGameObject();
				if(clickedGmObj != false)
				{
					clickedGmObj.SendMessage("OnMouseMode",4,SendMessageOptions.DontRequireReceiver);//translation mode
					InteractionMode = 4;
				}
				return;
			}
			
			preObj = clickedGmObj;
			clickedGmObj = GetClickedGameObject();
			
			if(leftClicked == false)
				leftClicked = true;
			else if(doubleClicked == false)
			{
				doubleClicked = true;
				if(clickedGmObj != false)
				{
					clickedGmObj.SendMessage("OnMouseMode",4,SendMessageOptions.DontRequireReceiver);//translation mode
					InteractionMode = 4;
				}		
			}
			
			
			
			if(rightClicked == true && doubleClicked == false)
			{
				if(clickedGmObj != null)
				{
					clickedGmObj.SendMessage("OnMouseMode", 4, SendMessageOptions.DontRequireReceiver);
					doubleClicked = true;
					InteractionMode = 4;
				}
			}
			
			if(clickedGmObj != null && doubleClicked == false)
			{ 
				//print (clickedGmObj.name);
				if(clickedGmObj.name.CompareTo(LightName) ==0)
				{
					clickedGmObj.SendMessage("OnMouseMode",1,SendMessageOptions.DontRequireReceiver);//translation mode
					InteractionMode = 1;
				}
				else
				{
					if(preObj != null)
					{
						clickedGmObj.SendMessage("OnMouseMode",-1,SendMessageOptions.DontRequireReceiver);//translation mode
						InteractionMode = -1;
					}
				}
			}
			else if(clickedGmObj == null)
			{
				if(preObj != null)
				{
					preObj.SendMessage("OnMouseMode",-1,SendMessageOptions.DontRequireReceiver);//translation mode
					InteractionMode = -1;
				}	
			}
			 
		}
		
		else if(Input.GetMouseButtonUp(rightHand))
		{
			leftClicked = false;
			doubleClicked = false;
			rightClicked = false;
			if(clickedGmObj != null)
			{
				clickedGmObj.SendMessage("OnMouseMode",-1,SendMessageOptions.DontRequireReceiver);//mouse deselected
				InteractionMode = -1;
			}
		}
		
		else if(Input.GetMouseButtonDown(1-rightHand))
		{
			if(Input.GetMouseButtonDown(rightHand))
			{
				clickedGmObj = GetClickedGameObject();
				if(clickedGmObj != false)
				{
					clickedGmObj.SendMessage("OnMouseMode",4,SendMessageOptions.DontRequireReceiver);//translation mode
					InteractionMode = 4;
				}
				return;
			}
			
			if(leftClicked == true && doubleClicked == false)
			{
				if(clickedGmObj != null)
				{
					clickedGmObj.SendMessage("OnMouseMode", 4, SendMessageOptions.DontRequireReceiver);
					doubleClicked = true;
					InteractionMode = 4;
				}
			}
			
			if(rightClicked == false)
				rightClicked = true;
			else if(doubleClicked == false)
			{
				doubleClicked = true;
				if(clickedGmObj != false)
				{
					clickedGmObj.SendMessage("OnMouseMode",4,SendMessageOptions.DontRequireReceiver);//translation mode
					InteractionMode = 4;
				}		
			}
			
				clickedGmObj = GetClickedGameObject();
				if(clickedGmObj != null && doubleClicked == false)
				{ 
					if(clickedGmObj.name.CompareTo(LightName) ==0)
					{
						clickedGmObj.SendMessage("OnMouseMode",2,SendMessageOptions.DontRequireReceiver);//rotation mode
						InteractionMode = 2;
					}			
				}
			
		}
		
		else if(Input.GetMouseButtonUp(1-rightHand))
		{
			doubleClicked = false;
			leftClicked = false;
			rightClicked = false;
			
			if(clickedGmObj != null)
			{
				clickedGmObj.SendMessage("OnMouseMode",-1,SendMessageOptions.DontRequireReceiver);//mouse deselected
				InteractionMode = -1;
			}
		}
		/*
		if(InteractionMode ==1)
		{
			float delta = Input.GetAxis("Mouse ScrollWheel");
			if (clickedGmObj != null)
        		clickedGmObj.SendMessage("TranslationZ", delta, SendMessageOptions.DontRequireReceiver);
		}
		*/
		else if(SpotLight.InteractionMode ==2 && InteractionMode ==1)//Translation and intensity
		{
			float delta = Input.GetAxis("Mouse ScrollWheel");
			if (clickedGmObj != null)
        		clickedGmObj.SendMessage("SetIntensity", delta, SendMessageOptions.DontRequireReceiver);
		}
		
		
		////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		/*
		if(Input.GetKeyDown(KeyCode.Alpha0))
		{
			if(clickedGmObj!=null)
				clickedGmObj.SendMessage("SetTargetID",0,SendMessageOptions.DontRequireReceiver);//object orientaion;
		}
		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			if(clickedGmObj!=null)
				clickedGmObj.SendMessage("SetTargetID",1,SendMessageOptions.DontRequireReceiver);//object orientaion;
		}
		
		if(Input.GetKeyDown(KeyCode.Alpha2))
		{
			if(clickedGmObj!=null)
				clickedGmObj.SendMessage("SetTargetID",2,SendMessageOptions.DontRequireReceiver);//object orientaion;
		}
		
		if(Input.GetKeyDown(KeyCode.Alpha3))
		{
			if(clickedGmObj!=null)
				clickedGmObj.SendMessage("SetTargetID",3,SendMessageOptions.DontRequireReceiver);//object orientaion;
		}
		
		if(Input.GetKeyDown(KeyCode.Alpha4))
		{
			if(clickedGmObj!=null)
				clickedGmObj.SendMessage("PrintTime",null,SendMessageOptions.DontRequireReceiver);//object orientaion;
		}
		*/
	
	}

	
}
