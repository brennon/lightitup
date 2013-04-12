using UnityEngine;
using System.Collections;
using System.Threading;

public class SpotLight : MonoBehaviour {
	
	public int  MouseMode = -1;
	public Vector3 targetPosition;
	
	public string[] target = {"TargetCube-1","TargetCube-2","TargetCube-3"};
	public int targetID = 0;
	
	private float rotation_factor = 2.0f;
	private Vector3 originia_position;
	private Quaternion original_rotation;
	private float   original_intensity;
	//private float   original_rotation = 0;
	// Use this for initialization
	
	
	void Start () {
		
		originia_position = transform.position;
		original_rotation = transform.rotation;	
		original_intensity = GetIntensity();
		
		targetPosition = new Vector3(0,0,0);
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if(MouseMode>=0) // light is selected
		{
			SelectedMode();
			
			if(MouseMode ==1) //translation
			{
				//OnMouseTranslation();
				OnTragetMode();
			}
			else if(MouseMode ==2)
			{
				Vector2 rot = new Vector2(0f,0f);
				rot.x = Input.GetAxis("Mouse X")*rotation_factor;
				rot.y = 0f-Input.GetAxis("Mouse Y")*rotation_factor;
				transform.Rotate(rot.y, rot.x, 0, Space.World);
			}
			else 
			{
				OnTragetMode();
			}
			 
		}
		else
		{
			DeselectedMode();
		}
		
//		print (transform.forward);
	
	}
	
	void OnMouseMode(int mode)
	{
		MouseMode = mode;
	}
	
	void SetTargetID(int id)
	{
		targetID = id;
	}
	
	
	void OnTragetMode()
	{
		OnMouseTranslation();
		
		if(targetID>0)
		{
			GameObject obj = GameObject.Find(target[targetID-1]);
			if(obj != null)
			{
				transform.LookAt(obj.transform.position);
			}
		}
			
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
		SetIntensity(original_intensity);
		
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
	
	
	float GetIntensity()
	{
		Light []  children;
		children = transform.gameObject.GetComponentsInChildren<Light>();
        foreach (Light child in children) {
            if(child.name =="light")
			{
				return child.intensity;
			}
        }
		return -1.0f;
		 
	}
	
	void SetIntensity(float values)
	{
		Light []  children;
		children = transform.gameObject.GetComponentsInChildren<Light>();
        foreach (Light child in children) {
            if(child.name =="light")
			{
				child.intensity = values;
			}
        }
	}
}
