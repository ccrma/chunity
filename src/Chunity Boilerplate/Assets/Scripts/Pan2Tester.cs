using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pan2Tester : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<ChuckSubInstance>().RunCode(@"
			SinOsc foo => Pan2 p => dac;
			-1 => p.pan;

			while( true )
			{
			    Math.random2f( 300, 1000 ) => foo.freq;
			    100::ms => now;
			}
		");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
