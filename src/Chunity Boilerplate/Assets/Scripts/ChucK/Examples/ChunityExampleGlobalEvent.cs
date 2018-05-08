using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunityExampleGlobalEvent : MonoBehaviour
{
	// This is an example of how to respond to
	// broadcasts from a ChucK global Event.
	// You may wish to look at 
	// ChunityExampleGlobalEventWithHelperComponents
	// instead.

	ChuckSubInstance myChuck;
	Chuck.VoidCallback myCallback;

	public MeshRenderer myBox;

	private int numTimesCallbackCalled = 0;

	void Start()
	{
		// get reference to chuck instance
		myChuck = GetComponent<ChuckSubInstance>();
		// create the callback we will pass
		myCallback = Chuck.CreateVoidCallback( CallbackFunction );

		// run code: make a global event, and every 250 ms, broadcast it to all listeners
		myChuck.RunCode( @"
			global Event notifier;
			while( true )
			{
				notifier.broadcast();
				250::ms => now;
			}
		" );

		// register myCallback as a listener of Event "notifier" until I tell it to stop
		myChuck.StartListeningForChuckEvent( "notifier", myCallback );
	}

	void CallbackFunction()
	{
		// store a message that the callback function was called
		// (we can't do Unity-specific things in here, since ChucK will be calling this, not Unity)
		numTimesCallbackCalled++;
	}


	void Update()
	{
		// check whether the callback function was called
		while( numTimesCallbackCalled > 0 )
		{
			// decrement because we are responding to the callback function being called
			numTimesCallbackCalled--;
			// do the thing we actually wanted to do when the callback function was called
			// (here, it randomizes the color of the myBox Renderer)
			myBox.material.color = Random.ColorHSV();
		}
	}


}
