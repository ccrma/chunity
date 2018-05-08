using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunityExampleGlobalEventWithHelperComponents : MonoBehaviour
{
	// This is an example of how to respond to
	// broadcasts from a ChucK global Event,
	// using Chunity helper components.
	// It stops responding to the broadcasts
	// when the user presses the space bar.

	ChuckSubInstance myChuck;

	void Start()
	{
		myChuck = GetComponent<ChuckSubInstance>();

		// broadcast "notifier" every 250 ms
		myChuck.RunCode( @"
            global Event notifier;
            while( true )
            {
                notifier.broadcast();
                250::ms => now;
            }
        " );

		// create a ChuckEventListener on this gameObject
		ChuckEventListener listener = gameObject.AddComponent<ChuckEventListener>();

		// call MyCallback() on the Update() thread after every broadcast from "notifier"
		listener.ListenForEvent( myChuck, "notifier", MyCallback );
	}

	void Update()
	{
		if( Input.GetKeyDown( "space" ) )
		{
			gameObject.GetComponent<ChuckEventListener>().StopListening();
		}
	}

	void MyCallback()
	{
		// react to event (in this case, rotate my object)
		transform.Rotate( new Vector3( 5, 10, 15 ) );
	}
}
