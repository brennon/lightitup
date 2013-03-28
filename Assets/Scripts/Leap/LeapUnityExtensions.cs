/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2013.                                   *
* Leap Motion proprietary and  confidential.  Not for distribution.            *
* Use subject to the terms of the Leap Motion SDK Agreement available at       *
* https://developer.leapmotion.com/sdk_agreement, or another agreement between *
* Leap Motion and you, your company or other organization.                     *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

namespace Leap {
	
	// Extension to the unity vector class. Provides automatic scaling into unity scene space.
	// Leap coordinates are in cm, so the .02f scaling factor means 1cm of hand motion = .02m scene motion
	public static class UnityVectorExtension
	{
		// Change scale and offset here, if desired.
		public static Vector3 InputScale = new Vector3(0.1f, 0.1f, 0.1f);
		public static Vector3 InputOffset = new Vector3(0,0,0);
		
		// For Directions
		public static Vector3 ToUnity(this Vector lv)
		{
			return FlippedZ(lv);
		}
		
		// For Acceleration/Velocity
		public static Vector3 ToUnityScaled(this Vector lv)
		{
			return Scaled(FlippedZ( lv ));
		}
		
		// For Positions
		public static Vector3 ToUnityTranslated(this Vector lv)
		{
			return Offset(Scaled(FlippedZ( lv )));
		}
		
		// For Rotations: change the angle to lightstage value
		public static float ToUnityPitch(this float a)
		{
			return a-45; //Quaternion.Euler(0,0,0);
		}
		
		public static float ToUnityYaw(this float a)
		{
			return a-90; //Quaternion.Euler(0,0,0);
		}
		
		// Change sign of z-axis.
		private static Vector3 FlippedZ( Vector v ) { return new Vector3( v.x, v.y, -v.z ); }
		
		// Apply InputScale to vector.
		private static Vector3 Scaled( Vector3 v ) { return new Vector3( v.x * InputScale.x,
																		 v.y * InputScale.y,
																		 v.z * InputScale.z ); }
		
		// Shift all vector values by InputOffset.
		private static Vector3 Offset( Vector3 v ) { return v + InputOffset; }
	}
}
