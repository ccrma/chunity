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

public class ChunityExampleGlobalIntArray : MonoBehaviour
{
	// This example shows how to use various
	// methods for getting and setting global
	// int arrays.

	ChuckSubInstance myChuck;
	Chuck.IntArrayCallback myIntArrayCallback;
	Chuck.IntCallback myIntCallback;

	public CK_INT[] myMidiNotes = { (CK_INT)60, (CK_INT)65, (CK_INT)69, (CK_INT)72 };

	// Use this for initialization
	void Start()
	{
		myChuck = GetComponent<ChuckSubInstance>();
		myChuck.RunCode( @"
			TriOsc myOsc;
			[60] @=> global int myNotes[];
			global Event playMyNotes;
			
			while( true )
			{
				playMyNotes => now;
				myOsc => dac;
				for( 0 => int i; i < myNotes.size(); i++ )
				{
					<<< ""myNotes["", i, ""] ="", myNotes[i] >>>;
					myNotes[i] => Math.mtof => myOsc.freq;
					100::ms => now;
				}
				<<< myNotes[""numPlayed""], ""played so far"" >>>;
				myOsc =< dac;
			}
		" );

		myIntArrayCallback = GetInitialArrayCallback;
		myIntCallback = GetANumberCallback;
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
				myChuck.SetIntArray( "myNotes", myMidiNotes );
			}
			// on any press, change the value of index 1
			myChuck.SetIntArrayValue( "myNotes", 1, (CK_INT)(60 + numPresses) );
			// set a dictionary value too
			myChuck.SetAssociativeIntArrayValue( "myNotes", "numPlayed", (CK_INT)numPresses );
			// actually play it!
			myChuck.BroadcastEvent( "playMyNotes" );


			// test some gets too
			myChuck.GetIntArray( "myNotes", myIntArrayCallback );

			#if UNITY_WEBGL
			// WebGL specific float callback signature: game object name, method name
			myChuck.GetIntArrayValue( "myNotes", 1, gameObject.name, "GetANumberCallback" );
			myChuck.GetAssociativeIntArrayValue( "myNotes", "numPlayed", gameObject.name, "GetANumberCallback" );
		
			// NOTE: can do it the below way if the callback is made into a *static* method
			#else
			myChuck.GetIntArrayValue( "myNotes", 1, myIntCallback );
			myChuck.GetAssociativeIntArrayValue( "myNotes", "numPlayed", myIntCallback );
			#endif

			numPresses++;
		}
	}

	#if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.IntArrayCallback))]
	#endif
	static void GetInitialArrayCallback( CK_INT[] values, CK_UINT numValues )
	{
		Debug.Log( "Int array has " + numValues.ToString() + " numbers which are: " );
		for( int i = 0; i < values.Length; i++ )
		{
			Debug.Log( "        " + values[i].ToString() );
		}
	}

	#if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.IntCallback))]
	#endif 
	static void GetANumberCallback( CK_INT value )
	{
		Debug.Log( "I got a number! " + value.ToString() );
	}
}
