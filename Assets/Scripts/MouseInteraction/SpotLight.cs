using UnityEngine;
using System.Collections;
using System.Threading;
using System;
using System.IO;
using System.Collections.Generic;


public class SpotLight : MonoBehaviour {
	
	public int  MouseMode = -1;
	public static Vector3 targetPosition;
	public static int InteractionMode;
	
	//public string[] target = {"TargetCube-1","TargetCube-2","TargetCube-3"};
	//public int targetID = 0;
	
	public string LightName = "NewSpotLight";
	
	//public static List<float> time_span = new List<float>();
	//public static List<string> mode_list = new List<string> ();
	//public static int times= 0;
	public static double selectedTime;
	
	private float rotation_factor = 2.0f;
	private float translation_factor = 1f; 
	private Vector3 originia_position;
	private Quaternion original_rotation;
	private float   original_intensity;
	//private float   original_rotation = 0;
	// Use this for initialization
	private int beam_id;
	private static ContactPoint contact;
	private GameObject occlisionObj;
	
	//private static float begin_time;
	//private static float end_time;
	
	void Start () {
		
		originia_position = transform.position;
		original_rotation = transform.rotation;	
		original_intensity = GetIntensity(transform.gameObject);
		
		targetPosition = new Vector3(0,0,0);
		
		//begin_time  =  end_time = -1.0f;
		//begin_time = Time.time;
		
		targetPosition = ExperimentManager.instance.currentLightTarget;
	}
	
	// Update is called once per frame
	void Update () {
		if (ExperimentManager.instance.currentTrial == -1) {
			if (ExperimentManager.instance.currentTask == ExperimentManager.Task.TranslationIntensity) {
				targetPosition = ExperimentManager.instance.currentLightTarget;
			}
		}
		
		if(Input.GetKeyDown(KeyCode.A))
			LightReset();
		
		if(MouseMode>=0) // light is selected
		{
			selectedTime += Time.deltaTime;
			SelectedMode();
			
			if(InteractionMode ==1) //translation and rotation
			{				
				if(MouseMode ==1)//translation
					OnMouseTranslation();
				else if(MouseMode ==2)
				{
					Vector2 rot = new Vector2(0f,0f);
					rot.x = Input.GetAxis("Mouse X")*rotation_factor;
					rot.y = 0f-Input.GetAxis("Mouse Y")*rotation_factor;
					OnMouseRotation(rot);
				}
				else if(MouseMode ==4)
					TranslationZZ();
					
			}
			
			else if(InteractionMode ==2)//translation and intensity
			{
				if(MouseMode ==1)//translation
				{
					OnMouseTranslation();
					OnTragetMode();
				}
				else if(MouseMode ==4)
				{
					TranslationZZ();
					OnTragetMode();
				}
				
			}	 
		}
		else
		{
			DeselectedMode();
		}
		
		if (ExperimentManager.instance.currentTask == ExperimentManager.Task.Setup) {
			targetPosition = ExperimentManager.instance.currentLightTarget;
			transform.LookAt(targetPosition);
		}
	}
	
	void OnMouseMode(int mode)
	{
		MouseMode = mode;	
		
	}
	
	void OnLevelWasLoaded()
	{
		selectedTime = 0;
	}
	
	void OnMouseRotation(Vector2 rot)
	{
		//transform.Rotate(rot.y, rot.x, 0, Space.World);
		Quaternion angles  = transform.rotation;
		Quaternion angles_x = Quaternion.AngleAxis(rot.y, new Vector3(1,0,0));
		Quaternion angles_y = Quaternion.AngleAxis(rot.x, new Vector3(0,1,0));
		
		angles = angles*angles_x*angles_y;
		//angles.z = 0;
		float sum  = angles.x*angles.x + angles.y*angles.y + angles.z*angles.z + angles.w*angles.w;
		
		sum = Mathf.Sqrt(sum);
		angles.x = angles.x /sum;
		angles.y = angles.y/sum;
		angles.z = angles.z/sum;
		angles.w = angles.w/sum;
		Vector3 euler_angle = angles.eulerAngles;
		euler_angle.z = 0.0f;
		angles = Quaternion.Euler(euler_angle);
		transform.rotation = angles;
		
	}
	
	void TranslationZ(float values)
	{
		Vector3 pos = transform.position;
		pos.z += values*translation_factor;
		transform.position = pos;
	}
	
	void TranslationZZ()
	{
		
		Vector3 pos = transform.position;
		//pos.z += Input.GetAxis("Mouse X")*translation_factor;
		//float tran_x = Input.GetAxis("Mouse X")*translation_factor;
		float tran_y = Input.GetAxis("Mouse Y")*translation_factor*0.3f;
		//pos.z += Mathf.Sqrt(tran_x*tran_x+ tran_y*tran_y);
		if(pos.z<=-3.6f && tran_y<0.0f)
		{
			return;
		}
		else if(pos.z>=7.0 && tran_y>0.0f)
		{
			return;
		}
		
		pos.z += tran_y;
		transform.position = pos;
		
	}
	
	void OnTragetMode()
	{
		transform.LookAt(targetPosition);	
	}
	
	void SelectedMode() //highlight the selected lighted
	{
		Renderer []  children;
		children = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer child in children) {
            if(child.name =="mesh")
			{
				Color color = new Color(0.2f,0.4f,0.0f,0.5f);
				child.enabled = true;
				Material material = new Material(Shader.Find("Transparent/Diffuse"));
		        material.color = color;
		        child.material = material;
				break;
			}
        }
	}
	
	
	
	void DeselectedMode()
	{
		Renderer []  children;
		children = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer child in children) {
            if(child.name =="mesh")
			{
				child.enabled = false;
				break;
			}
        }
	}
	
	void LightReset()
	{
		transform.position = originia_position;
		transform.rotation = original_rotation;
		//SetIntensity(original_intensity);
		
	}
	
	void OnMouseTranslation()
	{
			Vector3 mousePos= Camera.main.WorldToScreenPoint(transform.position);
			float z_pos = transform.position.z;

			
			mousePos.x = Input.mousePosition.x;
			mousePos.y = Input.mousePosition.y;

			Vector3 point = Camera.main.ScreenToWorldPoint(mousePos);
			point.z = z_pos;
			transform.position = point;
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
	
	void OnLeapSelected(bool selected)
	{
		NMouseController.SetGameObj(this.gameObject);
		if(selected == true)
			MouseMode = 0;
		else 
			MouseMode = -1;
	}
	
	void SetIntensity(float values)
	{
		Light []  children;
		children = transform.gameObject.GetComponentsInChildren<Light>();
        foreach (Light child in children) {
            if(child.name =="light")
			{
				child.intensity += values;
			}
        }
	}
	
		void OnIntensity(float values)
	{
		Light []  children;
		children = gameObject.GetComponentsInChildren<Light>();
        foreach (Light child in children) {
            if(child.name =="light")
			{
//				print ("value: "+values);
				child.intensity += values;
				break;
			}
        }
	}
	
	float Presion(float num)
	{
		return Mathf.Round(num * 10) / 10;
	}
	
	/*
	void OnGUI()
	{
		string label="";
		int m_mode = 3;
		GameObject gameobj =  null;
		if(MouseController.MouseMode)
		{
			m_mode = NMouseController.InteractionMode;
			gameobj = NMouseController.clickedGmObj;
		}
		if(m_mode ==1)
			label ="Mode: Translation";
		else if(m_mode ==2)
			label ="Mode: Rotation";
		else if(m_mode ==0)
			label ="Mode: Selected";
		
		
		
		if(gameobj == transform.gameObject)
		{
			string info_pos ="Position X: " + Presion(transform.position.x).ToString() +" Y: " + Presion(transform.position.y).ToString() + " Z: "+Presion(transform.position.z).ToString(); 
			string info_rot="";
			Vector3 rot = transform.rotation.eulerAngles;
			info_rot  ="Rotation X: " + Presion(rot.x).ToString() +" Y: " + Presion(rot.y).ToString() + " Z: "+Presion(rot.z).ToString(); 
				
			string info_intensity ="Intensity: "+Presion(GetIntensity(transform.gameObject));
			//infoclickedGmObj.transform.position.x.ToString;
			GUI.Label(new Rect(20,10,200,100),label);
			GUI.Label(new Rect(20,20,200,100),info_pos);
			GUI.Label(new Rect(20,30,200,100),info_rot);
			GUI.Label(new Rect(20,40,200,100),info_intensity);
			
		}
	}
	*/
	
	
	void OnCollisionEnter(Collision collision)	
	{
		
		// print("Collided with " + collision.gameObject.name);
		occlisionObj = collision.gameObject;
		
		
		
		if((collision.gameObject.name != LightName))
		{
			if(collision.gameObject.name=="struss-1")
				beam_id = 1;
			else if(collision.gameObject.name=="struss-2")
				beam_id = 2;
			else if(collision.gameObject.name=="struss-3")
				beam_id = 3;
			else if(collision.gameObject.name=="struss-4")
				beam_id = 4;
			else if(collision.gameObject.name=="struss-5")
				beam_id = 5;
			else if(collision.gameObject.name=="struss-6")
				beam_id = 6;
			else if(collision.gameObject.name=="struss-7")
				beam_id = 7;
			else if(collision.gameObject.name=="struss-8")
				beam_id = 8;
			else if(collision.gameObject.name=="struss-9")
				beam_id = 9;	
			else 
			{
				beam_id = -1;
			}
			
			if(beam_id >0)
			{
				Color color = new Color(0.0f,0.8f,0.2f,0.7f);
				Material material = new Material(Shader.Find("Transparent/Diffuse"));
		        material.color = color;
				collision.gameObject.renderer.material = material;
				
				
				MouseController.occlision = beam_id;
				contact = collision.contacts[0];
			}
			//BeamTranslation(beam_id);
		}
	}
	
	void OnCollisionExit(Collision collision) {
        
		occlisionObj = collision.gameObject;
		
		if((collision.gameObject.name != LightName))
		{
			if(collision.gameObject.name=="struss-1")
				beam_id = 1;
			else if(collision.gameObject.name=="struss-2")
				beam_id = 2;
			else if(collision.gameObject.name=="struss-3")
				beam_id = 3;
			else if(collision.gameObject.name=="struss-4")
				beam_id = 4;
			else if(collision.gameObject.name=="struss-5")
				beam_id = 5;
			else if(collision.gameObject.name=="struss-6")
				beam_id = 6;
			else if(collision.gameObject.name=="struss-7")
				beam_id = 7;
			else if(collision.gameObject.name=="struss-8")
				beam_id = 8;
			else if(collision.gameObject.name=="struss-9")
				beam_id = 9;
			else 
			{
				beam_id = -1;
			}
			
			if(beam_id >0)
			{
				Color color = new Color(0.7f,0.7f,0.7f,1.0f);
				Material material = new Material(Shader.Find("Transparent/Diffuse"));
		        material.color = color;
				collision.gameObject.renderer.material = material;
				contact = collision.contacts[0];
			}
		}
    }
	
}
