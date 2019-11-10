using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_WEBGL
using CK_INT = System.Int32;
using CK_UINT = System.UInt32;
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

	int notifyCount;

	float myPos;

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

		// NOTE: can use below call signature if the callback is a *static* method
		#else
		myChuck.StartListeningForChuckEvent( "notifier", myTimeStepCallback );
		#endif
	}

	// Update is called once per frame
	void Update()
	{
		float newTimeStep = Mathf.Clamp( Input.mousePosition.x, 250, 1000 ) / 1000.0f;

		myChuck.SetFloat( "timeStep", newTimeStep );

		#if UNITY_WEBGL
		// WebGL specific callback signature: game object name, method name
		myChuck.GetFloat( "pos", gameObject.name, "GetPosCallback" );

		// NOTE: can use below method too if the callback is made into a *static* method
		#else
		myChuck.GetFloat( "pos", myGetPosCallback );
		#endif

		transform.position = new Vector3( myPos % 4, 0, 0 );

		// an example of how to stop calling a callback 
		if( notifyCount > 5 )
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

	//[AOT.MonoPInvokeCallback(typeof(Chuck.FloatCallback))]
	void GetPosCallback( CK_FLOAT pos )
	{
		myPos = (float) pos;
	}

	//[AOT.MonoPInvokeCallback(typeof(Chuck.VoidCallback))]
	void BeNotified1()
	{
		transform.Rotate( new Vector3( 15, 30, 45 ) );
		notifyCount++;
	}
}
