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

/// <summary>
/// This class manipulates the hand representation in the unity scene based on the
/// input from the leap device. Fingers and Palm objects are moved around between
/// higher level 'hand' objects that mainly serve to organize.  Be aware that when
/// fingers are lost, unity does not dispatch OnTriggerExit events.
/// </summary>
public class LeapUnityHandController : MonoBehaviour 
{
	// Array for storing the Palms, Fingers, and Hands
	public GameObject[]				m_palms		= null;
	public GameObject[]				m_fingers	= null;
	public GameObject[]				m_hands 	= null;
	
	// Array for storing Materials for these
	public Material[]				m_materials = null;
	
	// Set to false to hide hands / true to draw them.
	public bool						m_DisplayHands = true;
	
	// These arrays allow us to use our game object arrays much like pools.
	// When a new hand/finger is found, we mark a game object by active
	// by storing it's id, and when it goes out of scope we make the
	// corresponding gameobject invisible & set the id to -1.
	private int[]					m_fingerIDs = null;
	private int[]					m_handIDs	= null;
		
	private Dictionary<int, string> FingerTypes = new Dictionary<int, string>();
	public static Pointable point;
		
	// Enable/disable collisions for components.
	void SetCollidable( GameObject obj, bool collidable )
	{
		// Enable/disable collisions for all components of type Collider 
		// that are owned by obj.
		foreach( Collider component in obj.GetComponents<Collider>() )
			component.enabled = collidable;
		
		// Enable/disable collisions for children of all components of type 
		// Collider that are owned by obj.
		foreach( Collider child in obj.GetComponentsInChildren<Collider>() )
			child.enabled = collidable;
	}
	
	// Enable/disable visibility of components.
	void SetVisible( GameObject obj, bool visible )
	{
		// Enable/disable visibility for all components of type Renderer
		// that are owned by obj, but only if m_DisplayHands is also true.
		foreach( Renderer component in obj.GetComponents<Renderer>() )
			component.enabled = visible && m_DisplayHands;
		
		// Enable/disable visibility for all children of components of type 
		// Renderer that are owned by obj, but only if m_DisplayHands is 
		// also true.
		foreach( Renderer child in obj.GetComponentsInChildren<Renderer>() )
			child.enabled = visible && m_DisplayHands;
	}
	
	// Start is called just before any of the Update methods is called the first time.
	void Start()
	{
		// Allocate new finger ID array and set all values to -1.
		m_fingerIDs = new int[10];
		for( int i = 0; i < m_fingerIDs.Length; i++ )
		{
			m_fingerIDs[i] = -1;	
		}
		
		// Allocate new hand array and set all values to -1.
		m_handIDs = new int[2];
		for( int i = 0; i < m_handIDs.Length; i++ )
		{
			m_handIDs[i] = -1;	
		}
		
		// Register a handler for each of the events declared in LeapInput.
		// The methods passed as parameters to the handlers are defined below.
		LeapInput.HandFound += new LeapInput.HandFoundHandler(OnHandFound);
		LeapInput.HandLost += new LeapInput.ObjectLostHandler(OnHandLost);
		LeapInput.HandUpdated += new LeapInput.HandUpdatedHandler(OnHandUpdated);
		LeapInput.PointableFound += new LeapInput.PointableFoundHandler(OnPointableFound);
		LeapInput.PointableLost += new LeapInput.ObjectLostHandler(OnPointableLost);
		LeapInput.PointableUpdated += new LeapInput.PointableUpdatedHandler(OnPointableUpdated);
		
		// Assign default materials to hand components (this is currently set to 
		// the material in m_UnknownHandMaterial in LeapUnityBridge.)
		foreach( Renderer r in m_hands[2].GetComponentsInChildren<Renderer>() )
		{
			r.material = m_materials[2];	
		}
		
		// Invalidate (hide) the palms.
		foreach( GameObject palm in m_palms )
		{
			updatePalm(Leap.Hand.Invalid, palm);
		}
		
		// Invalidate (hide) the fingers.
		foreach( GameObject finger in m_fingers)
		{
			updatePointable(Leap.Pointable.Invalid, finger);
		}
	}
	
	// Decides the interaction mode based on the visible fingers 
	// [NOT USED]
	void SetInteractionMode () {
		if (FingerTypes.Count == 0)
			print ("Nothing");
		else if (FingerTypes.Count == 1) {
			if (FingerTypes.ContainsValue("index"))	// two non-thumg fingers (index) -> Rotation
				LeapInput.EnableInteraction("Rotate");
			else
				LeapInput.EnableInteraction("Nothing");
		}
		else {
			if (FingerTypes.ContainsValue("thumb")) // if one out of two is a thumb -> Scaling
				LeapInput.EnableInteraction("Scale");
			else
				LeapInput.EnableInteraction("Translate"); // just one non-thumb (index) -> Translate
		}
	}
	
	// Evaluates the visible fingers and adds them in the FingerTypes dictionary (<int id>: <string type>)
	// [NOT USED]
	void EvaluateNewFinger (Pointable finger) {
		Leap.Vector handCenter = finger.Hand.SphereCenter;
		if (Math.Abs(handCenter.x - finger.TipPosition.x) < 40 && Math.Abs(handCenter.z - finger.TipPosition.z) > 15) {
//			print ("This is the index: "+finger.Id);
			FingerTypes.Add(finger.Id, "index");
		}
		else {
//			print ("This is the thumb: "+finger.Id);
			FingerTypes.Add(finger.Id, "thumb");
		}
	}
	
	// Get the max number of fingers that should be visible: 1 for translation/orientation, 2 for translation/scaling
	int GetAllowableFingers() {
		return LeapInput.TaskMode + 1;
	}
	
	public GameObject GetSecondFinger (GameObject firstFing) {
		// find if there is more than one finger in the finger's list
		int index = Array.FindIndex(m_fingerIDs, id => id == -1);
		// only the second position is empty => there is only one finger
		if (index == 1) {
			return firstFing;
		}
		else {
			foreach (GameObject f in m_fingers) {
				Transform tip = f.transform.Find("Tip");
				if (tip != firstFing.transform) {
//					Debug.Log ("FOUND Tip No "+i+" : "+f.transform.Find("Tip").gameObject);
					return f.transform.Find("Tip").gameObject;
				}
			}
			return firstFing;
		}	
	}
	
	// When an object is found, we find our first inactive game object, activate it, and assign it to the found id.
	// When lost, we deactivate the object & set it's id to -1.
	// When updated, load the new data.
	void OnPointableUpdated( Pointable p )
	{
		// Find the index of p.Id in m_fingerIDs.
		int index = Array.FindIndex(m_fingerIDs, id => id == p.Id);
		
		// If p.Id exists in m_fingerIDs:
		if( index != -1 )
		{
			// Update the pointable with the data passed from the event.
			updatePointable( p, m_fingers[index] );	
		}
	}
	
	void OnPointableFound( Pointable p )
	{
		
		// Find the first instance of -1 in m_fingerIDs
		// (the first unused index).
		int index = Array.FindIndex(m_fingerIDs, id => id == -1);
		// Allow only two fingers to be visible
		// if this is there are less than 2 fingers check which type is the new: thumb or index	
		if (index >= GetAllowableFingers()) {
			return;
//			EvaluateNewFinger (p);
//			// and change the interaciton mode accordingly
//			SetInteractionMode();
		}
//		else {
//			return;
//		}
		// If there is an available slot in the array:
		if( index != -1 )
		{
			// Change the index to match p.Id.
			m_fingerIDs[index] = p.Id;
			// Update the pointable with the data passed from the event.
			updatePointable( p, m_fingers[index] );
		}
	}
	
	void OnPointableLost( int lostID )
	{
		// Find lostID in m_fingerIDs.
		int index = Array.FindIndex(m_fingerIDs, id => id == lostID);
		
		// If it was found:
		if( index != -1 )
		{
			// Remove the lost finger from the dicitonary and update the interaction mode
			FingerTypes.Remove(lostID);
//			SetInteractionMode();
			// Change the state of the pointable to invalid.
			updatePointable( Pointable.Invalid, m_fingers[index] );
			// Free up the slot in m_fingerIDs by setting it to -1.
			m_fingerIDs[index] = -1;
		}
	}
	
	// Follow the same logic as above for OnPointableFound,
	// OnPointableUpdated, and OnPointableLost, but using
	// m_handIDs instead of m_fingerIDs.
	void OnHandFound( Hand h )
	{
		int index = Array.FindIndex(m_handIDs, id => id == -1);
		if( index != -1 )
		{
			m_handIDs[index] = h.Id;
			updatePalm(h, m_palms[index]);
		}
	}
	void OnHandUpdated( Hand h )
	{
		int index = Array.FindIndex(m_handIDs, id => id == h.Id);
		if( index != -1 )
		{
			updatePalm(	h, m_palms[index] );
		}
	}
	void OnHandLost(int lostID)
	{
		int index = Array.FindIndex(m_handIDs, id => id == lostID);
		if( index != -1 )
		{
			updatePalm(Hand.Invalid, m_palms[index]);
			m_handIDs[index] = -1;
		}
	}
	
	// Update a Unity GameObject that represents a Leap Pointable.
	void updatePointable( Leap.Pointable pointable, GameObject fingerObject )
	{
		// Update the parent object (the hand) of the pointable.
		updateParent( fingerObject, pointable.Hand.Id );
		
		// Change the pointable's visibility based on whether
		// or not the parent hand is valid.
		SetVisible(fingerObject, pointable.IsValid);
		// Do the same for the collidable property.
		SetCollidable(fingerObject, pointable.IsValid);
		
		// If the parent hand is valid:
		if ( pointable.IsValid )
		{
			point = pointable;
			// Create vectors for the finger's direction and position with the new 
			// direction and position from the Leap, but only after scaling/ofsetting 
			// with LeapUnityExtensions.
			Vector3 vFingerDir = pointable.Direction.ToUnity();
			Vector3 vFingerPos = pointable.TipPosition.ToUnityTranslated();
			
			// Apply the position vector to the Unity finger object.
			fingerObject.transform.localPosition = vFingerPos;
			// Apply the direction vector to the Unity finger object.
			// A Quaternion represents a rotation. For more information, see here:
			// http://docs.unity3d.com/Documentation/ScriptReference/Quaternion.FromToRotation.html
			fingerObject.transform.localRotation = Quaternion.FromToRotation( Vector3.forward, vFingerDir );
			// LIU: get the offset from the forward (pitch) and right (yaw) vectors
//			if (LeapUnitySelectionController.ActiveMode == "Rotating") {
//				float offsetY = Vector3.Angle(Vector3.up, vFingerDir);
//				float offsetX = Vector3.Angle(Vector3.right, vFingerDir);
//				float pitch = offsetY.ToUnityPitch();
//				float yaw = offsetX.ToUnityYaw();
				//fingerObject.GetComponent<SpotLight1>().OnLeapRotation(pitch, yaw);
//				Debug.Log ("Pitch: "+pitch+"  Yaw: "+yaw);
//				Vector2 rotation = new Vector2 (pitch,yaw);
//				LeapUnitySelectionController.DoRotation(rotation);
				//fingerObject.transform.RotateAroundLocal(new Vector3(0,1,0),yaw);
				//fingerObject.gameObject.SendMessage(OnLeapRotation, yaw,SendMessageOptions.DontRequireReceiver);
//			}
		}
	}
	
	// Update a palm.
	void updatePalm( Leap.Hand leapHand, GameObject palmObject )
	{
		// Update the palm's parent (the hand).
		updateParent( palmObject, leapHand.Id);
		
		// Change the palm's visibility based on whether
		// or not the parent hand is valid.
		SetVisible(palmObject, leapHand.IsValid);
		// Do the same for the collidable property.
		SetCollidable(palmObject, leapHand.IsValid);
		
		// If the parent hand is valid.
		if( leapHand.IsValid )
		{
			// Update the transform of the palm to match the incoming palm position from 
			// the Leap (after scaling/offsetting it in LeapUnityExtensions).
			palmObject.transform.localPosition = leapHand.PalmPosition.ToUnityTranslated();
			// NEW: updates the rotation of the palm based on the incoming palm rotation
//			Debug.Log(leapHand.Direction.ToUnityRotated());
//			Debug.Log(leapHand.Direction);
			Vector3 vPalmDir = leapHand.Direction.ToUnity(); //leapHand.Direction.ToUnityRotated();
			palmObject.transform.localRotation = Quaternion.FromToRotation( Vector3.forward, vPalmDir );
		}
	}	
	
	// Update a pointable's parent object.
	void updateParent( GameObject child, int handId )
	{
		// Find the handId in m_handIDs.
		int handIndex = Array.FindIndex(m_handIDs, id => id == handId);
		
		// If the handId is -1 or if it wasn't found in m_handIDs.
		if( handIndex == -1 || handId == -1 )
			// Set the hand index to the unknown hand index.
			handIndex = 2;
		
		// Based on the calculated handIndex, get the hand itself.
		GameObject parent = m_hands[handIndex];
		
		// If the pointable's (finger's) parent's (hand's) transform
		// and the pointable's transform itself are different, update
		// the parent's transform to match the pointable's.
		if( child.transform.parent != parent.transform )
		{
			child.transform.parent = parent.transform;
			
			// Change the material of all children objects (for instance,
			// if the pointable moved from one hand to the other, etc.
			foreach( Renderer r in child.GetComponents<Renderer>() )
				r.material = m_materials[handIndex];;	
			foreach( Renderer r in child.GetComponentsInChildren<Renderer>() )
				r.material = m_materials[handIndex];;
		}
	}
}
