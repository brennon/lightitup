/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2013.                                   *
* Leap Motion proprietary and  confidential.  Not for distribution.            *
* Use subject to the terms of the Leap Motion SDK Agreement available at       *
* https://developer.leapmotion.com/sdk_agreement, or another agreement between *
* Leap Motion and you, your company or other organization.                     *
\******************************************************************************/

using UnityEngine;
using System.Collections;

/// <summary>
/// Attach one of these to one of the objects in your scene to use Leap input.
/// It will take care of calling update on LeapInput and create hand objects
/// to represent the hand data in the scene using LeapUnityHandController.
/// It has a number of public fields so you can easily set the values from
/// the Unity inspector. Hands will 
/// </summary>
public class LeapUnityBridge : MonoBehaviour
{
	/// <summary>
	/// These values, set from the Inspector, set the corresponding fields in the
	/// LeapUnityExtension for translating vectors.
	/// </summary>
	public Vector3 m_LeapScaling = new Vector3(0.02f, 0.02f, 0.02f);
	public Vector3 m_LeapOffset = new Vector3(0,0,0);
	
	// These values can also all be set in the Unity Inspector
	public bool m_UseFixedUpdate = false; //If true, calls LeapInput.Update from FixedUpdate instead of Update
	public bool m_ShowInspectorFingers = true; //If false, hides the hand game objects in the inspector.
	public GameObject m_InputParent; //The parent of the hand objects for motion.  Useful 
	public GameObject m_FingerTemplate; //The template object to use for the fingers. Must have Tags set correctly
	public GameObject m_PalmTemplate; //The template object to use for the palms.
	
	/// <summary>
	/// The materials to use for the different hands.
	/// </summary>
	public Material m_PrimaryHandMaterial; 
	public Material m_SecondaryHandMaterial;
	public Material m_UnknownHandMaterial;
	
	private static bool m_Created = false; // True if a LeapUnityBridge has been instantiated
	
	// Awake is called when the script instance is being loaded.
	void Awake()
	{
		// If a LeapUnityBridge has already been instantiated
		if( m_Created )
		{
			// Throw an exception
			throw new UnityException("A LeapUnityBridge has already been created!");
		}
		m_Created = true;
		
		// Set the scaling and offset values in UnityVectorExtension with
		// those set above
		Leap.UnityVectorExtension.InputScale = m_LeapScaling;
		Leap.UnityVectorExtension.InputOffset = m_LeapOffset;
		
		// Ensure that m_FingerTemplate is set in the Unity inspector
		if( !m_FingerTemplate )
		{
			Debug.LogError("No Finger template set!");
			return;
		}
		
		// Ensure that m_PalmTemplate is set in the Unity inspector
		if( !m_PalmTemplate )
		{
			Debug.LogError("No Palm template set!");
			return;
		}
		
		// If no exceptions have been thrown, create an internal representation
		// of the user's hands
		CreateSceneHands();
	}
	
	// OnDestroy is called when the MonoBehaviour will be destroyed.
	void OnDestroy()
	{
		// State that there is no longer an active LeapUnityBridge
		m_Created = false;	
	}
	
	// Call Update in LeapInput
	// This gets the current frame and fires the appropriate events
	void FixedUpdate()
	{
		if( m_UseFixedUpdate )
			LeapInput.Update();
	}
	
	// Call Update in LeapInput
	// This gets the current frame and fires the appropriate events
	void Update()
	{
		if( !m_UseFixedUpdate )
			LeapInput.Update();
		
		// Check keyboard input to enable/disable different types of interaction
		if( Input.GetKeyDown(KeyCode.T) ) {
			LeapInput.EnableTranslation = !LeapInput.EnableTranslation;
			print ("Translation: "+LeapInput.EnableTranslation);
		}
		if( Input.GetKeyDown(KeyCode.R) ) {
			LeapInput.EnableRotation = !LeapInput.EnableRotation;
			print ("Rotation: "+LeapInput.EnableRotation);
		}
		if( Input.GetKeyDown(KeyCode.S) ) {
			LeapInput.EnableScaling = !LeapInput.EnableScaling;
			print ("Intensity: "+LeapInput.EnableScaling);
		}
		if (Input.GetKeyDown(KeyCode.Alpha0))
			LeapInput.ChangeMode(0);
		if (Input.GetKeyDown(KeyCode.Alpha1))
			LeapInput.ChangeMode(1);
		if (Input.GetKeyDown(KeyCode.Alpha5))
			LeapInput.ChangeMode(-1);	// No Leap Input
		
	}
	
	// Create the data structure that represents the user's hands
	private void CreateSceneHands()
	{
		// GameObject is the base class for all entities in Unity scenes (it contains components)
		GameObject hands = new GameObject("Leap Hands");
		
		// If the parent object for the Leap input was set in the inspector
		if( m_InputParent )
		{
			// Set the hands' transform to the parent's transform
			hands.transform.parent = m_InputParent.transform;
		}
		else
		{
			// Otherwise, set the hands' transform to the root transform in the 
			// transform hierarchy
			hands.transform.parent = transform;			
		}
		
		// Add a LeapUnityHandController component to the hands object
		// This controller governs the behavior of the hands object
		hands.AddComponent(typeof(LeapUnityHandController));
		
		// Get a reference to that component
		LeapUnityHandController behavior = hands.GetComponent<LeapUnityHandController>();
		
		// Create GameObject arrays for the palms, fingers, and hands
		behavior.m_palms = new GameObject[2];
		behavior.m_fingers = new GameObject[10];
		behavior.m_hands = new GameObject[3]; //extra 'invalid' hand for grouping purposes
		
		// Create a Material array for the materials used for the hands
		behavior.m_materials = new Material[] { m_PrimaryHandMaterial, m_SecondaryHandMaterial, m_UnknownHandMaterial };
		
		// Create each Hand and add it to the m_hands array
		for( int i = 0; i < behavior.m_hands.Length; i++ )
		{
			behavior.m_hands[i] = CreateHand(hands, i);	
		}
		
		// Create each Finger and add it to the m_fingers array
		for( int i = 0; i < behavior.m_fingers.Length; i++ )
		{
			behavior.m_fingers[i] = CreateFinger(behavior.m_hands[2], i);
		}
		
		// Create each Palm and add it to the m_palms array
		for( int i = 0; i < behavior.m_palms.Length; i++ )
		{
			behavior.m_palms[i] = CreatePalm(behavior.m_hands[2], i);	
		}
		
		// Get all GameObjects in the scene with the "FingerTip" tag
		// (This is set in the Unity Inspector)
		foreach( GameObject fingerTip in GameObject.FindGameObjectsWithTag("FingerTip") )
		{
			// Debug.Log ("adding component...");
			// Attach the LeapFingerCollisionDispatcher component to each
			// fingertip--see the LeapFingerCollisionDispatcher script
			fingerTip.AddComponent(typeof(LeapFingerCollisionDispatcher));	
		}
	}
	
	private GameObject CreateHand(GameObject parent, int index)
	{
		// Create a GameObject to hold the hand
		GameObject hand = new GameObject();
		
		// Assign the parent's transform to the hand's transform
		// property. As written, the parent is the hands object.
		hand.transform.parent = parent.transform;
		
		// Name each hand
		if( index == 0 )
			hand.name = "Primary Hand";
		else if( index == 1 )
			hand.name = "Secondary Hand";
		else
			hand.name = "Unknown Hand";
		
		return hand;
	}
	
	private GameObject CreateFinger(GameObject parent, int index)
	{
		// Instantiate (create a clone of) the finger template for the
		// Finger we are creating.
		GameObject finger = Instantiate(m_FingerTemplate) as GameObject;
		
		// Assign the parent's transform to the Finger's transform
		// property. As written, the parent is the m_hands array.
		finger.transform.parent = parent.transform;
		
		// Name each Finger sequentially using the index parameter.
		finger.name = "Finger " + index;
		
		return finger;
	}
	
	private GameObject CreatePalm(GameObject parent, int index)
	{
		// Instantiate (create a clone of) the palm template for the
		// Palm we are creating.
		GameObject palm = Instantiate(m_PalmTemplate) as GameObject;
		
		// Name each Palm sequentially using the index parameter.
		palm.name = "Palm " + index;
		
		// Assign the parent's transform to the Palm's transform
		// property. As written, the parent is the m_hands array.
		palm.transform.parent = parent.transform;
		
		return palm;
	}
};
