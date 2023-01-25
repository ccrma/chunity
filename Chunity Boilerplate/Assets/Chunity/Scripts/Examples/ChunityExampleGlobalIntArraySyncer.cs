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

public class ChunityExampleGlobalIntArraySyncer : MonoBehaviour
{
	// Identical to ChunityExampleGlobalIntArray test,
    // only now using IntArraySyncer, and no AssocArray tests

	ChuckSubInstance myChuck;
    ChuckIntArraySyncer intArraySyncer;

	public CK_INT[] myMidiNotes = { 60, 65, 69, 72 };

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
					myNotes[i] => Math.mtof => myOsc.freq;
					100::ms => now;
				}
				myOsc =< dac;
			}
		" );

        intArraySyncer = gameObject.AddComponent<ChuckIntArraySyncer>();
        intArraySyncer.SyncIntArray(myChuck, "myNotes");
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
                intArraySyncer.SetNewArray(myMidiNotes);
			}

			// on any press, change the value of index 1
            intArraySyncer.SetNewArrayValue(1, (CK_INT) (60 + numPresses));

			// test some gets too
            LogArray(intArraySyncer.GetCurrentArray());
            LogArrayValue(intArraySyncer.GetCurrentArrayValue(1));
			
            // actually play it!
			myChuck.BroadcastEvent( "playMyNotes" );

			numPresses++;
		}
	}

	static void LogArray( CK_INT[] values )
	{
		Debug.Log( "Float array has " + values.Length.ToString() + " numbers which are: " );
		for( int i = 0; i < values.Length; i++ )
		{
			Debug.Log( "        " + values[i].ToString() );
		}
	}

	static void LogArrayValue( CK_INT value )
	{
		Debug.Log( "I got a number! " + value.ToString() );
	}
}