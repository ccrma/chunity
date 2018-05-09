using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunityExampleStopScript : MonoBehaviour
{
	// This shows an example of how to stop
	// a script from Unity using global Events.

	// Use this for initialization
	void Start()
	{
		GetComponent<ChuckSubInstance>().RunCode( @"
            // name of global event
            global Event myCodeStopper;

            // broadcast, so that anyone else listening will be stopped
            myCodeStopper.broadcast();

            // do my thing
            SinOsc foo => dac;
            Math.random2f( 300, 1000 ) => foo.freq;

            // until I hear from myCodeStopper
            myCodeStopper => now;

        " );
	}

	// Update is called once per frame
	void Update()
	{
		if( Input.GetKeyDown( "space" ) )
		{
			GetComponent<ChuckSubInstance>().BroadcastEvent( "myCodeStopper" );
		}
	}

}
