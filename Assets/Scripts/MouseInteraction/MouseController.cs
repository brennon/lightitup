using UnityEngine;
using System.Collections;
using System;
using System.IO;



 
public class MouseController : MonoBehaviour
{
	public static GameObject clickedGmObj = null;
	public static bool MouseMode = true;
	public bool RightHandUser = true;
	public bool clickedGmObjAcquired = false;
	
	
	public string OnMouseTranslation = "OnMouseTranslation";
	public string OnMouseMode = "OnMouseMode";
	public string OnMouseSelected = "OnMouseSelected";
	public string OnIntensity = "OnIntensity";
	public string OnMouseTranslationZ = "OnMouseTranslationZ";
	public string SelectedMode = "SelectedMode";
	public string LightResetPos ="LightResetPos";
	public string DeselectedMode = "DeselectedMode";
	float time =0.0f;
	
	public static int mouseMode;
	private bool mouseSelected;
	private float time_delay;
	private bool lightSelect = false;
	private float DELAY_CONST = 0.2f;
	
	public static  int occlision;
 
    void Update()
    {
		
        if(MouseMode)
			Mouse_Interaction(RightHandUser);   
    
    }
	
	void OnLeapMode(int mode)
	{
		mouseMode = mode;
	}
	
	void OnSetLeapLight (GameObject gamObj) {
		clickedGmObj = gamObj;
	}
 
    GameObject GetClickedGameObject()
    {
        // Builds a ray from camera point of view to the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
 
        // Casts the ray and get the first game object hit
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
		{
			//return hit.transform.gameObject;
			if(hit.transform.FindChild("mesh").transform.gameObject != null)
				return hit.transform.gameObject;
			else return null;
		}
        else
		{
			return null;
		}
    }
	
	 void DeSelected()
	{
			GameObject.Find("/Scene/SpotLight/SpotLight-1").SendMessage(DeselectedMode,null, SendMessageOptions.DontRequireReceiver);
			GameObject.Find("/Scene/SpotLight/SpotLight-1").SendMessage(OnMouseSelected,false, SendMessageOptions.DontRequireReceiver);
			GameObject.Find("/Scene/SpotLight/SpotLight-2").SendMessage(DeselectedMode,null, SendMessageOptions.DontRequireReceiver);
			GameObject.Find("/Scene/SpotLight/SpotLight-2").SendMessage(OnMouseSelected,false, SendMessageOptions.DontRequireReceiver);
			GameObject.Find("/Scene/SpotLight/SpotLight-3").SendMessage(DeselectedMode,null, SendMessageOptions.DontRequireReceiver);
			GameObject.Find("/Scene/SpotLight/SpotLight-3").SendMessage(OnMouseSelected,false, SendMessageOptions.DontRequireReceiver);
	}
	 
	
	void Mouse_Interaction(bool RightHandMode)
	{
		int rightHand = 0;
		if(RightHandMode == false)
			rightHand = 1;
		
        if ( Input.GetMouseButtonDown(rightHand))
		{	
			time_delay = Time.time;
			if(lightSelect== true && clickedGmObj != null)
			{
							 
				clickedGmObj = GetClickedGameObject();
				clickedGmObj.SendMessage(OnMouseSelected, true,SendMessageOptions.DontRequireReceiver);
				
				clickedGmObjAcquired = true;
				mouseMode = 1;
				mouseSelected = true;
				return;
			}
			
			occlision = -1;
			clickedGmObj = GetClickedGameObject();
			clickedGmObj.SendMessage("TranslationReset", true,SendMessageOptions.DontRequireReceiver);
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
			float delay = Time.time-time_delay;
			print (delay);
			if(delay<DELAY_CONST)
				lightSelect = true;
			else
				lightSelect = false;
			
			DeSelected();
			if(occlision >0)
			{
				if(clickedGmObj != null)
					clickedGmObj.SendMessage(LightResetPos, occlision,SendMessageOptions.DontRequireReceiver);
				
			}
			if (clickedGmObj != null)
			{
				clickedGmObj.SendMessage(OnMouseMode,3,SendMessageOptions.DontRequireReceiver);
				mouseMode = 3;
				mouseSelected =false;
				print ("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
			}
			
			
		}
		
		else if ( Input.GetMouseButtonDown(1-rightHand))
		{
			clickedGmObj = GetClickedGameObject();
			if (clickedGmObj != null)
			{
				clickedGmObj.SendMessage(OnMouseMode,2,SendMessageOptions.DontRequireReceiver);
				clickedGmObj.SendMessage(OnMouseSelected, true,SendMessageOptions.DontRequireReceiver);
				clickedGmObjAcquired = true;
				mouseMode = 2;
				mouseSelected = true;
			}
			
		}
		else if(Input.GetMouseButtonUp(1-rightHand))
		{
			DeSelected();
			if (clickedGmObj != null)
			{
				clickedGmObj.SendMessage(OnMouseMode,3,SendMessageOptions.DontRequireReceiver);
				mouseMode = 3;
				mouseSelected =false;
			}
		}
		else if(clickedGmObj != null )
		{
			clickedGmObj.SendMessage(SelectedMode,null,SendMessageOptions.DontRequireReceiver);
			
		}
		
	
		if(mouseSelected)
		{
			if(mouseMode ==1)
			{
				print ("hello world~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
				float delta = Input.GetAxis("Mouse ScrollWheel");
				if (clickedGmObj != null)
            		clickedGmObj.SendMessage(OnMouseTranslationZ, delta, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			if(mouseMode ==3)
			{
				float delta = Input.GetAxis("Mouse ScrollWheel");
				if (clickedGmObj != null)
	            	clickedGmObj.SendMessage(OnIntensity, delta, SendMessageOptions.DontRequireReceiver);
			}
			
		}
	}
	
	
	Vector3 GetRotation(GameObject obj)
	{
		Vector3 rotation = new Vector3(0,0,0);
		foreach (Transform child in obj.transform)
		{
			if(child.gameObject.name =="mesh")
			{
				rotation.x = child.rotation.eulerAngles.x;
				rotation.y = child.rotation.eulerAngles.y;
				rotation.x = child.rotation.eulerAngles.z;
				break;
			}
		}
		return rotation;
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
		obj = GameObject.Find("/Scene/SpotLight/SpotLight-1");
		if(obj !=null)
		{
			output = obj.name;
			writer.WriteLine(output);
			output ="Position:	X:	"+obj.transform.position.x+"	Y:	"+obj.transform.position.y+"	Z:	"+obj.transform.position.z;
			writer.WriteLine(output);
			Vector3 rot = GetRotation(obj);
			output ="Rotation:	X:	"+rot.x+"	Y:	"+rot.y+"	Z:	"+rot.z;
			writer.WriteLine(output);
			output= "Intensity:	"+ GetIntensity(obj);
			writer.WriteLine(output);
		}
		writer.WriteLine("\n");
		obj = GameObject.Find("/Scene/SpotLight/SpotLight-2");
		if(obj !=null)
		{
			output = obj.name;
			writer.WriteLine(output);
			output ="Position:	X:	"+obj.transform.position.x+"	Y:	"+obj.transform.position.y+"	Z:	"+obj.transform.position.z;
			writer.WriteLine(output);
			Vector3 rot = GetRotation(obj);
			output ="Rotation:	X:	"+rot.x+"	Y:	"+rot.y+"	Z:	"+rot.z;
			writer.WriteLine(output);
			output= "Intensity:	"+ GetIntensity(obj);
			writer.WriteLine(output);
		}
		writer.WriteLine("\n");
		obj = GameObject.Find("/Scene/SpotLight/SpotLight-3");
		if(obj !=null)
		{
			output = obj.name;
			writer.WriteLine(output);
			output ="Position:	X:	"+obj.transform.position.x+"	Y:	"+obj.transform.position.y+"	Z:	"+obj.transform.position.z;
			writer.WriteLine(output);
			Vector3 rot = GetRotation(obj);
			output ="Rotation:	X:	"+rot.x+"	Y:	"+rot.y+"	Z:	"+rot.z;
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
