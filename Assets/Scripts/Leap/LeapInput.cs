/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2013.                                   *
* Leap Motion proprietary and  confidential.  Not for distribution.            *
* Use subject to the terms of the Leap Motion SDK Agreement available at       *
* https://developer.leapmotion.com/sdk_agreement, or another agreement between *
* Leap Motion and you, your company or other organization.                     *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

/// <summary>
/// This static class serves as a static wrapper to provide some helpful C# functionality.
/// The main use is simply to provide the most recently grabbed frame as a singleton.
/// Events on aquiring, moving or loosing hands are also provided.  If you want to do any
/// global processing of data or input event dispatching, add the functionality here.
/// It also stores leap input settings such as how you want to interpret data.
/// To use it, you must call Update from your game's main loop.  It is not fully thread safe
/// so take care when using it in a multithreaded environment.
/// </summary>
public static class LeapInput 
{	
	public static bool EnableTranslation = false;
	public static bool EnableRotation = false;
	public static bool EnableScaling = false;
	
	public static int TaskMode = 0;
	
	/// <summary>
	/// Delegates for the events to be dispatched. LeapUnityHandController implements these.
	/// </summary>
	public delegate void PointableFoundHandler( Pointable p );
	public delegate void PointableUpdatedHandler( Pointable p );
	public delegate void HandFoundHandler( Hand h );
	public delegate void HandUpdatedHandler( Hand h );
	public delegate void ObjectLostHandler( int id );
	
	/// <summary>
	/// Event delegates are trigged every frame in the following order:
	/// Hand Found, Pointable Found, Hand Updated, Pointable Updated,
	/// Hand Lost, Hand Found.
	/// </summary>
	public static event PointableFoundHandler PointableFound;
	public static event PointableUpdatedHandler PointableUpdated;
	public static event ObjectLostHandler PointableLost;
	
	public static event HandFoundHandler HandFound;
	public static event HandUpdatedHandler HandUpdated;
	public static event ObjectLostHandler HandLost;
	
	public static Leap.Frame Frame
	{
		// This makes the private frame publicly accessible.
		get { return m_Frame; }
	}
	
	public static void Update() 
	{
		// If there is a controller currently connected
		if( m_controller != null )
		{
			// If the frame null, set lastFrame to Frame.Invalid,
			// otherwise set it to the current value of m_Frame.
			Frame lastFrame = m_Frame == null ? Frame.Invalid : m_Frame;
			// Get a Frame from the Controller:
			m_Frame	= m_controller.Frame();
			
			// Process all events for the new Frame
			DispatchLostEvents(Frame, lastFrame);
			DispatchFoundEvents(Frame, lastFrame);
			DispatchUpdatedEvents(Frame, lastFrame);
		}
	}
	
	public static void ChangeMode (int mod) {
		TaskMode = mod;
		if (TaskMode >= 0) {
			EnableTranslation = true;
			if (TaskMode == 0) {
				Debug.Log ("TASK MODE: TRANSLATION/ROTATION");
				EnableRotation = true;
				EnableScaling = false;
			}
			else if (TaskMode == 1) {
				Debug.Log ("TASK MODE: TRANSLATION/INTENSITY");
				EnableRotation = false;
				EnableScaling = true;
			}
		}
		else {
			Debug.Log ("LEAP OFF");
			EnableTranslation = false;
			EnableRotation = false;
			EnableScaling = false;
		}
	}
	
	// NOT USED! changes the mode according to fingers visible (called from HandController>SetInteractionMode)
	public static void EnableInteraction (string type) {
		Debug.Log ("Action mode: " + type);
		LeapInput.EnableRotation = false;
		LeapInput.EnableTranslation = false;
		LeapInput.EnableScaling = false;
		if (type == "Rotate")
			LeapInput.EnableRotation = true;
		else if (type == "Translate")
			LeapInput.EnableTranslation = true;
		else if (type == "Scale")
			LeapInput.EnableScaling = true;
	}
	
	//*********************************************************************
	// Private data & functions
	//*********************************************************************
	private enum HandID : int
	{
		Primary		= 0,
		Secondary	= 1
	};
	
	//Private variables
	static Leap.Controller 		m_controller	= new Leap.Controller();
	static Leap.Frame			m_Frame			= null;
	
	private static void DispatchLostEvents(Frame newFrame, Frame oldFrame)
	{
		// Iterate over hands in the previous frame
		foreach( Hand h in oldFrame.Hands )
		{
			// If this hand in the previous frame was invalid, continue
			// (A valid hand is one that contains valid tracking data)
			if( !h.IsValid )
				continue;
			// If this hand in the previous frame was valid, but is now
			// invalid, and if the HandLost event has a handler
			if( !newFrame.Hand(h.Id).IsValid && HandLost != null )
				// Fire the HandLost event for this hand
				HandLost(h.Id);
		}
		// Do the same as above for pointables (Fingers and Tools)
		// (Note that Hands are not Pointables)
		foreach( Pointable p in oldFrame.Pointables )
		{
			if( !p.IsValid )
				continue;
			if( !newFrame.Pointable(p.Id).IsValid && PointableLost != null )
				PointableLost(p.Id);
		}
	}
	
	private static void DispatchFoundEvents(Frame newFrame, Frame oldFrame)
	{
		// Iterate over hands in the new frame
		foreach( Hand h in newFrame.Hands )
		{
			// If the current hand is not valid, continue (fire no events)
			if( !h.IsValid )
				continue;
			// If the current hand was not valid in the previous frame
			// and if the HandLost event has a handler
			if( !oldFrame.Hand(h.Id).IsValid && HandFound != null)
				// Fire the HandFound event for this hand
				HandFound(h);
		}
		// Do the same as above for pointables (Fingers and Tools)
		// (Note that Hands are not Pointables)
		foreach( Pointable p in newFrame.Pointables )
		{
			if( !p.IsValid )
				continue;
			if( !oldFrame.Pointable(p.Id).IsValid && PointableFound != null )
				PointableFound(p);
		}
	}
	
	private static void DispatchUpdatedEvents(Frame newFrame, Frame oldFrame)
	{
		// For each Hand in the new Frame
		foreach( Hand h in newFrame.Hands )
		{
			// Do nothing if the hand is invalid (fire no events)
			if( !h.IsValid )
				continue;
			// If the hand was valid in the previous Frame and is _still_
			// valid, and the HandUpdated event has a handler
			if( oldFrame.Hand(h.Id).IsValid && HandUpdated != null)
				// Fire the HandUpdated event for this Hand
				HandUpdated(h);
		}
		// Do the same as above for pointables (Fingers and Tools)
		// (Note that Hands are not Pointables)
		foreach( Pointable p in newFrame.Pointables )
		{
			if( !p.IsValid )
				continue;
			if( oldFrame.Pointable(p.Id).IsValid && PointableUpdated != null)
				PointableUpdated(p);
		}
	}
}
