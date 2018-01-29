using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyParamEditWhileRunning : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<ChuckSubInstance>().RunCode(@"
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
		");
	}
	
	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown( "space" ) )
		{
			// update the state of my SinOsc
			GetComponent<ChuckSubInstance>().RunCode(@"
				Math.random2f(300, 1000) => MyChuckStorageClass.s.freq;
			");
		}
	}
}
