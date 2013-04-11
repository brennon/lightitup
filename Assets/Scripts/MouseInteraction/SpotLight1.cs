using UnityEngine;
using System.Collections;
using System.Threading;

public class SpotLight1 : MonoBehaviour {
	
	
	public bool selected = false;
	public int  mode = 0;
	public string OnOcclusion = "OnOcclusion";
	
	 
	
	private float translation_factor = 1f;
	private float rotation_factor = 2.0f;
	private int beam_id = 0;
	private GameObject  occlisionObj;
	private static ContactPoint contact;
	private int pre_mode;
	private int response = 200;
//	private bool scrow_init = true; 
	public static Vector3 pre_position;
	public static float sum_x =0.0f;
	public static float sum_y =0.0f;
	public static float pre_sum_x =0.0f;
	public static float pre_sum_y = 0.0f;
	 
	void Start () 
	{
		 //Screen.fullScreen = true;
		pre_position = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		
		 
		if(selected)
		{
			SelectedMode();
			
			if(mode ==1)
			{	
				// print ("translation");
				OnMouseTranslation();
			}
			else if(mode == 2)
			{
				Vector2 rot = new Vector2(0f,0f);
				rot.x = Input.GetAxis("Mouse X")*rotation_factor;
				rot.y = 0f-Input.GetAxis("Mouse Y")*rotation_factor;
				OnMouseRotation(rot);
			}
			
		  
		}
	}
	
	void TranslationReset()
	{
		pre_position = transform.position;
	}
	
	void RotationReset()
	{
		sum_x = pre_sum_x;
		sum_y =  pre_sum_y;
	}
	
	void RotationSave()
	{
		 pre_sum_x = sum_x;
		 pre_sum_y = sum_y;
	}
	
	void OnMouseTranslation()
	{
			Vector3 mousePos= Camera.main.WorldToScreenPoint(transform.position);
			float z_pos = transform.position.z;
		
			// print ("pre_pos:"+pre_position.z +  "  current:"+ z_pos);
		
			if(z_pos != pre_position.z)
				return;
			
			mousePos.x = Input.mousePosition.x;
			mousePos.y = Input.mousePosition.y;

			Vector3 point = Camera.main.ScreenToWorldPoint(mousePos);
			point.z = z_pos;
			transform.position = point;
		
			pre_position = transform.position;
	}
	
	void OnMouseMode(int m_mode)
	{
		if (mode != m_mode)
		{
			if(m_mode ==1)
				Thread.Sleep(response);
			if(m_mode ==2)
				RotationReset();
		}
		mode =  m_mode;
		
	}
	
	void OnMouseSelected(bool m_select)
	{
		selected = m_select;
		if(selected == false)
			DeselectedMode();
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
	
	void OnMouseTranslationZ(float values)
	{
		/*
		if(scrow_init == true)
		{	
			scrow_init = false;
			mode = 3;
		}*/
		Vector3 pos = transform.position;
		pos.z += values*translation_factor;
		transform.position = pos;
	}
	
	void OnMouseRotation(Vector2 rot)
	{
		 
		 
		sum_x += rot.x;
		sum_y += rot.y;
	 
		// print ("sum_x:"+sum_x +" sum_y:"+sum_y);
		
		if((Mathf.Abs(sum_x)<90) && (Mathf.Abs(sum_y)<90))
		{
			transform.Rotate(rot.y, rot.x, 0, Space.World);
		 
		}
		
		
	
	
		
	}
	
	 void OnLeapRotation(float yaw)
	{
		// print ("Pitch: "+yaw+" , " + "yaw: " + yaw);
		//transform.Rotate((Input.GetAxis("Mouse Y")*rotation_factor), (Input.GetAxis("Mouse X")*rotation_factor), 0, Space.World);
		
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
	
	
	void LightResetPos(int beam)
	{
		// print ("reset pos");
		BeamTranslation(beam);
	}
	
	void SelectedMode()
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
	
	void OnCollisionEnter(Collision collision)	
	{
		
		Debug.Log("Collided with " + collision.gameObject.name);
		occlisionObj = collision.gameObject;
		
		
		if((collision.gameObject.name != "SpotLight-1")&&(collision.gameObject.name != "SpotLight-2")&&(collision.gameObject.name != "SpotLight-3")&&(collision.gameObject.name != "mesh"))
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
			
			else if(collision.gameObject.name=="struss-one" || collision.gameObject.name=="struss-two" || collision.gameObject.name=="struss-three" ||collision.gameObject.name=="struss-seven") 
				transform.eulerAngles = new Vector3(270, 0, 0);
			else if(collision.gameObject.name=="struss-four" || collision.gameObject.name=="struss-five" || collision.gameObject.name=="struss-six"||collision.gameObject.name=="struss-eight")
				transform.eulerAngles = new Vector3(90, 180, 0);
			else if (collision.gameObject.name=="struss-nine")
				transform.eulerAngles = new Vector3(0, 270, 90);			
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
		
		 
		
		if((collision.gameObject.name != "SpotLight-1")&&(collision.gameObject.name != "SpotLight-2")&&(collision.gameObject.name != "SpotLight-3")&&(collision.gameObject.name != "mesh"))
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
	
	
	void BeamTranslation(int id)
	{
		Vector3 pos = contact.point;
		// print ("contract point"+pos);
		
		if(occlisionObj != null)
		{
			if(id <=3)
			{
				if(pos.x !=0 || pos.y !=0 || pos.z !=0  )
				{
					pos.x = pos.x -0.22f;
					pos.y = occlisionObj.transform.position.y;
					transform.position = pos;
				}
			}
			else if(id <=6)
			{
				if(pos.x !=0 || pos.y !=0 || pos.z !=0  )
				{
					pos.x = pos.x +0.22f;
					pos.y = occlisionObj.transform.position.y;
					transform.position = pos;
				}
			}
			
			else if (id ==9)
			{
				if(pos.x !=0 || pos.y !=0 || pos.z !=0  )
				{
					 
					pos.y = occlisionObj.transform.position.y-0.11f;
					transform.position = pos;
				}
				
			}
			else if( id ==8)
			{
				if(pos.x !=0 || pos.y !=0 || pos.z !=0  )
				{
					pos.x = pos.x +0.13f; 
					
					pos.z = occlisionObj.transform.position.z;
					transform.position = pos;
				}
			}
			else if( id ==7)
			{
				if(pos.x !=0 || pos.y !=0 || pos.z !=0  )
				{
					pos.x = pos.x -0.13f; 
					
					pos.z = occlisionObj.transform.position.z;
					transform.position = pos;
				}
			}
		
		}
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
	
	float Presion(float num)
	{
		return Mathf.Round(num * 10) / 10;
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
	
	void OnGUI()
	{
		string label="";
		int m_mode = 3;
		GameObject gameobj = null;
		if(MouseController.MouseMode)
		{
			m_mode = MouseController.mouseMode;
			gameobj = MouseController.clickedGmObj;
		}
		if(m_mode ==1)
			label ="Mode: Translation";
		else if(m_mode ==2)
			label ="Mode: Rotation";
		else if(m_mode ==3)
			label ="Mode: Selected";
		
		
		
		if(gameobj == transform.gameObject)
		{
			string info_pos ="Position X: " + Presion(transform.position.x).ToString() +" Y: " + Presion(transform.position.y).ToString() + " Z: "+Presion(transform.position.z).ToString(); 
			string info_rot="";
			Vector3 rot = GetRotation(transform.gameObject);
			info_rot  ="Rotation X: " + Presion(rot.x).ToString() +" Y: " + Presion(rot.y).ToString() + " Z: "+Presion(rot.z).ToString(); 
				
			string info_intensity ="Intensity: "+Presion(GetIntensity(transform.gameObject));
			//infoclickedGmObj.transform.position.x.ToString;
			GUI.Label(new Rect(20,10,200,100),label);
			GUI.Label(new Rect(20,20,200,100),info_pos);
			GUI.Label(new Rect(20,30,200,100),info_rot);
			GUI.Label(new Rect(20,40,200,100),info_intensity);
			
		}
		
		 // Event e = Event.current;
         // if (e.button == 0 && e.isMouse)
            // Debug.Log("Left Click");
	}
	
	
	 
	
	 
}
