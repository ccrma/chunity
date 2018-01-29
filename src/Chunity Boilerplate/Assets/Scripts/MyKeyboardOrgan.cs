using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyKeyboardOrgan : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<ChuckSubInstance>().RunCode(@"
		// HID
		Hid hi;
		HidMsg msg;

		// which keyboard
		0 => int device;
		
		// open keyboard (get device number from command line)
		if( !hi.openKeyboard( device ) ) me.exit();
		<<< ""keyboard '"" + hi.name() + ""' ready"", """" >>>;

		// patch
		BeeThree organ => JCRev r => Echo e => Echo e2 => dac;
		r => dac;

		// set delays
		240::ms => e.max => e.delay;
		480::ms => e2.max => e2.delay;
		// set gains
		.6 => e.gain;
		.3 => e2.gain;
		.05 => r.mix;
		0 => organ.gain;

		// infinite event loop
		while( true )
		{
		    // wait for event
		    hi => now;

		    // get message
		    while( hi.recv( msg ) )
		    {
		        // check
		        if( msg.isButtonDown() )
		        {
// ========== msg.which contains the ASCII value of the keypress! ==========
		            Std.mtof( msg.which + 45 ) => float freq;
		            if( freq > 20000 ) continue;

		            freq => organ.freq;
		            .5 => organ.gain;
		            1 => organ.noteOn;

		            80::ms => now;
		        }
		        else
		        {
		            0 => organ.noteOff;
		        }
		    }
		}

		");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
