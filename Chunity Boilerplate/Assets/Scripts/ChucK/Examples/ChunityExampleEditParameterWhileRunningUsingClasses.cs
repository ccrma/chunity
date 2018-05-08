using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunityExampleEditParameterWhileRunningUsingClasses : MonoBehaviour
{
	// While it's possible to update a parameter
	// by using global variables, you can also 
	// store things in a public class.

	// Like a global variable, a public class is
	// shared by all Instances relying on the same
	// ChuckMainInstance. So, static variables in
	// public classes work similarly to global
	// variables.

	// This pattern is useful if you ever want
	// to have a global variable, but its type
	// is not enabled for the global keyword yet.
	// This file in particular was written when
	// UGens could not be global variables.


	// Use this for initialization
	void Start()
	{
		GetComponent<ChuckSubInstance>().RunCode( @"
			// public classes are accessible from other scripts
			public class MyChuckStorageClass
			{
				// static reference to a SinOsc
				static SinOsc @ s;
			}
			// must initialize static variables outside of class for now :(
			SinOsc s @=> MyChuckStorageClass.s;


			// play it forever
			MyChuckStorageClass.s => dac;
		
			while( true ) { 1::second => now; }
		" );
	}

	// Update is called once per frame
	void Update()
	{
		if( Input.GetKeyDown( "space" ) )
		{
			// update the state of my SinOsc
			GetComponent<ChuckSubInstance>().RunCode( @"
				Math.random2f(300, 1000) => MyChuckStorageClass.s.freq;
			" );
		}
	}
}
