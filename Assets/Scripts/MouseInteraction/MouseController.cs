using UnityEngine;
using System.Collections;
using System;
using System.IO;



 
public class MouseController : MonoBehaviour
{
	GameObject clickedGmObj = null;
	public bool MouseMode = true;
	public bool RightHandUser = true;
	public bool clickedGmObjAcquired = false;
	
	
	public string OnMouseTranslation = "OnMouseTranslation";
	public string OnMouseMode = "OnMouseMode";
	public string OnMouseSelected = "OnMouseSelected";
	public string OnIntensity = "OnIntensity";
	public string OnMouseTranslationZ = "OnMouseTranslationZ";
	float time =0.0f;
	
	private int mouseMode;
	private bool mouseSelected;
 
    void Update()
    {
		
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
		{
			return hit.transform.gameObject;
		}
        else
		{
			return null;
		}
    }
	
	
	void Mouse_Interaction(bool RightHandMode)
	{
		int rightHand = 0;
		if(RightHandMode == false)
			rightHand = 1;
		
        if ( Input.GetMouseButtonDown(rightHand))
		{
			 clickedGmObj = GetClickedGameObject();
			if (clickedGmObj != null)
			{
				clickedGmObj.SendMessage(OnMouseMode,1,SendMessageOptions.DontRequireReceiver);
				clickedGmObj.SendMessage(OnMouseSelected, true,SendMessageOptions.DontRequireReceiver);
				clickedGmObjAcquired = true;
				mouseMode = 1;
				mouseSelected = true;
			}
		}
		else if(Input.GetMouseButtonUp(rightHand))
		{
			
		 
			 if (clickedGmObj != null)
				clickedGmObj.SendMessage(OnMouseSelected, false,SendMessageOptions.DontRequireReceiver);
			else
			{
				clickedGmObj = GetClickedGameObject();
				if (clickedGmObj != null)
					clickedGmObj.SendMessage(OnMouseSelected, false,SendMessageOptions.DontRequireReceiver);	
			}
			mouseSelected = false;
			
		}
		
		if ( Input.GetMouseButtonDown(1-rightHand))
		{
			clickedGmObj = GetClickedGameObject();
			if (clickedGmObj != null)
			{
				clickedGmObj.SendMessage(OnMouseMode,2,SendMessageOptions.DontRequireReceiver);
				clickedGmObj.SendMessage(OnMouseSelected, true,SendMessageOptions.DontRequireReceiver);
				clickedGmObjAcquired = true;
				mouseMode = 1;
				mouseSelected = true;
			}
			
		}
		else if(Input.GetMouseButtonUp(1-rightHand))
		{
			if (clickedGmObj != null)
				clickedGmObj.SendMessage(OnMouseSelected, false,SendMessageOptions.DontRequireReceiver);
			else
			{
				clickedGmObj = GetClickedGameObject();
				if (clickedGmObj != null)
					clickedGmObj.SendMessage(OnMouseSelected, false,SendMessageOptions.DontRequireReceiver);	
			}
			mouseSelected = false;
		}
		
		if(mouseSelected)
		{
			if(mouseMode ==1)
			{
				float delta = Input.GetAxis("Mouse ScrollWheel");
				if (clickedGmObj != null)
            		clickedGmObj.SendMessage(OnMouseTranslationZ, delta, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			float delta = Input.GetAxis("Mouse ScrollWheel");
			if (clickedGmObj != null)
            	clickedGmObj.SendMessage(OnIntensity, delta, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	
	void SaveParameters(string name)
	{
		
		StreamWriter writer;
		writer  = new StreamWriter("data.txt");
		
		GameObject obj;
		String output=null;
		output = "UserName:	"+name+"\n";
		writer.WriteLine(output);
		output = "TotalTimer:	"+time+"s\n";
		writer.WriteLine(output);
		obj = GameObject.Find("/SpotLight/SpotLight-1");
		if(obj !=null)
		{
			output = obj.name;
			writer.WriteLine(output);
			output ="Position:	X:	"+obj.transform.position.x+"	Y:	"+obj.transform.position.y+"	Z:	"+obj.transform.position.z;
			writer.WriteLine(output);
			output ="Rotation:	X:	"+obj.transform.rotation.eulerAngles.x+"	Y:	"+obj.transform.rotation.eulerAngles.y+"	Z:	"+obj.transform.rotation.eulerAngles.z;
			writer.WriteLine(output);
			output= "Intensity:	"+ GetIntensity(obj);
			writer.WriteLine(output);
		}
		writer.WriteLine("\n");
		obj = GameObject.Find("/SpotLight/SpotLight-2");
		if(obj !=null)
		{
			output = obj.name;
			writer.WriteLine(output);
			output ="Position:	X:	"+obj.transform.position.x+"	Y:	"+obj.transform.position.y+"	Z:	"+obj.transform.position.z;
			writer.WriteLine(output);
			output ="Rotation:	X:	"+obj.transform.rotation.eulerAngles.x+"	Y:	"+obj.transform.rotation.eulerAngles.y+"	Z:	"+obj.transform.rotation.eulerAngles.z;
			writer.WriteLine(output);
			output= "Intensity:	"+ GetIntensity(obj);
			writer.WriteLine(output);
		}
		writer.WriteLine("\n");
		obj = GameObject.Find("/SpotLight/SpotLight-3");
		if(obj !=null)
		{
			output = obj.name;
			writer.WriteLine(output);
			output ="Position:	X:	"+obj.transform.position.x+"	Y:	"+obj.transform.position.y+"	Z:	"+obj.transform.position.z;
			writer.WriteLine(output);
			output ="Rotation:	X:	"+obj.transform.rotation.eulerAngles.x+"	Y:	"+obj.transform.rotation.eulerAngles.y+"	Z:	"+obj.transform.rotation.eulerAngles.z;
			writer.WriteLine(output);
			output= "Intensity:	"+ GetIntensity(obj);
			writer.WriteLine(output);
		}
		
		writer.Flush();
		writer.Close();
	}
	
	float GetIntensity(GameObject obj)
	{
		Light []  children;
		children = obj.GetComponentsInChildren<Light>();
        foreach (Light child in children) {
            if(child.name =="light")
			{
				return child.intensity;
			}
        }
		return -1.0f;
	}
}
