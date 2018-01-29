using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyChuckPrintStatements : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<ChuckSubInstance>().RunCode(@"
			SinOsc foo => dac;
			<<< ""Hello, I am a print statement and my script has connected foo to dac! "" >>>;
			chout <= ""Hello, I am another kind of print statement. "" <= IO.newline();

			while( true ) 
			{
				Math.random2f( 300, 1000 ) => foo.freq;
				cherr <= ""foo.freq is "" <= foo.freq() <= IO.newline();
				100::ms => now;
			}
		");

		GetComponent<ChuckSubInstance>().RunCode(" I am code with a syntax error ");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
