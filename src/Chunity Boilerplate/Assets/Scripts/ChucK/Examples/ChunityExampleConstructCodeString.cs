using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunityExampleConstructCodeString : MonoBehaviour
{
	// This is an example of how you can construct a
	// string of ChucK code at runtime. 
	// It was used to provide functionality similar
	// to global arrays before they were available.

	ChuckSubInstance myChuck;

	public float[] myMidiNotes = { 60, 65, 69, 72 };

	// Use this for initialization
	void Start()
	{
		myChuck = GetComponent<ChuckSubInstance>();

	}

	// Update is called once per frame
	void Update()
	{
		if( Input.GetKeyDown( "space" ) )
		{
			// convert float array to string array
			string[] stringMidiNotes = new string[myMidiNotes.Length];
			for( int i = 0; i < myMidiNotes.Length; i++ )
			{
				stringMidiNotes[i] = myMidiNotes[i].ToString( "0.00" );
			}

			// construct array
			string chuckArray = "[" + string.Join( ", ", stringMidiNotes ) + "]";
			// show it in console!
			Debug.Log( chuckArray );

			// make a script involving that array
			myChuck.RunCode( @"
				TriOsc myOsc => dac;
			" + chuckArray + @" @=> float myNotes[];
				
				for( 0 => int i; i < myNotes.size(); i++ )
				{
					myNotes[i] => Math.mtof => myOsc.freq;
					100::ms => now;
				}
			" );
		}
	}
}
