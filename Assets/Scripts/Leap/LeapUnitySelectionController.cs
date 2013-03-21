/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2013.                                   *
* Leap Motion proprietary and  confidential.  Not for distribution.            *
* Use subject to the terms of the Leap Motion SDK Agreement available at       *
* https://developer.leapmotion.com/sdk_agreement, or another agreement between *
* Leap Motion and you, your company or other organization.                     *
\******************************************************************************/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Leap;

// Contains all the logic for the focusing, selecting, highlighting, and moving of objects
// based on Leap input.  Depends on The LeapUnityHandController & the LeapFingerCollisionDispatcher
// to move the hand representations around and detect collisions.  Highlighting is achieved by adding
// a highlight material to the object, then manipulating it's color based on how close it is to being
// selected.  Also depends on LeapInput & LeapInputUnityBridge. Currently just a prototype, it is 
// disabled in the scene by default.

public class LeapUnitySelectionController : MonoBehaviour {
	
	// Get a reference to the LeapUnitySelectionController (this is available because the script
	// is attached by default to the LeapController object in the scene.
	public static LeapUnitySelectionController Get()
	{
		return (LeapUnitySelectionController)GameObject.FindObjectOfType(typeof(LeapUnitySelectionController));		
	}
	
	
	public virtual bool CheckEndSelection(Frame thisFrame)
	{
		// If nothing is touching the object, the selection should cease.
		if( m_Touching.Count == 0 )
			return true;
		
		// If the total time the object has been touched is longer than the minimum 
		// duration for which an object must remain selected, and the object has not moved 
		// for the minimum amount of time, the selection should cease.
		if( Time.time - m_FirstTouchedTime > kMinSelectionTime + kSelectionTime && Time.time - m_LastMovedTime > kIdleStartDeselectTime+kSelectionTime )
		{
			return true;
		}
		
		// If we reach this point, the object remains selected.
		return false;
	}
	
	public virtual bool CheckShouldMove(Frame thisFrame)
	{
		// Return true if translation is enabled and at least one 
		// pointable is touching the object.
		return LeapInput.EnableTranslation && m_Touching.Count >= 1;	
	}
	
	public virtual bool CheckShouldRotate(Frame thisFrame)
	{
		// Return true if rotation is enabled and at least two 
		// pointables are touching the object.
		return LeapInput.EnableRotation && m_Touching.Count == 1;
	}
	
	public virtual bool CheckShouldScale(Frame thisFrame)
	{
		// Return true if scaling is enabled and at least two 
		// pointables are touching the object.
		return LeapInput.EnableScaling && m_Touching.Count >= 2;
	}
	
	public virtual void DoMovement(Frame thisFrame)
	{
		// Initialize two empty vectors.
		Vector3 currPositionSum = new Vector3(0,0,0);
		Vector3 lastPositionSum = new Vector3(0,0,0);
		
		// Iterate over all objects (fingers) touching the currently
		// focused object.
		foreach( GameObject obj in m_Touching )
		{
			// Add the transform for this finger to the 
			// position sum vector.
			currPositionSum += obj.transform.position;
		}
		
		// Iterate of the positions of the fingers in the last frame.
		foreach( Vector3 vec in m_LastPos )
		{
			// Sum each position with the last position vector.
			lastPositionSum += vec;	
		}
		
		// Add the difference between the current position and last position vectors
		// (divided by the number of fingers touching the object) to the transform
		// of the currently selected object.
		m_FocusedObject.transform.position += (currPositionSum - lastPositionSum) / m_Touching.Count;
	}

	public virtual void DoRotation(Frame thisFrame)
	{
		// Here we only work with the first two fingers touching the currently focused object.
		// Subtract one from the other for their last positions.
		Vector3 lastVec = m_LastPos[0];
		// Do the same for their current positions.

		Vector3 currVec = m_Touching[0].transform.position;
//		Debug.Log(currVec);
		// If these vectors are different:
//		if( lastVec != currVec )
//		{
//			// Take the cross product of these two vectors.
//			Vector3 axis = Vector3.Cross(currVec, lastVec);
//			// Calculate the rotation and apply to the focused object.
//
//			float axisDist = axis.magnitude;
////			Debug.Log(axis +","+lastDist+","+currDist+","+ axisDist);
//
//			float angle = -Mathf.Asin(axisDist);
////			Debug.Log(angle);
//			m_FocusedObject.transform.RotateAround(axis/axisDist, angle);
//		}	
	}
	
	public virtual void DoRotation_Old(Frame thisFrame)
	{
		// Here we only work with the first two fingers touching the currently focused object.
		// Subtract one from the other for their last positions.
		Vector3 lastVec = m_LastPos[1] - m_LastPos[0];
		// Do the same for their current positions.
		Vector3 currVec = m_Touching[1].transform.position - m_Touching[0].transform.position;

		Debug.Log(currVec);
		// If these vectors are different:
		if( lastVec != currVec )
		{
			// Take the cross product of these two vectors.
			Vector3 axis = Vector3.Cross(currVec, lastVec);
			// Calculate the rotation and apply to the focused object.
			float lastDist = lastVec.magnitude;
			float currDist = currVec.magnitude;
			float axisDist = axis.magnitude;
			float angle = -Mathf.Asin(axisDist / (lastDist*currDist));

			m_FocusedObject.transform.RotateAround(axis/axisDist, angle);
		}	
	}

		public virtual void DoScaling(Frame thisFrame)
	{
		Vector3 lastVec = m_LastPos[1] - m_LastPos[0];
		Vector3 currVec = m_Touching[1].transform.position - m_Touching[0].transform.position;
		if( lastVec != currVec )
		{
			float lastDist = lastVec.magnitude;
			float currDist = currVec.magnitude;
			Debug.Log (currDist);
			//clamp the scale of the object so we don't shrink/grow too much
			Vector3 scaleClamped = m_FocusedObject.transform.localScale * Mathf.Clamp((currDist/lastDist), .8f, 1.2f);
			scaleClamped.x = Mathf.Clamp(scaleClamped.x, .3f, 5.0f);
			scaleClamped.y = Mathf.Clamp(scaleClamped.y, .3f, 5.0f);
			scaleClamped.z = Mathf.Clamp(scaleClamped.z, .3f, 5.0f);
			m_FocusedObject.transform.localScale = scaleClamped;
		}
	}
	
	public virtual void DoScaling_old(Frame thisFrame)
	{
		Vector3 lastVec = m_LastPos[1] - m_LastPos[0];
		Vector3 currVec = m_Touching[1].transform.position - m_Touching[0].transform.position;
		if( lastVec != currVec )
		{
			float lastDist = lastVec.magnitude;
			float currDist = currVec.magnitude;
			//clamp the scale of the object so we don't shrink/grow too much
			Vector3 scaleClamped = m_FocusedObject.transform.localScale * Mathf.Clamp((currDist/lastDist), .8f, 1.2f);
			scaleClamped.x = Mathf.Clamp(scaleClamped.x, .3f, 5.0f);
			scaleClamped.y = Mathf.Clamp(scaleClamped.y, .3f, 5.0f);
			scaleClamped.z = Mathf.Clamp(scaleClamped.z, .3f, 5.0f);
			m_FocusedObject.transform.localScale = scaleClamped;
		}
	}
	
	void Update()
	{
		Leap.Frame thisFrame = LeapInput.Frame;
		if( thisFrame == null ) 
			return;
		
		//Remove fingers which have been disabled
		int index;
		while( (index = m_Touching.FindIndex(i => i.collider && i.collider.enabled == false)) != -1 ) 
		{
			m_Touching.RemoveAt(index);
			m_LastPos.RemoveAt(index);
		}
		
		if( m_LastFrame != null && thisFrame != null && m_Selected)
		{
			float transMagnitude = thisFrame.Translation(m_LastFrame).MagnitudeSquared;
			if( transMagnitude > kMovementThreshold )
				m_LastMovedTime = Time.time;
		}
		
		//Set selection after the time has elapsed
		if( !m_Selected && m_FocusedObject && (Time.fixedTime - m_FirstTouchedTime) >= kSelectionTime )
			// m_Selected = true;
			Select();
		
		//Update the focused object's color
		float selectedT = m_FocusedObject != null ? (Time.time - m_FirstTouchedTime) / kSelectionTime : 0.0f;
		
		//If we have passed the minimum deselection threshold and are past the minimum time to start deselecting...
		if( m_Selected && Time.time - m_FirstTouchedTime > kIdleStartDeselectTime + kSelectionTime )
		{
			selectedT = 1.3f - (((Time.time - m_LastMovedTime) - kIdleStartDeselectTime) / kSelectionTime);
		}
		SetHighlightColor( Color.Lerp(kBlankColor, m_HighlightMaterial.color, selectedT) );
		
		//Process the movement of the selected object.
		if( m_Selected && thisFrame != m_LastFrame )
		{
			//End selection if we don't see any fingers or the scaling factor is going down quickly ( indicating we are making a fist )
			if( CheckEndSelection(thisFrame) )
			{
				ClearFocus();
			}
			else
			{
				if( CheckShouldMove(thisFrame) )
				{
					DoMovement(thisFrame);
				}
				if( CheckShouldRotate(thisFrame) )
				{
					DoRotation(thisFrame);
				}
				if( CheckShouldScale(thisFrame) )
				{
					DoScaling(thisFrame);
				}
			}
		}

		
		m_LastFrame = thisFrame;
		for( int i = 0; i < m_Touching.Count; ++i )
		{
			m_LastPos[i] = m_Touching[i].transform.position;	
		}
	}
	
	public void OnTouched(GameObject finger, Collider other)
	{	
		//if we're still just focused (not selected yet), change our focus
		if( !m_Selected && other.gameObject != m_FocusedObject )
		{
			ClearFocus();
			SetFocus(other.gameObject);
		}
		
		if( !m_Touching.Contains(finger) )
		{
			m_Touching.Add(finger);
			m_LastPos.Add(finger.transform.position);
		}
	}
	
	//Todo: Make this get called for fingers which get disabled.
	public void OnStoppedTouching(GameObject finger, Collider other)
	{
		/*int index = m_Touching.FindIndex(o => o == finger);
		if( index != -1 )
		{
			m_Touching.RemoveAt(index);
			m_LastPos.RemoveAt(index);
		}*/
		//we deal with changing focus in the update loop.
	}
	public void OnRayHit(RaycastHit info)
	{
			
	}
	
	public void ClearFocus()
	{
		if( m_FocusedObject != null )
		{
			List<Material> materials = new List<Material>( m_FocusedObject.renderer.materials );
			Material removeMaterial = materials.Find( m => m.name == m_HighlightMaterial.name + " (Instance)" );
			materials.Remove(removeMaterial);
			m_FocusedObject.renderer.materials = materials.ToArray();
			Destroy(removeMaterial); //cleanup instanced material;
		}
		m_FocusedObject = null;
		m_FirstTouchedTime = 0.0f;
		m_LastMovedTime = 0.0f;
		m_Selected = false;
		m_Touching.Clear();
		m_LastPos.Clear();
	}
	
	public void SetFocus(GameObject focus)
	{
		m_FocusedObject = focus;
		m_FirstTouchedTime = Time.time;
		m_LastMovedTime = Time.time + kMinSelectionTime;
		//Add the new material, but set it as blank so it doesn't really show up.
		List<Material> materials = new List<Material>( focus.renderer.materials );
		Material newMaterial = new Material(m_HighlightMaterial);
		newMaterial.color = new Color(0,0,0,0);
		materials.Add(newMaterial);
		focus.renderer.materials = materials.ToArray();
	}
	
	public void SetHighlightColor(Color c)
	{
		if( m_FocusedObject == true )
		{
			Material[] materials = m_FocusedObject.renderer.materials;
			Material changeMat = Array.Find(materials, m => m.name == m_HighlightMaterial.name + " (Instance)" );
			changeMat.color = c;
			m_FocusedObject.renderer.materials = materials;
		}
	}
	
	public void Select ()
	{
		m_Selected = !m_Selected;
	}
	
	void Start()
	{
		m_HighlightMaterial = Resources.Load("Materials/Highlight") as Material;
	}
	
	protected GameObject m_FocusedObject = null;
	protected Leap.Frame m_LastFrame = null;
	//m_Touching maintains a list of fingers currently touching the focused object.
	//m_LastPos is the list of their last positions, used durring the update loop.
	protected List<GameObject> 	m_Touching = new List<GameObject>();
	protected List<Vector3>		m_LastPos = new List<Vector3>();
	
	protected bool m_Selected = false;
	protected float m_FirstTouchedTime = 0.0f;
	protected float m_LastMovedTime = 0.0f;
	
	protected const float kSelectionTime = .25f;		// wait for how long until object is selected/deselected (transition time)
	protected const float kIdleStartDeselectTime = 3f;	// how long does it have to be idle before starting deselection
	protected const float kMinSelectionTime = 2.0f;		// minimum time an object can be selected
	protected const float kMovementThreshold = 2.0f;	// minimum translation before it is considered idle (counting from m_LastMovedTime)
	protected Color kBlankColor = new Color(1,0,0,1);	// color during selection/deselection period
	
	private Material m_HighlightMaterial = null;
	
	
	
}
