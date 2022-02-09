using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_WEBGL
using CK_INT = System.Int32;
using CK_UINT = System.UInt32;
#elif UNITY_ANDROID
using CK_INT = System.IntPtr;
using CK_UINT = System.UIntPtr;
#else
using CK_INT = System.Int64;
using CK_UINT = System.UInt64;
#endif
using CK_FLOAT = System.Double;

public class ChunityExampleTimeAdvancer : MonoBehaviour
{
	// This example shows a system where:
	// - A ChucK time step is set from Unity,
	//   depending on the mouse position
	// - ChucK calls a Unity callback every timestep
	// - ChucK also provides a float to Unity of 
	//   how far along the timestep it is.
	// This example uses callbacks on the audio thread.
	// You may find ChunityExampleTimeAdvancerWithHelperComponents
	// to be more approachable.

	ChuckSubInstance myChuck;
	Chuck.FloatCallback myGetPosCallback;
	Chuck.VoidCallback myTimeStepCallback;


	static float myPos;
	static bool beatHappened;
	static int notifyCount;

	// Use this for initialization
	void Start()
	{
		myChuck = GetComponent<ChuckSubInstance>();
		myGetPosCallback = GetPosCallback;
		myTimeStepCallback = BeNotified1;

		myPos = 0;

		myChuck.RunCode( @"
			1 => global float timeStep;
			global float pos;
			global Event notifier;

			fun void updatePos() {
				timeStep::second => dur currentTimeStep;
				currentTimeStep / 1000 => dur deltaTime;
				now => time startTime;
				
				pos => float originalPos;
								
				while( now < startTime + currentTimeStep )
				{
					deltaTime / currentTimeStep +=> pos;
					deltaTime => now;
				}
			}
			

			fun void playNote() {
				SinOsc foo => dac;
				0.2::second => now;
				foo =< dac;
			}

			while( true )
			{
				spork ~ playNote();
				spork ~ updatePos();
				notifier.broadcast();
				timeStep::second => now;
			}
		" );

		#if UNITY_WEBGL
		// WebGL specific callback signature: game object name, method name
		myChuck.StartListeningForChuckEvent( "notifier", gameObject.name, "BeNotified1" );
		#else
		// NOTE: can use below call signature if the callback is a *static* method for iOS or WebGL
		myChuck.StartListeningForChuckEvent( "notifier", myTimeStepCallback );
		#endif
	}

	// Update is called once per frame
	void Update()
	{
		// compute time step
		#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
		float newTimeStep = 1f;
		if( Input.touchCount > 0 )
		{
			Touch touch = Input.GetTouch(0);
			newTimeStep = Mathf.Clamp( touch.position.x, 250, 1000 ) / 1000.0f;
		}
		#else
		float newTimeStep = Mathf.Clamp( Input.mousePosition.x, 250, 1000 ) / 1000.0f;
        #endif

		myChuck.SetFloat( "timeStep", newTimeStep );

		#if UNITY_WEBGL
		// WebGL specific callback signature: game object name, method name
		myChuck.GetFloat( "pos", gameObject.name, "GetPosCallback" );

		// NOTE: can use below method too if the callback is made into a *static* method
		#else
		myChuck.GetFloat( "pos", myGetPosCallback );
		#endif

		transform.position = new Vector3( myPos % 4, 0, 0 );

		// respond to callback
		if( beatHappened )
		{
			transform.Rotate( new Vector3( 15, 30, 45 ) );
			beatHappened = false;
		}

		// an example of how to stop calling a callback 
		if( notifyCount > 10 )
		{
			#if UNITY_WEBGL
			// WebGL specific callback signature: game object name, method name
			myChuck.StopListeningForChuckEvent( "notifier", gameObject.name, "BeNotified1" );

			// NOTE: can use below call signature if the callback is a *static* method
			#else
			myChuck.StopListeningForChuckEvent( "notifier", myTimeStepCallback );
			#endif
		}
	}

	#if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.FloatCallback))]
	#endif
	static void GetPosCallback( CK_FLOAT pos )
	{
		myPos = (float) pos;
	}

	#if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.VoidCallback))]
	#endif
	static void BeNotified1()
	{
		beatHappened = true;
		notifyCount++;
	}
}
