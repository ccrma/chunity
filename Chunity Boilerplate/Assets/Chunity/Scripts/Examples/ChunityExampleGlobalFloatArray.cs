using System;
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

public class ChunityExampleGlobalFloatArray : MonoBehaviour
{
	// This example shows how to use various methods
	// for getting and setting global float arrays.

	ChuckSubInstance myChuck;
	Chuck.FloatArrayCallback myFloatArrayCallback;
	Chuck.FloatCallback myFloatCallback;

	public CK_FLOAT[] myMidiNotes = { 60, 65, 69, 72 };

	// Use this for initialization
	void Start()
	{
		myChuck = GetComponent<ChuckSubInstance>();
		myChuck.RunCode( @"
			TriOsc myOsc;
			[60.0] @=> global float myFloatNotes[];
			global Event playMyNotes;
			
			while( true )
			{
				playMyNotes => now;
				myOsc => dac;
				for( 0 => int i; i < myFloatNotes.size(); i++ )
				{
                    <<< ""myFloatNotes["", i, ""] ="", myFloatNotes[i] >>>;
					myFloatNotes[i] => Math.mtof => myOsc.freq;
					100::ms => now;
				}
				<<< myFloatNotes[""numPlayed""], ""played so far"" >>>;
				myOsc =< dac;
			}
		" );

		#if !UNITY_WEBGL
		myFloatArrayCallback = GetInitialArrayCallback;
		myFloatCallback = GetANumberCallback;
		#endif
	}

	// Update is called once per frame
	private int numPresses = 0;
	void Update()
	{
		if( ChunityDemo.InteractWithDemo() )
		{
			// on first press, set entire array
			if( numPresses == 0 )
			{
				myChuck.SetFloatArray( "myFloatNotes", myMidiNotes );
			}
			// on any press, change the value of index 1
			myChuck.SetFloatArrayValue( "myFloatNotes", 1, 60.5f + numPresses );
			// set a dictionary value too
			myChuck.SetAssociativeFloatArrayValue( "myFloatNotes", "numPlayed", numPresses );


			// test some gets too
			#if UNITY_WEBGL
			// WebGL: SendMessage can only pass string/number (arrays arrive as JSON)
			myChuck.GetFloatArray( "myFloatNotes", gameObject.name, "GetInitialArrayCallback" );
			myChuck.GetFloatArrayValue( "myFloatNotes", 1, gameObject.name, "GetANumberCallback" );
			myChuck.GetAssociativeFloatArrayValue( "myFloatNotes", "numPlayed", gameObject.name, "GetANumberCallback" );
			#else
			myChuck.GetFloatArray( "myFloatNotes", myFloatArrayCallback );
			myChuck.GetFloatArrayValue( "myFloatNotes", 1, myFloatCallback );
			myChuck.GetAssociativeFloatArrayValue( "myFloatNotes", "numPlayed", myFloatCallback );
			#endif
			
            // actually play it!
			myChuck.BroadcastEvent( "playMyNotes" );

			numPresses++;
		}
	}

#if UNITY_WEBGL
	[Serializable]
	private class FloatArrayJson
	{
		public CK_FLOAT[] items;
	}

	// Instance methods required for Unity SendMessage
	void GetInitialArrayCallback( string json )
	{
		FloatArrayJson parsed = JsonUtility.FromJson<FloatArrayJson>( json );
		CK_FLOAT[] values = ( parsed != null && parsed.items != null ) ? parsed.items : new CK_FLOAT[] {};
		Debug.Log( "Float array has " + values.Length.ToString() + " numbers which are: " );
		for( int i = 0; i < values.Length; i++ )
		{
			Debug.Log( "        " + values[i].ToString() );
		}
	}

	void GetANumberCallback( float value )
	{
		Debug.Log( "I got a number! " + value.ToString() );
	}
#else
	#if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.FloatArrayCallback))]
	#endif
	static void GetInitialArrayCallback( CK_FLOAT[] values, CK_UINT numValues )
	{
		Debug.Log( "Float array has " + numValues.ToString() + " numbers which are: " );
		for( int i = 0; i < values.Length; i++ )
		{
			Debug.Log( "        " + values[i].ToString() );
		}
	}

	#if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.FloatCallback))]
	#endif
	static void GetANumberCallback( CK_FLOAT value )
	{
		Debug.Log( "I got a number! " + value.ToString() );
	}
#endif
}
