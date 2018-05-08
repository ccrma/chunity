using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunityExampleEditParameterWhileRunning : MonoBehaviour
{
	// UGens can also be global variables!
	// Remember that global variables are
	// shared by all Instances relying on
	// the same ChuckMainInstance.


	// Use this for initialization
	void Start()
	{
		GetComponent<ChuckSubInstance>().RunCode( @"
			global SinOsc s;
			// play it forever
			s => dac;
		
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
                global SinOsc s;
				Math.random2f(300, 1000) => s.freq;
			" );
		}
	}
}
