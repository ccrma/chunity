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
        // evaluate code
        GetComponent<ChuckSubInstance>().RunCode( @"
            SinOsc foo => dac;
            // chuck print out
            <<< ""Hello, I am a print statement and my script has connected foo to dac! "" >>>;
            // cherr write to stderr and flushes IO every time
            cherr <= ""Hello, I am another kind of print statement. "" <= "" with TWO PARTS! "" <= IO.newline();

            // time loop
            while( true )
            {
                // randomize frequency
                Math.random2f( 300, 1000 ) => foo.freq;
                // chout writes to stdout; buffers output; flushes with IO.newline();
                chout <= ""foo.freq is "" <= foo.freq() <= IO.newline();
                // advance time
                1000::ms => now;
            }
        " );

        // shouldn't get here unless RunCode() encountered errors
        Debug.Log( "About to run some code that has a syntax error: " );
        GetComponent<ChuckSubInstance>().RunCode( " I am code with a syntax error " );
    }
}
