using UnityEngine;
using System.Collections;

public class SpotLight1 : MonoBehaviour {
	
	
	public bool selected = false;
	public int  mode = 0;
	
	private float translation_factor =100.0f;
	private float rotation_factor = 2.0f;
	private int beam_id = 0;
	private GameObject  occlisionObj;
	 
	void Start () 
	{
		 
	}
	
	// Update is called once per frame
	void Update () {
		
		 
		if(selected)
		{
			SelectedMode();
			
			if(mode ==1)
				OnMouseTranslation();
			else if(mode == 2)
				OnMouseRotation();
		}
		else
		{
			renderer.enabled = false;
		}
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
	
	void OnMouseMode(int m_mode)
	{
		mode =  m_mode;
	}
	
	void OnMouseSelected(bool m_select)
	{
		selected = m_select;
	}
	
	void OnIntensity(float values)
	{
		Light []  children;
		children = gameObject.GetComponentsInChildren<Light>();
        foreach (Light child in children) {
            if(child.name =="light")
			{
				child.intensity += values;
				break;
			}
        }
	}
	
	void OnMouseTranslationZ(float values)
	{
		
		Vector3 pos = transform.position;
		pos.z += values*translation_factor;
		transform.position = pos;
	}
	
	void OnMouseRotation()
	{
		transform.Rotate((Input.GetAxis("Mouse Y")*rotation_factor), (Input.GetAxis("Mouse X")*rotation_factor), 0, Space.World);
		
	}
	
	void SelectedMode()
	{
		Color color = new Color(0.2f,0.4f,0.0f,0.5f);
		renderer.enabled = true;
		Material material = new Material(Shader.Find("Transparent/Diffuse"));
        material.color = color;
        renderer.material = material;
	}
	
	void OnCollisionEnter(Collision collision)	
	{
		Debug.Log("Collided with " + collision.gameObject.name);
		occlisionObj = collision.gameObject;
		if((collision.gameObject.name != "SpotLight-1")&&(collision.gameObject.name != "SpotLight-2")&&(collision.gameObject.name != "SpotLight-3"))
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
				transform.eulerAngles = new Vector3(270, 180, 0);
			else if(collision.gameObject.name=="struss-four" || collision.gameObject.name=="struss-five" || collision.gameObject.name=="struss-six"||collision.gameObject.name=="struss-eight")
				transform.eulerAngles = new Vector3(90, 0, 0);
			else if (collision.gameObject.name=="struss-nine")
				transform.eulerAngles = new Vector3(0, 90, 90);			
			else beam_id = -1;
			
			if(beam_id >0)
			{
				Color color = new Color(0.0f,0.8f,0.2f,0.7f);
				renderer.enabled = true;
				Material material = new Material(Shader.Find("Transparent/Diffuse"));
		        material.color = color;
				collision.gameObject.renderer.material = material;
			}
			BeamTranslation(beam_id);
		}
	}
	
	void OnCollisionExit(Collision collision) {
        
		occlisionObj = collision.gameObject;
		if((collision.gameObject.name != "SpotLight-1")&&(collision.gameObject.name != "SpotLight-2")&&(collision.gameObject.name != "SpotLight-3"))
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
			else beam_id = -1;
			
			if(beam_id >0)
			{
				Color color = new Color(0.7f,0.7f,0.7f,1.0f);
				renderer.enabled = true;
				Material material = new Material(Shader.Find("Transparent/Diffuse"));
		        material.color = color;
				collision.gameObject.renderer.material = material;
			}
		}
    }
	
	
	void BeamTranslation(int id)
	{
		
		if(id <=3)
		{
			Vector3 pos = transform.position;
			pos.y = occlisionObj.transform.position.y;
			transform.position = pos;	 
		}
		else if(id <=6)
		{
			Vector3 pos = transform.position;
			pos.y = occlisionObj.transform.position.y;
			transform.position = pos;
		}
		else if(id ==9)
		{
			Vector3 pos = transform.position;
			pos.y = occlisionObj.transform.position.y;
			pos.z = occlisionObj.transform.position.z;
			transform.position = pos;
		}
		else
		{
			Vector3 pos = transform.position;
			pos.x = occlisionObj.transform.position.x;
			pos.z = occlisionObj.transform.position.z;
			transform.position = pos;
		}
	}
	
	
	
	 
	
	 
}
