using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunityExampleGlobalIntArray : MonoBehaviour
{
	// This example shows how to use various
	// methods for getting and setting global
	// int arrays.

	ChuckSubInstance myChuck;
	Chuck.IntArrayCallback myIntArrayCallback;
	Chuck.IntCallback myIntCallback;

	public long[] myMidiNotes = { 60, 65, 69, 72 };

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
				<<< myNotes[""numPlayed""], ""played so far"" >>>;
				myOsc =< dac;
			}
		" );

		myIntArrayCallback = myChuck.CreateGetIntArrayCallback( GetInitialArrayCallback );
		myIntCallback = myChuck.CreateGetIntCallback( GetANumberCallback );
	}

	// Update is called once per frame
	private int numPresses = 0;
	void Update()
	{

		if( Input.GetKeyDown( "space" ) )
		{
			// on first press, set entire array
			if( numPresses == 0 )
			{
				myChuck.SetIntArray( "myNotes", myMidiNotes );
			}
			// on any press, change the value of index 1
			myChuck.SetIntArrayValue( "myNotes", 1, 60 + numPresses );
			// set a dictionary value too
			myChuck.SetAssociativeIntArrayValue( "myNotes", "numPlayed", numPresses );
			// actually play it!
			myChuck.BroadcastEvent( "playMyNotes" );


			// test some gets too
			myChuck.GetIntArray( "myNotes", myIntArrayCallback );
			myChuck.GetIntArrayValue( "myNotes", 1, myIntCallback );
			myChuck.GetAssociativeIntArrayValue( "myNotes", "numPlayed", myIntCallback );

			numPresses++;
		}
	}

	void GetInitialArrayCallback( long[] values, ulong numValues )
	{
		Debug.Log( "Array has " + numValues.ToString() + " numbers which are: " );
		for( int i = 0; i < values.Length; i++ )
		{
			Debug.Log( "        " + values[i].ToString() );
		}
	}

	void GetANumberCallback( long value )
	{
		Debug.Log( "I got a number! " + value.ToString() );
	}
}
