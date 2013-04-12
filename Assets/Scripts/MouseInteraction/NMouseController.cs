using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class NMouseController : MonoBehaviour {
	
	public int taskMode; // This is the mode being set in each trial by the ExperimentManager
	public static GameObject clickedGmObj = null;
	public static bool MouseMode = true;
	public bool RightHandUser = true;
	

	public string LightName = "NewSpotLight";
	
	public int InteractionMode;
	private bool lightSelect = false;
	private float DELAY_CONST = 0.2f;
	 
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		  if(MouseMode)
			Mouse_Interaction(RightHandUser);
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
	
	
	void Mouse_Interaction(bool RightHandMode)
	{
		int rightHand = 0;
		if(RightHandMode == false)
			rightHand = 1;
		
		//left button down
		if (Input.GetMouseButtonDown(rightHand))
		{
			clickedGmObj = GetClickedGameObject();
			
			if(clickedGmObj != null)
			{ 
				print (clickedGmObj.name);
				if(clickedGmObj.name.CompareTo(LightName) ==0)
				{
					clickedGmObj.SendMessage("OnMouseMode",1,SendMessageOptions.DontRequireReceiver);//translation mode
					InteractionMode = 1;
				}	
			}
		}
		
		else if(Input.GetMouseButtonUp(rightHand))
		{
			if(clickedGmObj != null)
			{
				clickedGmObj.SendMessage("OnMouseMode",-1,SendMessageOptions.DontRequireReceiver);//mouse deselected
				InteractionMode = -1;
			}
		}
		
		else if(Input.GetMouseButtonDown(1-rightHand))
		{
			clickedGmObj = GetClickedGameObject();
			if(clickedGmObj != null)
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
			if(clickedGmObj != null)
			{
				clickedGmObj.SendMessage("OnMouseMode",-1,SendMessageOptions.DontRequireReceiver);//mouse deselected
				InteractionMode = -1;
			}
		}
		
		
		
		
		if(InteractionMode ==1)
		{
			float delta = Input.GetAxis("Mouse ScrollWheel");
			if (clickedGmObj != null)
        		clickedGmObj.SendMessage("TranslationZ", delta, SendMessageOptions.DontRequireReceiver);
		}
		else if(InteractionMode ==2)
		{
			float delta = Input.GetAxis("Mouse ScrollWheel");
			if (clickedGmObj != null)
        		clickedGmObj.SendMessage("SetIntensity", delta, SendMessageOptions.DontRequireReceiver);
		}
		
		
		////////////////////////////////////////////////////////////////////////////////////////////////////////
		if (Input.GetKeyDown(KeyCode.A)) 
		{
			if(clickedGmObj!=null)
				clickedGmObj.SendMessage("LightReset",null,SendMessageOptions.DontRequireReceiver);//reset
				
		}
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
	
	}

	
}
