using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyMidiInput : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<ChuckSubInstance>().RunCode(@"
            MidiIn min;
            MidiMsg msg;

            // open midi receiver, exit on fail
            if ( !min.open(1) )
            {
                <<< ""couldn't open midi interface 1 "" >>>;
                me.exit(); 
            }

            while( true )
            {
                // wait on midi event
                min => now;
                // receive midimsg(s)
                while( min.recv( msg ) )
                {
                    // print content
                    <<< msg.data1, msg.data2, msg.data3 >>>;
                }
            }
        ");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
