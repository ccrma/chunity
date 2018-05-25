using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunityExamplePrintStatements : MonoBehaviour
{
    // It's easy to print to the console from
    // ChucK. There are three main ways to do it:
    // <<< >>> error printing -- quick and dirty.
    // chout and cherr -- work similarly to cout and cerr
    // in C++.

    // Use this for initialization
    void Start()
    {
        GetComponent<ChuckSubInstance>().RunCode( @"
			SinOsc foo => dac;
			<<< ""Hello, I am a print statement and my script has connected foo to dac! "" >>>;
			chout <= ""Hello, I am another kind of print statement. "" <= "" with TWO PARTS! "" <= IO.newline();

			while( true ) 
			{
				Math.random2f( 300, 1000 ) => foo.freq;
				cherr <= ""foo.freq is "" <= foo.freq() <= IO.newline();
				1000::ms => now;
			}
		" );

        Debug.Log( "About to run some code that has a syntax error: " );
        GetComponent<ChuckSubInstance>().RunCode( " I am code with a syntax error " );
    }
}
