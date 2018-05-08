using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunityExampleMidiInput : MonoBehaviour
{
	// Just as it's possible to use the keyboard
	// and mouse from ChucK, you can also access
	// MIDI devices! As always, if this isn't behaving
	// how you think it should, check the device number.

	void Start()
	{
		GetComponent<ChuckSubInstance>().RunCode( @"
            MidiIn min;
            MidiMsg msg;
            
            0 => int device;

            // open midi receiver, exit on fail
            if ( !min.open( device ) )
            {
                <<< ""couldn't open midi interface "", device >>>;
                me.exit(); 
            }

            while( true )
            {
                // wait on midi event
                min => now;
                // receive midimsg( s )
                while( min.recv( msg ) )
                {
                    // print content
                    <<< msg.data1, msg.data2, msg.data3 >>>;
                }
            }
        " );
	}
}
