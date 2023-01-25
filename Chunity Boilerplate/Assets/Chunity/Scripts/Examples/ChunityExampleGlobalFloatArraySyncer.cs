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

public class ChunityExampleGlobalFloatArraySyncer : MonoBehaviour
{
	// Identical to ChunityExampleGlobalFloatArray test,
    // only now using FloatArraySyncer, and no AssocArray tests

	ChuckSubInstance myChuck;
    ChuckFloatArraySyncer floatArraySyncer;

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
					myFloatNotes[i] => Math.mtof => myOsc.freq;
					100::ms => now;
				}
				myOsc =< dac;
			}
		" );

        floatArraySyncer = gameObject.AddComponent<ChuckFloatArraySyncer>();
        floatArraySyncer.SyncFloatArray(myChuck, "myFloatNotes");
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
                floatArraySyncer.SetNewArray(myMidiNotes);
			}
			// on any press, change the value of index 1
            floatArraySyncer.SetNewArrayValue(1, 60.5f + numPresses);

			// test some gets too
            LogArray(floatArraySyncer.GetCurrentArray());
            LogArrayValue(floatArraySyncer.GetCurrentArrayValue(1));
			
            // actually play it!
			myChuck.BroadcastEvent( "playMyNotes" );

			numPresses++;
		}
	}

	static void LogArray( CK_FLOAT[] values )
	{
		Debug.Log( "Float array has " + values.Length.ToString() + " numbers which are: " );
		for( int i = 0; i < values.Length; i++ )
		{
			Debug.Log( "        " + values[i].ToString() );
		}
	}

	static void LogArrayValue( CK_FLOAT value )
	{
		Debug.Log( "I got a number! " + value.ToString() );
	}
}