using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRunCodeStopper : MonoBehaviour {

	// Use this for initialization
	void Start () {
		RunMyCode();
	}
	
	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown( "space" ) )
		{
			RunMyCode();
		}
	}

	void RunMyCode()
	{
		GetComponent<ChuckSubInstance>().RunCode(@"
			// name of external event
			external Event myCodeStopper;

			// broadcast, so that anyone else listening will be stopped
			myCodeStopper.broadcast();

			// do my thing
			SinOsc foo => dac;
			Math.random2f( 300, 1000 ) => foo.freq;

			// until I hear from myCodeStopper
			myCodeStopper => now;

		");
	}
}
